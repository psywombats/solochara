using UnityEngine;

public class PrefixBoost : Prefix {

    [Tooltip("Affected spells have some values increased by a spell-specific constants times this value, usually 1.0")]
    public float effectiveness;

    public override float ModifyDamage(WarheadDamage source, float damage) {
        return source.boostInfluence * effectiveness * damage;
    }

    public override float ModifyHeal(WarheadHeal source, float heal) {
        return source.boostInfluence * effectiveness * heal;
    }

    public override float ModifyStagger(WarheadStagger source, float stagger) {
        return source.boostInfluence * effectiveness * stagger;
    }
}
