using UnityEngine;
using System.Collections;
using MoonSharp.Interpreter;
using Coroutine = MoonSharp.Interpreter.Coroutine;
using System;
using UnityEngine.Assertions;

// a wrapper around Script that represents an environment where a script can execute
public class LuaContext : MonoBehaviour {

    private static readonly string DefinesPath = "Assets/Resources/Scripts/global_defines.lua";
    private static string defines;

    public Script lua { get; private set; }

    private LuaScript activeScript;
    private int blockingRoutines;

    public LuaContext() {
        lua = new Script();

        if (defines == null) {
            TextAsset luaText = Resources.Load<TextAsset>(DefinesPath);
            defines = luaText.text;
        }
        lua.DoString(defines);
    }

    // make sure the luaobject has been registered via [MoonSharpUserData]
    public void SetGlobal(string key, object luaObject) {
        lua.Globals[key] = luaObject;
    }

    public DynValue CreateObject() {
        return lua.DoString("return {}");
    }

    public DynValue Marshal(object toMarshal) {
        return DynValue.FromObject(lua, toMarshal);
    }

    public Coroutine CreateScript(string fullScript) {
        try {
            DynValue scriptFunction = lua.DoString(fullScript);
            return lua.CreateCoroutine(scriptFunction).Coroutine;
        } catch (SyntaxErrorException e) {
            Debug.LogError("bad script: " + fullScript);
            throw e;
        }
    }

    // all coroutines that are meant to block execution of the script should go through here
    public void RunRoutineFromLua(IEnumerator routine) {
        blockingRoutines += 1;
        StartCoroutine(CoUtils.RunWithCallback(routine, () => {
            blockingRoutines -= 1;
            if (blockingRoutines == 0) {
                activeScript.scriptRoutine.Resume();
            }
        }));
    }

    // meant to be evaluated synchronously
    public LuaCondition CreateCondition(string luaScript) {
        return new LuaCondition(lua.LoadString(luaScript));
    }

    // evaluates a lua function in the global context
    public DynValue Evaluate(DynValue function) {
        return lua.Call(function);
    }

    // hang on to a chunk of lua to run later
    public DynValue Load(string luaChunk) {
        return lua.LoadString(luaChunk);
    }

    public virtual IEnumerator RunRoutine(LuaScript script) {
        Assert.IsNull(this.activeScript);
        this.activeScript = script;
        script.scriptRoutine.Resume();
        while (script.scriptRoutine.State != CoroutineState.Dead) {
            yield return null;
        }
        this.activeScript = null;
    }

    protected virtual void AssignGlobals() {
        lua.Globals["debugLog"] = (Action<DynValue>)DebugLog;
        lua.Globals["playSFX"] = (Action<DynValue>)PlaySFX;
        lua.Globals["cs_wait"] = (Action<DynValue>)Wait;
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
