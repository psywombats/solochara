using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RandomUtils {

    public static T RandomItem<T>(System.Random r, List<T> items) {
        int index = (int) Mathf.Floor((float)r.NextDouble() * items.Count);
        return items[index];
    }
}
