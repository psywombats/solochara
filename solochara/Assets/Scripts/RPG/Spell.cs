using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Spell", menuName = "Data/RPG/Spell")]
public class Spell : ScriptableObject {

    public string spellName;
    public int apCost;
    public TargetType targets;

    public IEnumerator ResolveRoutine(Intent intent) {

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
                yield return controller.allySelect.SelectSpecificallyRoutine(intent);
                break;
        }
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
