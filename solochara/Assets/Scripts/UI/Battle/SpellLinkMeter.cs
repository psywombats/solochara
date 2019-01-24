using System.Collections.Generic;
using UnityEngine;

public class SpellLinkMeter : MonoBehaviour {

    public Pip pipPrefab;
    public Pip pipConnectorPrefab;
    public Pip emptyPipPrefab;

    public GameObject pipAttachmentPoint;
    public GameObject pipConnectorAttachmentPoint;

    private int totalAP;

    public void Start() {
        Clear();
    }

    public void SetUp(int totalAP) {
        this.totalAP = totalAP;
        Clear();
    }

    public void Populate(List<Spell> spells) {
        Clear();
        int pipsAdded = 0;
        for (int i = 0; i < spells.Count; i += 1) {
            Spell spell = spells[i];
            for (int j = 0; j < spell.apCost; j += 1) {
                AddPip(pipPrefab, pipAttachmentPoint, spell);
                if (j == 0) {
                    if (i > 0) {
                        Spell previous = spells[i - 1];
                        if (previous.LinksToNextSpell()) {
                            AddPip(pipConnectorPrefab, pipConnectorAttachmentPoint, null);
                        } else {
                            AddPip(null, pipConnectorAttachmentPoint, null);
                        }
                    }
                } else {
                    AddPip(pipConnectorPrefab, pipConnectorAttachmentPoint, spell);
                }
                pipsAdded += 1;
            }
        }
        for (; pipsAdded < totalAP; pipsAdded += 1) {
            AddPip(emptyPipPrefab, pipAttachmentPoint, null);
        }
    }

    public void Clear() {
        foreach (Transform child in pipAttachmentPoint.transform) {
            Destroy(child.gameObject);
        }
        foreach (Transform child in pipConnectorAttachmentPoint.transform) {
            Destroy(child.gameObject);
        }
    }

    private void AddPip(Pip prefab, GameObject attachmentPoint, Spell spell) {
        GameObject pipObject;
        if (prefab != null) {
            Pip pip = Instantiate(prefab);
            if (spell != null) {
                pip.Populate(spell);
            }
            pipObject = pip.gameObject;
        } else {
            pipObject = new GameObject("(no connector)");
        }
        pipObject.transform.SetParent(attachmentPoint.transform);
    }
}
