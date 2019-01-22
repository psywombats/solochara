using UnityEngine;
using System.Collections;

[System.Serializable]
public class SpellChainMutation {

    public Spell precedingSpell;
    public Spell turnsThisIntoSpell;
}
