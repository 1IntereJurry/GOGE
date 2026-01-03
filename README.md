GOGE – A Console‑Based RPG Adventure
A lightweight, text‑driven RPG running entirely in the Windows console.

Introduction
GOGE is a fully text‑based role‑playing game built in C#. It blends classic RPG mechanics—combat, dungeons, loot, leveling, and character progression—into a nostalgic command‑line experience. The project is modular, easy to extend, and designed to be both a learning resource for new developers and a foundation for more advanced RPG systems. Its architecture separates gameplay logic, data models, and utility functions to ensure long‑term maintainability.

Installation Instructions (Users)
Requirements
• 	Windows 10 or later
• 	.NET Runtime 6.0 or higher
How to Install
1. 	Download the latest release (ZIP or EXE).
2. 	Extract the files.
3. 	Run  from the extracted folder.
Optional Enhancements
• 	Change the console font to Consolas or Cascadia Mono
• 	Run the console in fullscreen
• 	Replace the EXE icon with a custom one

Installation Instructions (Developers)
Requirements
• 	.NET SDK 6.0 or higher
• 	Visual Studio, Rider, or VS Code
• 	Windows CMD or PowerShell
Setup

Project Structure


Contributor Expectations
• 	Small, focused pull requests
Each change should be isolated and easy to review.
• 	Consistent code style
• 	PascalCase for classes
• 	camelCase for variables
• 	Avoid magic numbers
• 	Keep methods short and descriptive
• 	Documentation
Any new system or complex logic must be commented or documented.
• 	Testing
Contributors should manually test gameplay changes before submitting.
• 	Respectful communication
Discussions should remain constructive and professional.

Known Issues
• 	Enemy AI is still basic
• 	Dungeon generation is static (3 rooms + boss)
• 	Status effects are not fully integrated
• 	Save system does not yet store equipment bonuses
• 	No Linux/Mac support due to console dependencies
• 	High‑level balancing is incomplete
