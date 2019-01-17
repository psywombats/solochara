using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Unit", menuName = "Data/RPG/Unit")]
public class Unit : ScriptableObject {

    public string unitName;
    public Sprite appearance;
    public bool unique;
    public Alignment align;
    public List<Spell> spells;
    public StatSet stats;
}
