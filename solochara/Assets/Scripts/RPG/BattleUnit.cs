using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// representation of a unit in battle
public class BattleUnit {

    public Unit unit { get; private set; }
    public Battle battle { get; private set; }
    public Alignment align { get { return unit.align; } }
    public bool hasActedThisTurn { get; private set; }

    public Doll doll {
        get {
            return battle.controller.GetDollForUnit(this);
        }
    }

    // === INITIALIZATION ==========================================================================

    // we create battle units from three sources
    //  - unit, this is a keyed by what comes in from tiled and used to look up hero/enemy in db
    //  - battle, the parent battle creating this unit for
    public BattleUnit(Unit unit, Battle battle) {
        this.unit = unit;
        this.battle = battle;
    }

    // === STATE MACHINE ===========================================================================

    // called at the end of this unit's action
    public void MarkActionTaken() {
        hasActedThisTurn = true;
    }

    // called at the beginning of this unit's faction's turn
    public void ResetForNewTurn() {
        hasActedThisTurn = false;
        unit.stats.Set(StatTag.AP, Get(StatTag.MAP));
    }

    public IEnumerator TakeDamageRoutine(int damage) {
        battle.Log(this + " took " + damage + " damages");
        unit.stats.Sub(StatTag.HP, damage);
        yield return doll.damagePopup.ActivateRoutine(damage);
        yield return CoUtils.Wait(0.7f);
        yield return doll.damagePopup.DeactivateRoutine();
        if (IsDead()) {
            yield return DeathRoutine();
        }
    }

    public IEnumerator HealRoutine(int heal) {
        int effectiveHeal = (int) Mathf.Min(heal, Get(StatTag.MHP) - Get(StatTag.HP));
        battle.Log(this + " healed for " + heal);
        unit.stats.Add(StatTag.HP, effectiveHeal);
        yield return null;
    }

    public IEnumerator DeathRoutine() {
        // ??? todo I guess
        battle.Log(this + " died");
        doll.appearance.sprite = null;
        yield return null;
    }

    // === RPG =====================================================================================

    public float Get(StatTag tag) {
        return unit.stats.Get(tag);
    }

    public bool Is(StatTag tag) {
        return unit.stats.Is(tag);
    }

    // checks for deadness and dead-like conditions like petrification
    public bool IsDead() {
        return unit.stats.Get(StatTag.HP) <= 0;
    }

    // === MISC ====================================================================================

    public override string ToString() {
        return unit.ToString();
    }
}
