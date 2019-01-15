using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEvent2D : MapEvent {

    public Vector2 PositionPx2D {
        get { return new Vector2(gameObject.transform.position.x, gameObject.transform.position.y); }
        private set { gameObject.transform.position = new Vector3(value.x, value.y, gameObject.transform.position.z); }
    }

    public override Vector3 CalculateOffsetPositionPx(OrthoDir dir) {
        throw new System.NotImplementedException();
    }

    protected override void SetDepth() {
        if (Parent != null) {
            for (int i = 0; i < Parent.transform.childCount; i += 1) {
                if (Layer == Parent.transform.GetChild(i).gameObject.GetComponent<ObjectLayer>()) {
                    float depthPerLayer = -.1f;
                    float z = depthPerLayer * ((float)positionXY.y / (float)Parent.height) + (depthPerLayer * (float)i);
                    gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, z);
                }
            }
        }
    }
}
