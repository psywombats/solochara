using UnityEditor;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Linq;
using UnityEngine;

[CustomEditor(typeof(Spell))]
public class SpellEditor : Editor {

    private static Dictionary<string, Type> cachedWarheadSubclasses;

    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        Spell spell = (Spell)target;

        int warheadIndex = 0;
        if (spell.warhead != null) {
            warheadIndex = GetWarheadSubclasses().Values.ToList().IndexOf(spell.warhead.GetType());
        }
        string[] names = GetWarheadSubclasses().Keys.ToArray();
        int selectedIndex = EditorGUILayout.Popup("Warhead type", warheadIndex, names);
        
        if (selectedIndex != warheadIndex) {
            if (spell.warhead != null) {
                AssetDatabase.DeleteAsset(PathForWarhead(spell));
                spell.warhead = null;
            }
            if (selectedIndex != 0) {
                Type warheadType = GetWarheadSubclasses().Values.ToArray()[selectedIndex];
                Warhead warhead = ScriptableObject.CreateInstance(warheadType) as Warhead;
                string name = spell.name + "_warhead";
                AssetDatabase.CreateAsset(warhead, PathForWarhead(spell));
                spell.warhead = warhead;
            }
        }
    }

    private Dictionary<string, Type> GetWarheadSubclasses() {
        if (cachedWarheadSubclasses == null) {
            cachedWarheadSubclasses = new Dictionary<string, Type>();
            cachedWarheadSubclasses.Add("(none)", null);
            foreach (Assembly ass in AppDomain.CurrentDomain.GetAssemblies()) {
                foreach (Type type in ass.GetTypes()) {
                    if (typeof(Warhead).IsAssignableFrom(type) && !type.IsAbstract) {
                        cachedWarheadSubclasses.Add(type.Name, type);
                    }
                }
            }
        }
        return cachedWarheadSubclasses;
    }

    private string PathForWarhead(Spell spell) {
        return "Assets/Resources/Database/Warheads/" + spell.name + "_warhead.asset";
    }
}