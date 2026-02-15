using GOGE.Models;
using GOGE.Utils;

namespace GOGE.Systems
{
    public enum FightOutcome
    {
        Victory,
        Defeat,
        Escaped
    }

    public enum RunResult
    {
        Escaped,
        Failed,
        Stumble
    }

    public static class CombatSystem
    {
        public static FightOutcome StartFight(Character player, Enemy enemy, InventorySystem inventory)
        {
            Console.Clear();
            bool escaped = false;

            while (player.CurrentHP > 0 && enemy.CurrentHP > 0)
            {
                TextHelper.ShowTitleBanner();
                Console.WriteLine(Localization.TF("Combat.NewEnemy", enemy.Name));

                Console.WriteLine($"\n{player.Name}: {player.CurrentHP}/{player.MaxHP} HP");
                Console.WriteLine($"{enemy.Name}: {enemy.CurrentHP}/{enemy.MaxHP} HP\n");

                Console.WriteLine(Localization.T("Combat.Option.Attack"));
                Console.WriteLine(Localization.T("Combat.Option.Potion"));
                Console.WriteLine(Localization.T("Combat.Option.Run"));
                Console.Write("\n" + Localization.T("Menu.ChooseOption"));

                string input = Console.ReadLine()?.Trim().ToLower();

                Console.WriteLine();

                switch (input)
                {
                    case "1":
                        PlayerAttack(player, enemy);
                        // only let enemy attack if still alive
                        if (enemy.CurrentHP > 0)
                            EnemyAttack(player, enemy);
                        break;

                    case "2":
                        UsePotion(player, inventory);
                        // enemy may attack after player uses potion
                        if (enemy.CurrentHP > 0)
                            EnemyAttack(player, enemy);
                        break;

                    case "3":
                        // attempt to run with three outcome probabilities
                        var runResult = TryToRun();

                        if (runResult == RunResult.Escaped)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine(Localization.T("Combat.Escape.Success"));
                            Console.ResetColor();

                            Pause();
                            Console.Clear();
                            escaped = true;
                        }
                        else if (runResult == RunResult.Failed)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine(Localization.T("Combat.Escape.Fail"));
                            Console.ResetColor();

                            // on failed run the enemy gets a free attack
                            if (enemy.CurrentHP > 0)
                                EnemyAttack(player, enemy);

                            Pause();
                        }
                        else // Stumble and fall death scenario
                        {
                            // special outcome: player falls asleep (instant defeat) for testing
                            Console.ForegroundColor = ConsoleColor.DarkMagenta;
                            Console.WriteLine("");
                            Console.ResetColor();

                            // set HP to 0 to trigger defeat handling
                            player.CurrentHP = 0;

                            Pause();
                        }

                        break;

                    default:
                        Console.WriteLine(Localization.T("Combat.InvalidInput"));
                        Pause();
                        break;
                }

                if (escaped)
                    break;

                Console.Clear();
            }

            if (escaped)
            {
                // successful escape - do not award loot
                return FightOutcome.Escaped;
            }

            return EndFight(player, enemy, inventory);
        }

        // ---------------------------------------------------------
        // Player Attack
        // ---------------------------------------------------------
        private static void PlayerAttack(Character player, Enemy enemy)
        {
            int dmg = player.GetAttackDamage();
            enemy.CurrentHP -= dmg;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(Localization.TF("Combat.Hit", enemy.Name, dmg));
            Console.ResetColor();
        }

        // ---------------------------------------------------------
        // Enemy Attack
        // ---------------------------------------------------------
        private static void EnemyAttack(Character player, Enemy enemy)
        {
            int dmg = enemy.GetDamage();

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(Localization.TF("Combat.EnemyHit", enemy.Name, dmg));
            Console.ResetColor();

            player.TakeDamage(dmg);
            Pause();
        }

        // ---------------------------------------------------------
        // Run Away
        // ---------------------------------------------------------
        private static RunResult TryToRun()
        {
            int roll = new Random().Next(1, 101); // 1..100

            if (roll <= 65)
                return RunResult.Escaped; 

            if (roll <= 90)
                return RunResult.Failed;

            return RunResult.Stumble;
        }

        // ---------------------------------------------------------
        // Use Potion
        // ---------------------------------------------------------
        private static void UsePotion(Character player, InventorySystem inventory)
        {
            var potion = inventory.Items.OfType<Potion>().FirstOrDefault();

            if (potion == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(Localization.T("Inventory.NoPotions"));
                Console.ResetColor();
                Pause();
            }
            else
            {
                // Use the potion's actual effect
                inventory.Use(potion, player);
                Pause();
            }
        }

        // ---------------------------------------------------------
        // End of Fight
        // ---------------------------------------------------------
        private static FightOutcome EndFight(Character player, Enemy enemy, InventorySystem inventory)
        {
            Console.Clear();
            TextHelper.ShowTitleBanner();

            if (player.CurrentHP <= 0)
            {
                Console.WriteLine(Localization.TF("Character.Defeated", player.Name));
                return FightOutcome.Defeat;
            }

            Console.WriteLine(Localization.TF("Combat.EnemyDefeat", enemy.Name));

            player.AddXP(enemy.XPReward);
            player.Gold += enemy.GoldReward;

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(Localization.TF("Combat.EnemyLooted", enemy.XPReward, enemy.GoldReward));
            Console.ResetColor();

            // Add loot to inventory (skip Gold items)
            if (enemy.LootTable.Count > 0)
            {
                var nonGold = enemy.LootTable.Where(l => !(l is Gold)).ToList();
                var goldItems = enemy.LootTable.OfType<Gold>().ToList();

                // directly credit gold items
                foreach (var g in goldItems)
                {
                    player.Gold += g.Amount; // note: GoldReward already added above for base gold, this is additional gold drops
                }

                if (nonGold.Count > 0)
                {
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(Localization.T("Combat.LootObtained"));
                    Console.ResetColor();

                    foreach (var loot in nonGold)
                    {
                        inventory.Add(loot);
                    }
                }
            }

            Pause();
            Console.Clear();

            SaveSystem.SaveGame(player, inventory, isAutoSave: true);
            return FightOutcome.Victory;
        }

        private static void Pause()
        {
            Console.WriteLine("\n" + Localization.T("Pause.PressEnter"));
            Console.ReadLine();
        }
    }
}