using UnityEngine;
using System.Collections;

public class AnimationPlayer : MonoBehaviour {
    
    public AnimationTarget target;
    public LuaAnimation anim;

    public bool isPlayingAnimation { get; private set; }

    public virtual void EditorReset() {
        isPlayingAnimation = false;
    }

    public virtual IEnumerator PlayAnimationRoutine() {
        isPlayingAnimation = true;
        LuaScript script = anim.ToScript();
        script.SetGlobal("target", target);
        yield return script.RunRoutine();
        isPlayingAnimation = false;
    }

    public IEnumerator PlayAnimationRoutine(LuaAnimation anim) {
        this.anim = anim;
        yield return PlayAnimationRoutine();
    }
}
