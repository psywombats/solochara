using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Spell", menuName = "Data/RPG/Spell")]
public class Spell : ScriptableObject {

    public string spellName;
    public int apCost;
}
