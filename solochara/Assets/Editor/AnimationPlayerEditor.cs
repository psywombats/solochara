using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AnimationPlayer))]
public class AnimationPlayerDrawer : Editor {

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        AnimationPlayer player = (AnimationPlayer)target;
        if (player.anim != null && Application.IsPlaying(player)) {
            Editor.CreateEditor(player.anim).DrawDefaultInspector();
            if (!player.isPlayingAnimation) {
                if (GUILayout.Button("Play animation")) {
                    player.StartCoroutine(CoUtils.RunWithCallback(player.PlayAnimationRoutine(), () => {
                        Repaint();
                    }));
                }
            } else {
                GUILayout.Label("Running...");
                if (GUILayout.Button("(cancel)")) {
                    player.EditorReset();
                }
            }
        }
    }
}