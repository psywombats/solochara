using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IntentSpell : Intent {

    private Spell spell;
    private List<BattleUnit> targets;

    public IntentSpell(Battle battle, BattleUnit actor, Spell spell) : base(battle, actor) {
        this.spell = spell;
    }

    public override IEnumerator ResolveRoutine() {
        if (!this.actor.IsDead()) {
            yield return spell.ResolveRoutine(this);
        }
    }

    public override int APCost() {
        return spell.apCost;
    }

    public IEnumerator AcquireTargetsRoutine(Result<List<BattleUnit>> result) {
        yield return spell.AcquireTargetsRoutine(result, this);
        if (!result.canceled) {
            targets = result.value;
        }
    }
}
