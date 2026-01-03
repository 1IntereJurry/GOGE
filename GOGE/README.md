GOGE – A Console‑Based RPG Adventure
A lightweight, text‑driven RPG running entirely in the Windows console.

**Introduction**

GOGE is a fully text‑based role‑playing game built in C#. It blends classic RPG mechanics—combat, dungeons, loot, leveling, and character progression—into a nostalgic command‑line experience. The project is modular, easy to extend, and designed to be both a learning resource for new developers and a foundation for more advanced RPG systems. Its architecture separates gameplay logic, data models, and utility functions to ensure long‑term maintainability.

#

**Installation Instructions (Users)**

*Requirements* <br />
• 	Windows 10 or later
• 	.NET Runtime 6.0 or higher
*How to Install*
1. 	Download the latest release (ZIP or EXE).
2. 	Extract the files.
3. 	Run *GOGE.exe* from the extracted folder.

---

**Installation Instructions (Developers)**

*Requirements*
• 	.NET SDK 6.0 or higher
• 	Visual Studio, Rider, or VS Code
• 	Windows CMD or PowerShell


*Setup*

git clone https://github.com/1IntereJurry/GOGE.git <br />
cd GOGE  <br />
dotnet build  <br />
dotnet run  <br />

---

**Project Structure:**

GOGE/

├── Models/           # Characters, enemies, items, weapons, armor <br />
├── Systems/          # Combat, events, menus, save/load logic  <br />
├── Utils/            # Helper functions (colors, formatting)  <br />
├── GameEngine.cs     # Main game loop and core logic  <br />
└── Program.cs        # Entry point

#

**Known Issues**

• 	Enemy AI is still basic
• 	Dungeon generation is static (3 rooms + boss)
• 	Status effects are not fully integrated
• 	Save system does not yet store equipment bonuses
• 	No Linux/Mac support guaranteed (not tested yet)
• 	High‑level balancing is incomplete
