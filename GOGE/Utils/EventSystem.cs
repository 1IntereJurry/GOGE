using GOGE.Models;
using GOGE.Systems;

namespace GOGE.Utils
{
    public static class EventSystem
    {
        private static readonly Random rng = new();

        public static void TriggerEvent(Character player, InventorySystem inventory, ref bool dungeonAvailable)
        {
            Console.Clear();
            int roll = rng.Next(1, 101);

            if (roll <= 25)
            {
                // Loot-Event
                Console.WriteLine(Localization.TF("Event.SmallGoldChest", 10));
                player.Gold += 10;
            }
            else if (roll <= 45)
            {
                // Story-Event
                Console.WriteLine(Localization.T("Event.DistantHowl"));
            }
            else if (roll <= 65)
            {
                // Merchant-Event
                ShopSystem.ShowMerchant(player, inventory);
            }
            else if (roll <= 80)
            {
                // Risiko-Event
                Console.WriteLine(Localization.T("Event.SparklingPuddle.Prompt"));
                Console.WriteLine("\n" + Localization.T("Event.Choice.Yes"));
                Console.WriteLine(Localization.T("Event.Choice.No"));

                Console.Write("\n" + Localization.T("Menu.ChooseOption"));
                string choice = Console.ReadLine() ?? "";

                if (choice == "1")
                {
                    if (rng.Next(1, 101) <= 50)
                    {
                        Console.WriteLine(Localization.TF("Event.SparklingPuddle.Success", 10));
                        player.CurrentHP += 10;
                    }
                    else
                    {
                        Console.WriteLine(Localization.TF("Event.SparklingPuddle.Failure", 10));
                        player.CurrentHP -= 10;
                    }
                }
                else
                {
                    Console.WriteLine(Localization.T("Event.SparklingPuddle.Leave"));
                }
            }
            else if (roll <= 95)
            {
                // Kampf-Event
                Console.WriteLine(Localization.T("Event.EliteOpponent"));
                Enemy elite = EnemyFactory.CreateEnemy(player.Level + 1);
                elite.Type = EnemyType.Elite;
                CombatSystem.StartFight(player, elite, inventory);
            }
            else
            {
                // Dungeon-Event
                Console.WriteLine(Localization.T("Event.DungeonOpens"));
                dungeonAvailable = true;
            }

            Console.WriteLine("\n" + Localization.T("Pause.PressEnter"));
            Console.ReadLine();
        }
    }
}