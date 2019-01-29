using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SpellSelector : MonoBehaviour, InputListener {

    public GameObject attachmentPoint;
    public SpellSelectable dummyCard;

    private Dictionary<EightDir, SpellCard> cards;
    private Result<Selectable> awaitingResult;
    private List<OrthoDir> downButtons;

    public void Start() {
        downButtons = new List<OrthoDir>();
        cards = new Dictionary<EightDir, SpellCard>();
        foreach (Transform child in attachmentPoint.transform) {
            SpellCard card = child.GetComponent<SpellCard>();
            cards[card.arrow.direction] = card;
            card.gameObject.SetActive(false);
        }
    }

    public bool OnCommand(InputManager.Command command, InputManager.Event eventType) {
        switch (eventType) {
            case InputManager.Event.Down:
                Debug.Log(command + " " + eventType);
                switch (command) {
                    case InputManager.Command.Cancel:
                        awaitingResult.Cancel();
                        break;
                    case InputManager.Command.Confirm:
                        awaitingResult.value = dummyCard;
                        break;
                    case InputManager.Command.Down:
                        AddKey(OrthoDir.South);
                        break;
                    case InputManager.Command.Up:
                        AddKey(OrthoDir.North);
                        break;
                    case InputManager.Command.Left:
                        AddKey(OrthoDir.West);
                        break;
                    case InputManager.Command.Right:
                        AddKey(OrthoDir.East);
                        break;
                }
                break;
            case InputManager.Event.Up:
                Debug.Log(command + " " + eventType);
                switch (command) {
                    case InputManager.Command.Down:
                        RemoveKey(OrthoDir.South);
                        break;
                    case InputManager.Command.Up:
                        RemoveKey(OrthoDir.North);
                        break;
                    case InputManager.Command.Left:
                        RemoveKey(OrthoDir.West);
                        break;
                    case InputManager.Command.Right:
                        RemoveKey(OrthoDir.East);
                        break;
                }
                break;
        }
        return true;
    }

    public IEnumerator EnableRoutine(List<Spell> spells) {
        List<EightDir> directions;
        if (spells.Count < 4) {
            directions = new List<EightDir>() { EightDir.North, EightDir.East, EightDir.South, EightDir.West };
        } else {
            directions = new List<EightDir>() { EightDir.North, EightDir.Northeast, EightDir.East, EightDir.Southeast,
                EightDir.South, EightDir.Southwest, EightDir.West, EightDir.Northwest };
        }
        for (int i = 0; i < spells.Count; i += 1) {
            SpellCard card = cards[directions[i]];
            Spell spell = spells[i];
            card.selected = false;
            card.gameObject.SetActive(true);
            card.Populate(spell);
        }
        
        Global.Instance().Input.PushListener(this);
        yield return null;
    }

    public IEnumerator DisableRoutine() {
        foreach (SpellCard card in cards.Values) {
            card.gameObject.SetActive(false);
        }
        Global.Instance().Input.RemoveListener(this);
        yield return null;
    }

    public IEnumerator SelectSpellRoutine(Result<Selectable> result, List<Spell> previous) {
        this.awaitingResult = result;
        UpdateHighlight();
        while (!awaitingResult.finished) {
            yield return null;
        }
        awaitingResult = null;
    }

    private void AddKey(OrthoDir dir) {
        downButtons.Add(dir);
        UpdateHighlight();
    }

    private void RemoveKey(OrthoDir dir) {
        if (downButtons.Contains(dir)) {
            foreach (SpellCard card in cards.Values) {
                if (card.selected) {
                    awaitingResult.value = card;
                }
            }
            downButtons.Clear();
        }
    }

    private void UpdateHighlight() {
        foreach (SpellCard card in cards.Values) {
            card.selected = false;
        }
        if (downButtons.Count > 0) {
            EightDir dir = EightDir.South;
            if (downButtons.Contains(OrthoDir.East)) {
                if (downButtons.Contains(OrthoDir.North)) {
                    dir = EightDir.Northeast;
                } else if (downButtons.Contains(OrthoDir.South)) {
                    dir = EightDir.Southeast;
                } else {
                    dir = EightDir.East;
                }
            } else if (downButtons.Contains(OrthoDir.West)) {
                if (downButtons.Contains(OrthoDir.North)) {
                    dir = EightDir.Northwest;
                } else if (downButtons.Contains(OrthoDir.South)) {
                    dir = EightDir.Southwest;
                } else {
                    dir = EightDir.West;
                }
            } else if (downButtons.Contains(OrthoDir.North)) {
                dir = EightDir.North;
            } else if (downButtons.Contains(OrthoDir.South)) {
                dir = EightDir.South;
            }
            if (cards.ContainsKey(dir)) {
                cards[dir].selected = true;
            }
        }
    }
}
