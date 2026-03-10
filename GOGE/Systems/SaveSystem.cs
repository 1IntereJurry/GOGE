using GOGE.Models;
using GOGE.Utils;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Linq;

namespace GOGE.Systems
{
    public static class SaveSystem
    {
        // Use a per-user application data folder so users don't need admin rights
        private static readonly string BaseAppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "GOGE");
        private static readonly string SaveFolder = Path.Combine(BaseAppData, "Saves");
        private static readonly string AutoSaveFolder = Path.Combine(SaveFolder, "Auto");
        private const int CURRENT_VERSION = 4;

        public class SaveData
        {
            public int Version { get; set; } = CURRENT_VERSION;
            public Character Player { get; set; } = null!;
            [JsonIgnore]
            public InventorySystem Inventory { get; set; } = new InventorySystem();
            public List<string> InventoryItems { get; set; } = new();
            public string? Location { get; set; }
            public DateTime SaveTime { get; set; } = DateTime.Now;

            // Persist equipped items by id (preferred) and name (for compatibility)
            public string? EquippedWeaponId { get; set; }
            public string? EquippedHelmetId { get; set; }
            public string? EquippedChestId { get; set; }
            public string? EquippedPantsId { get; set; }
            public string? EquippedBootsId { get; set; }

            public string? EquippedWeaponName { get; set; }
            public string? EquippedHelmetName { get; set; }
            public string? EquippedChestName { get; set; }
            public string? EquippedPantsName { get; set; }
            public string? EquippedBootsName { get; set; }
        }

        // ---------------------------------------------------------
        // SAVE GAME
        // ---------------------------------------------------------
        public static void SaveGame(Character player, InventorySystem inventory, bool isAutoSave = false)
        {
            try
            {
                string targetFolder = isAutoSave ? AutoSaveFolder : SaveFolder;
                if (!Directory.Exists(targetFolder))
                    Directory.CreateDirectory(targetFolder);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                Console.WriteLine("Could not create save folder: " + ex.Message);
                return;
            }

            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

            string rawName = isAutoSave
                ? $"AUTO - {player.Name} ({player.Class}) - {timestamp}"
                : $"{player.Name} ({player.Class}) - {timestamp}";

            string safeName = SanitizeFileName(rawName);
            string targetFolder2 = isAutoSave ? AutoSaveFolder : SaveFolder;
            string path = Path.Combine(targetFolder2, $"{safeName}.json");

            var data = new SaveData
            {
                Version = CURRENT_VERSION,
                Player = player,
                Inventory = inventory,
                Location = "Unknown",
                SaveTime = DateTime.Now,
                EquippedWeaponId = player.EquippedWeapon?.Id,
                EquippedHelmetId = player.EquippedHelmet?.Id,
                EquippedChestId = player.EquippedChestplate?.Id,
                EquippedPantsId = player.EquippedPants?.Id,
                EquippedBootsId = player.EquippedBoots?.Id,

                EquippedWeaponName = player.EquippedWeapon?.Name,
                EquippedHelmetName = player.EquippedHelmet?.Name,
                EquippedChestName = player.EquippedChestplate?.Name,
                EquippedPantsName = player.EquippedPants?.Name,
                EquippedBootsName = player.EquippedBoots?.Name
            };

            // serialize inventory items as JSON strings including a 'type' discriminator
            data.InventoryItems = new List<string>();
            foreach (var item in inventory.Items)
            {
                try
                {
                    string serialized = JsonSerializer.Serialize(item, item.GetType());
                    var node = System.Text.Json.Nodes.JsonNode.Parse(serialized) as System.Text.Json.Nodes.JsonObject ?? new System.Text.Json.Nodes.JsonObject();
                    string typeLabel = item.GetType().Name;
                    if (item is ArmorPiece) typeLabel = "ArmorPiece";
                    node["type"] = typeLabel;
                    data.InventoryItems.Add(node.ToJsonString());
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                    try
                    {
                        var fallback = new System.Text.Json.Nodes.JsonObject { ["type"] = item.GetType().Name, ["name"] = item.Name, ["id"] = item.Id };
                        data.InventoryItems.Add(fallback.ToJsonString());
                    }
                    catch { }
                }
            }

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            try
            {
                string json = JsonSerializer.Serialize(data, options);
                File.WriteAllText(path, json);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                Console.WriteLine("Failed to save game: " + ex.Message);
                return;
            }

            if (!isAutoSave)
                Console.WriteLine(Localization.TF("Save.Saved", safeName));
        }

