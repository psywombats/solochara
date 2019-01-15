using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEvent3D : MapEvent {

    public static Vector3 TileToWorldCoords(IntVector2 position) {
        return new Vector3(position.x, 0.0f, -1.0f * position.y);
    }

    public override Vector3 CalculateOffsetPositionPx(OrthoDir dir) {
        return PositionPx + dir.Px3D();
    }
    
    protected override void SetDepth() {
        // our global height is identical to the height of the parent layer
        transform.localPosition = new Vector3(gameObject.transform.localPosition.x, 0.0f, gameObject.transform.localPosition.z);
    }
}
