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

    public IEnumerator AcquireTargetsRoutine() {
        Result<List<BattleUnit>> result = new Result<List<BattleUnit>>();
        yield return spell.AcquireTargetsRoutine(result, this);
        targets = result.value;
    }
}
