using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NumericalBar : MonoBehaviour {

    public SliderBar bar;
    public Text max;
    public Text actual;
    [Tooltip("in units per second")]
    public float defaultSpeed = 50.0f;

    private float currentValue;

    public void Populate(float max, float actual) {
        this.max.text = ((int)max).ToString();
        this.actual.text = ((int)actual).ToString();
        this.bar.Populate(max, actual);
        this.currentValue = actual;
    }

    public IEnumerator AnimateWithTimeRoutine(float max, float actual, float duration) {
        yield return AnimateWithTimeRoutine(max, actual, (max - actual) / duration);
    }

    public IEnumerator AnimateWithSpeedRoutine(float max, float actual, float unitsPerSecond = 0.0f) {
        float rate = unitsPerSecond == 0.0f ? defaultSpeed : unitsPerSecond;
        float sign = currentValue < actual ? 1 : -1;
        while (currentValue != actual) {
            currentValue += sign * unitsPerSecond * Time.deltaTime;
            if (sign > 0 && currentValue > actual) {
                currentValue = actual;
            } else if (sign < 0 && currentValue < actual) {
                currentValue = actual;
            }
            Populate(max, currentValue);
            yield return null;
        }
    }
}
