﻿using UnityEngine;
using System.Collections;

public class PrefixRiderStatus : Prefix {

    public StatusEffect status;
    public float baseInflictChance = 1.0f;

    public override IEnumerator PostHitRoutine(BattleUnit caster, BattleUnit target) {
        yield return base.PostHitRoutine(caster, target);
        float chance = (float)target.battle.r.NextDouble() * baseInflictChance;
        yield return target.TryInflictStatusRoutine(status, chance);
    }

    public override IEnumerator PostHealRoutine(BattleUnit actor, BattleUnit target) {
        if (target.Has(status)) {
            yield return target.RemoveStatusRoutine(status);
        }
    }
}
