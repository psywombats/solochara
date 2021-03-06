﻿using UnityEditor;
using System;
using UnityEngine;

[CustomEditor(typeof(BaseSpell))]
[CanEditMultipleObjects]
public class BaseSpellEditor : PolymorphicFieldEditor {

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        if (serializedObject.FindProperty("warhead").hasMultipleDifferentValues) {
            return;
        }

        BaseSpell spell = (BaseSpell)target;
        spell.warhead = DrawSelector(spell.warhead);
    }

    protected override Type GetBaseType() {
        return typeof(Warhead);
    }

    protected override string PathForTarget() {
        return "Assets/Resources/Database/Warheads/" + ((BaseSpell)target).name + "_warhead.asset";
    }
}