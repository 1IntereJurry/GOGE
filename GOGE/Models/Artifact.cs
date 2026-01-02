namespace GOGE.Models
{
    public class Artifact : Item
    {
        public Artifact()
        {
        }

        public Artifact(string name, string rarity, string description)
        {
            Name = name;
            Rarity = rarity;
            Description = description;
        }

        public override void ApplyEffect(Character character)
        {
            // artifacts have passive effects, so this method can be left empty
            // add special artifact effects here if needed in the future
        }
    }
}