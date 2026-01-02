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
            // Artefakte haben meist passive Effekte.
            // Du kannst hier später Spezialeffekte einbauen.
        }
    }
}