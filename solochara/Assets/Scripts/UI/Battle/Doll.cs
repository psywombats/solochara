using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using System;

[MoonSharpUserData]
public class Doll : Selectable {

    private static string AnimPath = "Sprites/Anim/";
    private static string ArgDuration = "duration";
    private static string ArgSpritesheet = "sheet";
    private static string ArgFrame = "frame";
    private static string ArgFrames = "frames";
    private static string ArgCount = "count";
    private static string ArgEnable = "enable";
    private static string ArgDisable = "disable";
    private static string ArgPower = "power";
    private static string ArgSpeed = "speed";
    private static string ArgRed = "r";
    private static string ArgBlue = "b";
    private static string ArgGreen = "g";

    private static float DefaultFrameDuration = 0.12f;
    private static float DefaultJumpHeight = 1.2f;
    private static float DefaultJumpReturnHeight = 0.4f;

    public enum Type {
        Attacker,
        Defender,
    }

    public SpriteRenderer appearance;
    
    public BattleAnimationPlayer player { get; private set; }
    public BattleUnit unit { get; private set; }

    private Vector3 originalDollPos;
    private Type type;

    private CharaAnimator _animator;
    public CharaAnimator animator {
        get {
            if (_animator == null) {
                _animator = GetComponent<CharaAnimator>();
            }
            return _animator;
        }
    }

    [MoonSharpHidden]
    public void Populate(BattleUnit unit) {
        this.unit = unit;
        if (unit != null) {
            this.appearance.sprite = unit.unit.appearance;
        } else {
            this.appearance.sprite = null;
        }
        selected = false;
    }

    [MoonSharpHidden]
    public void PrepareForAnimation(BattleAnimationPlayer player, Type type) {
        this.player = player;
        this.type = type;
        if (animator != null) {
            animator.PrepareForAnimation();
        }
        originalDollPos = transform.position;
    }

    [MoonSharpHidden]
    public void ResetAfterAnimation() {
        if (animator != null) {
            animator.ResetAfterAnimation();
        }
        transform.position = originalDollPos;
        // GetComponent<AfterimageComponent>().enabled = false;
    }

    // === COMMAND HELPERS =========================================================================
    
    private void CSRun(IEnumerator routine, DynValue args) {
        StartCoroutine(routine);
    }
    
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
    
    private float FloatArg(DynValue args, string argName, float defaultValue) {
        if (args == DynValue.Nil || args == null || args.Table == null) {
            return defaultValue;
        } else {
            DynValue value = args.Table.Get(argName);
            return (value == DynValue.Nil) ? defaultValue : (float)value.Number;
        }
    }
    
    private bool BoolArg(DynValue args, string argName, bool defaultValue) {
        if (args == DynValue.Nil || args == null || args.Table == null) {
            return defaultValue;
        } else {
            DynValue value = args.Table.Get(argName);
            return (value == DynValue.Nil) ? defaultValue : value.Boolean;
        }
    }
    
    private bool EnabledArg(DynValue args, bool defaultValue = true) {
        if (args == DynValue.Nil || args == null || args.Table == null) {
            return defaultValue;
        } else {
            return BoolArg(args, ArgEnable, !BoolArg(args, ArgDisable, !defaultValue));
        }
    }

