using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

/**
 * Responsible for user input and rendering during a battle. Control flow is actually handled by
 * the Battle class.
 */
[RequireComponent(typeof(Map))]
public class BattleController : MonoBehaviour {

    private const string ListenerId = "BattleControllerListenerId";

    // properties required upon initializion
    public Battle battle;

    // all of these behaviors read + managed internally
    public Map map { get { return GetComponent<Map>(); } }

    // internal state
    private Dictionary<BattleUnit, DollTargetEvent> dolls;

    // === INITIALIZATION ==========================================================================

    public BattleController() {
        dolls = new Dictionary<BattleUnit, DollTargetEvent>();
    }

    // this should take a battle memory at some point
    public void Setup(string battleKey) {
        this.battle = Resources.Load<Battle>("Database/Battles/" + battleKey);
        Debug.Assert(this.battle != null, "Unknown battle key " + battleKey);
    }

    // === GETTERS AND BOOKKEEPING =================================================================

    public DollTargetEvent GetDollForUnit(BattleUnit unit) {
        return dolls[unit];
    }

    // === STATE MACHINE ===========================================================================

    public IEnumerator TurnBeginAnimationRoutine(Alignment align) {
        yield break;
    }

    public IEnumerator TurnEndAnimationRoutine(Alignment align) {
        List<IEnumerator> routinesToRun = new List<IEnumerator>();
        foreach (BattleUnit unit in battle.UnitsByAlignment(align)) {
            // TODO: new project
            //routinesToRun.Add(unit.doll.PostTurnRoutine());
        }
        yield return CoUtils.RunParallel(routinesToRun.ToArray(), this);
    }
    
    public IEnumerator PlayNextHumanActionRoutine() {
        yield return null;
    }
}
