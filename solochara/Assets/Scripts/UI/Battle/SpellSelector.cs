using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SpellSelector : MonoBehaviour, InputListener {

    public GameObject attachmentPoint;
    public SpellCard spellCardPrefab;
    public Selectable dummyCardPrefab;

    private Result<Selectable> awaitingResult;
    private int selectionIndex = 0;

    public void Start() {
        DestroyCards();
    }

    public bool OnCommand(InputManager.Command command, InputManager.Event eventType) {
        if (eventType == InputManager.Event.Down) {
            switch (command) {
                case InputManager.Command.Cancel:
                    awaitingResult.Cancel();
                    break;
                case InputManager.Command.Confirm:
                    awaitingResult.value = GetSelectedCard();
                    break;
                case InputManager.Command.Down:
                    MoveSelection(1);
                    break;
                case InputManager.Command.Up:
                    MoveSelection(-1);
                    break;
            }
        }
        return true;
    }

    public IEnumerator EnableRoutine(List<Spell> spells) {
        DestroyCards();
        selectionIndex = 0;
        foreach (Spell spell in spells) {
            SpellCard card = Instantiate(this.spellCardPrefab).GetComponent<SpellCard>();
            card.transform.SetParent(attachmentPoint.transform);
            card.Populate(spell);
        }
        Instantiate(this.dummyCardPrefab).transform.SetParent(attachmentPoint.transform);
        Global.Instance().Input.PushListener(this);
        yield return null;
    }

    public IEnumerator DisableRoutine() {
        Global.Instance().Input.RemoveListener(this);
        yield return null;
    }

    public IEnumerator SelectSpellRoutine(Result<Selectable> result) {
        GetSelectedCard().selected = true;
        this.awaitingResult = result;
        while (!awaitingResult.finished) {
            yield return null;
        }
        awaitingResult = null;
    }
    
    private Selectable GetSelectedCard() {
        return GetCardAt(selectionIndex);
    }

    private Selectable GetCardAt(int index) {
        return attachmentPoint.transform.GetChild(index).GetComponent<Selectable>();
    }

    private void MoveSelection(int delta) {
        Global.Instance().Audio.PlaySFX("cursor");

        GetSelectedCard().selected = false;
        selectionIndex += delta;
        int max = attachmentPoint.transform.childCount;
        if (selectionIndex < 0) {
            selectionIndex = max - 1;
        } else if (selectionIndex >= max) {
            selectionIndex = 0;
        }
        GetSelectedCard().selected = true;
    }

    private void DestroyCards() {
        foreach (Transform child in attachmentPoint.transform) {
            Destroy(child.gameObject);
        }
    }
}
