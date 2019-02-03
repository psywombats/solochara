using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SliderBar : MonoBehaviour {

    public Mask backer;
    public Image bar;
    public int inset = 16;
    public float ratio = 0.5f;

    public void OnValidate() {
        UpdateScale();
    }

    public void Populate(float max, float actual) {
        ratio = actual / max;
        UpdateScale();
    }

    private void UpdateScale() {
        float maxOffset = bar.rectTransform.rect.width - inset;
        bar.rectTransform.localPosition = new Vector3(
            -1.0f * (1.0f - ratio) * maxOffset,
            bar.rectTransform.localPosition.y,
            bar.rectTransform.localPosition.z);
    }
}
