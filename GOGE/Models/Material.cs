namespace GOGE.Models
{
    public class Material : Item
    {
        public Material()
        {
        }

        public Material(string name, string rarity, string description)
        {
            Name = name;
            Rarity = rarity;
            Description = description;
        }

        public override void ApplyEffect(Character character)
        {
        }
    }
}