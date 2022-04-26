using UnityEngine;
using Avrahamy;

namespace Product {
    public class DebugLifecycle : MonoBehaviour {
        protected void Awake() {
            DebugLog.Log($"Awake {this}", this);
        }

        protected void Start() {
            DebugLog.Log($"Start {this}", this);
        }

        protected void OnDestroy() {
            DebugLog.Log($"OnDestroy {this}");
        }

        protected void OnEnable() {
            DebugLog.Log($"OnEnable {this}", this);
        }

        protected void OnDisable() {
            DebugLog.Log($"OnDisable {this}", this);
        }
    }
}
