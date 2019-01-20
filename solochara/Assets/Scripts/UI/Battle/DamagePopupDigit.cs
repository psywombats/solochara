using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteRenderer))]
public class DamagePopupDigit : MonoBehaviour {

    public List<Sprite> digits;

    public float width {
        get { return digits[0].bounds.size.x; }
    }

    public void SetDigit(int digit) {
        GetComponent<SpriteRenderer>().sprite = digits[digit];
    }

    public void AnimateBounce() {

    }
}
