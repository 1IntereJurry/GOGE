namespace GOGE.Models
{
    public class Weapon : Item
    {
        public int Damage { get; set; }
        public double CritChance { get; set; }

        public Weapon(string name, int damage, string rarity)
        {
            Name = name;
            Damage = damage;
            Rarity = rarity;
            Description = "";
            CritChance = 0.05;
        }
    }
}