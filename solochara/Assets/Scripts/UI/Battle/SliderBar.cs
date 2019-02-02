using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SliderBar : MonoBehaviour {

    public Mask backer;
    public Image bar;
    public int inset = 16;
    public float ratio = 0.5f;

    public void Update() {
        UpdateScale();
    }

    public void OnValidate() {
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
