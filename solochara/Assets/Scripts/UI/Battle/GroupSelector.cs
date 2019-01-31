using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GroupSelector : MonoBehaviour, InputListener {

    // to be set in editor
    public List<Doll> dolls;

    private Result<BattleUnit> awaitingResult;
    private int selectionIndex;
    private int dollFillIndex = 0;
    private bool awaitingConfirm;
    private bool canceledConfirm;

    public void Start() {
        //int pivot = (int)Mathf.Floor(((float)(dolls.Count - 1)) / 2.0f);
        //int offset = 0;
        //int direction = -1;
        //for (int i = pivot; i >= 0 && i < dolls.Count; i = pivot + offset * direction) {
        //    dolls[i] = dollSlots
        //    if (direction < 0) {
        //        offset += 1;
        //    }
        //    direction *= -1;
        //}
        foreach (Doll doll in dolls) {
            doll.Populate(null);
        }
    }

    // call during initialization only
    public Doll AssignNextDoll(BattleUnit unit) {
        Doll doll = dolls[dollFillIndex];
        doll.Populate(unit);
        dollFillIndex += 1;
        return doll;
    }

    public bool OnCommand(InputManager.Command command, InputManager.Event eventType) {
        if (eventType == InputManager.Event.Down) {
            if (awaitingConfirm) {
                if (command == InputManager.Command.Confirm) {
                    Global.Instance().Audio.PlaySFX("confirm");
                    awaitingConfirm = false;
                } else if (command == InputManager.Command.Cancel) {
                    canceledConfirm = true;
                    Global.Instance().Audio.PlaySFX("cancel");
                    awaitingConfirm = false;
                }
            } else {
                switch (command) {
                    case InputManager.Command.Cancel:
                        awaitingResult.Cancel();
                        Global.Instance().Audio.PlaySFX("cancel");
                        break;
                    case InputManager.Command.Confirm:
                        awaitingResult.value = GetSelectedDoll().unit;
                        Global.Instance().Audio.PlaySFX("confirm");
                        break;
                    case InputManager.Command.Down:
                        MoveSelection(1);
                        break;
                    case InputManager.Command.Up:
                        MoveSelection(-1);
                        break;
                }
            }
        }
        return true;
    }

    public IEnumerator SelectAnyOneRoutine(Result<BattleUnit> result, Intent intent) {
        StartSingle(result);
        while (!awaitingResult.finished) {
            yield return null;
        }
        EndSingle();
    }

    public IEnumerator SelectAnyExceptRoutine(Result<BattleUnit> result, Intent intent) {
        yield break;
    }

    public IEnumerator SelectAllRoutine(Result<List<BattleUnit>> result, Intent intent) {
        StartMulti();
        while (awaitingConfirm) {
            yield return null;
        }
        EndMulti(result);
    }

    public IEnumerator SelectAllExceptRoutine(Result<List<BattleUnit>> result, Intent intent) {
        yield break;
    }

    public IEnumerator SelectSpecificallyRoutine(Result<BattleUnit> result, Intent intent) {
        StartSingle(null);
        GetSelectedDoll().GetComponent<Selectable>().selected = false;
        for (int i = selectionIndex; GetSelectedDoll().unit != intent.actor; i += 1) ;
        GetSelectedDoll().GetComponent<Selectable>().selected = true;
        awaitingConfirm = true;
        while (awaitingConfirm) {
            yield return null;
        }
        if (!canceledConfirm) {
            result.value = intent.actor;
        } else {
            result.Cancel();
        }
        EndSingle();
    }

    private void StartSingle(Result<BattleUnit> result) {
        awaitingResult = result;
        selectionIndex = -1;
        MoveSelection(1);
        GetSelectedDoll().GetComponent<Selectable>().selected = true;
        Global.Instance().Input.PushListener(this);
    }

    private void EndSingle() {
        awaitingResult = null;
        GetSelectedDoll().GetComponent<Selectable>().selected = false;
        Global.Instance().Input.RemoveListener(this);
    }

    private void StartMulti() {
        canceledConfirm = false;
        awaitingConfirm = true;
        foreach (Doll doll in dolls) {
            doll.GetComponent<Selectable>().selected = true;
        }
        Global.Instance().Input.PushListener(this);
    }

    private void EndMulti(Result<List<BattleUnit>> result) {
        if (canceledConfirm) {
            result.Cancel();
        } else {
            result.value = new List<BattleUnit>();
            foreach (Doll doll in dolls) {
                doll.GetComponent<Selectable>().selected = false;
                result.value.Add(doll.unit);
            }
        }
        Global.Instance().Input.RemoveListener(this);
    }

    private Doll GetSelectedDoll() {
        if (selectionIndex == -1) return null;
        return dolls[selectionIndex];
    }

    private void MoveSelection(int delta) {
        Doll oldSelected = GetSelectedDoll();
        if (oldSelected != null) {
            oldSelected.GetComponent<Selectable>().selected = false;
        }

        int max = dolls.Count;
        do {
            selectionIndex += delta;
            if (selectionIndex < 0) {
                selectionIndex = max - 1;
            } else if (selectionIndex >= max) {
                selectionIndex = 0;
            }
        } while (GetSelectedDoll().unit.IsDead());

        Doll newSelected = GetSelectedDoll();
        newSelected.GetComponent<Selectable>().selected = true;

        if (oldSelected != newSelected) {
            Global.Instance().Audio.PlaySFX("cursor");
        }
    }
}
