using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using GOGE.Models;

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
            public InventorySystem Inventory { get; set; } = new InventorySystem();
            public string? Location { get; set; }
            public DateTime SaveTime { get; set; } = DateTime.Now;
        }

        // ---------------------------------------------------------
        // SAVE GAME
        // ---------------------------------------------------------
        public static void SaveGame(Character player, InventorySystem inventory)
        {
            if (!Directory.Exists(SaveFolder))
                Directory.CreateDirectory(SaveFolder);

            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string rawName = $"{player.Name} ({player.Class}) - {timestamp}";
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

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                // Optional: add converters for polymorphic Item handling later
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            string json = JsonSerializer.Serialize(data, options);
            File.WriteAllText(path, json);

            Console.WriteLine($"Game saved as:\n{safeName}.json");
        }

        // ---------------------------------------------------------
        // LOAD GAME
        // ---------------------------------------------------------
        public static SaveData? LoadGame(string fileName)
        {
            string path = Path.Combine(SaveFolder, $"{fileName}.json");

            if (!File.Exists(path))
            {
                Console.WriteLine("This save file does not exist.");
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
                Console.WriteLine($"Error deserializing save file: {ex.Message}");
                return null;
            }

            if (data == null)
            {
                Console.WriteLine("Error loading save file.");
                return null;
            }

            // MIGRATION
            UpgradeSaveData(data);

            // Ensure Inventory is not null (System.Text.Json may create an empty InventorySystem)
            data.Inventory ??= new InventorySystem();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nSave file '{fileName}' loaded!");
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
                // example migration
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

        public static bool DeleteSave(string fileName)
        {
            string path = Path.Combine(SaveFolder, $"{fileName}.json");

            if (!File.Exists(path))
                return false;

            File.Delete(path);
            return true;
        }

        // Helper: remove invalid filename chars
        private static string SanitizeFileName(string name)
        {
            var invalid = Path.GetInvalidFileNameChars();
            var cleaned = new string(name.Select(ch => invalid.Contains(ch) ? '-' : ch).ToArray());
            return cleaned;
        }
    }
}