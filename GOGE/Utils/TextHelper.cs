namespace GOGE.Utils
{
    class TextHelper
    {
        public static void LoadingScreen()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("GOGE Engine Booting...\n");
            Console.ResetColor();

            string[] steps =
            {
            "[OK] Loading Assets",
            "[OK] Loading Items",
            "[OK] Initializing Systems",
            "[OK] Preparing Game Engine",
            "[OK] Finalizing"
            };

            // Boot‑Up‑Screen
            foreach (var step in steps)
            {
                Console.WriteLine(step);
                Thread.Sleep(300);
            }

            Console.WriteLine();
            Console.WriteLine("Starting Engine...\n");

            // Fortschrittsbalken
            for (int i = 0; i <= 20; i++)
            {
                Console.Write("\r[" + new string('█', i) + new string(' ', 20 - i) + $"] {i * 5}%");
                Thread.Sleep(80);
            }

            Console.ResetColor();
            Console.WriteLine("\n\nEngine Ready!");
            Thread.Sleep(500);

        }

        public static void ShowTitleBanner()
        {
            Console.WriteLine("                                              ");
            Console.WriteLine("     ██████╗  ██████╗  ██████╗ ███████╗       ");
            Console.WriteLine("    ██╔════╝ ██╔═══██╗██╔════╝ ██╔════╝       ");
            Console.WriteLine("    ██║  ███╗██║   ██║██║  ███╗█████╗         ");
            Console.WriteLine("    ██║   ██║██║   ██║██║   ██║██╔══╝         ");
            Console.WriteLine("    ╚██████╔╝╚██████╔╝╚██████╔╝███████╗       ");
            Console.WriteLine("     ╚═════╝  ╚═════╝  ╚═════╝ ╚══════╝       ");
            Console.WriteLine("                                              ");
        }

        public static void ShowTitleScreen()
        {
            Console.Clear();
            TextHelper.ShowTitleBanner();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(Localization.T("Title.StartNew"));
            Console.WriteLine(Localization.T("Title.Load"));
            Console.WriteLine(Localization.T("Title.Credits"));
            Console.WriteLine(Localization.T("Title.Language"));
            Console.WriteLine(Localization.T("Title.Exit"));
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(Localization.T("Menu.ChooseOption"));
            Console.ResetColor();
        }

        public static void ShowCredits()
        {
            ShowTitleBanner();
            Console.WriteLine("\nCreated by (GitHub: 1IntereJurry). Powered by imagination.");
            Console.ReadKey(true);
        }
    }
}