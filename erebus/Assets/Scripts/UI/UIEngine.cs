using System;
using UnityEngine;
using System.Collections;

public class UIEngine : MonoBehaviour, InputListener {
    
    public UnityEngine.UI.Text DebugBox;

    public void Start() {
        Global.Instance().Input.PushListener(this);
    }

    public bool OnCommand(InputManager.Command command, InputManager.Event eventType) {
        if (eventType != InputManager.Event.Down) {
            return false;
        }
        if (command == InputManager.Command.Menu) {
            StartCoroutine(PauseRoutine());
            return true;
        }
        return false;
    }

    public IEnumerator GlobalFadeRoutine(bool fadeOut) {
        // TODO: new project
        yield return null;
    }

    private IEnumerator PauseRoutine() {
        // TODO: new project
        yield return null;
    }
}
