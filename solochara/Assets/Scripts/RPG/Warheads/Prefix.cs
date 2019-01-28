using UnityEngine;
using System.Collections;

public abstract class Prefix : AutoExpandingScriptableObject {

    public virtual float ModifyHeal(WarheadHeal source, float heal) {
        return heal;
    }

    public virtual float ModifyDamage(WarheadDamage source, float damage) {
        return damage;
    }

    public virtual IEnumerator PostHitRoutine(BattleUnit caster, BattleUnit target) {
        yield return null;
    }
}
