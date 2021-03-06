﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Prefix Spell", menuName = "Data/RPG/Prefix Spell")]
public class PrefixSpell : Spell {

    public Prefix effect;

    public override bool LinksToNextSpell() {
        return true;
    }

    public override void ModifyIntent(IntentSpell next) {
        next.AddPrefix(this);
    }

    public override List<BattleUnit> AcquireAITargets(IntentSpell intent) {
        List<BattleUnit> units = new List<BattleUnit> {
            intent.actor
        };
        return units;
    }

    public override IEnumerator AcquireTargetsRoutine(Result<List<BattleUnit>> result, Intent intent) {
        Result<BattleUnit> unitResult = new Result<BattleUnit>();
        yield return intent.battle.controller.allySelect.SelectAnyOneRoutine(unitResult, intent);
        CopyUnitResult(result, unitResult);
    }

    public override IEnumerator ResolveRoutine(Intent intent, List<PrefixSpell> prefixes) {
        yield return intent.battle.controller.animator.PlayAnimationRoutine(animation,
            intent.actor.doll,
            intent.actor.doll);
    }
}
