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
            float damage = Range(damageLow, damageHigh);
            foreach (Prefix prefix in this.prefixes) {
                damage = prefix.ModifyDamage(this, damage);
            }
            yield return target.TakeDamageRoutine((int)damage);
            if (!target.IsDead()) {
                foreach (Prefix prefix in prefixes) {
                    yield return prefix.PostHitRoutine(actor, target);
                }
            }
        }
    }
}
