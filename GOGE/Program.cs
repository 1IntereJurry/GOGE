using System;
using System.Threading;
using GOGE;
using GOGE.Models;
using GOGE.Systems;
using GOGE.Utils;

class Program
{
    static void Main()
    {
        //wenn das klappt hat es gepusht - bitte
        // Load localization before any UI text
        Localization.Set(Language.English); // primary language at start-up is automaticly set to English

        // Bootup
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
                // go back to title screen (MainMenu.Show) on next loop iteration
                continue;
            }

            // exit application
            break;
        }
    }
}