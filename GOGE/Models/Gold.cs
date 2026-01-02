namespace GOGE.Models
{
    public class Gold : Item
    {
        public int Amount { get; set; }

        public Gold(int amount)
        {
            Name = "Gold";
            Amount = amount;
        }
    }
}