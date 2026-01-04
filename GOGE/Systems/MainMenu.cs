using GOGE.Models;
using GOGE.Utils;

namespace GOGE.Systems
{
    public static class MainMenu
    {
        public static (Character player, InventorySystem inventory) Show()
        {
            while (true)
            {
                TextHelper.ShowTitleScreen();
                string input = Console.ReadLine()?.Trim().ToLower();

                switch (input)
                {
                    case "1":
                        return CreateNewGame();

                    case "2":
                        return LoadExistingGame();

                    case "3":
                        TextHelper.ShowCredits();
                        break;

                    case "4":
                        ChooseLanguage();
                        break;

                    case "x":
                        Environment.Exit(0);
                        break;

                    default:
                        Console.WriteLine(Localization.T("Main.InvalidInput"));
                        Pause();
                        break;
                }
            }
        }

        private static void ChooseLanguage()
        {
            Console.Clear();
            TextHelper.ShowTitleBanner();

            Console.WriteLine();
            Console.WriteLine(Localization.T("Language.Choose"));
            Console.WriteLine(Localization.T("Language.English"));
            Console.WriteLine(Localization.T("Language.German"));
            Console.Write(Localization.T("Menu.ChooseOption"));

            string input = Console.ReadLine()?.Trim().ToLower();

            switch (input)
            {
                case "1":
                case "english":
                case "e":
                    Localization.Set(Language.English);
                    Console.WriteLine(Localization.TF("Language.Changed", Localization.T("Language.Name.English")));
                    Pause();
                    break;

                case "2":
                case "german":
                case "d":
                    Localization.Set(Language.German);
                    Console.WriteLine(Localization.TF("Language.Changed", Localization.T("Language.Name.German")));
                    Pause();
                    break;

                default:
                    Console.WriteLine(Localization.T("Main.InvalidInput"));
                    Pause();
                    break;
            }
        }

        private static (Character?, InventorySystem?) CreateNewGame()
        {
            Console.Clear();
            TextHelper.ShowTitleBanner();

            // create character - name input (required)
            string name;
            while (true)
            {
                Console.Write(Localization.T("Input.Name.Prompt"));
                name = Console.ReadLine()?.Trim() ?? string.Empty;

                if (!string.IsNullOrWhiteSpace(name))
                    break;

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(Localization.T("Input.Name.Empty"));
                Console.ResetColor();
                Thread.Sleep(750);
                Console.Clear();
                TextHelper.ShowTitleBanner();
            }

            // Class selection (required)
            string[] classes = new[] { "Knight", "Mage", "Rogue", "Berserker" };
            string charClass = string.Empty;

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine(Localization.T("Input.Class.Choose"));
                for (int i = 0; i < classes.Length; i++)
                {
                    Console.WriteLine($"{i + 1}) {classes[i]}");
                }

                Console.Write(Localization.T("Input.Class.ChoicePrompt"));
                string input = Console.ReadLine()?.Trim() ?? string.Empty;

                if (int.TryParse(input, out int idx) && idx >= 1 && idx <= classes.Length)
                {
                    charClass = classes[idx - 1];
                    break;
                }

                // Try match by name (case-insensitive)
                foreach (var c in classes)
                {
                    if (string.Equals(c, input, StringComparison.OrdinalIgnoreCase))
                    {
                        charClass = c;
                        break;
                    }
                }

                if (!string.IsNullOrEmpty(charClass))
                    break;

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(Localization.T("Input.Class.Invalid"));
                Console.ResetColor();
                Thread.Sleep(750);
                Console.Clear();

                TextHelper.ShowTitleBanner();
                Console.Write(Localization.TF("Character.Label.Name", name));
                Console.WriteLine();
            }

            Character player = new Character(name, charClass);
            InventorySystem inventory = new InventorySystem();

            Console.WriteLine();
            Console.WriteLine(Localization.T("Character.Created"));
            Pause();

            return (player, inventory);
        }

