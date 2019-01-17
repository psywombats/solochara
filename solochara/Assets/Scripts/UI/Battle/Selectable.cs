using UnityEngine;
using System.Collections;

public class Selectable : MonoBehaviour {

    public GameObject unselectedBacker;
    public GameObject selectedBacker;

    private bool _selected;
    public bool selected {
        get { return _selected; }
        set {
            _selected = value;
            if (unselectedBacker != null) {
                unselectedBacker.SetActive(!_selected);
            }
            if (selectedBacker != null) {
                selectedBacker.SetActive(_selected);
            }
        }
    }
}
