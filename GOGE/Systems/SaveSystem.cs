using GOGE.Models;
using GOGE.Utils;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Nodes;

namespace GOGE.Systems
{
    public static class SaveSystem
    {
        private static readonly string SaveFolder = "Saves";
        private const int CURRENT_VERSION = 3;

        public class SaveData
        {
            public int Version { get; set; } = CURRENT_VERSION;

            public Character Player { get; set; } = null!;

            // Inventory is reconstructed after deserialization. Do not serialize Inventory directly.
            [JsonIgnore]
            public InventorySystem Inventory { get; set; } = new InventorySystem();

            // Serialized representation of inventory items (JSON strings with type)
            public List<string> InventoryItems { get; set; } = new();

            public string? Location { get; set; }
            public DateTime SaveTime { get; set; } = DateTime.Now;
        }

        // ---------------------------------------------------------
        // SAVE GAME
        // ---------------------------------------------------------
        public static void SaveGame(Character player, InventorySystem inventory, bool isAutoSave = false)
        {
            if (!Directory.Exists(SaveFolder))
                Directory.CreateDirectory(SaveFolder);

            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

            string rawName = isAutoSave
                ? $"AUTO - {player.Name} ({player.Class}) - {timestamp}"
                : $"{player.Name} ({player.Class}) - {timestamp}";

            string safeName = SanitizeFileName(rawName);
            string path = Path.Combine(SaveFolder, $"{safeName}.json");

            var data = new SaveData
            {
                Version = CURRENT_VERSION,
                Player = player,
                Inventory = inventory,
                Location = "Unknown",
                SaveTime = DateTime.Now
            };

            // serialize inventory items as JSON strings including a 'type' discriminator
            data.InventoryItems = new List<string>();
            foreach (var item in inventory.Items)
            {
                try
                {
                    string serialized = JsonSerializer.Serialize(item, item.GetType());
                    // parse to JsonNode to inject type
                    var node = JsonNode.Parse(serialized) as JsonObject ?? new JsonObject();
                    string typeLabel = item.GetType().Name;
                    // normalize armor type label
                    if (item is ArmorPiece) typeLabel = "ArmorPiece";

                    node["type"] = typeLabel;
                    data.InventoryItems.Add(node.ToJsonString());
                }
                catch
                {
                    // fallback: serialize simple representation
                    try
                    {
                        var fallback = new JsonObject { ["type"] = item.GetType().Name, ["name"] = item.Name };
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

            string json = JsonSerializer.Serialize(data, options);
            File.WriteAllText(path, json);

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
                catch
                {
                    // ignore individual item failures
                }
            }

            data.Inventory = inv;

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
        }

        // ---------------------------------------------------------
        // LIST ALL SAVE FILES
        // ---------------------------------------------------------
        public static List<string> GetSaveFiles()
        {
            if (!Directory.Exists(SaveFolder))
                return new List<string>();

            return Directory.GetFiles(SaveFolder, "*.json")
                            .Select(f => Path.GetFileNameWithoutExtension(f))
                            .ToList();
        }

        // ---------------------------------------------------------
        // FILTERED SAVE LISTS
        // ---------------------------------------------------------
        public static List<string> GetAutoSaves()
        {
            if (!Directory.Exists(SaveFolder))
                Directory.CreateDirectory(SaveFolder);
            return Directory.GetFiles(SaveFolder, "*.json")
                .Where(f => Path.GetFileName(f).StartsWith("AUTO -"))
                .OrderByDescending(f => File.GetCreationTime(f))
                .Take(15)
                .Select(f => Path.GetFileNameWithoutExtension(f))
                .ToList();
        }

        public static List<string> GetManualSaves()
        {
            if (!Directory.Exists(SaveFolder))
                Directory.CreateDirectory(SaveFolder);
            return Directory.GetFiles(SaveFolder, "*.json")
                .Where(f => !Path.GetFileName(f).StartsWith("AUTO -"))
                .OrderByDescending(f => File.GetCreationTime(f))
                .Select(f => Path.GetFileNameWithoutExtension(f))
                .ToList();
        }

        // ---------------------------------------------------------
        // DELETE OLD AUTOSAVES (KEEP 3 NEWEST)
        // ---------------------------------------------------------
        public static void DeleteOldAutoSavesForPlayer(string playerName)
        {
            var autoSaveFiles = Directory.GetFiles(SaveFolder, "*.json")
                .Where(f => Path.GetFileName(f).StartsWith("AUTO -"))
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