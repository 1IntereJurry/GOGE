using GOGE.Models;
using GOGE.Utils;

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

                // Header
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("=== Inventory ===\n");
                Console.ResetColor();

                // Show name with level and current/needed XP
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(Localization.TF("Character.Label.Name", character.Name) + $" (Lv {character.Level})  XP: {character.XP}/{character.XPToNextLevel}");
                Console.ResetColor();

                Console.WriteLine();

                // Equipped
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine(Localization.T("Inventory.Equipped"));
                Console.ResetColor();

                // Helper to write equipped item with rarity color
                void WriteEquipped(string label, Item? it)
                {
                    if (it == null)
                    {
                        Console.WriteLine($"  {label}: None");
                        return;
                    }

                    var rarity = (it.Rarity ?? "").ToLower();
                    var color = rarity switch
                    {
                        "common" => ConsoleColor.Gray,
                        "uncommon" => ConsoleColor.Green,
                        "rare" => ConsoleColor.Blue,
                        "epic" => ConsoleColor.Magenta,
                        "legendary" => ConsoleColor.Yellow,
                        _ => ConsoleColor.White
                    };

                    Console.Write($"  {label}: ");
                    var old = Console.ForegroundColor;
                    Console.Write($"{it.Name} [");
                    Console.ForegroundColor = color;
                    Console.Write(it.Rarity);
                    Console.ForegroundColor = old;
                    Console.Write("]");
                    Console.WriteLine();
                }

                WriteEquipped("Weapon", character.EquippedWeapon);
                WriteEquipped("Chest", character.EquippedChestplate);
                WriteEquipped("Pants", character.EquippedPants);
                WriteEquipped("Boots", character.EquippedBoots);
                WriteEquipped("Helmet", character.EquippedHelmet);

                Console.WriteLine();

                // Items (exclude Gold)
                var itemsToShow = _items.Where(i => !(i is Gold)).ToList();

                if (itemsToShow.Count == 0)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine(Localization.T("Inventory.EmptyMessage"));
                    Console.ResetColor();
                    Pause();
                    return;
                }

                // List items with colored rarity
                for (int i = 0; i < itemsToShow.Count; i++)
                {
                    var item = itemsToShow[i];
                    var rarity = (item.Rarity ?? "").ToLower();
                    var color = rarity switch
                    {
                        "common" => ConsoleColor.Gray,
                        "uncommon" => ConsoleColor.Green,
                        "rare" => ConsoleColor.Blue,
                        "epic" => ConsoleColor.Magenta,
                        "legendary" => ConsoleColor.Yellow,
                        _ => ConsoleColor.White
                    };

                    Console.Write($"{i + 1,2}) [");
                    var old = Console.ForegroundColor;
                    Console.ForegroundColor = color;
                    Console.Write(item.Rarity);
                    Console.ForegroundColor = old;
                    Console.Write($"] {item.Name}");
                    Console.ForegroundColor = ConsoleColor.DarkGray;

                    // show slot for armor pieces, otherwise show type name
                    string typeLabel = item is ArmorPiece ap ? ap.Slot.ToString() : item.GetType().Name;
                    Console.Write($" (Type: {typeLabel})");
                    Console.ForegroundColor = old;

                    if (!string.IsNullOrWhiteSpace(item.Description))
                        Console.Write($" - {item.Description}");

                    Console.WriteLine();
                }

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("U = Use    E = Equip    S = Sell    X = Exit");
                Console.ResetColor();

                Console.Write("\n" + Localization.T("Menu.ChooseOption"));
                var actionInput = (Console.ReadLine() ?? "").Trim().ToUpper();

                if (string.IsNullOrWhiteSpace(actionInput))
                    continue;

                if (actionInput == "X")
                    return;

                if (actionInput != "U" && actionInput != "E" && actionInput != "S")
                {
                    Console.WriteLine(Localization.T("Main.InvalidInput"));
                    Pause();
                    continue;
                }

                Console.Write(Localization.T("Menu.ChooseOption"));
                var idxInput = (Console.ReadLine() ?? "").Trim();
                if (!int.TryParse(idxInput, out int idx) || idx < 1 || idx > itemsToShow.Count)
                {
                    Console.WriteLine(Localization.T("Main.InvalidInput"));
                    Pause();
                    continue;
                }

                var selected = itemsToShow[idx - 1];

                if (actionInput == "U")
                {
                    Use(selected, character);
                    // no extra Pause - user already pressed Enter for the index
                    continue;
                }

                if (actionInput == "E")
                {
                    Equip(selected, character);
                    // no extra Pause - user already pressed Enter for the index
                    continue;
                }

                if (actionInput == "S")
                {
                    int price = ShopSystem_GetPrice(selected) / 2;
                    Console.WriteLine(Localization.TF("Merchant.SellConfirm", selected.Name, price));
                    Console.Write(Localization.T("Menu.ChooseOption"));
                    var confirm = (Console.ReadLine() ?? "").Trim().ToLower();
                    if (!(confirm == "1" || confirm == "y" || confirm == "yes"))
                    {
                        Pause();
                        continue;
                    }

                    var duplicates = _items.Count(i => i.Name == selected.Name && !(i is Gold));
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
                            Pause();
                            continue;
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