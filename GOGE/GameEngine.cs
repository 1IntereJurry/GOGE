using GOGE.Models;
using GOGE.Systems;
using GOGE.Utils;
using System.Numerics;

namespace GOGE
{
    public class GameEngine
    {
        private bool _dungeonAvailable = false;
        private readonly Random rng = new();

        private InventorySystem _inventory;
        private Character _player;

        public static Character? CurrentPlayer { get; private set; }
        public static InventorySystem? CurrentInventory { get; private set; }
        public GameEngine(Character player, InventorySystem inventory)
        {
            _player = player;
            _inventory = inventory;
        }

        public bool StartGame()
        {
            while (true)
            {
                // If player is dead, show death screen and go back to title or quit
                if (_player.CurrentHP <= 0)
                {
                    ShowDeathScreen();
                    return true;
                }

                Console.Clear();
                TextHelper.ShowTitleBanner();
                Console.WriteLine(Localization.T("Menu.MainTitle"));
                Console.WriteLine(Localization.TF("Character.Label.Name", _player.Name) + $" (Level {_player.Level})  " + Localization.TF("Character.Label.Gold", _player.Gold));
                Console.WriteLine();
                Console.WriteLine(Localization.T("Menu.Fight"));

                if (_dungeonAvailable)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(Localization.T("Menu.EnterDungeon"));
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine(Localization.T("Menu.EnterDungeonUnavailable"));
                }
                Console.ResetColor();

                Console.WriteLine(Localization.T("Menu.ViewInventory"));
                Console.WriteLine(Localization.T("Menu.SaveGame"));
                Console.WriteLine(Localization.T("Menu.QuitToTitle"));

                Console.Write("\n" + Localization.T("Menu.ChooseOption"));
                string choice = Console.ReadLine() ?? "";

                switch (choice)
                {
                    case "1":
                        StartNormalFight();
                        // if player died during the fight, show death screen and return to title
                        if (_player.CurrentHP <= 0)
                        {
                            ShowDeathScreen();
                            return true;
                        }
                        break;

                    case "2":
                        HandleDungeonSelection();
                        // if player died in dungeon, ShowDeathScreen will have been called and Environment may have exited
                        if (_player.CurrentHP <= 0)
                        {
                            ShowDeathScreen();
                            return true;
                        }
                        break;

                    case "3":
                        _inventory.ShowInventory(_player);
                        break;

                    case "4":
                        SaveGame();
                        break;

                    case "5":
                        return true;

                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(Localization.T("Main.InvalidInput"));
                        Console.ResetColor();
                        Pause();
                        break;
                }
            }
        }

        // Show death screen and ask user to return to Main Menu or Quit. If Quit chosen, exit process.
        private void ShowDeathScreen()
        {
            Console.Clear();
            TextHelper.ShowTitleBanner();

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(Localization.TF("Character.Defeated", _player.Name));
            Console.ResetColor();

            Console.WriteLine();
            _player.ShowStats();

            Console.WriteLine();
            Console.WriteLine("[M] Main Menu    [Q] Quit");
            Console.Write(Localization.T("Menu.ChooseOption"));
            var input = (Console.ReadLine() ?? "").Trim().ToLower();

            if (input == "q")
            {
                Console.WriteLine("Goodbye...");
                Thread.Sleep(500);
                Environment.Exit(0);
            }

            // otherwise return to main menu by returning from StartGame
        }

        // ---------------------------------------------------------
        // NORMAL FIGHT / EXPLORATION
        // ---------------------------------------------------------
        private void StartNormalFight()
        {
            Console.Clear();
            TextHelper.ShowTitleBanner();
            Console.WriteLine(Localization.T("Game.YouHeadOut"));
            Pause();

            int roll = rng.Next(1, 101);

            if (roll <= 20)
            {
                EventSystem.TriggerEvent(_player, _inventory, ref _dungeonAvailable);
                return;
            }

            Enemy enemy = EnemyFactory.CreateEnemy(_player.Level);
            Console.WriteLine($"A {enemy.Name} appears!"); //outsorce localization

            var outcome = CombatSystem.StartFight(_player, enemy, _inventory);

            if (outcome == FightOutcome.Defeat)
            {
                // CombatSystem already handles defeat messaging and autosave; just return so StartGame can react
                return;
            }

            if (rng.Next(1, 101) <= 10)
            {
                _dungeonAvailable = true;
                Console.WriteLine("\n" + Localization.T("Game.DungeonOpened"));
                Pause();
            }
        }

