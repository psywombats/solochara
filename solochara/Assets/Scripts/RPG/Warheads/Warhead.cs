using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/**
 *  Generic class for everything a spell can do
 */
public abstract class Warhead : AutoExpandingScriptableObject {

    protected IntentSpell intent;
    protected List<Prefix> prefixes;

    protected Spell spell { get { return intent.spell; } }
    protected BattleUnit actor { get { return intent.actor; } }
    protected Battle battle { get { return intent.battle; } }
    protected LuaAnimation anim { get { return spell.animation; } }
    protected BattleController controller { get { return intent.battle.controller; } }
    protected BattleAnimationPlayer animator { get { return controller.animator; } }

    public IEnumerator ResolveRoutine(IntentSpell intent, List<Prefix> prefixes) {
        this.intent = intent;
        this.prefixes = prefixes;
        yield return InternalResolveRoutine();
    }

    protected abstract IEnumerator InternalResolveRoutine();

    protected List<BattleUnit> GetLivingTargets() {
        List<BattleUnit> units = new List<BattleUnit>();
        foreach (BattleUnit unit in intent.targets) {
            if (!unit.IsDead()) {
                units.Add(unit);
            }
        }
        return units;
    }

    protected int Range(int low, int high) {
        return (int)Math.Floor(intent.battle.r.NextDouble() * (high - low + 1) + low);
    }
}
