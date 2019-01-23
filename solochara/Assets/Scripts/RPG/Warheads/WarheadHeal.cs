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
            int heal = Range(healLow, healHigh);
            yield return target.HealRoutine(heal);
        }
    }
}
