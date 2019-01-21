using MoonSharp.Interpreter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Coroutine = MoonSharp.Interpreter.Coroutine;

// represents an runnable piece of Lua, usually from an event field
public class LuaScript {

    private static readonly string DefinesPath = "Assets/Resources/Scripts/global_defines.lua";

    protected MonoBehaviour owner;
    protected Script context;

    private Coroutine scriptRoutine;
    private int blockingRoutines;

    private static string defines;

    public LuaScript(MonoBehaviour owner, string scriptString) {
        this.owner = owner;
        this.context = new Script();

        string fullScript = "return function()\n" + scriptString + "\nend";
        try {
            DynValue scriptFunction = context.DoString(scriptString);
            this.scriptRoutine = context.CreateCoroutine(scriptFunction).Coroutine;
        } catch (SyntaxErrorException e) {
            Debug.LogError("bad script: " + scriptString);
            throw e;
        }

        if (defines == null) {
            TextAsset luaText = Resources.Load<TextAsset>(DefinesPath);
            defines = luaText.text;
        }
        context.DoString(defines);

        AssignGlobals();
    }

    public virtual IEnumerator RunRoutine() {
        bool done = false;
        scriptRoutine.Resume();
        while (!done) {
            yield return null;
        }
    }

    protected virtual void AssignGlobals() {
        context.Globals["debugLog"] = (Action<DynValue>)DebugLog;
        context.Globals["playSFX"] = (Action<DynValue>)PlaySFX;
        context.Globals["cs_wait"] = (Action<DynValue>)Wait;
    }

    // all coroutines that are meant to block execution of the script should go through here
    protected void RunRoutineFromLua(IEnumerator routine) {
        blockingRoutines += 1;
        owner.StartCoroutine(CoUtils.RunWithCallback(routine, () => {
            blockingRoutines -= 1;
            if (blockingRoutines == 0) {
                scriptRoutine.Resume();
            }
        }));
    }

    protected DynValue Marshal(object toMarshal) {
        return DynValue.FromObject(context, toMarshal);
    }

    // === LUA CALLABLE ============================================================================

    private void DebugLog(DynValue message) {
        Debug.Log(message.CastToString());
    }

    private void Wait(DynValue seconds) {
        RunRoutineFromLua(CoUtils.Wait((float)seconds.Number));
    }

    private void PlaySFX(DynValue sfxKey) {
        Global.Instance().Audio.PlaySFX(sfxKey.String);
    }
}
