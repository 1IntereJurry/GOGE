using GOGE.Utils;

namespace GOGE.Models
{
    public class Character
    {
        // Basic chances
        public double DodgeChance { get; set; } = 0.05;
        public double BlockChance { get; set; } = 0.05;

        // Required for JSON deserialization
        public Character() { }

        // Constructor for new game
        public Character(string name, string charClass)
        {
            Name = name;
            Class = charClass;
            InitializeStats();
        }

        // Status effects
        public List<StatusEffect> ActiveEffects { get; set; } = new();
        public bool ForceEscape { get; set; } = false;

        // Basic info
        public string Name { get; set; } = string.Empty;
        public string Class { get; set; } = string.Empty;
        public int Level { get; set; } = 1;

        // Currency
        public int Gold { get; set; } = 0;

        // XP system
        public int XP { get; set; }
        public int XPToNextLevel { get; set; }

        // Core stats
        public int MaxHP { get; set; }
        public int CurrentHP { get; set; }

        public int MaxMana { get; set; }
        public int CurrentMana { get; set; }

        public int Strength { get; set; }
        public int Speed { get; set; }
        public int Dodge { get; set; }
        public int Flexibility { get; set; }

        public bool IsStunned { get; set; }

        // Energy system
        public int MaxEnergy { get; set; }
        public int CurrentEnergy { get; set; }

        // Equipment
        public Weapon? EquippedWeapon { get; set; }
        public ArmorPiece? EquippedBoots { get; set; }
        public ArmorPiece? EquippedPants { get; set; }
        public ArmorPiece? EquippedChestplate { get; set; }
        public ArmorPiece? EquippedHelmet { get; set; }

        // Inventory
        public List<Item> Inventory { get; set; } = new();

        // XP scaling parameters
        private const int BaseXPForLevel = 100;
        private const double XPLevelGrowth = 1.60; // each level requires 40% more XP than previous

        // ---------------------------------------------------------
        // CLASS-BASED STAT INITIALIZATION
        // ---------------------------------------------------------
        private void InitializeStats()
        {
            switch (Class.ToLower())
            {
                case "knight":
                    MaxHP = 140;
                    Strength = 12;
                    Speed = 8;
                    Dodge = 3;
                    Flexibility = 4;
                    MaxMana = 40;
                    break;

                case "mage":
                    MaxHP = 90;
                    Strength = 6;
                    Speed = 10;
                    Dodge = 5;
                    Flexibility = 8;
                    MaxMana = 120;
                    break;

                case "rogue":
                    MaxHP = 100;
                    Strength = 10;
                    Speed = 14;
                    Dodge = 12;
                    Flexibility = 10;
                    MaxMana = 60;
                    break;

                case "berserker":
                    MaxHP = 130;
                    Strength = 15;
                    Speed = 9;
                    Dodge = 4;
                    Flexibility = 5;
                    MaxMana = 30;
                    break;

                default:
                    MaxHP = 100;
                    Strength = 10;
                    Speed = 10;
                    Dodge = 5;
                    Flexibility = 5;
                    MaxMana = 50;
                    break;
            }

            CurrentHP = MaxHP;
            CurrentMana = MaxMana;

            MaxEnergy = 10;
            CurrentEnergy = MaxEnergy;

            // initialize XP requirement for current level
            XPToNextLevel = CalculateXPForLevel(Level);
        }

        // ---------------------------------------------------------
        // XP & LEVEL-UP
        // ---------------------------------------------------------
        public void AddXP(int amount)
        {
            if (amount <= 0)
                return;

            XP += amount;

            // Prevent rapid mass-leveling by recalculating XPToNextLevel on each level up
            while (XP >= XPToNextLevel)
            {
                XP -= XPToNextLevel;
                LevelUp();
            }
        }

        private void LevelUp()
        {
            Level++;

            // increase stats on level up
            MaxHP += 10;
            Strength += 2;
            Speed += 1;
            Dodge += 1;

            CurrentHP = MaxHP;

            // set new XP requirement for next level
            XPToNextLevel = CalculateXPForLevel(Level);

            Console.WriteLine(Localization.TF("Character.LevelUp", Name, Level));
        }

        private int CalculateXPForLevel(int level)
        {
            // exponential growth: BaseXPForLevel * (growth)^(level-1)
            double xp = BaseXPForLevel * Math.Pow(XPLevelGrowth, Math.Max(0, level - 1));
            return Math.Max(10, (int)Math.Round(xp));
        }

        // ---------------------------------------------------------
        // ENERGY MANAGEMENT
        // ---------------------------------------------------------
        public void StartTurn()
        {
            CurrentEnergy = MaxEnergy;
        }

        public bool CanPerformAction(int energyCost)
        {
            return CurrentEnergy >= energyCost;
        }

