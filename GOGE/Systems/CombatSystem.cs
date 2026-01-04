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

    public static class CombatSystem
    {
        public static FightOutcome StartFight(Character player, Enemy enemy, InventorySystem inventory)
        {
            Console.Clear();

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
                        EnemyAttack(player, enemy);
                        break;

                    case "2":
                        UsePotion(player, inventory);
                        break;

                    case "3":
                        if (TryToRun())
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine(Localization.T("Combat.Escape.Success"));
                            Console.ResetColor();

                            Pause();
                            Console.Clear();
                            return FightOutcome.Escaped;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine(Localization.T("Combat.Escape.Fail"));
                            Console.ResetColor();
                            Pause();
                        }
                        break;

                    default:
                        Console.WriteLine(Localization.T("Combat.InvalidInput"));
                        Pause();
                        break;
                }

                Console.Clear();
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
        private static bool TryToRun()
        {
            return new Random().Next(0, 100) < 50;
        }

        // ---------------------------------------------------------
        // Use Potion
        // ---------------------------------------------------------
        private static void UsePotion(Character player, InventorySystem inventory)
        {
            var potion = inventory.Items.FirstOrDefault(i => i.Name.ToLower().Contains("potion"));

            if (potion == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(Localization.T("Inventory.NoPotions"));
                Console.ResetColor();
                Pause();
            }
            else
            {
                player.CurrentHP = Math.Min(player.MaxHP, player.CurrentHP + 30);
                inventory.Remove(potion);
                Console.WriteLine(Localization.T("Inventory.Added"));
                Console.WriteLine(Localization.T("Character.Created"));
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
            Console.WriteLine(Localization.TF("Combat.EnemyLooted", enemy.XPReward, enemy.GoldReward)); // message recieved exp & gold
            Console.ResetColor();
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