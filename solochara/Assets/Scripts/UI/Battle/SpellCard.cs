using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SpellCard : Selectable {

    public Text skillName;
    public Text newName;
    public APMeter apMeter;

    public Spell spell { get; private set; }

    public void Populate(Spell spell) {
        this.spell = spell;
        skillName.text = spell.spellName;
        newName.text = "";
        apMeter.Populate(spell);
        selected = false;
    }

    public void Populate(Spell baseSpell, Spell actualSpell) {
        Populate(actualSpell);
        skillName.text = baseSpell.spellName;
        newName.text = "to " + actualSpell.spellName;
    }
}