        public void ConsumeEnergy(int amount)
        {
            CurrentEnergy = Math.Max(0, CurrentEnergy - amount);
        }

        // ---------------------------------------------------------
        // COMBAT CALCULATIONS
        // ---------------------------------------------------------
        public int CritChance => Flexibility + (Speed / 2);

        public int GetAttackDamage()
        {
            int baseDamage = Strength + (EquippedWeapon?.Damage ?? 5);

            bool isCrit = new Random().Next(0, 100) < CritChance;

            if (isCrit)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(Localization.T("Character.CritHit"));
                Console.ResetColor();

                return (int)(baseDamage * 1.5);
            }

            return baseDamage;
        }

        public void TakeDamage(int amount)
        {
            int effectiveDodge = Math.Clamp(Dodge, 0, 50);
            bool dodged = new Random().Next(0, 100) < effectiveDodge;

            if (dodged)
            {
                Console.WriteLine(Localization.TF("Character.Dodged", Name));
                return;
            }

            // Sum damage reduction from equipped armor pieces
            double totalReduction = 0.0;
            if (EquippedHelmet != null) totalReduction += EquippedHelmet.DamageReductionPercent;
            if (EquippedChestplate != null) totalReduction += EquippedChestplate.DamageReductionPercent;
            if (EquippedPants != null) totalReduction += EquippedPants.DamageReductionPercent;
            if (EquippedBoots != null) totalReduction += EquippedBoots.DamageReductionPercent;

            // cap total reduction at 75%
            totalReduction = Math.Min(0.75, totalReduction);

            int reduced = Math.Max(1, (int)Math.Round(amount * (1.0 - totalReduction)));

            if (totalReduction > 0)
            {
                Console.WriteLine($"Armor reduced incoming damage by {Math.Round(totalReduction * 100)}%.");
            }

            CurrentHP -= reduced;

            // do not print defeat here; CombatSystem.EndFight handles defeat messaging centrally
        }

        // ---------------------------------------------------------
        // EQUIPMENT
        // ---------------------------------------------------------
        public void EquipItem(Item item)
        {
            if (item is Weapon weapon)
            {
                EquippedWeapon = weapon;
            }
            else if (item is ArmorPiece armor)
            {
                switch (armor.Slot)
                {
                    case ArmorSlot.Feet: EquippedBoots = armor; break;
                    case ArmorSlot.Legs: EquippedPants = armor; break;
                    case ArmorSlot.Chest: EquippedChestplate = armor; break;
                    case ArmorSlot.Head: EquippedHelmet = armor; break;
                }
            }

            Console.WriteLine(Localization.TF("Character.Equipped", Name, item.Name));
        }

        // ---------------------------------------------------------
        // DISPLAY
        // ---------------------------------------------------------
        public void ShowStats()
        {
            Console.WriteLine();
            Console.WriteLine(Localization.T("Character.Title"));
            Console.WriteLine(Localization.TF("Character.Label.Name", Name));
            Console.WriteLine(Localization.TF("Character.Label.Class", Class));
            Console.WriteLine(Localization.TF("Character.Label.Level", Level));
            Console.WriteLine(Localization.TF("Character.Label.XP", XP, XPToNextLevel));
            Console.WriteLine(Localization.TF("Character.Label.HP", CurrentHP, MaxHP));
            Console.WriteLine(Localization.TF("Character.Label.Mana", CurrentMana, MaxMana));
            Console.WriteLine(Localization.TF("Character.Label.Gold", Gold));
            Console.WriteLine(Localization.TF("Character.Label.Strength", Strength));
            Console.WriteLine(Localization.TF("Character.Label.Speed", Speed));
            Console.WriteLine(Localization.TF("Character.Label.Dodge", Dodge));
            Console.WriteLine(Localization.TF("Character.Label.Flexibility", Flexibility));
            Console.WriteLine(Localization.TF("Character.Label.Weapon", EquippedWeapon?.Name ?? "None"));
            Console.WriteLine(Localization.TF("Character.Label.Chest", EquippedChestplate?.Name ?? "None"));
            Console.WriteLine(Localization.TF("Character.Label.Pants", EquippedPants?.Name ?? "None"));
            Console.WriteLine(Localization.TF("Character.Label.Boots", EquippedBoots?.Name ?? "None"));
            Console.WriteLine(Localization.TF("Character.Label.Helmet", EquippedHelmet?.Name ?? "None"));
        }

        // ---------------------------------------------------------
        // STATUS EFFECTS
        // ---------------------------------------------------------
        public void ApplyStatusEffect(StatusEffect effect)
        {
            if (effect == StatusEffect.None)
                return;

            if (!ActiveEffects.Contains(effect))
                ActiveEffects.Add(effect);
        }

        public void RemoveStatusEffect(StatusEffect effect)
        {
            if (ActiveEffects.Contains(effect))
                ActiveEffects.Remove(effect);
        }
    }
}