        // ---------------------------------------------------------
        // DUNGEON
        // ---------------------------------------------------------
        private void HandleDungeonSelection()
        {
            if (!_dungeonAvailable)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(Localization.T("Game.DungeonUnavailable"));
                Console.ResetColor();
                Pause();
                return;
            }

            StartDungeon();
            _dungeonAvailable = false;
        }

        private void StartDungeon()
        {
            Console.Clear();
            Console.WriteLine(Localization.T("Game.DungeonOpened"));

            for (int i = 1; i <= 3; i++)
            {
                Console.WriteLine(Localization.TF("Game.DungeonRoom", i));

                Enemy enemy = EnemyFactory.CreateEnemy(_player.Level, isDungeon: true);
                var outcome = CombatSystem.StartFight(_player, enemy, _inventory);

                if (outcome == FightOutcome.Defeat)
                {
                    // CombatSystem printed defeat; stop dungeon
                    Pause();
                    return;
                }

                if (outcome == FightOutcome.Escaped)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(Localization.T("Combat.Escape.Success"));
                    Console.ResetColor();
                    Pause();
                    return;
                }
            }

            Console.WriteLine(Localization.T("Game.DungeonBossRoom"));
            Enemy boss = EnemyFactory.CreateEnemy(_player.Level, isDungeon: true, isBoss: true);
            var bossOutcome = CombatSystem.StartFight(_player, boss, _inventory);

            if (bossOutcome == FightOutcome.Defeat)
            {
                Pause();
                return;
            }

            if (bossOutcome == FightOutcome.Escaped)
            {
                Console.WriteLine(Localization.T("Combat.Escape.Success"));
                Pause();
                return;
            }

            Console.WriteLine(Localization.T("Game.DungeonCleared"));
            SaveSystem.SaveGame(_player, _inventory, isAutoSave: true);
            Pause();
        }

        // ---------------------------------------------------------
        // SAVE / LOAD
        // ---------------------------------------------------------
        private void SaveGame()
        {
            Console.Clear();
            SaveSystem.SaveGame(_player, _inventory);
            Pause();
        }

        private void LoadGame()
        {
            Console.Clear();
            var files = SaveSystem.GetSaveFiles();

            if (files.Count == 0)
            {
                Console.WriteLine(Localization.T("Load.NoSaves"));
                Pause();
                return;
            }

            Console.WriteLine("Available Saves:"); // outsorce localization
            for (int i = 0; i < files.Count; i++)
                Console.WriteLine($"{i + 1}. {files[i]}");

            Console.Write("\n" + Localization.T("Input.Class.ChoicePrompt"));
            if (!int.TryParse(Console.ReadLine(), out int index) ||
                index < 1 || index > files.Count)
            {
                Console.WriteLine(Localization.T("Load.InvalidSelection"));
                Pause();
                return;
            }

            string selected = files[index - 1];

            var save = SaveSystem.LoadGame(selected);
            if (save == null)
            {
                Console.WriteLine(Localization.T("Save.NotFound"));
                Pause();
                return;
            }

            _player = save.Player;
            _inventory = save.Inventory;

            Console.WriteLine(Localization.TF("Save.Loaded", selected));
            Pause();
        }

        // ---------------------------------------------------------
        // UTILITY
        // ---------------------------------------------------------
        private void Pause()
        {
            Console.WriteLine("\n" + Localization.T("Pause.PressEnter"));
            Console.ReadLine();
        }
    }
}