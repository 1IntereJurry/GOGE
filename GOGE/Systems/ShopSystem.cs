using GOGE.Models;
using GOGE.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GOGE.Systems
{
    public static class ShopSystem
    {
        private static readonly Random rng = new();

        public static void ShowMerchant(Character player, InventorySystem inventory)
        {
            Console.Clear();
            TextHelper.ShowTitleBanner();
            Console.WriteLine(Localization.T("Merchant.Greeting"));
            Console.WriteLine();

            int count = rng.Next(3, 6);
            var shopItems = new List<Item>();
            for (int i = 0; i < count; i++)
                shopItems.Add(LootTable.GetRandomLoot());

            while (true)
            {
                Console.WriteLine(Localization.T("Merchant.Offer"));
                Console.WriteLine();

                for (int i = 0; i < shopItems.Count; i++)
                {
                    var it = shopItems[i];
                    var price = GetPrice(it);

                    // determine type/slot label (for armor show slot name)
                    string typeLabel;
                    if (it is ArmorPiece ap)
                        typeLabel = ap.Slot.ToString();
                    else
                        typeLabel = it.GetType().Name;

                    // print with slot/type in dark gray
                    var line = $"{i + 1}) [{it.Rarity}] {it.Name}";
                    if (!string.IsNullOrWhiteSpace(it.Description))
                        line += $" - {it.Description}";
                    line += $" ({Localization.TF("Merchant.Price", price)})";

                    Console.Write(line);
                    // pad then show type/slot in dark gray
                    var old = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write($"  ({typeLabel})");
                    Console.ForegroundColor = old;

                    Console.WriteLine();
                }

                Console.WriteLine();
                Console.WriteLine("[B] " + Localization.T("Merchant.BuyXPOption"));
                Console.WriteLine("[S] " + Localization.T("Merchant.SellOption"));
                Console.WriteLine("[X] " + Localization.T("Merchant.Exit"));
                Console.Write("\n" + Localization.T("Menu.ChooseOption"));

                string input = Console.ReadLine()?.Trim().ToLower() ?? "";

                if (input == "x")
                {
                    Console.WriteLine(Localization.T("Merchant.Goodbye"));
                    break;
                }

                if (input == "b")
                {
                    int xpCost = 50;
                    int goldCost = Math.Max(1, player.Level * 10);
                    Console.WriteLine(Localization.TF("Merchant.BuyXPConfirm", xpCost, goldCost));
                    Console.Write(Localization.T("Menu.ChooseOption"));
                    var confirm = Console.ReadLine()?.Trim().ToLower();
                    if (confirm == "1" || confirm == "y" || confirm == "yes")
                    {
                        if (player.Gold >= goldCost)
                        {
                            player.Gold -= goldCost;
                            player.AddXP(xpCost);
                            Console.WriteLine(Localization.TF("Merchant.XPBought", xpCost, goldCost));
                        }
                        else
                        {
                            Console.WriteLine(Localization.T("Merchant.NotEnoughGold"));
                        }
                    }
                    Pause();
                    Console.Clear();
                    TextHelper.ShowTitleBanner();
                    continue;
                }

                if (input == "s")
                {
                    if (inventory.Items.Count == 0)
                    {
                        Console.WriteLine(Localization.T("Inventory.EmptyMessage"));
                        Pause();
                        Console.Clear();
                        TextHelper.ShowTitleBanner();
                        continue;
                    }

                    Console.WriteLine(Localization.T("Merchant.SellList"));
                    for (int i = 0; i < inventory.Items.Count; i++)
                    {
                        var it = inventory.Items[i];
                        var sellPrice = GetPrice(it) / 2;
                        Console.WriteLine($"{i + 1}) [{it.Rarity}] {it.Name} - {Localization.TF("Merchant.Price", sellPrice)}");
                    }

                    Console.Write("\n" + Localization.T("Menu.ChooseOption"));
                    if (!int.TryParse(Console.ReadLine(), out int sellIdx) || sellIdx < 1 || sellIdx > inventory.Items.Count)
                    {
                        Console.WriteLine(Localization.T("Main.InvalidInput"));
                        Pause();
                        Console.Clear();
                        TextHelper.ShowTitleBanner();
                        continue;
                    }

                    var toSell = inventory.Items[sellIdx - 1];
                    int amount = GetPrice(toSell) / 2;
                    inventory.Remove(toSell);
                    player.Gold += amount;
                    Console.WriteLine(Localization.TF("Merchant.Sold", toSell.Name, amount));
                    Pause();
                    Console.Clear();
                    TextHelper.ShowTitleBanner();
                    continue;
                }

                if (int.TryParse(input, out int idx) && idx >= 1 && idx <= shopItems.Count)
                {
                    var chosen = shopItems[idx - 1];
                    int price = GetPrice(chosen);

                    if (player.Gold < price)
                    {
                        Console.WriteLine(Localization.T("Merchant.NotEnoughGold"));
                        Pause();
                        Console.Clear();
                        TextHelper.ShowTitleBanner();
                        continue;
                    }

                    player.Gold -= price;
                    inventory.Add(CloneItem(chosen));
                    Console.WriteLine(Localization.TF("Merchant.Purchased", chosen.Name, price));
                    shopItems.RemoveAt(idx - 1);

                    if (shopItems.Count == 0)
                    {
                        int restock = rng.Next(1, 4);
                        for (int i = 0; i < restock; i++)
                            shopItems.Add(LootTable.GetRandomLoot());
                    }

                    Pause();
                    Console.Clear();
                    TextHelper.ShowTitleBanner();
                    continue;
                }

                Console.WriteLine(Localization.T("Main.InvalidInput"));
                Pause();
                Console.Clear();
                TextHelper.ShowTitleBanner();
            }
        }

        private static void Pause()
        {
            Console.WriteLine("\n" + Localization.T("Pause.PressEnter"));
            Console.ReadLine();
        }

        private static Item CloneItem(Item proto)
        {
            return proto switch
            {
                Weapon w => new Weapon(w.Name, w.Damage, w.Rarity) { Description = w.Description, CritChance = w.CritChance },
                ArmorPiece a => new ArmorPiece(a.Name, a.Armor, a.Rarity) { Description = a.Description, Strength = a.Strength, Agility = a.Agility, Vitality = a.Vitality, Slot = a.Slot },
                Potion p => new Potion(p.Name, p.HealAmount, p.HealPercent) { Description = p.Description, Effect = p.Effect },
                _ => new Gold(0) as Item ?? proto
            };
        }

        private static int GetPrice(Item item)
        {
            var rarityMultiplier = item.Rarity?.ToLower() switch
            {
                "common" => 10,
                "uncommon" => 25,
                "rare" => 75,
                "epic" => 200,
                "legendary" => 500,
                _ => 10
            };

            return item switch
            {
                Weapon w => Math.Max(1, w.Damage * rarityMultiplier / 1),
                ArmorPiece a => Math.Max(1, a.Armor * (rarityMultiplier / 1)),
                Potion p => p.HealAmount > 0 ? Math.Max(1, p.HealAmount / 2 + rarityMultiplier / 2) : rarityMultiplier,
                Gold g => Math.Max(1, g.Amount),
                _ => rarityMultiplier
            };
        }
    }
}
