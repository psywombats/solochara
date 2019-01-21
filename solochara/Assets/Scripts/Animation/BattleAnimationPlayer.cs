using UnityEngine;
using System.Collections;
using MoonSharp.Interpreter;

public class BattleAnimationPlayer : AnimationPlayer {

    public Doll attacker = null;
    public Doll defender = null;

    public override void EditorReset() {
        base.EditorReset();
        attacker.ResetAfterAnimation();
        defender.ResetAfterAnimation();
    }

    public IEnumerator PlayAnimationRoutine(LuaAnimation anim, Doll attacker, Doll defender) {
        this.attacker = attacker;
        this.defender = defender;
        yield return PlayAnimationRoutine(anim);
    }

    public override IEnumerator PlayAnimationRoutine() {
        Global.Instance().Lua.SetGlobal("attacker", attacker);
        Global.Instance().Lua.SetGlobal("battle", attacker); // lol, too cheap to set as global
        Global.Instance().Lua.SetGlobal("defender", defender);
        attacker.PrepareForBattleAnimation(this, Doll.Type.Attacker);
        defender.PrepareForBattleAnimation(this, Doll.Type.Defender);
        yield return base.PlayAnimationRoutine();
        attacker.ResetAfterAnimation();
        defender.ResetAfterAnimation();
    }
}
