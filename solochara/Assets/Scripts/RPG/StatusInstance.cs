using UnityEngine;
using System.Collections;

public class StatusInstance {

    public StatusEffect effect { get; private set; }
    public BattleUnit unit { get; private set; }

    private int turnsActive;

    public StatusInstance(StatusEffect effect, BattleUnit unit) {
        this.effect = effect;
        this.unit = unit;
    }

    public virtual IEnumerator ActionStartRoutine() {
        if (effect.expiryTiming == StatusEffect.ExpiryTiming.BeforeAction) {
            yield return DecrementRoutine();
        }
    }

    public virtual IEnumerator ActionEndRoutine() {
        if (effect.expiryTiming == StatusEffect.ExpiryTiming.AfterAction) {
            yield return DecrementRoutine();
        }
    }

    public virtual IEnumerator TurnStartRoutine() {
        if (effect.expiryTiming == StatusEffect.ExpiryTiming.BeforeTurn) {
            yield return DecrementRoutine();
        }
    }

    public virtual IEnumerator TurnEndRoutine() {
        if (effect.expiryTiming == StatusEffect.ExpiryTiming.AfterTurn) {
            yield return DecrementRoutine();
        }
    }

    private IEnumerator DecrementRoutine() {
        turnsActive += 1;
        if (turnsActive >= effect.duration) {
            yield return unit.RemoveStatusRoutine(effect);
        }
    }
}
