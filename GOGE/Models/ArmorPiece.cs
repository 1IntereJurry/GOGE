namespace GOGE.Models
{
    public class ArmorPiece : Item
    {
        public ArmorSlot Slot { get; set; }
        public int Armor { get; set; }
        public int Strength { get; set; }
        public int Agility { get; set; }
        public int Vitality { get; set; }

        // Damage reduction as fraction (e.g. 0.05 = 5%) provided by this armor piece
        public double DamageReductionPercent { get; set; }

        public ArmorPiece(string name, int armor, string rarity)
        {
            Name = name;
            Armor = armor;
            Rarity = rarity;
            Description = "";

            if (name.Contains("Helmet", StringComparison.OrdinalIgnoreCase) || name.Contains("Helm", StringComparison.OrdinalIgnoreCase) || name.Contains("Cap", StringComparison.OrdinalIgnoreCase) || name.Contains("Hood", StringComparison.OrdinalIgnoreCase))
                Slot = ArmorSlot.Head;
            else if (name.Contains("Boots", StringComparison.OrdinalIgnoreCase) || name.Contains("Boot", StringComparison.OrdinalIgnoreCase))
                Slot = ArmorSlot.Feet;
            else if (name.Contains("Pants", StringComparison.OrdinalIgnoreCase) || name.Contains("Greaves", StringComparison.OrdinalIgnoreCase) || name.Contains("Legs", StringComparison.OrdinalIgnoreCase) || name.Contains("Legguards", StringComparison.OrdinalIgnoreCase) || name.Contains("Legplates", StringComparison.OrdinalIgnoreCase))
                Slot = ArmorSlot.Legs;
            else
                Slot = ArmorSlot.Chest;

            Strength = 0;
            Agility = 0;
            Vitality = 0;

            // Basic damage reduction: 5% per armor point, capped at 25% per piece
            DamageReductionPercent = Math.Min(0.25, armor * 0.05);
        }
    }
}