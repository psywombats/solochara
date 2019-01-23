using UnityEngine;
using System.Collections;

public class WarheadDamage : Warhead {

    public int damageLow;
    public int damageHigh;

    [Tooltip("Final damage will be multiplied by this amount if a boosting prefix is cast (usually 2.0 for 1 AP)")]
    public float boostInfluence;

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
