using UnityEngine;
using UnityEditor;
using Avrahamy.EditorGadgets;

namespace Avrahamy.Math {
    [CustomPropertyDrawer(typeof(IntRange), true)]
    public class IntRangeDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            label = EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, label);

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var minProp = property.FindPropertyRelative("min");
            var maxProp = property.FindPropertyRelative("max");

            var minValue = minProp.intValue;
            var maxValue = maxProp.intValue;

            var style = EditorStyles.numberField;
            if (minValue > maxValue) {
                style = new GUIStyle(style) {
                    normal = {
                        textColor = Color.red
                    }
                };
            }

            var rangeMin = 0;
            var rangeMax = 1;

            var ranges = (MinMaxRangeAttribute[])fieldInfo.GetCustomAttributes(typeof(MinMaxRangeAttribute), true);
            var gotAttribute = ranges.Length > 0;
            if (gotAttribute) {
                rangeMin = Mathf.RoundToInt(ranges[0].min);
                rangeMax = Mathf.RoundToInt(ranges[0].max);
            }

            var rangeBoundsLabelWidth = gotAttribute ? 60f : position.width * 0.5f;

            EditorGUI.BeginChangeCheck();
            var rangeBoundsLabel1Rect = new Rect(position) {
                width = rangeBoundsLabelWidth - 5
            };
            if (!gotAttribute) {
                var labelPosition = new Rect(rangeBoundsLabel1Rect) {
                    width = 30f
                };
                EditorGUI.LabelField(labelPosition, "Min");
                rangeBoundsLabel1Rect.xMin += labelPosition.width;
            }
            minValue = EditorGUI.IntField(rangeBoundsLabel1Rect, minValue, style);
            position.xMin += rangeBoundsLabelWidth;

            var rangeBoundsLabel2Rect = new Rect(position);
            rangeBoundsLabel2Rect.xMin = rangeBoundsLabel2Rect.xMax - rangeBoundsLabelWidth + 5;
            if (!gotAttribute) {
                var labelPosition = new Rect(rangeBoundsLabel2Rect) {
                    width = 30f
                };
                EditorGUI.LabelField(labelPosition, "Max");
                rangeBoundsLabel2Rect.xMin += labelPosition.width;
            }
            maxValue = EditorGUI.IntField(rangeBoundsLabel2Rect, maxValue, style);
            position.xMax -= rangeBoundsLabelWidth;

            if (gotAttribute) {
                var floatMinValue = (float)minValue;
                var floatMaxValue = (float)maxValue;
                EditorGUI.MinMaxSlider(position, ref floatMinValue, ref floatMaxValue, rangeMin, rangeMax);
                minValue = Mathf.RoundToInt(floatMinValue);
                maxValue = Mathf.RoundToInt(floatMaxValue);
            }
            if (EditorGUI.EndChangeCheck()) {
                minProp.intValue = minValue;
                maxProp.intValue = maxValue;
            }

            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }
}
