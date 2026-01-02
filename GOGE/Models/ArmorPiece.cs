namespace GOGE.Models
{
    public class ArmorPiece : Item
    {
        public ArmorSlot Slot { get; set; }
        public int Armor { get; set; }
        public int Strength { get; set; }
        public int Agility { get; set; }
        public int Vitality { get; set; }

        public ArmorPiece(string name, int armor, string rarity)
        {
            Name = name;
            Armor = armor;
            Rarity = rarity;
            Description = "";

            if (name.Contains("Helmet", StringComparison.OrdinalIgnoreCase))
                Slot = ArmorSlot.Head;
            else if (name.Contains("Boots", StringComparison.OrdinalIgnoreCase))
                Slot = ArmorSlot.Feet;
            else if (name.Contains("Legs", StringComparison.OrdinalIgnoreCase))
                Slot = ArmorSlot.Legs;
            else
                Slot = ArmorSlot.Chest;

            Strength = 0;
            Agility = 0;
            Vitality = 0;
        }
    }
}