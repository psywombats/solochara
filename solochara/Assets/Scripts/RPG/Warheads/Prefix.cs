using UnityEngine;
using System.Collections;

public abstract class Prefix : AutoExpandingScriptableObject {

    public virtual float ModifyHeal(WarheadHeal source, float heal) {
        return heal;
    }

    public virtual float ModifyDamage(WarheadDamage source, float damage) {
        return damage;
    }

    public virtual IEnumerator PostHitRoutine(BattleUnit actor, BattleUnit target) {
        yield return null;
    }

    public virtual IEnumerator PostHealRoutine(BattleUnit actor, BattleUnit target) {
        yield return null;
    }
}
