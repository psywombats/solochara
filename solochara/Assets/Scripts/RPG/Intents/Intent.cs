using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 *  Similar to MGNE's intent, this indicates an action (usually a spell) and a target, and can be
 *  resolved when ready in battle.
 */
public abstract class Intent {

    public Battle battle { get; private set; }
    public BattleUnit actor { get; private set; }

    public Intent(Battle battle, BattleUnit actor) {
        this.actor = actor;
        this.battle = battle;
    }

    public abstract IEnumerator ResolveRoutine();

    public abstract int APCost();

    public abstract bool ModifiesNextIntent();

    public abstract void ModifyNextIntent(Intent intent);

    public abstract void ConsumePrefixSpell(IntentSpell intent);
}
