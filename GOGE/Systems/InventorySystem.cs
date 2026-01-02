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
                Console.WriteLine(Localization.TF("Character.Equipped", character.Name, item.Name));
            }
            else
            {
                Console.WriteLine($"{item.Name} cannot be used directly."); // outsource to localization
            }
        }

        public void Equip(Item item, Character character)
        {
            character.EquipItem(item);
            Remove(item);
        }

        public void Show()
        {
            Console.WriteLine(Localization.T("Inventory.ShowTitle"));
            foreach (var item in _items)
            {
                Console.WriteLine($"[{item.Rarity}] {item.Name} - {item.Description}");
            }
        }

        public void ShowInventory()
        {
            Show();
        }

        public List<T> Get<T>() where T : Item
        {
            return _items.OfType<T>().ToList();
        }
    }
}