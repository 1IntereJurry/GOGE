using GOGE;
using GOGE.Systems;
using GOGE.Utils;

class Program
{
    static void Main()
    {
        Localization.Set(Language.English); // primary language at start-up is automaticly set to English

        // Bootup
        Console.Title = "GOGE - The Grand Odyssey of Grandiose Explorers";
        TextHelper.LoadingScreen();

        while (true)
        {
            // Main Menu
            var (player, inventory) = MainMenu.Show();

            // If loading failed → return to menu
            if (player == null || inventory == null)
            {
                Console.WriteLine("Returning to main menu...");
                Thread.Sleep(1000);
                continue;
            }

            // Start game
            GameEngine engine = new GameEngine(player, inventory);
            bool returnToTitle = engine.StartGame();

            if (returnToTitle)
            {
                continue;
            }

            break;
        }
    }
}