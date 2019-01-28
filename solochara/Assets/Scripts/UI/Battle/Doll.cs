using UnityEngine;
using System.Collections;
using MoonSharp.Interpreter;

[MoonSharpUserData]
[RequireComponent(typeof(Selectable))]
[RequireComponent(typeof(AnimationPlayer))]
public class Doll : AnimationTarget {

    private static readonly float DefaultJumpHeight = 1.2f;
    private static readonly float DefaultJumpReturnHeight = 0.4f;

    public enum Type {
        Attacker,
        Defender,
    }
    
    public BattleAnimationPlayer battleAnimPlayer { get; private set; }
    public BattleUnit unit { get; private set; }

    public DamagePopup damagePopup;
    public LuaAnimation deathAnimation;

    private Type type;

    [MoonSharpHidden]
    public void Populate(BattleUnit unit) {
        this.unit = unit;
        if (unit != null) {
            this.appearance.sprite = unit.unit.appearance;
        } else {
            this.appearance.sprite = null;
        }
        GetComponent<Selectable>().selected = false;
    }

    [MoonSharpHidden]
    public void PrepareForBattleAnimation(BattleAnimationPlayer player, Type type) {
        PrepareForAnimation();
        this.battleAnimPlayer = player;
        this.type = type;
        if (animator != null) {
            animator.PrepareForAnimation();
        }
    }

    [MoonSharpHidden]
    public override void ResetAfterAnimation() {
        base.ResetAfterAnimation();
        if (animator != null) {
            animator.ResetAfterAnimation();
        }
        if (GetComponent<AfterimageComponent>() != null) {
            GetComponent<AfterimageComponent>().enabled = false;
        }
    }

    public IEnumerator PlayAnimationRoutine(LuaAnimation animation) {
        yield return GetComponent<AnimationPlayer>().PlayAnimationRoutine(animation);
    }

    // === COMMAND HELPERS =========================================================================

    private Vector3 CalculateJumpOffset(Vector3 startPos, Vector3 endPos) {
        Vector3 dir = (endPos - startPos).normalized;
        return endPos - 0.75f * dir;
    }

    private IEnumerator JumpRoutine(Vector3 endPos, float duration, float height) {
        Vector3 startPos = transform.position;
        float elapsed = 0.0f;
        while (transform.position != endPos) {
            elapsed += Time.deltaTime;
            Vector3 lerped = Vector3.Lerp(startPos, endPos, elapsed / duration);
            transform.position = new Vector3(
                lerped.x,
                lerped.y + ((elapsed >= duration)
                    ? 0
                    : Mathf.Sin(elapsed / duration * Mathf.PI) * height),
                lerped.z);
            yield return null;
        }
    }

    // === LUA FUNCTIONS ===========================================================================

    // jumpToDefender({});
    public void jumpToDefender(DynValue args) { CSRun(cs_jumpToDefender(args), args); }
    private IEnumerator cs_jumpToDefender(DynValue args) {
        Vector3 endPos = CalculateJumpOffset(transform.position, battleAnimPlayer.defender.transform.position);
        float duration = (float)args.Table.Get(ArgDuration).Number;
        yield return JumpRoutine(endPos, duration, DefaultJumpHeight);
    }

    // jumpReturn({duration?});
    public void jumpReturn(DynValue args) { CSRun(cs_jumpReturn(args), args); }
    private IEnumerator cs_jumpReturn(DynValue args) {
        float overallDuration = (float)args.Table.Get(ArgDuration).Number;
        float fraction = (2.0f / 3.0f);
        Vector3 midPos = Vector3.Lerp(transform.position, originalPos, fraction);
        yield return JumpRoutine(midPos,
                    overallDuration * fraction,
                    DefaultJumpReturnHeight * fraction);
        yield return JumpRoutine(originalPos,
                overallDuration * (1.0f - fraction) * 1.5f,
                DefaultJumpReturnHeight * (1.0f - fraction));
    }

    // afterimage({enable?, count?, duration?});
    public void afterimage(DynValue args) {
        AfterimageComponent imager = GetComponent<AfterimageComponent>();
        if (EnabledArg(args)) {
            float imageDuration = FloatArg(args, ArgDuration, 0.05f);
            int count = (int)FloatArg(args, ArgCount, 3);
            imager.enabled = true;
            imager.afterimageCount = count;
            imager.afterimageDuration = imageDuration;
        } else {
            imager.enabled = false;
        }
    }

    // strike({power? duration?})
    public void strike(DynValue args) { CSRun(cs_strike(args), args); }
    private IEnumerator cs_strike(DynValue args) {
        float elapsed = 0.0f;
        float duration = FloatArg(args, ArgDuration, 0.4f);
        float power = FloatArg(args, ArgPower, 16f);
        Vector3 startPos = transform.localPosition;
        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            transform.localPosition = new Vector3(
                    startPos.x + UnityEngine.Random.Range(-power, power),
                    startPos.y,
                    startPos.z);
            yield return null;
        }
        transform.localPosition = startPos;
    }
}
