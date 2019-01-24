using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "Base Spell", menuName = "Data/RPG/Base Spell")]
public class BaseSpell : Spell {

    public TargetType targets;
    public Warhead warhead;

    public override IEnumerator ResolveRoutine(Intent intent) {
        yield return warhead.ResolveRoutine((IntentSpell)intent);
    }

    public override bool LinksToNextSpell() {
        return false;
    }

    public override IEnumerator AcquireTargetsRoutine(Result<List<BattleUnit>> result, Intent intent) {
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

    public override List<BattleUnit> AcquireAITargets(IntentSpell intent) {
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
                return RandomLiving(intent, Without(GetAllies(intent), intent.actor));
            case TargetType.Anyone:
            case TargetType.Enemy:
            case TargetType.AnyoneNotSelf:
                return RandomLiving(intent, GetEnemies(intent));
            case TargetType.Self:
                return new List<BattleUnit> { intent.actor };
            default:
                return null;
        }
    }
}
