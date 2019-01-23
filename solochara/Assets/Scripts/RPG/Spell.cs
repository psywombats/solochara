using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Spell", menuName = "Data/RPG/Spell")]
public class Spell : ScriptableObject {

    public string spellName;
    public int apCost;
    public LuaAnimation animation;
    public TargetType targets;
    public Warhead warhead;

    public IEnumerator ResolveRoutine(Intent intent) {
        yield return warhead.ResolveRoutine((IntentSpell) intent);
    }

    public IEnumerator AcquireTargetsRoutine(Result<List<BattleUnit>> result, Intent intent) {
        BattleController controller = intent.battle.controller;
        Result<BattleUnit> unitResult = new Result<BattleUnit>();
        switch (targets) {
            case TargetType.All:
                yield return controller.allSelect.SelectAllRoutine(result, intent);
                break;
            case TargetType.AllAllies:
                yield return controller.allySelect.SelectAllRoutine(result, intent);
                break;
            case TargetType.AllEnemies:
                yield return controller.enemySelect.SelectAllRoutine(result, intent);
                break;
            case TargetType.AllNotSelf:
                yield return controller.allSelect.SelectAllExceptRoutine(result, intent);
                break;
            case TargetType.Ally:
                yield return controller.allySelect.SelectAnyOneRoutine(unitResult, intent);
                CopyUnitResult(result, unitResult);
                break;
            case TargetType.AllyNotSelf:
                yield return controller.allySelect.SelectAnyExceptRoutine(unitResult, intent);
                CopyUnitResult(result, unitResult);
                break;
            case TargetType.Anyone:
                yield return controller.allSelect.SelectAnyOneRoutine(unitResult, intent);
                CopyUnitResult(result, unitResult);
                break;
            case TargetType.AnyoneNotSelf:
                yield return controller.allSelect.SelectAnyExceptRoutine(unitResult, intent);
                CopyUnitResult(result, unitResult);
                break;
            case TargetType.Enemy:
                yield return controller.enemySelect.SelectAnyOneRoutine(unitResult, intent);
                CopyUnitResult(result, unitResult);
                break;
            case TargetType.Self:
                yield return controller.allySelect.SelectSpecificallyRoutine(unitResult, intent);
                CopyUnitResult(result, unitResult);
                break;
        }
    }

    public List<BattleUnit> AcquireAITargets(IntentSpell intent) {
        switch (targets) {
            case TargetType.All:
                return intent.battle.AllUnits().ToList();
            case TargetType.AllAllies:
                return GetAllies(intent);
            case TargetType.AllEnemies:
                return GetEnemies(intent);
            case TargetType.AllNotSelf:
                return Without(intent.battle.AllUnits().ToList(), intent.actor);
            case TargetType.Ally:
                return RandomLiving(intent, GetAllies(intent));
            case TargetType.AllyNotSelf:
                return RandomLiving(intent, Without(GetAllies(intent), intent.actor));  case TargetType.Anyone:
            case TargetType.Enemy:
            case TargetType.AnyoneNotSelf:
                return RandomLiving(intent, GetEnemies(intent));
            case TargetType.Self:
                return new List<BattleUnit> { intent.actor };
            default:
                return null;
        }
    }
    
    private List<BattleUnit> GetAllies(IntentSpell intent) {
        return intent.battle.UnitsByAlignment(intent.actor.align).ToList();
    }

    private List<BattleUnit> GetEnemies(IntentSpell intent) {
        List<BattleUnit> result = new List<BattleUnit>();
        foreach (BattleFaction faction in intent.battle.GetFactions()) {
            if (!faction.GetUnits().Contains(intent.actor)) {
                result.AddRange(faction.GetUnits());
            }
        }
        return result;
    }

    private List<BattleUnit> Without(List<BattleUnit> units, BattleUnit toExclude) {
        List<BattleUnit> newUnits = new List<BattleUnit>(units);
        newUnits.Remove(toExclude);
        return newUnits;
    }

    private List<BattleUnit> RandomLiving(Intent intent, List<BattleUnit> units) {
        List<BattleUnit> living = new List<BattleUnit>();
        foreach (BattleUnit unit in units) {
            if (!unit.IsDead()) {
                living.Add(unit);
            }
        }
        if (living.Count == 0) {
            return living;
        }
        return new List<BattleUnit>() { RandomUtils.RandomItem(intent.battle.r, living) };
    }

    private void CopyUnitResult(Result<List<BattleUnit>> target, Result<BattleUnit> source) {
        if (source.canceled) {
            target.Cancel();
        } else {
            target.value = new List<BattleUnit>();
            target.value.Add(source.value);
        }
    }
}