        // ---------------------------------------------------------
        // LOAD GAME
        // ---------------------------------------------------------
        public static SaveData? LoadGame(string fileName)
        {
            string path = Path.Combine(SaveFolder, $"{fileName}.json");

            if (!File.Exists(path))
            {
                Console.WriteLine(Localization.T("Safe.NotFound"));
                return null;
            }

            string json = File.ReadAllText(path);

            SaveData? data;
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                data = JsonSerializer.Deserialize<SaveData>(json, options);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                Console.WriteLine(Localization.TF("Load.ErrorDeserializing", ex.Message));
                return null;
            }

            if (data == null)
            {
                Console.WriteLine(Localization.T("Load.ErrorLoading"));
                return null;
            }

            UpgradeSaveData(data);

            // reconstruct InventorySystem from InventoryItems (JSON strings)
            var inv = new InventorySystem();
            foreach (var itemJson in data.InventoryItems ?? new List<string>())
            {
                try
                {
                    using var doc = JsonDocument.Parse(itemJson);
                    var elem = doc.RootElement.Clone();
                    var item = ItemFactory.CreateItem(elem);
                    if (item != null)
                        inv.Add(item);
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                    // ignore individual item failures
                }
            }

            data.Inventory = inv;

            // restore equipped items based on ids (preferred) with name fallback
            try
            {
                Character player = data.Player;

                if (!string.IsNullOrEmpty(data.EquippedWeaponId))
                    player.EquippedWeapon = data.Inventory.Items.FirstOrDefault(i => i.Id == data.EquippedWeaponId) as Weapon;
                else if (!string.IsNullOrEmpty(data.EquippedWeaponName))
                    player.EquippedWeapon = data.Inventory.Items.OfType<Weapon>().FirstOrDefault(w => w.Name == data.EquippedWeaponName);

                if (!string.IsNullOrEmpty(data.EquippedHelmetId))
                    player.EquippedHelmet = data.Inventory.Items.FirstOrDefault(i => i.Id == data.EquippedHelmetId) as ArmorPiece;
                else if (!string.IsNullOrEmpty(data.EquippedHelmetName))
                    player.EquippedHelmet = data.Inventory.Items.OfType<ArmorPiece>().FirstOrDefault(a => a.Name == data.EquippedHelmetName);

                if (!string.IsNullOrEmpty(data.EquippedChestId))
                    player.EquippedChestplate = data.Inventory.Items.FirstOrDefault(i => i.Id == data.EquippedChestId) as ArmorPiece;
                else if (!string.IsNullOrEmpty(data.EquippedChestName))
                    player.EquippedChestplate = data.Inventory.Items.OfType<ArmorPiece>().FirstOrDefault(a => a.Name == data.EquippedChestName);

                if (!string.IsNullOrEmpty(data.EquippedPantsId))
                    player.EquippedPants = data.Inventory.Items.FirstOrDefault(i => i.Id == data.EquippedPantsId) as ArmorPiece;
                else if (!string.IsNullOrEmpty(data.EquippedPantsName))
                    player.EquippedPants = data.Inventory.Items.OfType<ArmorPiece>().FirstOrDefault(a => a.Name == data.EquippedPantsName);

                if (!string.IsNullOrEmpty(data.EquippedBootsId))
                    player.EquippedBoots = data.Inventory.Items.FirstOrDefault(i => i.Id == data.EquippedBootsId) as ArmorPiece;
                else if (!string.IsNullOrEmpty(data.EquippedBootsName))
                    player.EquippedBoots = data.Inventory.Items.OfType<ArmorPiece>().FirstOrDefault(a => a.Name == data.EquippedBootsName);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }

