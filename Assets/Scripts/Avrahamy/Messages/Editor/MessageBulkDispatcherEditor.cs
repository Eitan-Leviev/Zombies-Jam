using UnityEngine;
using UnityEditor;

namespace Avrahamy.Messages {
    [CustomEditor(typeof(MessageBulkDispatcher), true)]
    public class MessageBulkDispatcherEditor : Editor {
        public override void OnInspectorGUI() {
            DrawDefaultInspector();

            var wasEnabled = GUI.enabled;
            var isPlaying = Application.isPlaying;
            GUI.enabled = isPlaying;
            if (GUILayout.Button(isPlaying ? "Dispatch" : "Can 'Dispatch' in Play Mode")) {
                var dispatcher = target as MessageBulkDispatcher;
                dispatcher.Dispatch();
            }
            GUI.enabled = wasEnabled;
        }
    }
}
