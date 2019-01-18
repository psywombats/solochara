using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BattleAnimationPlayer))]
public class BattleAnimationDrawer : Editor {

    public override void OnInspectorGUI() {
        if (!Application.isPlaying) {
            return;
        }
        base.OnInspectorGUI();
        BattleAnimationPlayer player = (BattleAnimationPlayer)target;
        if (player.anim != null) {
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