            data.Inventory ??= new InventorySystem();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(Localization.TF("Save.Loaded", fileName));
            Console.ResetColor();

            return data;
        }

        // ---------------------------------------------------------
        // MIGRATION / VERSION UPGRADE
        // ---------------------------------------------------------
        private static void UpgradeSaveData(SaveData data)
        {
            if (data.Version < 2)
            {
                data.Version = 2;
            }

            if (data.Version < 3)
            {
                data.Location ??= "Unknown";
                data.Version = 3;
            }

            if (data.Version < 4)
            {
                // new fields added in v4
                data.Version = 4;
            }
        }

        // ---------------------------------------------------------
        // LIST ALL SAVE FILES
        // ---------------------------------------------------------
        public static List<string> GetSaveFiles()
        {
            try
            {
                if (!Directory.Exists(SaveFolder))
                    Directory.CreateDirectory(SaveFolder);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return new List<string>();
            }

            return Directory.GetFiles(SaveFolder, "*.json")
                            .Select(f => Path.GetFileNameWithoutExtension(f))
                            .ToList();
        }

        // ---------------------------------------------------------
        // FILTERED SAVE LISTS
        // ---------------------------------------------------------
        public static List<string> GetAutoSaves()
        {
            try
            {
                if (!Directory.Exists(AutoSaveFolder))
                    Directory.CreateDirectory(AutoSaveFolder);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return new List<string>();
            }

            return Directory.GetFiles(AutoSaveFolder, "*.json")
                .OrderByDescending(f => File.GetCreationTime(f))
                .Take(15)
                .Select(f => Path.GetFileNameWithoutExtension(f))
                .ToList();
        }

        public static List<string> GetManualSaves()
        {
            try
            {
                if (!Directory.Exists(SaveFolder))
                    Directory.CreateDirectory(SaveFolder);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return new List<string>();
            }

            return Directory.GetFiles(SaveFolder, "*.json")
                .OrderByDescending(f => File.GetCreationTime(f))
                .Select(f => Path.GetFileNameWithoutExtension(f))
                .ToList();
        }

        // ---------------------------------------------------------
        // DELETE OLD AUTOSAVES (KEEP 3 NEWEST)
        // ---------------------------------------------------------
        public static void DeleteOldAutoSavesForPlayer(string playerName)
        {
            try
            {
                if (!Directory.Exists(AutoSaveFolder))
                    Directory.CreateDirectory(AutoSaveFolder);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return;
            }

            var autoSaveFiles = Directory.GetFiles(AutoSaveFolder, "*.json")
                .OrderByDescending(f => File.GetCreationTime(f))
                .ToList();

            var playerAutoSaves = autoSaveFiles
                .Where(f =>
                {
                    try
                    {
                        string json = File.ReadAllText(f);
                        var data = JsonSerializer.Deserialize<SaveData>(json);
                        return data?.Player?.Name == playerName;
                    }
                    catch
                    {
                        return false;
                    }
                })
                .ToList();

            var toDelete = playerAutoSaves.Skip(3);

            foreach (var file in toDelete)
                File.Delete(file);
        }

        // ---------------------------------------------------------
        // DELETE SAVE
        // ---------------------------------------------------------
        public static bool DeleteSave(string fileName)
        {
            try
            {
                if (!Directory.Exists(SaveFolder))
                    Directory.CreateDirectory(SaveFolder);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return false;
            }

            string path = Path.Combine(SaveFolder, $"{fileName}.json");

            if (!File.Exists(path))
                return false;

            File.Delete(path);
            return true;
        }

        // ---------------------------------------------------------
        // SANITIZE FILE NAME
        // ---------------------------------------------------------
        private static string SanitizeFileName(string name)
        {
            var invalid = Path.GetInvalidFileNameChars();
            var cleaned = new string(name.Select(ch => invalid.Contains(ch) ? '-' : ch).ToArray());
            return cleaned;
        }
    }
}