namespace GOGE.Models
{
    public class Potion : Item
    {
        public int HealAmount { get; set; } = 0;
        public int HealPercent { get; set; } = 0;

        public StatusEffect Effect { get; set; } = StatusEffect.None;

        public Potion()
        {
        }

        // Healing potion constructor
        public Potion(string name, int healAmount = 0, int healPercent = 0)
        {
            Name = name;
            HealAmount = healAmount;
            HealPercent = healPercent;
            Rarity = "Common";
            Description = "";
        }

        // Effect potion constructor
        public Potion(string name, StatusEffect effect)
        {
            Name = name;
            Effect = effect;
            Rarity = "Common";
            Description = "";
        }

        public override void ApplyEffect(Character character)
        {
            // Healing
            if (HealAmount > 0)
                character.CurrentHP = Math.Min(character.MaxHP, character.CurrentHP + HealAmount);

            if (HealPercent > 0)
            {
                int heal = (int)(character.MaxHP * (HealPercent / 100.0));
                character.CurrentHP = Math.Min(character.MaxHP, character.CurrentHP + heal);
            }

            // Status effects
            switch (Effect)
            {
                case StatusEffect.Regeneration:
                case StatusEffect.Strength:
                case StatusEffect.Speed:
                case StatusEffect.CritBoost:
                case StatusEffect.BlockBoost:
                case StatusEffect.Focus:
                case StatusEffect.StoneSkin:
                    character.ApplyStatusEffect(Effect);
                    break;

                case StatusEffect.CurePoison:
                    character.RemoveStatusEffect(StatusEffect.Poison);
                    break;

                case StatusEffect.CureBleed:
                    character.RemoveStatusEffect(StatusEffect.Bleed);
                    break;

                case StatusEffect.Escape:
                    character.ForceEscape = true;
                    break;
            }
        }
    }
}