using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DamagePopup : MonoBehaviour {

    public DamagePopupDigit digitPrefab;
    public GameObject attachmentPoint;
    public Color damageColor = new Color(1, 0, 0);
    public Color staggerColor = new Color(1, 1, 0);
    public float padding = 1.0f;
    public float digitDelay = 0.1f;

    private List<DamagePopupDigit> digits = new List<DamagePopupDigit>();

    public IEnumerator DamageRoutine(int damage) {
        foreach (DamagePopupDigit digit in digits) {
            digit.SetColor(damageColor);
        }
        yield return ActivateRoutine(damage);
    }

    public IEnumerator StaggerDamageRoutine(int damage) {
        foreach (DamagePopupDigit digit in digits) {
            digit.SetColor(staggerColor);
        }
        yield return ActivateRoutine(damage);
    }

    public IEnumerator DeactivateRoutine() {
        List<IEnumerator> toPlay = new List<IEnumerator>();
        foreach (DamagePopupDigit digit in digits) {
            toPlay.Add(digit.DeactivateRoutine());
        }
        yield return CoUtils.RunParallel(toPlay.ToArray(), this);
        foreach (DamagePopupDigit digit in digits) {
            Destroy(digit.gameObject);
        }
        digits.Clear();
    }

    protected IEnumerator ActivateRoutine(int damage) {
        string damageString = damage.ToString();
        List<IEnumerator> toPlay = new List<IEnumerator>();

        float width = damageString.Length * digitPrefab.width + (damageString.Length - 1) * padding;
        for (int i = 0; i < damageString.Length; i += 1) {
            DamagePopupDigit digit = Instantiate(digitPrefab);
            digits.Add(digit);
            digit.transform.parent = transform;
            float x = (digitPrefab.width + padding) * i - width / 2.0f;
            digit.transform.localPosition = new Vector3(x, 0.0f, 0.0f);
            int n;
            int.TryParse(damageString[i].ToString(), out n);
            toPlay.Add(CoUtils.Delay(digitDelay * i, digit.ActivateRoutine(n)));
        }

        yield return CoUtils.RunParallel(toPlay.ToArray(), this);
    }
}
