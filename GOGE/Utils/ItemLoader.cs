using GOGE.Models;
using System.Text.Json;

namespace GOGE.Utils
{
    public static class ItemLoader
    {
        public static List<Item> LoadItems(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"Item file not found: {path}");

            var json = File.ReadAllText(path);
            var doc = JsonDocument.Parse(json);
            var items = new List<Item>();

            foreach (var element in doc.RootElement.EnumerateArray())
            {
                items.Add(ItemFactory.CreateItem(element));
            }

            return items;
        }
    }
}