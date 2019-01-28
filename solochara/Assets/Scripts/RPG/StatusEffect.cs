using UnityEngine;
using System.Collections;
using System;

// base abstract classes for status effects
[CreateAssetMenu(fileName = "StatusEffect", menuName = "Data/RPG/StatusEffect")]
public class StatusEffect : ScriptableObject {

    public enum ExpiryTiming {
        AfterAction,
        BeforeAction,
        AfterTurn,
        BeforeTurn,
    }

    public string statusName;
    public Color color;
    public StatTag resistanceStat;

    [Space]
    [Header("Infliction and curing")]
    [Tooltip("Plays on the unit when the status is inflicted")]
    public LuaAnimation inflictAnimation;
    [Tooltip("Plays on the unit when the status is removed (whether by spell or auto)")]
    public LuaAnimation cureAnimation;
    [Tooltip("When inflicted, prints. Replaces $target with the target's name.")]
    public string inflictMessage = "$target was affected!";
    [Tooltip("Prints on the target's turn when affected. Replaces $target with the target's name")]
    public string statusMessage = "$target is under the effect of a status effect.";
    [Tooltip("When cured or expires, prints. Replaces $target with the target's name")]
    public string cureMessage = "$target is cured.";

    [Space]
    [Header("Auto-expiry")]
    [Tooltip("Duration in turns, or 0 for never expires")]
    public int duration;
    [Tooltip("'Action' would be when the enemy makes its move, 'turn' would be when all enemies begin attacking, etc")]
    public ExpiryTiming expiryTiming = ExpiryTiming.AfterTurn;

    [Space]
    [Header("Flags")]
    public bool preventsAction;

    [Space]
    [Header("Damage over time")]
    [Tooltip("Plays when this unit takes damage from the DOT effect")]
    public LuaAnimation dotAnimation;
    [Tooltip("Prints when damage inflicted. $target for victim and $damage for numerical amount")]
    public string damageMessage = "$target took $damage damage";
    public int fixedDamagePerTurn;
    [Tooltip("Percent 0.0 - 1.0, a monster with 50/100 and .1 here would take 10 damage a turn")]
    public float percentMaxDamagePerTurn;
    [Tooltip("Percent 0.0 - 1.0, a monster with 50/100 and .1 here would take 5 damage a turn")]
    public float percentCurrentDamagePerTurn;
}
