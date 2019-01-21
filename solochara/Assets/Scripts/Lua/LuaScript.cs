using MoonSharp.Interpreter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Coroutine = MoonSharp.Interpreter.Coroutine;

// represents an runnable piece of Lua, usually from an event field
public class LuaScript {
    
    protected LuaContext context;

    public Coroutine scriptRoutine { get; private set;  }
    public bool done { get; private set; }

    private string stringDescription;

    public LuaScript(LuaContext context, string scriptString) {
        this.context = context;
        this.stringDescription = scriptString;

        string fullScript = "return function()\n" + scriptString + "\nend";
        this.scriptRoutine = context.CreateScript(fullScript);
    }

    public LuaScript(LuaContext context, DynValue function) {
        this.scriptRoutine = context.lua.CreateCoroutine(function).Coroutine;
    }

    public IEnumerator RunRoutine() {
        done = false;
        yield return context.RunRoutine(this);
        done = true;
    }

    public override string ToString() {
        if (stringDescription != null) {
            return stringDescription;
        } else {
            return base.ToString();
        }
    }
}
