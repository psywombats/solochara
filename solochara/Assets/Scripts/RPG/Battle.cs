using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/**
 * A battle in progress. Responsible for all battle logic, state, and control flow. The actual
 * battle visual representation is contained in the BattleController. 
 * 
 * Flow for battles works like this:
 *  - A BattleController is created
 *  - The BattleController loads a serialized instance of this class via key
 *  - All the Tiled events participating in the battle register to the controller using the 'unit'
 *    key, and we then register them here
 */
[CreateAssetMenu(fileName = "Battle", menuName = "Data/RPG/Battle")]
public class Battle : ScriptableObject {

    public AIController ai;
    public List<Unit> initialUnits;

    public BattleController controller { get; private set; }
    public System.Random r { get; private set; }
    private List<BattleUnit> units = new List<BattleUnit>();
    private Dictionary<Alignment, BattleFaction> factions = new Dictionary<Alignment, BattleFaction>();

    // === INITIALIZATION===========================================================================

    public void SetUpWithController(BattleController controller) {
        this.controller = controller;
        this.r = new System.Random();

        foreach (Unit baseUnit in this.initialUnits) {
            Unit unit = baseUnit.unique ? UnitFromKey(baseUnit.name) : Instantiate(baseUnit);
            AddUnit(new BattleUnit(unit, this));
        }

        this.ai.ConfigureForBattle(this);
    }

    public void AddUnit(BattleUnit unit) {
        units.Add(unit);
        if (!factions.ContainsKey(unit.align)) {
            factions[unit.align] = new BattleFaction(this, unit.align);
        }
    }

    public Unit UnitFromKey(string unitKey) {
        Unit unit = Global.Instance().Party.LookUpUnit(unitKey);
        Debug.Assert(unit != null, "Unknown unit key " + unitKey);

        return unit;
    }


    // === BOOKKEEPING AND GETTERS =================================================================

    public ICollection<BattleUnit> AllUnits() {
        return units;
    }

    public IEnumerable<BattleUnit> UnitsByAlignment(Alignment align) {
        return units.Where(unit => (unit.align == align));
    }
    public List<BattleFaction> GetFactions() {
        return new List<BattleFaction>(factions.Values);
    }

    public BattleFaction GetFaction(Alignment align) {
        return this.factions[align];
    }

    // === STATE MACHINE ===========================================================================

    // runs and executes this battle
    public IEnumerator BattleRoutine() {
        while (true) {
            yield return NextRoundRoutine();
            if (CheckGameOver() != Alignment.None) {
                yield break;
            }
        }
    }
    
    // coroutine to play out a single round of combat
    private IEnumerator NextRoundRoutine() {
        yield return PlayTurnRoutine(Alignment.Hero);
        if (CheckGameOver() != Alignment.None) {
            yield break;
        }
        yield return PlayTurnRoutine(Alignment.Enemy);
        if (CheckGameOver() != Alignment.None) {
            yield break;
        }
    }

    // returns which alignment won the game, or Alignment.None if no one did
    private Alignment CheckGameOver() {
        foreach (BattleFaction faction in factions.Values) {
            if (faction.HasWon()) {
                return faction.align;
            } else if (faction.align == Alignment.Hero && faction.HasLost()) {
                return Alignment.Enemy;
            } else if (faction.align == Alignment.Enemy && faction.HasLost()) {
                return Alignment.Hero;
            }
        }
        return Alignment.None;
    }

    // responsible for changing ui state to this unit's turn, then 
    private IEnumerator PlayTurnRoutine(Alignment align) {
        if (!factions.ContainsKey(align)) {
            yield break;
        }
        factions[align].ResetForNewTurn();
        yield return controller.TurnBeginAnimationRoutine(align);
        while (factions[align].HasUnitsLeftToAct()) {
            yield return PlayNextActionRoutine(align);
        }
        yield return controller.TurnEndAnimationRoutine(align);
    }

    private IEnumerator PlayNextActionRoutine(Alignment align) {
        switch (align) {
            case Alignment.Hero:
                yield return PlayHumanTurnRoutine();
                break;
            case Alignment.Enemy:
                yield return ai.PlayNextAIActionRoutine();
                yield break;
            default:
                Debug.Assert(false, "bad align " + align);
                yield break;
        }
    }

    private IEnumerator PlayHumanTurnRoutine() {
        BattleUnit hero = this.GetFaction(Alignment.Hero).GetUnits().First();
        Result<List<Intent>> intentsResult = new Result<List<Intent>>();
        yield return controller.SelectSpellsRoutine(intentsResult, hero);

        List<Intent> prefixBuffer = new List<Intent>();
        foreach (Intent intent in intentsResult.value) {
            if (intent.ModifiesNextIntent()) {
                prefixBuffer.Add(intent);
            } else {
                foreach (Intent prefix in prefixBuffer) {
                    prefix.ModifyNextIntent(intent);
                }
                prefixBuffer.Clear();
            }
            yield return CoUtils.Wait(0.8f);
            yield return intent.ResolveRoutine();
        }

        hero.MarkActionTaken();
    }

    // === MISC ====================================================================================

    public void Log(string message) {
        Global.Instance().UIEngine.DebugBox.text = message;
    }
}
