using UnityEngine;

namespace Avrahamy {
    public static class SingletonsContainer {
        private static Transform _instance;

        public static Transform Instance {
            get {
                if (_instance == null) {
                    var container = new GameObject("SingletonsContainer");
                    _instance = container.transform;
                    Object.DontDestroyOnLoad(container);
                }

                return _instance;
            }
        }

        public static void MakeSingleton(Transform transform) {
            transform.parent = Instance;
        }
    }
}
