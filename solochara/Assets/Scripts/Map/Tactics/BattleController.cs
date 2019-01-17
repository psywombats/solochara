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

    // UI hookup
    public SpellSelector spellSelect;
    public GroupSelector allySelect;
    public GroupSelector enemySelect;
    public GroupSelector allSelect;

    // internal state
    private Dictionary<BattleUnit, Doll> dolls;

    // === INITIALIZATION ==========================================================================

    public BattleController() {
        dolls = new Dictionary<BattleUnit, Doll>();
    }

    // this should take a battle memory at some point
    public void Setup(string battleKey) {
        this.battle = Resources.Load<Battle>("Database/Battles/" + battleKey);
        Debug.Assert(this.battle != null, "Unknown battle key " + battleKey);
    }

    // === GETTERS AND BOOKKEEPING =================================================================

    public Doll GetDollForUnit(BattleUnit unit) {
        return dolls[unit];
    }

    // === STATE MACHINE ===========================================================================

    public IEnumerator BattleBeginRoutine() {
        this.spellSelect.gameObject.SetActive(false);
        yield break;
    }

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
    
    public IEnumerator SelectSpellsRoutine(Result<List<Spell>> result, BattleUnit hero) {
        List<Spell> queuedSpells = new List<Spell>();
        while (hero.Get(StatTag.AP) > 0) {
            Result<Selectable> cardResult = new Result<Selectable>();
            yield return spellSelect.SelectSpellRoutine(hero, cardResult);
            if (cardResult.canceled) {
                // canceled the selection
                if (queuedSpells.Count > 0) {
                    Global.Instance().Audio.PlaySFX("cancel");
                    Spell canceled = queuedSpells.Last();
                    hero.unit.stats.Add(StatTag.AP, canceled.apCost);
                    queuedSpells.RemoveAt(queuedSpells.Count - 1);
                } else {
                    Global.Instance().Audio.PlaySFX("error");
                }
            } else if (cardResult.value.GetComponent<SpellCard>()) {
                // selected a spell
                Spell spell = cardResult.value.GetComponent<SpellCard>().spell;
                if (hero.Get(StatTag.AP) >= spell.apCost) {
                    Global.Instance().Audio.PlaySFX("confirm");
                    queuedSpells.Add(spell);
                    hero.unit.stats.Sub(StatTag.AP, spell.apCost);
                } else {
                    Global.Instance().Audio.PlaySFX("error");
                }
            } else {
                // selected the go ahead
                if (queuedSpells.Count > 0) {
                    Global.Instance().Audio.PlaySFX("confirm");
                    break;
                } else {
                    Global.Instance().Audio.PlaySFX("error");
                }
            }
        }
        result.value = queuedSpells;
    }
}
