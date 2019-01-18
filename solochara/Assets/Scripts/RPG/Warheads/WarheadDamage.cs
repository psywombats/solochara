using UnityEngine;
using System.Collections;

public class WarheadDamage : Warhead {

    public int damageLow;
    public int damageHigh;

    protected override IEnumerator InternalResolveRoutine() {
        foreach (BattleUnit target in GetLivingTargets()) {
            yield return animator.PlayAnimationRoutine(anim, actor.doll, target.doll);
        }
        foreach (BattleUnit target in GetLivingTargets()) {
            int damage = Range(damageLow, damageHigh);
            yield return target.TakeDamageRoutine(damage);
        }
    }
}
