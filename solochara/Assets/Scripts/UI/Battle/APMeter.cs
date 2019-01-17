using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class APMeter : MonoBehaviour {
    
    public GameObject pipPrefab;
    
    public void Populate(Spell spell) {
        foreach (Transform child in transform) {
            Destroy(child.gameObject);
        }
        for (int i = 0; i < spell.apCost; i += 1) {
            Instantiate(this.pipPrefab).transform.SetParent(transform);
        }
    }
}
