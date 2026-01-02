using GOGE.Models;

namespace GOGE.Models
{
    public abstract class Item
    {
        public string Name { get; set; } = "";
        public string Rarity { get; set; } = "Common";
        public string Description { get; set; } = "";

        public virtual void ApplyEffect(Character character)
        {
        }
    }
}