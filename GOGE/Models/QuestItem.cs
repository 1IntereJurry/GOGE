namespace GOGE.Models
{
    public class QuestItem : Item
    {
        public QuestItem()
        {
        }

        public QuestItem(string name, string rarity, string description)
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