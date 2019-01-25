using UnityEngine;
using System.Collections;

public abstract class Prefix : AutoExpandingScriptableObject {

    public abstract float ModifyHeal(WarheadHeal source, float heal);

    public abstract float ModifyDamage(WarheadDamage source, float damage);
}
