using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DamagePopup : MonoBehaviour {

    public DamagePopupDigit digitPrefab;
    public GameObject attachmentPoint;
    public float padding = 0.0f;
    public float digitDelay = 0.1f;

    private List<DamagePopupDigit> digits = new List<DamagePopupDigit>();
    
    public IEnumerator ActivateRoutine(int damage) {
        string damageString = damage.ToString();
        List<IEnumerator> toPlay = new List<IEnumerator>();

        float width = damageString.Length * digitPrefab.width + (damageString.Length - 1) * padding;
        float startX = transform.position.x - width / 2.0f;
        for (int i = 0; i < damageString.Length; i += 1) {
            DamagePopupDigit digit = Instantiate(digitPrefab);
            digit.transform.parent = transform;
            float x = startX + (width + padding) * i;
            digit.transform.position = new Vector3(x, transform.position.y, transform.position.z);
            int n;
            Int32.TryParse(damageString[i].ToString(), out n);
            toPlay.Add(CoUtils.Delay(digitDelay * i, digit.ActivateRoutine(n)));
        }

        yield return CoUtils.RunParallel(toPlay.ToArray(), this);
    }

    public IEnumerator DeactivateRoutine() {
        List<IEnumerator> toPlay = new List<IEnumerator>();
        foreach (DamagePopupDigit digit in digits) {
            toPlay.Add(digit.DeactivateRoutine());
        }
        yield return CoUtils.RunParallel(toPlay.ToArray(), this);
        foreach (DamagePopupDigit digit in digits) {
            Destroy(digit);
        }
    }
}
