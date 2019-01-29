using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEditor;

[RequireComponent(typeof(Image))]
public class DropshadowedImage : MonoBehaviour {

    public Vector3 offset = new Vector3(0, -1, 0);

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

    public GameObject shadowedObject;

    public void OnValidate() {
        secondaryImage.sprite = primaryImage.sprite;
        secondaryImage.color = Color.black;
        secondaryImage.rectTransform.localPosition = offset;
        secondaryImage.rectTransform.sizeDelta = primaryImage.rectTransform.sizeDelta;
    }
}
