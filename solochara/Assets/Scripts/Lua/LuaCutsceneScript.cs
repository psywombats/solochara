using UnityEngine;
using System.Collections;
using System;
using MoonSharp.Interpreter;

public class LuaCutsceneScript : LuaScript {

    public LuaCutsceneScript(MonoBehaviour owner, string script) : base(owner, script) {

    }

    protected override void AssignGlobals() {
        context.Globals["getSwitch"] = (Func<DynValue, DynValue>)GetSwitch;
        context.Globals["setSwitch"] = (Action<DynValue, DynValue>)SetSwitch;
        context.Globals["eventNamed"] = (Func<DynValue, LuaMapEvent>)EventNamed;
        context.Globals["playBGM"] = (Action<DynValue>)PlayBGM;
        context.Globals["cs_teleportCoords"] = (Action<DynValue, DynValue, DynValue>)Teleport;
        context.Globals["cs_teleport"] = (Action<DynValue, DynValue>)Teleport;
        context.Globals["cs_fadeOutBGM"] = (Action<DynValue>)FadeOutBGM;
        context.Globals["cs_fadeIn"] = (Action)FadeIn;
        context.Globals["cs_fadeOut"] = (Action)FadeOut;
    }

    public override IEnumerator RunRoutine() {
        if (Global.Instance().Maps.Avatar != null) {
            Global.Instance().Maps.Avatar.PauseInput();
        }
        yield return base.RunRoutine();
        if (Global.Instance().Maps.Avatar != null) {
            Global.Instance().Maps.Avatar.UnpauseInput();
        }
    }

    // meant to be evaluated synchronously
    public LuaCondition CreateCondition(string luaScript) {
        return new LuaCondition(context.LoadString(luaScript));
    }

    // creates an empty table as the lua representation of some c# object
    public LuaMapEvent CreateEvent(MapEvent mapEvent) {
        return new LuaMapEvent(CreateEmptyTable(), mapEvent);
    }

    // returns an empty dictionary/tablething in the global context
    public DynValue CreateEmptyTable() {
        return context.DoString("return {}");
    }

    // evaluates a lua function in the global context
    public DynValue Evaluate(DynValue function) {
        return context.Call(function);
    }

    // make sure the luaobject has been registered via [MoonSharpUserData]
    public void SetGlobal(string key, object luaObject) {
        context.Globals[key] = luaObject;
    }

    // === LUA CALLABLE ============================================================================

    private LuaMapEvent EventNamed(DynValue eventName) {
        MapEvent mapEvent = Global.Instance().Maps.ActiveMap.GetEventNamed(eventName.String);
        if (mapEvent == null) {
            return null;
        } else {
            return mapEvent.luaObject;
        }
    }

    private DynValue GetSwitch(DynValue switchName) {
        bool value = Global.Instance().Memory.GetSwitch(switchName.String);
        return Marshal(value);
    }

    private void SetSwitch(DynValue switchName, DynValue value) {
        Global.Instance().Memory.SetSwitch(switchName.String, value.Boolean);
    }

    private void PlayBGM(DynValue bgmKey) {
        Global.Instance().Audio.PlayBGM(bgmKey.String);
    }

    private void Teleport(DynValue mapName, DynValue x, DynValue y) {
        RunRoutineFromLua(Global.Instance().Maps.TeleportRoutine(mapName.String, new IntVector2((int)x.Number, (int)y.Number)));
    }

    private void Teleport(DynValue mapName, DynValue targetEventName) {
        RunRoutineFromLua(Global.Instance().Maps.TeleportRoutine(mapName.String, targetEventName.String));
    }

    private void FadeOutBGM(DynValue seconds) {
        RunRoutineFromLua(Global.Instance().Audio.FadeOutRoutine((float)seconds.Number));
    }

    private void FadeIn() {
        RunRoutineFromLua(Global.Instance().UIEngine.GlobalFadeRoutine(false));
    }

    private void FadeOut() {
        RunRoutineFromLua(Global.Instance().UIEngine.GlobalFadeRoutine(true));
    }
}
