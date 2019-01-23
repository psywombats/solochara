using UnityEditor;
using System;
using UnityEngine;

[CustomEditor(typeof(PrefixSpell))]
public class PrefixSpellEditor : PolymorphicFieldEditor {

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        PrefixSpell spell = (PrefixSpell)target;
        spell.effect = DrawSelector(spell.effect);
    }

    protected override Type GetBaseType() {
        return typeof(Prefix);
    }

    protected override string PathForTarget() {
        return "Assets/Resources/Database/PrefixEffects/" + ((PrefixSpell)target).name + "_prefix.asset";
    }
}
