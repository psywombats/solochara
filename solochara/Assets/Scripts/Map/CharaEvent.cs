﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/**
 * For our purposes, a CharaEvent is anything that's going to be moving around the map
 * or has a physical appearance. For parallel process or whatevers, they won't have this.
 */
[RequireComponent(typeof(MapEvent))]
[DisallowMultipleComponent]
public class CharaEvent : MonoBehaviour {

    public static readonly string FaceEvent = "eventFace";

    private static readonly string PropertySprite = "sprite";
    private static readonly string PropertyFacing = "face";

    // Editor
    public OrthoDir initialFacing;
    public GameObject doll;
    public CharaAnimator animator;

    // Public
    public Map parent { get { return GetComponent<MapEvent>().Parent; } }
    public ObjectLayer layer { get { return GetComponent<MapEvent>().Layer; } }

    private OrthoDir internalFacing;
    public OrthoDir facing {
        get {
            return internalFacing;
        }
        set {
            if (internalFacing != value) {
                internalFacing = value;
                GetComponent<Dispatch>().Signal(FaceEvent, value);
            }
        }
    }

    public void Start() {
        facing = initialFacing;
        GetComponent<Dispatch>().RegisterListener(MapEvent.EventMove, (object payload) => {
            facing = (OrthoDir)payload;
        });
    }

    public void Populate(IDictionary<string, string> properties) {
        if (properties.ContainsKey(PropertyFacing)) {
            initialFacing = OrthoDirExtensions.Parse(properties[PropertyFacing]);
            facing = initialFacing;
        }
        if (properties.ContainsKey(PropertySprite)) {
            if (GetComponent<MapEvent3D>() != null) {
                doll = new GameObject("Doll");
                doll.transform.parent = gameObject.transform;
                doll.transform.localPosition = new Vector3(0.5f, 0.0f, -0.5f);
                animator = doll.AddComponent<CharaAnimator>();
                animator.parentEvent = GetComponent<MapEvent>();
            } else {
                animator = gameObject.AddComponent<CharaAnimator>();
            }
            animator.SetSpriteByKey(properties[PropertySprite]);
            animator.Populate(properties);
        }
        GetComponent<MapEvent>().Passable = false;
    }

    public void FaceToward(IntVector2 pos) {
        facing = OrthoDirExtensions.DirectionOf(pos - GetComponent<MapEvent>().positionXY);
    }

    public void SetAppearance(string spriteKey) {
        animator.SetSpriteByKey(spriteKey);
    }

    // returns the sprite key currently in use
    public string GetAppearance() {
        return animator.spriteName;
    }

    public IEnumerator PathToRoutine(IntVector2 location) {
        List<IntVector2> path = parent.FindPath(this, location);
        if (path == null) {
            yield break;
        }
        foreach (IntVector2 target in path) {
            OrthoDir dir = OrthoDirExtensions.DirectionOf(target - GetComponent<MapEvent>().positionXY);
            yield return StartCoroutine(GetComponent<MapEvent>().StepRoutine(dir));
        }
    }
}
