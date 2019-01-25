using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IntentSpell : Intent {

    public Spell spell { get; private set; }
    public List<BattleUnit> targets { get; private set; }
    protected List<PrefixSpell> prefixes { get; private set; }

    public IntentSpell(Battle battle, BattleUnit actor, Spell spell) : base(battle, actor) {
        this.spell = spell;
        this.prefixes = new List<PrefixSpell>();
    }

    public override IEnumerator ResolveRoutine() {
        if (!this.actor.IsDead()) {
            battle.Log(actor + " uses " + spell.spellName);
            yield return CoUtils.Wait(0.5f);
            yield return spell.ResolveRoutine(this, prefixes);
        }
    }

    public override int APCost() {
        return spell.apCost;
    }

    public override bool ModifiesNextIntent() {
        return spell.LinksToNextSpell();
    }

    public override void ModifyNextIntent(Intent intent) {
        intent.ConsumePrefixSpell(this);
    }

    public override void ConsumePrefixSpell(IntentSpell intent) {
        intent.spell.ModifyIntent(this);
    }

    public IEnumerator AcquireTargetsRoutine(Result<List<BattleUnit>> result) {
        yield return spell.AcquireTargetsRoutine(result, this);
        if (!result.canceled) {
            targets = result.value;
        }
    }

    public void AcquireAITargets() {
        targets = spell.AcquireAITargets(this);
    }

    public void AddPrefix(PrefixSpell prefix) {
        this.prefixes.Add(prefix);
    }
}
