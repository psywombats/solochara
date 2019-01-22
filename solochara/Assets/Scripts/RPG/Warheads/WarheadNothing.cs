using UnityEngine;
using System.Collections;

public class WarheadNothing : Warhead {

    protected override IEnumerator InternalResolveRoutine() {
        yield return animator.PlayAnimationRoutine(anim, actor.doll, actor.doll);
    }
}
