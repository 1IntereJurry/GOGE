using GOGE.Models;
using System.Text.Json;

namespace GOGE.Utils
{
    public static class ItemFactory
    {
        public static Item CreateItem(JsonElement json)
        {
            string? type = json.GetProperty("type").GetString();
            if (type == null)
                throw new Exception("Item type missing in JSON");

            return type switch
            {
                "Weapon" => JsonSerializer.Deserialize<Weapon>(json.GetRawText())!,
                "ArmorPiece" => JsonSerializer.Deserialize<ArmorPiece>(json.GetRawText())!,
                "Potion" => JsonSerializer.Deserialize<Potion>(json.GetRawText())!,
                "Artifact" => JsonSerializer.Deserialize<Artifact>(json.GetRawText())!,
                "QuestItem" => JsonSerializer.Deserialize<QuestItem>(json.GetRawText())!,
                "Material" => JsonSerializer.Deserialize<Material>(json.GetRawText())!,
                _ => throw new NotSupportedException($"Unknown item type: {type}")
            };
        }
    }
}