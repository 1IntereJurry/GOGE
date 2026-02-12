using GOGE.Models;
using GOGE.Utils;
using System;
using System.Linq;

namespace GOGE.Systems
{
    public class InventorySystem
    {
        private readonly List<Item> _items = new();

        public IReadOnlyList<Item> Items => _items;

        public void Add(Item item)
        {
            _items.Add(item);
            Console.WriteLine(Localization.TF("Inventory.Added", item.Name));
        }

        public bool Remove(Item item)
        {
            return _items.Remove(item);
        }

        public void Use(Item item, Character character)
        {
            if (item is Potion potion)
            {
                potion.ApplyEffect(character);
                Remove(item);
                Console.WriteLine(Localization.TF("Inventory.PotionUsed", potion.Name));
            }
            else
            {
                Console.WriteLine(Localization.TF("Inventory.UseFailed", item.Name)); 
            }
        }

        public void Equip(Item item, Character character)
        {
            character.EquipItem(item);
            Remove(item);
        }

        public void ShowInventory(Character character)
        {
            while (true)
            {
                Console.Clear();
                TextHelper.ShowTitleBanner();
                Console.WriteLine(Localization.T("Inventory.ShowTitle"));

                Console.WriteLine();
                Console.WriteLine(Localization.T("Inventory.Equipped"));

                var weaponName = character?.EquippedWeapon?.Name ?? "None";
                var chestName = character?.EquippedChestplate?.Name ?? "None";
                var pantsName = character?.EquippedPants?.Name ?? "None";
                var bootsName = character?.EquippedBoots?.Name ?? "None";
                var helmetName = character?.EquippedHelmet?.Name ?? "None";

                Console.WriteLine(Localization.TF("Inventory.Equipped.Weapon", weaponName));
                Console.WriteLine(Localization.TF("Inventory.Equipped.Chest", chestName));
                Console.WriteLine(Localization.TF("Inventory.Equipped.Pants", pantsName));
                Console.WriteLine(Localization.TF("Inventory.Equipped.Boots", bootsName));
                Console.WriteLine(Localization.TF("Inventory.Equipped.Helmet", helmetName));

                Console.WriteLine();

                if (_items.Count == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(Localization.T("Inventory.EmptyMessage"));
                    Console.ResetColor();
                    Pause();
                    return;
                }

                for (int i = 0; i < _items.Count; i++)
                {
                    var item = _items[i];
                    Console.WriteLine($"{i + 1}) [{item.Rarity}] {item.Name} - {item.Description}");
                }

                Console.WriteLine();
                Console.WriteLine("[U] Use item   [E] Equip item   [S] Sell item   [X] Exit");
                Console.Write("\n" + Localization.T("Menu.ChooseOption"));
                string input = (Console.ReadLine() ?? "").Trim();

                if (string.IsNullOrWhiteSpace(input))
                    continue;

                input = input.ToLower();

                if (input == "x")
                    return;

                // If single-letter command, prompt for index
                if (input == "u" || input == "e" || input == "s")
                {
                    Console.Write(Localization.T("Menu.ChooseOption"));
                    var idxInput = (Console.ReadLine() ?? "").Trim();
                    if (!int.TryParse(idxInput, out int idx) || idx < 1 || idx > _items.Count)
                    {
                        Console.WriteLine(Localization.T("Main.InvalidInput"));
                        Pause();
                        continue;
                    }

                    HandleCommand(input, idx, character);
                    Pause();
                    continue;
                }

                // Handle command with index: e.g. "u 1" or "e 2"
                var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 2)
                {
                    var cmd = parts[0];
                    if (!int.TryParse(parts[1], out int idx) || idx < 1 || idx > _items.Count)
                    {
                        Console.WriteLine(Localization.T("Main.InvalidInput"));
                        Pause();
                        continue;
                    }

                    HandleCommand(cmd, idx, character);
                    Pause();
                    continue;
                }

                Console.WriteLine(Localization.T("Main.InvalidInput"));
                Pause();
            }
        }

        private void HandleCommand(string cmd, int idx, Character character)
        {
            var selected = _items[idx - 1];

            switch (cmd)
            {
                case "u":
                    Use(selected, character);
                    break;

                case "e":
                    Equip(selected, character);
                    break;

                case "s":
                    int price = ShopSystem_GetPrice(selected) / 2;

                    Console.WriteLine(Localization.TF("Merchant.SellConfirm", selected.Name, price));
                    Console.Write(Localization.T("Menu.ChooseOption"));
                    var confirm = (Console.ReadLine() ?? "").Trim().ToLower();
                    if (confirm == "1" || confirm == "y" || confirm == "yes")
                    {
                        var duplicates = _items.Count(i => i.Name == selected.Name);
                        if (duplicates > 1)
                        {
                            Console.WriteLine(Localization.T("Merchant.SellOfferXP"));
                            Console.WriteLine("1) Sell for gold");
                            Console.WriteLine("2) Convert to XP (gain half price in XP)");
                            Console.Write(Localization.T("Menu.ChooseOption"));
                            var choice = (Console.ReadLine() ?? "").Trim();
                            if (choice == "2")
                            {
                                int xp = price / 2;
                                character.AddXP(xp);
                                Remove(selected);
                                Console.WriteLine(Localization.TF("Merchant.SoldForXP", selected.Name, xp));
                                return;
                            }
                        }

                        Remove(selected);
                        character.Gold += price;
                        Console.WriteLine(Localization.TF("Merchant.Sold", selected.Name, price));
                    Pause();
                    continue;
                    }
            }
        }

        private void Pause()
        {
            Console.WriteLine("\n" + Localization.T("Pause.PressEnter"));
            Console.ReadLine();
        }

        // small adapter to reuse shop pricing logic
        private int ShopSystem_GetPrice(Item item)
        {
            // replicate ShopSystem.GetPrice logic (kept simple)
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