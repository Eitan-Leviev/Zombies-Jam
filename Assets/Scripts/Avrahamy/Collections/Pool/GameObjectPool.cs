using UnityEngine;
using System.Collections.Generic;
using Avrahamy.EditorGadgets;
using Avrahamy.Utils;

namespace Avrahamy.Collections {
    public class GameObjectPool : MonoBehaviour {
        [PrefabOnly]
        [SerializeField] GameObject pooledObjectPrefab;
        [SerializeField] int poolSize = 10;
        [Tooltip("Set to 0 to not allow auto expanding the pool.")]
        [SerializeField] int increaseBy = 0;
        [SerializeField] bool returnAllWhenDisabled;

        public GameObject PooledObjectPrefab {
            get {
                return pooledObjectPrefab;
            }
        }

        public int PoolSize {
            get {
                return pool.Count;
            }
        }

        public int InitialPoolSize {
            get {
                return poolSize;
            }
        }

        public int IncreaseBy {
            get {
                return increaseBy;
            }
        }

        private int lastReturned = 0;
        protected readonly List<GameObject> pool = new List<GameObject>();

        protected void Awake() {
            if (pooledObjectPrefab != null && pool.Count < poolSize) {
                Add(pooledObjectPrefab, poolSize - pool.Count);
            }
        }

        protected void Start() {
            if (pooledObjectPrefab != null && pool.Count < poolSize) {
                Add(pooledObjectPrefab, poolSize - pool.Count);
            }
        }

        protected void OnDisable() {
            if (returnAllWhenDisabled) {
                ReturnAll();
            }
        }

        public void Initialize(GameObject pooledObjectPrefab, int poolSize = 10, int increaseBy = 0) {
            pool.Clear();
            this.pooledObjectPrefab = pooledObjectPrefab;
            this.poolSize = poolSize;
            this.increaseBy = increaseBy;

            if (pool.Count < poolSize) {
                Add(pooledObjectPrefab, poolSize - pool.Count);
            }
        }

        public void Add(GameObject pooledObjectPrefab, int count = 1) {
            var isActive = pooledObjectPrefab.activeSelf;
            pooledObjectPrefab.SetActive(false);

            for (int i = 0; i < count; i++) {
                var obj = Instantiate(pooledObjectPrefab, transform, false);
                obj.name = pooledObjectPrefab.name + pool.Count;
                pool.Add(obj);
            }

            pooledObjectPrefab.SetActive(isActive);
        }

        /// <summary>
        /// Removes all the objects from the pool.
        /// </summary>
        public void Clear() {
            if (pool == null) return;

            foreach (var go in pool) {
                DestroyImmediate(go);
            }
            pool.Clear();
            lastReturned = 0;
        }

        public virtual void ReturnAll() {
            if (pool == null) return;

            foreach (var go in pool) {
                if (go == null) continue;
                var comp = go.GetComponent<IPoolable>();
                comp?.ReturnSelf();
                go.SetActive(false);
            }
        }

        public T Borrow<T>(params object[] activateParams) where T : class, IPoolable {
            var go = GetGameObject();
            if (go == null) return null;
            return ActivateComponent<T>(go, activateParams);
        }

        /// <summary>
        /// Does not initialize the object. The pooled object does not need to be an
        /// IPoolable.
        /// </summary>
        public T BorrowSimple<T>() where T : Component {
            var go = GetGameObject();
            if (go == null) return null;
            go.SetActive(true);
            return go.GetComponent<T>();
        }

        public GameObject BorrowGameObject() {
            var go = GetGameObject();
            if (go == null) return null;
            go.SetActive(true);
            return go;
        }

        protected virtual T ActivateComponent<T>(GameObject go, params object[] activateParams) where T : IPoolable {
            var comp = go.GetComponent<T>();
            DebugAssert.Assert(comp != null, "Didn't find component of type '" + typeof(T) + "' on " + go.transform.PathInHierarchy());
            go.SetActive(true);
            comp.Activate(activateParams);
            return comp;
        }

        public int CountActiveObjects() {
            var activeObjects = 0;
            foreach (var go in pool) {
                if (!go.activeInHierarchy) continue;
                ++activeObjects;
            }
            return activeObjects;
        }

        public List<GameObject> GetActiveObjects(List<GameObject> container = null) {
            var activeObjects = container ?? new List<GameObject>();
            activeObjects.Clear();
            foreach (var go in pool) {
                if (!go.activeInHierarchy) continue;
                activeObjects.Add(go);
            }
            return activeObjects;
        }

        private GameObject GetGameObject() {
            DebugAssert.Assert(pool != null, "Destroyed pool is being used! " + this);
            for (int i = lastReturned; i < pool.Count; i++) {
                DebugAssert.Assert(pool[i] != null, "Pooled item destroyed! " + this + " at index: " + i);
                if (!pool[i].activeInHierarchy) {
                    lastReturned = i;
                    return pool[i];
                }
            }
            for (int i = 0; i < lastReturned; i++) {
                DebugAssert.Assert(pool[i] != null, "Pooled item destroyed! " + this + " at index: " + i);
                if (!pool[i].activeInHierarchy) {
                    lastReturned = i;
                    return pool[i];
                }
            }
            // All objects are active.
            if (increaseBy > 0) {
                lastReturned = pool.Count;
                Add(pooledObjectPrefab, increaseBy);
                return pool[lastReturned];
            }
            return null;
        }
    }
}
