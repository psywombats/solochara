using UnityEngine;
using System.Collections;

public class SpellSelectable : MonoBehaviour {

    public GameObject unselectedBacker;
    public GameObject selectedBacker;

    private bool _selected;
    public bool selected {
        get { return _selected; }
        set {
            _selected = value;
            unselectedBacker.SetActive(!_selected);
            selectedBacker.SetActive(_selected);
        }
    }
}
