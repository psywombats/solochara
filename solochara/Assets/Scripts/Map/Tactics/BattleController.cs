using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

/**
 * Responsible for user input and rendering during a battle. Control flow is actually handled by
 * the Battle class.
 */
public class BattleController : MonoBehaviour {

    private const string ListenerId = "BattleControllerListenerId";

    // properties required upon initializion
    public Battle battle;

    // UI hookup
    public SpellSelector spellSelect;
    public GroupSelector allySelect;
    public GroupSelector enemySelect;
    public GroupSelector allSelect;
    public BattleAnimationPlayer animator;
    public SpellLinkMeter linker;

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

    public IEnumerator StartBattleRoutine() {
        this.battle.SetUpWithController(this);

        foreach (BattleUnit unit in this.battle.UnitsByAlignment(Alignment.Enemy)) {
            Doll doll = this.enemySelect.AssignNextDoll(unit);
            dolls[unit] = doll;
        }
        foreach (BattleUnit unit in this.battle.UnitsByAlignment(Alignment.Hero)) {
            Doll doll = this.allySelect.AssignNextDoll(unit);
            dolls[unit] = doll;
        }
        
        yield return this.battle.BattleRoutine();
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
    
    public IEnumerator SelectSpellsRoutine(Result<List<Intent>> result, BattleUnit hero) {
        List<Intent> queuedIntents = new List<Intent>();
        List<Spell> previousSpells = new List<Spell>();
        linker.SetUp((int)hero.Get(StatTag.MAP));
        yield return spellSelect.EnableRoutine(hero.unit.spells);
        while (hero.Get(StatTag.AP) > 0) {
            linker.Populate(previousSpells);
            Result<Selectable> cardResult = new Result<Selectable>();
            yield return spellSelect.SelectSpellRoutine(cardResult, previousSpells);
            if (cardResult.canceled) {
                // canceled the selection
                if (queuedIntents.Count > 0) {
                    Global.Instance().Audio.PlaySFX("cancel");
                    Intent canceled = queuedIntents.Last();
                    hero.unit.stats.Add(StatTag.AP, canceled.APCost());
                    queuedIntents.RemoveAt(queuedIntents.Count - 1);
                    previousSpells.RemoveAt(previousSpells.Count - 1);
                }
            } else if (cardResult.value.GetComponent<SpellCard>()) {
                // selected a spell
                Spell spell = cardResult.value.GetComponent<SpellCard>().spell;
                linker.Populate(new List<Spell>(previousSpells) { spell });
                if (hero.Get(StatTag.AP) > spell.apCost ||
                        (hero.Get(StatTag.AP) >= spell.apCost && !spell.LinksToNextSpell())) {
                    Global.Instance().Audio.PlaySFX("confirm");
                    hero.unit.stats.Sub(StatTag.AP, spell.apCost);
                    // find a target for the spell
                    Result<List<BattleUnit>> targetsResult = new Result<List<BattleUnit>>();
                    IntentSpell intent = new IntentSpell(this.battle, hero, spell);
                    yield return intent.AcquireTargetsRoutine(targetsResult);
                    if (!targetsResult.canceled) {
                        queuedIntents.Add(intent);
                        previousSpells.Add(spell);
                    } else {
                        hero.unit.stats.Add(StatTag.AP, spell.apCost);
                    }
                } else {
                    Global.Instance().Audio.PlaySFX("error");
                }
            } else {
                // selected the go ahead
                if (queuedIntents.Count > 0) {
                    Global.Instance().Audio.PlaySFX("confirm");
                    break;
                } else {
                    Global.Instance().Audio.PlaySFX("error");
                }
            }
        }
        yield return spellSelect.DisableRoutine();
        result.value = queuedIntents;
    }
}
