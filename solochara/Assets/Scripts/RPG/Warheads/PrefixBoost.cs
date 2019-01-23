using UnityEngine;
using System.Collections;

public class PrefixBoost : Prefix {

    [Tooltip("Affected spells have some values increased by a spell-specific constants times this value, usually 1.0")]
    public float effectiveness;
}
