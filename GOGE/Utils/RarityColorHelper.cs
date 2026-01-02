using GOGE.Models;

namespace GOGE.Utils
{
    public static class RarityColorHelper
    {
        public static void WriteColored(string text, ItemRarity rarity)
        {
            var oldColor = Console.ForegroundColor;

            Console.ForegroundColor = rarity switch
            {
                ItemRarity.Common => ConsoleColor.White,
                ItemRarity.Rare => ConsoleColor.Blue,
                ItemRarity.Epic => ConsoleColor.Magenta,
                ItemRarity.Legendary => ConsoleColor.DarkYellow,
                _ => ConsoleColor.White
            };

            Console.Write(text);
            Console.ForegroundColor = oldColor;
        }
    }
}