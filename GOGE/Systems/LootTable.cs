using GOGE.Models;

public static class LootTable
{
    private static Random rng = new Random();

    // -----------------------------
    // 1. Weapons
    // -----------------------------
    private static readonly List<Weapon> Weapons = new()
    {
        // Common
        new Weapon("Rusty Shortsword", 2, "Common"),
        new Weapon("Wooden Club", 1, "Common"),
        new Weapon("Worn Dagger", 1, "Common"),
        new Weapon("Broken Spear", 1, "Common"),
        new Weapon("Old Axe", 2, "Common"),

        // Uncommon
        new Weapon("Iron Longsword", 4, "Uncommon"),
        new Weapon("Hunting Bow", 4, "Uncommon"),
        new Weapon("War Club", 5, "Uncommon"),
        new Weapon("Iron Hammer", 6, "Uncommon"),
        new Weapon("Steel Dagger", 4, "Uncommon"),

        // Rare
        new Weapon("Rune Blade", 8, "Rare"),
        new Weapon("Storm Bow", 8, "Rare"),
        new Weapon("Blood Dagger", 7, "Rare"),
        new Weapon("Frost Spear", 9, "Rare"),
        new Weapon("Thunder Axe", 10, "Rare"),

        // Epic
        new Weapon("Dragonfang Blade", 14, "Epic"),
        new Weapon("Phoenix Sword", 15, "Epic"),
        new Weapon("Titan Hammer", 16, "Epic"),
        new Weapon("Storm Spear", 15, "Epic"),

        // Legendary
        new Weapon("Excalibur Fragment", 20, "Legendary"),
        new Weapon("Sword of the Sun King", 22, "Legendary"),
        new Weapon("Night Soul", 18, "Legendary"),
        new Weapon("Worldbreaker", 25, "Legendary")
    };

    // -----------------------------
    // 2. Armor Pools by Slot
    // -----------------------------
    private static readonly List<ArmorPiece> Helmets = new()
    {
        new ArmorPiece("Leather Cap", 1, "Common"),
        new ArmorPiece("Cloth Hood", 1, "Common"),
        new ArmorPiece("Iron Helm", 3, "Uncommon"),
        new ArmorPiece("Rune Helm", 6, "Rare"),
        new ArmorPiece("Phoenix Circlet", 9, "Epic"),
        new ArmorPiece("Helm of the Eternal Watch", 14, "Legendary")
    };

    private static readonly List<ArmorPiece> Chestplates = new()
    {
        new ArmorPiece("Leather Tunic", 1, "Common"),
        new ArmorPiece("Cloth Robe", 1, "Common"),
        new ArmorPiece("Chainmail", 4, "Uncommon"),
        new ArmorPiece("Plate Armor", 7, "Rare"),
        new ArmorPiece("Dragonhide Armor", 10, "Epic"),
        new ArmorPiece("Armor of the Immortal", 15, "Legendary")
    };

    private static readonly List<ArmorPiece> Pants = new()
    {
        new ArmorPiece("Cloth Pants", 1, "Common"),
        new ArmorPiece("Leather Pants", 1, "Common"),
        new ArmorPiece("Reinforced Leather Pants", 3, "Uncommon"),
        new ArmorPiece("Plate Greaves", 6, "Rare"),
        new ArmorPiece("Titan Legguards", 9, "Epic"),
        new ArmorPiece("Legplates of Legends", 13, "Legendary")
    };

    private static readonly List<ArmorPiece> Boots = new()
    {
        new ArmorPiece("Old Boots", 1, "Common"),
        new ArmorPiece("Leather Boots", 1, "Common"),
        new ArmorPiece("Shadow Boots", 6, "Rare"),
        new ArmorPiece("Swiftstep Boots", 4, "Uncommon"),
        new ArmorPiece("Boots of the Voidwalker", 13, "Legendary"),
        new ArmorPiece("Dragonbone Boots", 9, "Epic")
    };

    // -----------------------------
    // 3. Potions
    // -----------------------------
    private static readonly List<Potion> Potions = new()
    {
        new Potion("Small Health Potion", 20),
        new Potion("Medium Health Potion", 40),
        new Potion("Large Health Potion", 80),
        new Potion("Mega Elixir", 150),
        new Potion("Regeneration Potion", StatusEffect.Regeneration),

        new Potion("Potion of Strength", StatusEffect.Strength),
        new Potion("Potion of Speed", StatusEffect.Speed),
        new Potion("Potion of Focus", StatusEffect.CritBoost),
        new Potion("Potion of Stone Skin", StatusEffect.BlockBoost),

        new Potion("Antidote", StatusEffect.CurePoison),
        new Potion("Bandages", StatusEffect.CureBleed),
        new Potion("Smoke Bomb", StatusEffect.Escape)
    };

    // helper to get combined armor pool
    private static List<ArmorPiece> AllArmor => Helmets.Concat(Chestplates).Concat(Pants).Concat(Boots).ToList();

    // -----------------------------
    // Loot Selection
    // -----------------------------
    public static Item GetRandomLoot()
    {
        int roll = rng.Next(1, 101);

        if (roll <= 50) return Weapons[rng.Next(Weapons.Count)];
        if (roll <= 85)
        {
            var pool = AllArmor;
            return pool[rng.Next(pool.Count)];
        }
        return Potions[rng.Next(Potions.Count)];
    }

    public static Item GetLootForEnemy(EnemyType type, int enemyLevel)
    {
        // Determine rarity thresholds based on enemy type
        int roll = rng.Next(1, 101);
        string rarity;

        switch (type)
        {
            case EnemyType.Elite:
                rarity = roll <= 40 ? "Common"
                    : roll <= 75 ? "Uncommon"
                    : roll <= 90 ? "Rare"
                    : roll <= 98 ? "Epic" : "Legendary";
                break;

            case EnemyType.Boss:
                rarity = roll <= 20 ? "Common"
                    : roll <= 50 ? "Uncommon"
                    : roll <= 80 ? "Rare"
                    : roll <= 95 ? "Epic" : "Legendary";
                break;

            default:
                rarity = roll <= 70 ? "Common"
                    : roll <= 90 ? "Uncommon"
                    : roll <= 98 ? "Rare" : "Epic";
                break;
        }

        // Decide item category
        int cat = rng.Next(1, 101);
        if (cat <= 50)
        {
            // Weapon
            var candidates = Weapons.Where(w => string.Equals(w.Rarity, rarity, StringComparison.OrdinalIgnoreCase)).ToList();
            if (!candidates.Any()) candidates = Weapons.ToList();
            var proto = candidates[rng.Next(candidates.Count)];
            // scale damage based on enemy level
            int scaled = proto.Damage + Math.Max(0, enemyLevel / 2);
            return new Weapon(proto.Name, scaled, proto.Rarity) { Description = proto.Description };
        }
        else if (cat <= 85)
        {
            // Armor - pick from combined armor pool filtered by rarity
            var pool = AllArmor.Where(a => string.Equals(a.Rarity, rarity, StringComparison.OrdinalIgnoreCase)).ToList();
            if (!pool.Any()) pool = AllArmor.ToList();
            var proto = pool[rng.Next(pool.Count)];
            int scaledArmor = proto.Armor + Math.Max(0, enemyLevel / 3);
            var piece = new ArmorPiece(proto.Name, scaledArmor, proto.Rarity) { Description = proto.Description };
            return piece;
        }
        else
        {
            // Potion
            var proto = Potions[rng.Next(Potions.Count)];
            // return a copy
            return new Potion(proto.Name, proto.HealAmount, proto.HealPercent) { Description = proto.Description, Effect = proto.Effect };
        }
    }
}