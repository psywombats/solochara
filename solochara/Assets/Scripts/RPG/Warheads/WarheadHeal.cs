using UnityEngine;
using System.Collections;

public class WarheadHeal : Warhead {

    public int healLow;
    public int healHigh;

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
