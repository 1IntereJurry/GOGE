using GOGE;
using GOGE.Systems;
using GOGE.Utils;
using System.Diagnostics;

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

    // Hidden debug command to open saves folder
    public static void OpenSavesFolder()
    {
        string savesPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "GOGE", "Saves");

        try
        {
            if (!Directory.Exists(savesPath))
                Directory.CreateDirectory(savesPath);

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "explorer.exe",
                Arguments = savesPath,
                UseShellExecute = true
            };
            Process.Start(psi);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to open saves folder: {ex.Message}");
        }
    }
}