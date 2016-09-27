using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ObjectRecycler : MonoBehaviour {
    [Serializable]
    private struct Entry : IEquatable<Entry> {
        public GameObject Prefab;
        public int Capacity;

        public Entry(GameObject prefab, int capacity = 1) {
            Prefab = prefab;
            Capacity = capacity;
        }

        public bool Equals(Entry other) {
            return Prefab == other.Prefab;
        }

        public override int GetHashCode() {
            return Prefab.GetHashCode();
        }
    }

    [SerializeField]
    private List<Entry> _register = new List<Entry>();

    private Dictionary<GameObject, List<Recyclable>> _pool = new Dictionary<GameObject, List<Recyclable>>();
    private HashSet<GameObject> _spawned = new HashSet<GameObject>();

    public void RegisterPrefab(GameObject prefab, int capacity) {
        if (prefab.GetComponent<Recyclable>() == null) {
            throw new ArgumentException("Prefab must have a Poolable component to be added to an ObjectPool.");
        }

        Entry newEntry = new Entry(prefab, capacity);
        int idx = _register.FindIndex(ent => ent.Prefab == prefab);
        if (idx < 0) {
            _register.Add(newEntry);
        } else {
            _register[idx] = newEntry;
        }
    }

    public void UnregisterPrefab() {

    }

    public IEnumerable<KeyValuePair<GameObject, int>> GetRegistry() {
        foreach (Entry entry in _register) {
            yield return new KeyValuePair<GameObject, int>(entry.Prefab, entry.Capacity);
        }
    }

    public GameObject Spawn(GameObject prefab) {
        GameObject spawned = null;
        List<Recyclable> instances;
        _pool.TryGetValue(prefab, out instances);

        if (instances == null) {
            throw new ArgumentException("The given prefab is not registered with this pool", "prefab");
        } else {
            Recyclable recyclable = null;

            // If there's enough room, create a new instance of the prefab
            if (instances.Count < instances.Capacity) {
                spawned = GameObject.Instantiate<GameObject>(prefab);
                recyclable = spawned.GetComponent<Recyclable>();
                instances.Add(recyclable);
                spawned.SetActive(true);
            } else {
                // Otherwise, check if there's a dormant instance in the pool, and re-initialize it
                recyclable = instances.FirstOrDefault(obj => !obj.gameObject.activeSelf);
                if (recyclable != null) {
                    spawned = recyclable.gameObject;
                    spawned.SetActive(true);
                }
            }

            // If an instance was successfully spawned, trigger its spawning event to notify other components to
            // initialize
            if (spawned != null && recyclable != null) {
                _spawned.Add(spawned);
                recyclable.OnSpawn(this, prefab);
            } else {
                // Should never happen
                throw new InvalidOperationException("Somehow a Recyclable was found for a null GameObject, or vise versa???");
            }
        }

        return spawned;
    }

    public void Awake() {
        Initialize();
    }

    private void Initialize() {
        foreach (Entry entry in _register) {
            _pool.Add(entry.Prefab, new List<Recyclable>(entry.Capacity));
        }
    }
}
