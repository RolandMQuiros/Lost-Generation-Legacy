using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ObjectRecycler : MonoBehaviour {
    public class NotRegisteredException : Exception {
        public NotRegisteredException(string prefabName)
            : base("Prefab with name \"" + prefabName + "\" was not registered with this ObjectRecycler") { }
        public NotRegisteredException(GameObject prefab)
            : base("Prefab " + prefab + " is not registered with this ObjectRecycler") { }
    }

    [Serializable]
    private struct Entry {
        public GameObject Prefab;
        public int Capacity;

        public Entry(GameObject prefab, int capacity = 1) {
            Prefab = prefab;
            Capacity = capacity;
        }
    }

    [SerializeField]
    private List<Entry> _register = new List<Entry>();

    private Dictionary<GameObject, List<Recyclable>> _pool = new Dictionary<GameObject, List<Recyclable>>();
    private HashSet<Recyclable> _spawned = new HashSet<Recyclable>();
    private HashSet<Recyclable> _despawned = new HashSet<Recyclable>();

    public void RegisterPrefab(GameObject prefab, int capacity) {
        if (prefab.GetComponent<Recyclable>() == null) {
            throw new ArgumentException("Prefab must have a Recyclable component to be added to an ObjectRecycler.", "prefab");
        }

        if (_register.Exists(entry => entry.Prefab != prefab && entry.Prefab.name == prefab.name)) {
            throw new ArgumentException("Prefab must have a name unique from other prefabs in this ObjectRecycler's registry", "prefab");
        }

        Entry newEntry = new Entry(prefab, capacity);
        int idx = _register.FindIndex(entry => entry.Prefab == prefab);
        if (idx < 0) {
            _register.Add(newEntry);
        } else {
            _register[idx] = newEntry;
        }
    }

    public void UnregisterPrefab(GameObject prefab) {
        int idx = _register.FindIndex(entry => entry.Prefab == prefab);
        if (idx >= 0) {
            _register.RemoveAt(idx);
        }
    }

    public void UnregisterPrefab(string prefabName) {
        int idx = _register.FindIndex(entry => entry.Prefab.name == prefabName);
        if (idx >= 0) {
            _register.RemoveAt(idx);
        }
    }

    public bool IsRegistered(GameObject prefab) {
        return _register.FindIndex(entry => entry.Prefab == prefab) >= 0;
    }

    public bool IsRegistered(string prefabName) {
        return _register.FindIndex(entry => entry.Prefab.name == prefabName) >= 0;
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
            throw new NotRegisteredException(prefab);
        } else {
            Recyclable recyclable = null;

            // If there's enough room, create a new instance of the prefab
            if (instances.Count < instances.Capacity) {
                spawned = GameObject.Instantiate<GameObject>(prefab);
                recyclable = spawned.GetComponent<Recyclable>();
                instances.Add(recyclable);
            } else {
                // Otherwise, check if there's a dormant instance in the pool, and re-initialize it
                recyclable = instances.FirstOrDefault(r => !r.gameObject.activeSelf && _despawned.Contains(r));
                if (recyclable != null) {
                    spawned = recyclable.gameObject;
                }
            }

            // If an instance was successfully spawned, trigger its spawning event to notify other components to
            // initialize
            if (spawned != null && recyclable != null) {
                _despawned.Remove(recyclable);
                _spawned.Add(recyclable);
                recyclable.OnSpawn(this, prefab);
            } else {
                // Should never happen
                throw new InvalidOperationException("Somehow a Recyclable was found for a null GameObject, or vise versa???");
            }
        }

        return spawned;
    }

    public GameObject Spawn(string prefabName) {
        Entry registered = _register.FirstOrDefault(entry => entry.Prefab.name == prefabName);
        GameObject instance = null;

        if (registered.Prefab != null) {
            instance = Spawn(registered.Prefab);
        }

        return instance;
    }

    public void Despawn(Recyclable instance) {
        if (instance.Recycler != this) {
            throw new ArgumentException("Despawning instance was not originally spawned by this ObjectRecycler", "instance");
        }

        _spawned.Remove(instance);
        _despawned.Add(instance);
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
