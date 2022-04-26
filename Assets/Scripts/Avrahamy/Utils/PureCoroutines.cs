using UnityEngine;
using System;
using System.Collections;
using Avrahamy.Utils;

namespace Avrahamy {
    /// <summary>
    /// Used to run coroutines from non MonoBehaviours.
    /// </summary>
    public class PureCoroutines : MonoBehaviour {
        public static PureCoroutines Instance {
            get {
                if (_instance == null) {
                    if (isQuiting) return null;

                    var go = new GameObject("PureCoroutines", typeof(PureCoroutines));
                    go.GetComponent<PureCoroutines>();
                }
                return _instance;
            }
        }
        private static PureCoroutines _instance;
        private static bool isQuiting;

        protected void Awake() {
            if (_instance != null) {
                Destroy(gameObject);
                return;
            }
            SingletonsContainer.MakeSingleton(transform);
            _instance = this;
        }

        public static Coroutine StartRoutine(IEnumerator routine) {
            return Instance.StartCoroutine(routine);
        }

        public static Coroutine DelaySeconds(Action action, float delay) {
            return CoroutineUtils.DelaySeconds(Instance, action, delay);
        }

        /// <summary>
        /// Removes all coroutines attached to this instance.
        /// </summary>
        public void RemoveAllCoroutines() {
            gameObject.SetActive(false);
            gameObject.SetActive(true);
        }

        protected void OnApplicationQuit() {
            isQuiting = true;
        }
    }
}
