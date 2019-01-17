using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SpellCard : SpellSelectable {

    public Text skillName;
    public APMeter apMeter;

    public Spell spell { get; private set; }

    public void Populate(Spell spell) {
        this.spell = spell;
        skillName.text = spell.spellName;
        apMeter.Populate(spell);
        selected = false;
    }
}