    private IEnumerator ColorRoutine(DynValue args, float a, Func<Color> getColor, Action<Color> applyColor) {
        float elapsed = 0.0f;
        float duration = FloatArg(args, ArgDuration, 0.4f);
        float speed = FloatArg(args, ArgSpeed, 0.2f);
        Vector3 startPos = transform.localPosition;
        float r = (float)args.Table.Get(ArgRed).Number;
        float g = (float)args.Table.Get(ArgGreen).Number;
        float b = (float)args.Table.Get(ArgBlue).Number;
        Color originalColor = getColor();
        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            float t;
            if (elapsed < speed) {
                t = elapsed / speed;
            } else if (elapsed > (duration - speed)) {
                t = (duration - elapsed) / speed;
            } else {
                t = 1.0f;
            }
            Color color = new Color(
                Mathf.Lerp(originalColor.r, r, t),
                Mathf.Lerp(originalColor.g, g, t),
                Mathf.Lerp(originalColor.b, b, t),
                Mathf.Lerp(originalColor.a, a, t));
            applyColor(color);
            yield return null;
        }
        applyColor(originalColor);
    }

    // === LUA FUNCTIONS ===========================================================================

    // jumpToDefender({});
    public void jumpToDefender(DynValue args) { CSRun(cs_jumpToDefender(args), args); }
    private IEnumerator cs_jumpToDefender(DynValue args) {
        Vector3 endPos = CalculateJumpOffset(transform.position, player.defender.transform.position);
        float duration = (float)args.Table.Get(ArgDuration).Number;
        yield return JumpRoutine(endPos, duration, DefaultJumpHeight);
    }

    // jumpReturn({duration?});
    public void jumpReturn(DynValue args) { CSRun(cs_jumpReturn(args), args); }
    private IEnumerator cs_jumpReturn(DynValue args) {
        float overallDuration = (float)args.Table.Get(ArgDuration).Number;
        float fraction = (2.0f / 3.0f);
        Vector3 midPos = Vector3.Lerp(transform.position, originalDollPos, fraction);
        yield return JumpRoutine(midPos, 
                    overallDuration * fraction, 
                    DefaultJumpReturnHeight * fraction);
        yield return JumpRoutine(originalDollPos, 
                overallDuration * (1.0f - fraction) * 1.5f, 
                DefaultJumpReturnHeight * (1.0f - fraction));
    }

    // setFrame({sheet, frame});
    public void setFrame(DynValue args) {
        string spriteName = args.Table.Get(ArgSpritesheet).String;
        int spriteFrame = (int)args.Table.Get(ArgFrame).Number;
        Sprite[] sprites = Resources.LoadAll<Sprite>(AnimPath + spriteName);
        Sprite sprite = sprites[spriteFrame];
        animator.SetOverrideSprite(sprite);
    }

    // setAnim({sheet, frame[]}, duration?);
    public void setAnim(DynValue args) {
        string spriteName = args.Table.Get(ArgSpritesheet).String;
        float frameDuration = FloatArg(args, ArgDuration, DefaultFrameDuration);
        Sprite[] sprites = Resources.LoadAll<Sprite>(AnimPath + spriteName);
        List<Sprite> frames = new List<Sprite>();
        foreach (DynValue value in args.Table.Get(ArgFrames).Table.Values) {
            frames.Add(sprites[(int)value.Number]);
        }
        animator.SetOverrideAnim(frames, frameDuration);
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

    // quake({power? duration?})
    public void quake(DynValue args) { CSRun(cs_quake(args), args); }
    private IEnumerator cs_quake(DynValue args) {
        float elapsed = 0.0f;
        float duration = FloatArg(args, ArgDuration, 0.25f);
        float power = FloatArg(args, ArgPower, 0.2f);
        DuelCam cam = DuelCam.Instance();
        Vector3 camPosition = cam.transform.localPosition;
        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            cam.transform.localPosition = new Vector3(
                    camPosition.x + UnityEngine.Random.Range(-power, power),
                    camPosition.y + UnityEngine.Random.Range(-power, power),
                    camPosition.z);
            yield return null;
        }
        cam.transform.localPosition = camPosition;
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

    // tint({r, g, b, duration?, speed?})
    public void tint(DynValue args) { CSRun(cs_tint(args), args); }
    private IEnumerator cs_tint(DynValue args) {
        yield return ColorRoutine(args, 1.0f, () => {
            return appearance.color;
        }, (Color c) => {
            appearance.color = c;
        });
    }

    // flash({r, g, b, duration?, speed?, power?})
    public void flash(DynValue args) { CSRun(cs_flash(args), args); }
    private IEnumerator cs_flash(DynValue args) {
        float r = (float)args.Table.Get(ArgRed).Number;
        float g = (float)args.Table.Get(ArgGreen).Number;
        float b = (float)args.Table.Get(ArgBlue).Number;
        Color color = new Color(r, g, b, 0.0f);
        yield return ColorRoutine(args, FloatArg(args, ArgPower, 0.9f), () => {
            return color;
        }, (Color c) => {
            color = c;
            appearance.material.SetColor("_Flash", c);
        });
    }
}
