using GOGE.Models;

public static class LootTable
{
	private static Random rng = new Random();

	// -----------------------------
	// 1. Weapons
	// -----------------------------
	private static readonly List<Item> Weapons = new()
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
	// 2. Armor
	// -----------------------------
	private static readonly List<Item> Armor = new()
	{
        // Common
        new ArmorPiece("Leather Helmet", 1, "Common"),
		new ArmorPiece("Cloth Robe", 1, "Common"),
		new ArmorPiece("Old Boots", 1, "Common"),

        // Uncommon
        new ArmorPiece("Iron Helmet", 3, "Uncommon"),
		new ArmorPiece("Chainmail", 4, "Uncommon"),
		new ArmorPiece("Reinforced Leather Gloves", 3, "Uncommon"),

        // Rare
        new ArmorPiece("Rune Helmet", 6, "Rare"),
		new ArmorPiece("Plate Armor", 7, "Rare"),
		new ArmorPiece("Shadow Boots", 6, "Rare"),

        // Epic
        new ArmorPiece("Dragonhide Armor", 10, "Epic"),
		new ArmorPiece("Phoenix Helmet", 9, "Epic"),
		new ArmorPiece("Titan Gloves", 9, "Epic"),

        // Legendary
        new ArmorPiece("Armor of the Immortal", 15, "Legendary"),
		new ArmorPiece("Helm of the Eternal Watch", 14, "Legendary"),
		new ArmorPiece("Boots of the Voidwalker", 13, "Legendary")
	};

	// -----------------------------
	// 3. Potions
	// -----------------------------
	private static readonly List<Item> Potions = new()
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

	// -----------------------------
	// Loot Selection
	// -----------------------------
	public static Item GetRandomLoot()
	{
		int roll = rng.Next(1, 101);

		if (roll <= 50) return Weapons[rng.Next(Weapons.Count)];
		if (roll <= 85) return Armor[rng.Next(Armor.Count)];
		return Potions[rng.Next(Potions.Count)];
	}
}