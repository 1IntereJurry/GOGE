using GOGE.Models;

namespace GOGE.Utils
{
    public static class EnemyFactory
    {
        private static readonly Random rng = new();

        private static readonly List<string> NormalEnemies = new()
        {
            "Goblin", "Wolf", "Skeleton", "Bandit", "Slime", "Zombie"
        };

        private static readonly List<string> EliteEnemies = new()
        {
            "Orc Warrior", "Dire Wolf", "Ghoul", "Dark Knight"
        };

        private static readonly List<string> BossEnemies = new()
        {
            "Ogre King", "Necromancer", "Ancient Dragonling", "Shadow Beast"
        };

        public static Enemy CreateEnemy(int playerLevel, bool isDungeon = false, bool isBoss = false)
        {
            int enemyLevel;

            // LEVEL SCALING
            if (isBoss)
            {
                enemyLevel = playerLevel + 5;
            }
            else if (isDungeon)
            {
                enemyLevel = rng.Next(playerLevel + 1, playerLevel + 4); // +1 to +3
            }
            else
            {
                enemyLevel = rng.Next(Math.Max(1, playerLevel - 1), playerLevel + 2); // -1 to +1
            }

            // TYPE & NAME
            EnemyType type;
            string name;

            if (isBoss)
            {
                type = EnemyType.Boss;
                name = BossEnemies[rng.Next(BossEnemies.Count)];
            }
            else if (isDungeon)
            {
                int roll = rng.Next(1, 101);

                if (roll <= 60)
                {
                    type = EnemyType.Normal;
                    name = NormalEnemies[rng.Next(NormalEnemies.Count)];
                }
                else if (roll <= 90)
                {
                    type = EnemyType.Elite;
                    name = EliteEnemies[rng.Next(EliteEnemies.Count)];
                }
                else
                {
                    type = EnemyType.Boss;
                    name = BossEnemies[rng.Next(BossEnemies.Count)];
                }
            }
            else
            {
                int roll = rng.Next(1, 101);

                if (roll <= 70)
                {
                    type = EnemyType.Normal;
                    name = NormalEnemies[rng.Next(NormalEnemies.Count)];
                }
                else if (roll <= 95)
                {
                    type = EnemyType.Elite;
                    name = EliteEnemies[rng.Next(EliteEnemies.Count)];
                }
                else
                {
                    type = EnemyType.Boss;
                    name = BossEnemies[rng.Next(BossEnemies.Count)];
                }
            }

            // CREATE ENEMY
            Enemy enemy = new Enemy(name, enemyLevel, type);

            // STAT SCALING
            if (isDungeon && !isBoss)
            {
                enemy.MaxHP = (int)(enemy.MaxHP * 1.20);
                enemy.CurrentHP = enemy.MaxHP;

                enemy.Damage = (int)(enemy.Damage * 1.10);
            }

            if (isBoss)
            {
                enemy.MaxHP = (int)(enemy.MaxHP * 1.50);
                enemy.CurrentHP = enemy.MaxHP;

                enemy.Damage = (int)(enemy.Damage * 1.30);
            }

            return enemy;
        }
    }
}