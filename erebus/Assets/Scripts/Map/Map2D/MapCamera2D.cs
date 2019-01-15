using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCamera2D : MapCamera {

    public override void ManualUpdate() {
        base.ManualUpdate();
        Vector3 targetPos = target.transform.position;
        Vector3 oldPos = GetComponent<Camera>().transform.position;
        Vector3 newPos = new Vector3(targetPos.x * OrthoDir.North.Y(), 
            targetPos.y * OrthoDir.East.X(), 
            oldPos.z);
        GetComponent<Camera>().transform.position = newPos;
    }
}
