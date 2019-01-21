using MoonSharp.Interpreter;
using System;
using UnityEngine;

[MoonSharpUserData]
public class LuaMapEvent {

    public DynValue LuaValue { get; private set; }

    private MapEvent mapEvent;
    private LuaContext context;

    public LuaMapEvent(MapEvent mapEvent) {
        this.mapEvent = mapEvent;
        this.context = mapEvent.GetComponent<LuaContext>();
    }

    // meant to be called with the key/value of a lualike property on a Tiled object
    // accepts nil and zero-length as no-ops
    [MoonSharpHidden]
    public void Set(string name, string luaChunk) {
        if (luaChunk != null && luaChunk.Length > 0) {
            LuaValue.Table.Set(name, context.Load(luaChunk));
        }
    }

    [MoonSharpHidden]
    public void Run(string eventName, Action callback = null) {
        DynValue function = LuaValue.Table.Get(eventName);
        if (function == DynValue.Nil) {
            if (callback != null) {
                callback();
            }
        } else {
            LuaScript script = new LuaScript(context, function);
            mapEvent.StartCoroutine(CoUtils.RunWithCallback(script.RunRoutine(), callback));
        }
    }

    [MoonSharpHidden]
    public DynValue Evaluate(string propertyName) {
        DynValue function = LuaValue.Table.Get(propertyName);
        if (function == DynValue.Nil) {
            return DynValue.Nil;
        } else {
            return context.Evaluate(function);
        }
    }

    [MoonSharpHidden]
    public bool EvaluateBool(string propertyName, bool defaultValue = false) {
        DynValue result = Evaluate(propertyName);
        if (result == DynValue.Nil) {
            return defaultValue;
        } else {
            return result.Boolean;
        }
    }

    // === CALLED BY LUA === 

    public void face(string directionName) {
        mapEvent.GetComponent<CharaEvent>().facing = OrthoDirExtensions.Parse(directionName);
    }

    public void faceToward(LuaMapEvent other) {
        mapEvent.GetComponent<CharaEvent>().facing = mapEvent.DirectionTo(other.mapEvent);
    }

    public int x() {
        return mapEvent.positionXY.x;
    }

    public int y() {
        return mapEvent.positionXY.y;
    }

    public void debuglog() {
        Debug.Log("Debug: " + mapEvent.name);
    }

    public void cs_pathTo(int x, int y) {
        context.RunRoutineFromLua(mapEvent.GetComponent<CharaEvent>().PathToRoutine(new IntVector2(x, y)));
    }

    public void cs_walk(string directionName, int count) {
       context.RunRoutineFromLua(mapEvent.GetComponent<MapEvent>().StepMultiRoutine(OrthoDirExtensions.Parse(directionName), count));
    }

    public void cs_step(string directionName) {
        context.RunRoutineFromLua(mapEvent.GetComponent<MapEvent>().StepRoutine(OrthoDirExtensions.Parse(directionName)));
    }
}
