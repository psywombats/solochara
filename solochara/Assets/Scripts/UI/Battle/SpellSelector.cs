using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(GridLayoutGroup))]
public class SpellSelector : MonoBehaviour {

    public SpellCard spellCardPrefab;

    public void Populate(List<Spell> spells) {
        this.transform.DetachChildren();
        foreach (Spell spell in spells) {
            SpellCard card = Instantiate(this.spellCardPrefab).GetComponent<SpellCard>();
            card.transform.parent = this.transform;
            card.Populate(spell);
        }
    }
}
