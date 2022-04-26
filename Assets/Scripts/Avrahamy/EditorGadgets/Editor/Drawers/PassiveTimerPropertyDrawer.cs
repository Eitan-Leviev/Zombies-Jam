using UnityEditor;
using UnityEngine;

namespace Avrahamy.EditorGadgets {
    [CustomPropertyDrawer(typeof(PassiveTimer), true)]
    public class PassiveTimerPropertyDrawer : PropertyDrawer {
        private const int PADDING = 2;

        private static GUIContent iconNotSet;
        private static GUIContent iconActive;
        private static GUIContent iconNotActive;

        protected virtual float Now {
            get {
                return Time.time;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            iconNotSet ??= EditorGUIUtility.IconContent("UnityEditor.AnimationWindow");
            iconActive ??= EditorGUIUtility.IconContent("UnityEditor.ProfilerWindow");
            iconNotActive ??= EditorGUIUtility.IconContent("d_ol_minus_act");

            var durationProperty = property.FindPropertyRelative("duration");

            var timer = property.GetValue(fieldInfo) as ITimer;

            var icon = iconNotSet;
            if (Application.isPlaying) {
                var endTime = timer.EndTime;
                if (endTime > 0f) {
                    icon = Now < endTime ? iconActive : iconNotActive;
                }
            }

            var size = position.height;
            var rect = new Rect(position.x + position.width - size, position.y, size, size);
            GUI.Label(rect, icon);

            position.width -= size + PADDING;
            if (durationProperty.floatValue < 0f) {
                durationProperty.floatValue = 0f;
            }
            EditorGUI.PropertyField(position, durationProperty, label);
        }
    }
}