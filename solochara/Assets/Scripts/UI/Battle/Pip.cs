using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Pip : MonoBehaviour {

    public void Populate(Spell spell) {
        GetComponent<Image>().color = spell.pipColor;
    }
}
