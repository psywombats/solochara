using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(GridLayoutGroup))]
public class SpellSelector : MonoBehaviour, InputListener {

    public GameObject attachmentPoint;
    public SpellCard spellCardPrefab;
    public SpellSelectable dummyCardPrefab;

    private Result<SpellSelectable> awaitingResult;
    private int selectionIndex = 0;

    public bool OnCommand(InputManager.Command command, InputManager.Event eventType) {
        if (eventType == InputManager.Event.Down) {
            switch (command) {
                case InputManager.Command.Cancel:
                    awaitingResult.Cancel();
                    break;
                case InputManager.Command.Click:
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

    public IEnumerator EnableRoutine() {
        attachmentPoint.SetActive(true);
        Global.Instance().Input.PushListener(this);
        yield break;
    }

    public IEnumerator DisableRoutine() {
        attachmentPoint.SetActive(false);
        Global.Instance().Input.RemoveListener(this);
        yield break;
    }

    public IEnumerator SelectSpellRoutine(BattleUnit hero, Result<SpellSelectable> result) {
        Populate(hero.unit.spells);
        this.awaitingResult = result;
        while (!awaitingResult.finished) {
            yield return null;
        }
        awaitingResult = null;
    }


    private void Populate(List<Spell> spells) {
        attachmentPoint.transform.DetachChildren();
        foreach (Spell spell in spells) {
            SpellCard card = Instantiate(this.spellCardPrefab).GetComponent<SpellCard>();
            card.transform.parent = attachmentPoint.transform;
            card.Populate(spell);
        }
        Instantiate(this.dummyCardPrefab).transform.parent = this.transform;
    }

    private SpellSelectable GetSelectedCard() {
        return GetCardAt(selectionIndex);
    }

    private SpellSelectable GetCardAt(int index) {
        return attachmentPoint.transform.GetChild(index).GetComponent<SpellSelectable>();
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
}
