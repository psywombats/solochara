using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SpellCard : MonoBehaviour {

    public Text skillName;
    public APMeter apMeter;
    public GameObject unselectedBacker;
    public GameObject selectedBacker;

    private bool _selected;
    public bool selected {
        get { return _selected; }
        private set {
            _selected = value;
            unselectedBacker.SetActive(!_selected);
            selectedBacker.SetActive(_selected);
        }
    }

    public void Populate(Spell spell) {
        skillName.text = spell.spellName;
        apMeter.Populate(spell);
        selected = false;
    }
}
