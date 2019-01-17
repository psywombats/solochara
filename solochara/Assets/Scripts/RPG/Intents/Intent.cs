using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 *  Similar to MGNE's intent, this indicates an action (usually a spell) and a target, and can be
 *  resolved when ready in battle.
 */
public abstract class Intent {

    private Battle battle;
    private List<BattleUnit> targets;

    public Intent(Battle battle, List<BattleUnit> targets) {
        this.battle = battle;
        this.targets = targets;
    }

    public abstract IEnumerator ResolveRoutine();
}
