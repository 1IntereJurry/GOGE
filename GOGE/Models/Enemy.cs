namespace GOGE.Models
{
    public class Enemy
    {
        public string Name { get; set; }
        public int Level { get; set; }

        // CombatSystem compatibility
        public int MaxHP { get; set; }
        public int CurrentHP { get; set; }
        public int Damage { get; set; }

        public int XPReward { get; set; }
        public int GoldReward { get; set; }

        public EnemyType Type { get; set; }
        public string Description { get; set; }

        public StatusEffect Effect { get; set; }

        public List<Item> LootTable { get; set; } = new();

        public Enemy(string name, int level, EnemyType type)
        {
            Name = name;
            Level = level;
            Type = type;

            // Base stats
            MaxHP = 20 + (level * 10);
            Damage = 3 + (level * 2);

            if (type == EnemyType.Elite)
            {
                MaxHP = (int)(MaxHP * 1.5);
                Damage = (int)(Damage * 1.3);
            }

            if (type == EnemyType.Boss)
            {
                MaxHP = (int)(MaxHP * 2.5);
                Damage = (int)(Damage * 1.8);
            }

            CurrentHP = MaxHP;

            XPReward = type switch
            {
                EnemyType.Normal => level * 10,
                EnemyType.Elite => level * 20,
                EnemyType.Boss => level * 50,
                _ => level * 10
            };

            GoldReward = type switch
            {
                EnemyType.Normal => level * 5,
                EnemyType.Elite => level * 10,
                EnemyType.Boss => level * 20,
                _ => level * 5
            };

            Description = GenerateDescription();
            Effect = RollStatusEffect();
            GenerateLoot();
        }

        private string GenerateDescription()
        {
            string[] traits =
            {
                "looks aggressive",
                "seems hungry",
                "is covered in scars",
                "has glowing eyes",
                "moves unnaturally fast",
                "smells terrible",
                "growls at you",
                "is dripping with slime",
                "has cracked bones",
                "is foaming at the mouth"
            };

            Random rng = new Random();
            return traits[rng.Next(traits.Length)];
        }

        private StatusEffect RollStatusEffect()
        {
            Random rng = new Random();
            int roll = rng.Next(1, 101);

            return Type switch
            {
                EnemyType.Normal => roll < 5 ? StatusEffect.Poison : StatusEffect.None,
                EnemyType.Elite => roll < 15 ? StatusEffect.Bleed : StatusEffect.None,
                EnemyType.Boss => roll < 25 ? StatusEffect.Stun : StatusEffect.None,
                _ => StatusEffect.None
            };
        }

        private void GenerateLoot()
        {
            LootTable.Add(new Gold(GoldReward));
        }

        public int GetDamage()
        {
            return Damage;
        }

        public void DealFixedDamage(Character target, int amount)
        {
            target.CurrentHP -= amount;
            Console.WriteLine($"{target.Name} takes {amount} damage!");
        }
    }
}