        private static (Character?, InventorySystem?) LoadExistingGame()
        {
            while (true)
            {
                Console.Clear();
                TextHelper.ShowTitleBanner();

                var manualSaves = SaveSystem.GetManualSaves();

                if (manualSaves.Count == 0)
                {
                    Console.WriteLine(Localization.T("Load.NoSaves"));
                    Pause();
                    return (null, null);
                }

                Console.WriteLine(Localization.T("Main.Load.Title"));
                for (int i = 0; i < manualSaves.Count; i++)
                    Console.WriteLine($"{i + 1}) {manualSaves[i]}");

                Console.WriteLine();
                Console.WriteLine("[A] " + Localization.T("Main.Load.ShowAutosaves"));
                Console.WriteLine("[Q] " + Localization.T("Main.Load.Back"));

                Console.Write("\n" + Localization.T("Input.Class.ChoicePrompt"));
                var key = Console.ReadKey(true).Key;

                // Zurück
                if (key == ConsoleKey.Q)
                    return (null, null);

                // Autosave-Menü
                if (key == ConsoleKey.A)
                {
                    var (player, inventory) = ShowAutoSaveMenu(null);

                    if (player != null)
                        return (player, inventory);

                    continue;
                }

                // Manuelle Saves laden
                if (char.IsDigit((char)key))
                {
                    int index = (int)char.GetNumericValue((char)key) - 1;

                    if (index >= 0 && index < manualSaves.Count)
                    {
                        var save = SaveSystem.LoadGame(manualSaves[index]);
                        if (save == null)
                        {
                            Console.WriteLine(Localization.T("Save.NotFound"));
                            Pause();
                            return (null, null);
                        }

                        Pause();
                        return (save.Player, save.Inventory);
                    }
                }

                Console.WriteLine(Localization.T("Load.InvalidSelection"));
                Pause();
            }
        }

        private static (Character?, InventorySystem?) ShowAutoSaveMenu(Character? currentPlayer)
        {
            while (true)
            {
                Console.Clear();
                TextHelper.ShowTitleBanner();

                var autoSaves = SaveSystem.GetAutoSaves();

                Console.WriteLine("=== AUTOSAVES ===");

                if (autoSaves.Count == 0)
                {
                    Console.WriteLine(Localization.T("Save.NoAutoSaves"));
                }
                else
                {
                    for (int i = 0; i < autoSaves.Count; i++)
                        Console.WriteLine($"{i + 1}) {autoSaves[i]}");
                }

                Console.WriteLine();
                Console.WriteLine("[M] Manuelle Saves anzeigen");
                Console.WriteLine("[D] Alte Autosaves löschen");
                Console.WriteLine("[Q] Zurück");

                Console.Write("\n" + Localization.T("Input.Class.ChoicePrompt"));
                var key = Console.ReadKey(true).Key;

                // back
                if (key == ConsoleKey.Q)
                    return (null, null);

                // back to manual saves
                if (key == ConsoleKey.M)
                    return (null, null);

                // delete old autosaves
                if (key == ConsoleKey.D)
                {
                    if (currentPlayer == null)
                    {
                        Console.WriteLine("Kein Spieler geladen – kann Autosaves nicht filtern.");
                        Pause();
                        continue;
                    }

                    Console.WriteLine(Localization.T("Save.CleaningAutosaves"));
                    SaveSystem.DeleteOldAutoSavesForPlayer(currentPlayer.Name);
                    Console.WriteLine(Localization.T("Save.CleaningDone"));
                    Pause();
                    continue;
                }

                // Autosave laden
                if (char.IsDigit((char)key))
                {
                    int index = (int)char.GetNumericValue((char)key) - 1;

                    if (index >= 0 && index < autoSaves.Count)
                    {
                        var save = SaveSystem.LoadGame(autoSaves[index]);
                        if (save == null)
                        {
                            Console.WriteLine(Localization.T("Save.NotFound"));
                            Pause();
                            return (null, null);
                        }

                        Pause();
                        return (save.Player, save.Inventory);
                    }
                }

                Console.WriteLine(Localization.T("Load.InvalidSelection"));
                Pause();
            }
        }

        private static void Pause()
        {
            Console.WriteLine("\n" + Localization.T("Pause.PressEnter"));
            Console.ReadLine();
        }
    }
}