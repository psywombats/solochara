using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DirectionArrow : MonoBehaviour {

    public Sprite downArrow;
    public Sprite downRightArrow;

    public Image image;
    public EightDir direction;

    public void OnValidate() {
        float angleOffset = -90.0f;
        switch (direction) {
            case EightDir.East:
            case EightDir.North:
            case EightDir.West:
            case EightDir.South:
                image.sprite = downArrow;
                break;
            case EightDir.Southeast:
            case EightDir.Northeast:
            case EightDir.Southwest:
            case EightDir.Northwest:
                angleOffset = -135.0f;
                image.sprite = downRightArrow;
                break;
        }
        float radians = Mathf.Atan2(direction.Y(), -direction.X());
        image.rectTransform.localEulerAngles = new Vector3(0.0f, 0.0f, radians / (2.0f * Mathf.PI) * 360.0f + angleOffset);
    }

    public void SetDirection(EightDir direction) {
        this.direction = direction;
        OnValidate();
    }
}
