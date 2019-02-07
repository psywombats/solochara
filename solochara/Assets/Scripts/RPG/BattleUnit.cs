using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// representation of a unit in battle
public class BattleUnit {

    private readonly float MaxStagger = 10.0f;

    public Unit unit { get; private set; }
    public Battle battle { get; private set; }
    public Alignment align { get { return unit.align; } }

    public List<StatusInstance> statuses { get; private set; }
    public bool hasActedThisTurn { get; private set; }

    public Doll doll {
        get {
            return battle.controller.GetDollForUnit(this);
        }
    }

    public string name {
        get {
            return unit.unitName;
        }
    }

    // === INITIALIZATION ==========================================================================

    // we create battle units from three sources
    //  - unit, this is a keyed by what comes in from tiled and used to look up hero/enemy in db
    //  - battle, the parent battle creating this unit for
    public BattleUnit(Unit unit, Battle battle) {
        this.unit = unit;
        this.battle = battle;

        statuses = new List<StatusInstance>();

        if (align == Alignment.Hero) {
            battle.controller.playerHP.Populate(Get(StatTag.MHP), Get(StatTag.HP));
        }
    }

    // === STATE MACHINE ===========================================================================

    // called at the end of this unit's action
    public void MarkActionTaken() {
        hasActedThisTurn = true;
    }

    // called at the beginning of this unit's faction's turn
    public IEnumerator TurnStartRoutine() {
        hasActedThisTurn = false;
        unit.stats.Set(StatTag.AP, Get(StatTag.MAP));
        
        foreach (StatusInstance status in statuses) {
            yield return status.TurnStartRoutine();
        }
    }

    public IEnumerator TurnEndRoutine() {
        foreach (StatusInstance status in statuses) {
            yield return status.TurnEndRoutine();
        }
    }

    public IEnumerator ActionStartRoutine() {
        foreach (StatusInstance status in statuses) {
            yield return status.ActionStartRoutine();
        }
        if (align == Alignment.Enemy) {
            yield return battle.controller.enemyHUD.enableRoutine(this);
        }
    }

    public IEnumerator ActionEndRoutine() {
        foreach (StatusInstance status in statuses) {
            yield return status.ActionEndRoutine();
        }
    }

    public IEnumerator TakeDamageRoutine(int damage, bool includeMessage = true) {
        battle.Log(this + " took " + damage + " damages");
        if (align == Alignment.Hero) {
            unit.stats.Sub(StatTag.HP, damage);
            yield return CoUtils.RunParallel(new IEnumerator[] {
                        doll.damagePopup.DamageRoutine(damage),
                        battle.controller.playerHP.AnimateWithSpeedRoutine(Get(StatTag.MHP), Get(StatTag.HP)),
                        CoUtils.Wait(0.4f),
                    }, battle.controller);
            yield return CoUtils.Wait(0.3f);
        } else {
            yield return battle.controller.enemyHUD.enableRoutine(this);
            unit.stats.Sub(StatTag.HP, damage);
            yield return CoUtils.RunParallel(new IEnumerator[] {
                        doll.damagePopup.DamageRoutine(damage),
                        battle.controller.enemyHUD.hpBar.AnimateWithSpeedRoutine(Get(StatTag.HP) / Get(StatTag.MHP)),
                        CoUtils.Wait(0.4f),
                    }, battle.controller);
            yield return CoUtils.Wait(0.3f);
            yield return battle.controller.enemyHUD.disableRoutine();
        }
        yield return doll.damagePopup.DeactivateRoutine();
        if (IsDead()) {
            yield return DeathRoutine();
        }
    }

    public IEnumerator TakeStaggerRoutine(float stagger) {
        battle.Log(this + " staggered by " + stagger + " staggerdamagers.");
        if (align == Alignment.Hero) {
            unit.stats.Sub(StatTag.STAGGER, stagger);
            yield return CoUtils.RunParallel(new IEnumerator[] {
                        doll.damagePopup.StaggerDamageRoutine((int)stagger),
                        battle.controller.playerStagger.AnimateWithSpeedRoutine(MaxStagger, Get(StatTag.STAGGER)),
                        CoUtils.Wait(0.4f),
                    }, battle.controller);
            yield return CoUtils.Wait(0.3f);
        } else {
            yield return battle.controller.enemyHUD.enableRoutine(this);
            unit.stats.Sub(StatTag.STAGGER, stagger);
            yield return CoUtils.RunParallel(new IEnumerator[] {
                        doll.damagePopup.StaggerDamageRoutine((int)stagger),
                        battle.controller.enemyHUD.stagger.AnimateWithSpeedRoutine(MaxStagger / Get(StatTag.STAGGER)),
                        CoUtils.Wait(0.4f),
                    }, battle.controller);
            yield return CoUtils.Wait(0.3f);
            yield return battle.controller.enemyHUD.disableRoutine();
        }
        yield return doll.damagePopup.DeactivateRoutine();
    }

    public IEnumerator HealRoutine(int heal) {
        int effectiveHeal = (int) Mathf.Min(heal, Get(StatTag.MHP) - Get(StatTag.HP));
        battle.Log(this + " healed for " + heal);
        unit.stats.Add(StatTag.HP, effectiveHeal);
        yield return null;
    }

    public IEnumerator DeathRoutine() {
        battle.Log(this + " died");
        yield return PlayAnimationRoutine(doll.deathAnimation);
        doll.appearance.sprite = null;
        yield return null;
    }

    // inflict power is a prerolled chance 0.0-1.0, we'll check against it
    public IEnumerator TryInflictStatusRoutine(StatusEffect effect, float baseInflictPower) {
        float resistance = 0.0f;
        if (effect.resistanceStat != StatTag.None) {
            resistance += Get(effect.resistanceStat);
        }
        if (baseInflictPower > resistance || Has(effect)) {
            yield return InflictStatusRoutine(effect);
        } else {
            battle.Log(name + " resisted.");
            yield return CoUtils.Wait(0.7f);
        }
    }

    public IEnumerator InflictStatusRoutine(StatusEffect effect) {
        battle.Log(effect.inflictMessage, new Dictionary<string, string>() { ["$target"] = name });
        yield return PlayAnimationRoutine(effect.inflictAnimation);
        statuses.Add(new StatusInstance(effect, this));
    }

    public IEnumerator RemoveStatusRoutine(StatusEffect effect) {
        battle.Log(effect.cureMessage, new Dictionary<string, string>() { ["$target"] = name });
        if (effect.cureAnimation != null) {
            yield return PlayAnimationRoutine(effect.cureAnimation);
        } else {
            yield return CoUtils.Wait(0.7f);
        }
        statuses = statuses.Where(status => status.effect != effect).ToList();
    }

    // === RPG =====================================================================================

    public float Get(StatTag tag) {
        return unit.stats.Get(tag);
    }

    public bool Is(StatTag tag) {
        return unit.stats.Is(tag);
    }

    public bool Has(StatusEffect effect) {
        foreach (StatusInstance status in this.statuses) {
            if (status.effect == effect) {
                return true;
            }
        }
        return false;
    }

    // checks for deadness and dead-like conditions like petrification
    public bool IsDead() {
        return unit.stats.Get(StatTag.HP) <= 0;
    }

    // === MISC ====================================================================================

    public IEnumerator PlayAnimationRoutine(LuaAnimation animation) {
        yield return doll.PlayAnimationRoutine(animation);
    }

    public override string ToString() {
        return unit.ToString();
    }
}
