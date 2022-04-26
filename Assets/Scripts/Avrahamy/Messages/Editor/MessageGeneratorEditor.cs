using UnityEngine;
using UnityEditor;

namespace Avrahamy.Messages {
    [CustomEditor(typeof(MessageGeneratorBase), true)]
    public class MessageGeneratorEditor : Editor {
        public override void OnInspectorGUI() {
            DrawDefaultInspector();

            var wasEnabled = GUI.enabled;
            var isPlaying = Application.isPlaying;
            GUI.enabled = isPlaying;
            if (GUILayout.Button(isPlaying ? "Dispatch" : "Can 'Dispatch' in Play Mode")) {
                GlobalMessagesHub.Instance.Dispatch(((MessageGeneratorBase)target).New());
            }
            GUI.enabled = wasEnabled;
        }
    }
}
