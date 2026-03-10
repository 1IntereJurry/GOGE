namespace GOGE.Models
{
    public abstract class Item
    {
        public string Id { get; set; } = GOGE.Utils.IdGenerator.NewId();
        public string Name { get; set; } = "";
        public string Rarity { get; set; } = "Common";
        public string Description { get; set; } = "";

        public virtual void ApplyEffect(Character character)
        {
        }
    }
}