using UnityEditor;
using UnityEngine;

namespace Avrahamy.EditorGadgets {
    [CustomPropertyDrawer(typeof(PassiveRealTimeTimer), true)]
    public class PassiveRealTimeTimerPropertyDrawer : PassiveTimerPropertyDrawer {
        protected override float Now {
            get {
                return Time.realtimeSinceStartup;
            }
        }
    }
}