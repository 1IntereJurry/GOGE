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
                Console.WriteLine(Localization.TF("Inventory.PotionUsed", character.Name));
            }
            else
            {
                Console.WriteLine("Item.UseFailed", item.Name); 
            }
        }

        public void Equip(Item item, Character character)
        {
            character.EquipItem(item);
            Remove(item);
        }

        public void ShowInventory()
        {
            Console.Clear();
            TextHelper.ShowTitleBanner();
            Console.WriteLine(Localization.T("Inventory.ShowTitle"));

            if (_items.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(Localization.T("Inventory.EmptyMessage"));
                Console.ResetColor();
                return;
            }

            foreach (var item in _items)
            {
                Console.WriteLine($"[{item.Rarity}] {item.Name} - {item.Description}");
            }
        }

        public List<T> Get<T>() where T : Item
        {
            return _items.OfType<T>().ToList();
        }
    }
}