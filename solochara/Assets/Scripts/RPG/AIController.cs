﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * A big bad brain that dictates how enemy units take their turns. It's a scriptable object that
 * serializes along with battles, the idea being that "ai easymode" or "ai brutal" can be drag-
 * dropped onto battle instances.
 */
[CreateAssetMenu(fileName = "AIController", menuName = "Data/RPG/AI")]
public class AIController : ScriptableObject {

    private Battle battle;
    private BattleController controller;

    // set up internal state at the start of a battle
    public void ConfigureForBattle(Battle battle) {
        this.battle = battle;
        this.controller = battle.controller;
    }

    // called repeatedly by the battle while ai units still have moves left
    public IEnumerator PlayNextAIActionRoutine() {
        BattleUnit actor = battle.GetFaction(Alignment.Enemy).NextMoveableUnit();

        yield return actor.ActionStartRoutine();
        if (actor.IsDead()) {
            yield break;
        }

        // TODO: AI

        Spell spell = RandomUtils.RandomItem(battle.r, actor.unit.spells);
        IntentSpell intent = new IntentSpell(battle, actor, spell);
        intent.AcquireAITargets();
        yield return intent.ResolveRoutine();

        yield return actor.ActionEndRoutine();

        actor.MarkActionTaken();
        yield return null;
    }
}
