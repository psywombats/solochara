using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(AnimationPlayer))]
public class DamagePopupDigit : MonoBehaviour {

    public List<Sprite> digits;
    public LuaAnimation activateAnimation;
    public LuaAnimation deactivateAnimation;

    public float width {
        get { return digits[0].bounds.size.x; }
    }

    public void Start() {
        Color c = GetComponent<SpriteRenderer>().color;
        GetComponent<SpriteRenderer>().color = new Color(c.r, c.g, c.b, 0.0f);
    }

    public IEnumerator ActivateRoutine (int digit) {
        GetComponent<SpriteRenderer>().sprite = digits[digit];
        yield return GetComponent<AnimationPlayer>().PlayAnimationRoutine(activateAnimation);
    }

    public IEnumerator DeactivateRoutine() {
        yield return GetComponent<AnimationPlayer>().PlayAnimationRoutine(deactivateAnimation);
    }
}
