using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Spell : ScriptableObject {

    public string spellName;
    public int apCost;
    public Color pipColor;
    public LuaAnimation animation;

    public abstract IEnumerator ResolveRoutine(Intent intent, List<PrefixSpell> prefixes);

    public abstract IEnumerator AcquireTargetsRoutine(Result<List<BattleUnit>> result, Intent intent);

    public abstract List<BattleUnit> AcquireAITargets(IntentSpell intent);

    public abstract bool LinksToNextSpell();

    public abstract void ModifyIntent(IntentSpell next);

    protected List<BattleUnit> GetAllies(IntentSpell intent) {
        return intent.battle.UnitsByAlignment(intent.actor.align).ToList();
    }

    protected List<BattleUnit> GetEnemies(IntentSpell intent) {
        List<BattleUnit> result = new List<BattleUnit>();
        foreach (BattleFaction faction in intent.battle.GetFactions()) {
            if (!faction.GetUnits().Contains(intent.actor)) {
                result.AddRange(faction.GetUnits());
            }
        }
        return result;
    }

    protected List<BattleUnit> Without(List<BattleUnit> units, BattleUnit toExclude) {
        List<BattleUnit> newUnits = new List<BattleUnit>(units);
        newUnits.Remove(toExclude);
        return newUnits;
    }

    protected List<BattleUnit> RandomLiving(Intent intent, List<BattleUnit> units) {
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

    protected void CopyUnitResult(Result<List<BattleUnit>> target, Result<BattleUnit> source) {
        if (source.canceled) {
            target.Cancel();
        } else {
            target.value = new List<BattleUnit> {
                source.value
            };
        }
    }
}
