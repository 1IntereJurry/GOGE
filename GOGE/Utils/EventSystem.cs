using System;
using GOGE.Models;
using GOGE.Utils;
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

            if (roll <= 30)
            {
                // Loot-Event
                Console.WriteLine("You find a small chest and receive 10 gold!");
                player.Gold += 10;
            }
            else if (roll <= 50)
            {
                // Story-Event
                Console.WriteLine("You hear a distant howl... the air is getting colder.");
            }
            else if (roll <= 70)
            {
                // Risiko-Event
                Console.WriteLine("You find a sparkling puddle. Do you drink it?");
                Console.WriteLine("\n1. Yes");
                Console.WriteLine("2. No");

                Console.Write("\nChoice: ");
                string choice = Console.ReadLine() ?? "";

                if (choice == "1")
                {
                    if (rng.Next(1, 101) <= 50)
                    {
                        Console.WriteLine("You feel refreshed! +10 HP");
                        player.CurrentHP += 10;
                    }
                    else
                    {
                        Console.WriteLine("You'll feel sick... -10 HP");
                        player.CurrentHP -= 10;
                    }
                }
                else
                {
                    Console.WriteLine("You leave the puddle alone.");
                }
            }
            else if (roll <= 90)
            {
                // Kampf-Event
                Console.WriteLine("An elite opponent lurks in the bushes!");
                Enemy elite = EnemyFactory.CreateEnemy(player.Level + 1);
                elite.Type = EnemyType.Elite;
                CombatSystem.StartFight(player, elite, inventory);
            }
            else
            {
                // Dungeon-Event
                Console.WriteLine("The ground shakes... a dungeon opens!");
                dungeonAvailable = true;
            }

            Console.WriteLine("\nContinue using ENTER...");
            Console.ReadLine();
        }
    }
}