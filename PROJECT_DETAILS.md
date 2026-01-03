# GOGE – Project Details & Technical Documentation

This document provides an in‑depth explanation of the internal structure, design decisions, and architecture of the GOGE console RPG. While the main README focuses on onboarding and usage, this file is intended for developers who want to understand how the system works under the hood.

---

## 1. Project Purpose

GOGE was created as a modular, extendable console RPG written in C#.  
The goals of the project are:

- To demonstrate clean separation of systems in a game architecture  
- To provide a learning-friendly codebase for beginners  
- To offer a nostalgic, text‑driven RPG experience  
- To allow easy expansion (new classes, enemies, items, systems)  

---

## 2. High-Level Architecture

GOGE follows a **system‑based architecture**, where each major gameplay feature is encapsulated in its own subsystem. This keeps the code clean, testable, and easy to maintain.

### Core Components

| Component | Responsibility |
|----------|----------------|
| **GameEngine** | Main game loop, menu navigation, flow control |
| **Models** | Data structures (Character, Enemy, Items, Equipment) |
| **Systems** | Gameplay logic (Combat, Events, Saving, Inventory) |
| **Utils** | Helper functions (colors, formatting, randomization) |
| **Program.cs** | Entry point that initializes the game |

---

## 3. Folder & File Breakdown

Below is a detailed explanation of what each file and folder does.

### 📁 **Models/**
Contains all data models used by the game.

| File | Purpose |
|------|---------|
| **Character.cs** | Represents the player character. Handles stats, leveling, combat calculations, energy, equipment, and status effects. |
| **Enemy.cs** | Defines enemy stats and behavior. |
| **EnemyFactory.cs** | Creates enemies based on player level and dungeon difficulty. |
| **Item.cs** | Base class for all items. |
| **Weapon.cs** | Defines weapon stats and damage values. |
| **ArmorPiece.cs** | Defines armor stats and slot types (Head, Chest, Legs, Feet). |
| **StatusEffect.cs** | Enum for effects like Stun, Poison, etc. |

---

### 📁 **Systems/**
Contains all gameplay systems.

| File | Purpose |
|------|---------|
| **CombatSystem.cs** | Handles turn‑based combat, damage calculation, crits, dodging, blocking, and applying effects. |
| **InventorySystem.cs** | Manages item storage, equipping, and displaying inventory. |
| **EventSystem.cs** | Random world events (loot, encounters, dungeon discovery). |
| **SaveSystem.cs** | Serializes and deserializes game state to JSON files. |

---

### 📁 **Utils/**
Utility helpers that support the main systems.

| File | Purpose |
|------|---------|
| **ConsoleHelper.cs** | Color formatting, styled output, helper methods for UI. |
| **RandomHelper.cs** | Centralized random number generation. |
| **TextUtils.cs** | Formatting, spacing, string helpers. |

---

### 📄 **GameEngine.cs**
The heart of the game.

Responsibilities:

- Displays the main menu  
- Starts fights  
- Triggers events  
- Handles dungeon flow  
- Calls Save/Load  
- Manages game progression  

This file orchestrates all other systems.

---

### 📄 **Program.cs**
The entry point of the application.

Responsibilities:

- Initialize player  
- Initialize inventory  
- Create GameEngine instance  
- Start the game loop  

---

## 4. Gameplay Flow

1. Player starts the game  
2. Main menu appears  
3. Player chooses: Fight, Dungeon, Inventory, Save, Load, Quit  
4. Combat or exploration happens  
5. Player gains XP, loot, gold  
6. Level‑ups adjust stats  
7. Dungeon unlocks randomly  
8. Game can be saved/loaded anytime  

---

## 5. Future Roadmap

- Dynamic dungeon generation  
- Improved enemy AI  
- Magic system with spells  
- Shops and merchants  
- World map exploration  
- Achievements  
- Sound effects (console beeps or external library)  

---

## 6. Known Limitations

- Console UI limits visual complexity  
- No cross‑platform support (Windows only)  
- Save files do not yet store all equipment modifiers  
- Combat AI is intentionally simple  

---

## 7. Contributing Guidelines

- Follow C# naming conventions  
- Keep methods short and focused  
- Document complex logic  
- Test gameplay changes manually  
- Use small, focused pull requests  
- Be respectful and constructive  

---

## 8. Contact

For questions, ideas, or contributions:  
**https://github.com/1IntereJurry** – Project Lead  
