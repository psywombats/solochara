using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StatusInstance {

    public StatusEffect effect { get; private set; }
    public BattleUnit unit { get; private set; }

    private int turnsActive;

    public StatusInstance(StatusEffect effect, BattleUnit unit) {
        this.effect = effect;
        this.unit = unit;
    }

    public virtual IEnumerator ActionStartRoutine() {
        if (effect.expiryTiming == StatusEffect.TurnTiming.BeforeAction) {
            yield return DecrementRoutine();
        }
        if (effect.dotTiming == StatusEffect.TurnTiming.BeforeAction) {
            yield return DamageRoutine();
        }
    }

    public virtual IEnumerator ActionEndRoutine() {
        if (effect.expiryTiming == StatusEffect.TurnTiming.AfterAction) {
            yield return DecrementRoutine();
        }
        if (effect.dotTiming == StatusEffect.TurnTiming.AfterAction) {
            yield return DamageRoutine();
        }
    }

    public virtual IEnumerator TurnStartRoutine() {
        if (effect.expiryTiming == StatusEffect.TurnTiming.BeforeTurn) {
            yield return DecrementRoutine();
        }
        if (effect.dotTiming == StatusEffect.TurnTiming.BeforeTurn) {
            yield return DamageRoutine();
        }
    }

    public virtual IEnumerator TurnEndRoutine() {
        if (effect.expiryTiming == StatusEffect.TurnTiming.AfterTurn) {
            yield return DecrementRoutine();
        }
        if (effect.dotTiming == StatusEffect.TurnTiming.AfterTurn) {
            yield return DamageRoutine();
        }
    }

    private IEnumerator DecrementRoutine() {
        turnsActive += 1;
        if (turnsActive >= effect.duration && effect.duration > 0) {
            yield return unit.RemoveStatusRoutine(effect);
        } else {
            if (effect.statusMessage.Length > 0) {
                unit.battle.Log(effect.statusMessage, new Dictionary<string, string>() { ["$target"] = unit.name });
            }
        }
    }

    private IEnumerator DamageRoutine() {
        float damage = effect.fixedDamagePerTurn;
        damage += effect.percentCurrentDamagePerTurn * unit.Get(StatTag.HP);
        damage += effect.percentMaxDamagePerTurn * unit.Get(StatTag.MHP);
        if (damage != 0) {
            unit.battle.Log(effect.damageMessage, new Dictionary<string, string>() {
                ["$target"] = unit.name,
                ["$damage"] = damage.ToString(),
            });
            yield return unit.PlayAnimationRoutine(effect.dotAnimation);
            yield return unit.TakeDamageRoutine((int)damage, false);
        }
    }
}
