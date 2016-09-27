using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class Recyclable : MonoBehaviour {
    public GameObject Original { get; private set; }
    public ObjectRecycler Recycler { get; private set; }
    public Action<Recyclable> Spawned;

    public void OnSpawn(ObjectRecycler recycler, GameObject prefab) {
        Recycler = recycler;
        Original = prefab;
        if (Spawned != null) {
            Spawned(this);
        }
    }
}
