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
        GetComponent<LuaContext>().SetGlobal("attacker", attacker);
        GetComponent<LuaContext>().SetGlobal("defender", defender);
        attacker.PrepareForBattleAnimation(this, Doll.Type.Attacker);
        defender.PrepareForBattleAnimation(this, Doll.Type.Defender);
        yield return base.PlayAnimationRoutine();
        attacker.ResetAfterAnimation();
        defender.ResetAfterAnimation();
    }
}
