using UnityEngine;
using System.Collections;

public class WarheadStagger : Warhead {

    public int staggerLow;
    public int staggerHigh;

    [Tooltip("Final damage will be multiplied by this amount if a boosting prefix is cast (usually 2.0 for 1 AP)")]
    public float boostInfluence;

    protected override IEnumerator InternalResolveRoutine() {
        foreach (BattleUnit target in GetLivingTargets()) {
            yield return animator.PlayAnimationRoutine(anim, actor.doll, target.doll);
        }
        foreach (BattleUnit target in GetLivingTargets()) {
            float stagger = Range(staggerLow, staggerHigh);
            foreach (Prefix prefix in prefixes) {
                stagger = prefix.ModifyStagger(this, stagger);
            }
            yield return target.TakeStaggerRoutine(stagger);
            if (!target.IsDead()) {
                foreach (Prefix prefix in prefixes) {
                    yield return prefix.PostHitRoutine(actor, target);
                }
            }
        }
    }
}
