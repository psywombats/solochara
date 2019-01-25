using UnityEngine;
using System.Collections;

public class WarheadHeal : Warhead {

    public int healLow;
    public int healHigh;

    [Tooltip("Final damage will be multiplied by this amount if a boosting prefix is cast")]
    public float boostInfluence;

    protected override IEnumerator InternalResolveRoutine() {
        foreach (BattleUnit target in GetLivingTargets()) {
            yield return animator.PlayAnimationRoutine(anim, actor.doll, target.doll);
        }
        foreach (BattleUnit target in GetLivingTargets()) {
            float heal = Range(healLow, healHigh);
            foreach (Prefix prefix in this.prefixes) {
                heal = prefix.ModifyHeal(this, heal);
            }
            yield return target.HealRoutine((int)heal);
        }
    }
}
