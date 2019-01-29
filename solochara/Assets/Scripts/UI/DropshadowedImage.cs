using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEditor;

[RequireComponent(typeof(Image))]
public class DropshadowedImage : MonoBehaviour {

    public Vector3 offset = new Vector3(0, -1, 0);
    public GameObject shadowedObject;

    public Image primaryImage {
        get {
            return GetComponent<Image>();
        }
    }

    public Image secondaryImage {
        get {
            return shadowedObject.GetComponent<Image>();
        }
    }

    public void OnValidate() {
        secondaryImage.sprite = primaryImage.sprite;
        secondaryImage.color = Color.black;
        secondaryImage.rectTransform.sizeDelta = primaryImage.rectTransform.sizeDelta;
        secondaryImage.rectTransform.rotation = primaryImage.rectTransform.rotation;
        secondaryImage.rectTransform.localPosition = primaryImage.rectTransform.localPosition + offset;

    }
}
