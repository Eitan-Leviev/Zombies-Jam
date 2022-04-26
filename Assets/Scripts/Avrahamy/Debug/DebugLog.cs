// Define the following scripting define symbols to enable debug functionality:
// DEBUG_LOG
using UnityEngine;
using System;
using System.Diagnostics;
using Avrahamy.Utils;

namespace Avrahamy {
    public enum LogTag {
        Default = 0,
        Gameplay = 1,
        Audio = 20,
        UI = 21,
        Input = 22,
        Messages = 30,
        Web = 40,
        LowPriority = 98,
        MediumPriority = 99,
        HighPriority = 100,
    }

    [CreateAssetMenu(menuName = "Avrahamy/Setup/Debug", fileName = "DebugLog")]
    public class DebugLog : ScriptableObject {
        private const string COLOR_TAG_FORMAT = "<color=#{0}>{1}</color>";

        [Serializable]
        public struct LogTagAndColor {
            public bool show;
            public LogTag tag;
            public Color color;
        }

        private static DateTime LogStartTime {
            get {
                if (!hasStarted) {
                    hasStarted = true;
                    _startTime = DateTime.Now;
                }
                return _startTime;
            }
        }
        private static DateTime _startTime;
        private static bool hasStarted;

        [SerializeField] bool _logEverything = true;
        [SerializeField] LogTagAndColor[] _activeTags;

        private static bool logEverything = true;
        private static LogTagAndColor[] activeTags;

        protected void Awake() {
            OnEnable();
        }

        protected void OnEnable() {
            logEverything = _logEverything;
            activeTags = _activeTags;
            hideFlags = HideFlags.DontUnloadUnusedAsset;
        }

#region Log
        [Conditional("DEBUG_LOG")]
        public static void Log(object message, UnityEngine.Object context = null) {
            UnityEngine.Debug.Log(Time.realtimeSinceStartup.ToString("0.00") + ": " + message, context);
        }

        [Conditional("DEBUG_LOG")]
        internal static void LogIf(bool condition, object message, UnityEngine.Object context = null) {
            if (condition) Log(message, context);
        }

        [Conditional("DEBUG_LOG")]
        public static void LogWarning(object message, UnityEngine.Object context = null) {
            UnityEngine.Debug.LogWarning(Time.realtimeSinceStartup.ToString("0.00") + ": " + message, context);
        }

        [Conditional("DEBUG_LOG")]
        public static void LogError(object message, UnityEngine.Object context = null) {
            UnityEngine.Debug.LogError(Time.realtimeSinceStartup.ToString("0.00") + ": " + message, context);
        }
#endregion // Log

#region Log with Color
        [Conditional("DEBUG_LOG")]
        public static void Log(object message, Color color, UnityEngine.Object context = null) {
            message = GetLogWithColor(message, color);
            UnityEngine.Debug.Log(Time.realtimeSinceStartup.ToString("0.00") + ": " + message, context);
        }

        [Conditional("DEBUG_LOG")]
        internal static void LogIf(bool condition, object message, Color color, UnityEngine.Object context = null) {
            if (condition) {
                message = GetLogWithColor(message, color);
                Log(message, context);
            }
        }

        [Conditional("DEBUG_LOG")]
        public static void LogWarning(object message, Color color, UnityEngine.Object context = null) {
            message = GetLogWithColor(message, color);
            UnityEngine.Debug.LogWarning(Time.realtimeSinceStartup.ToString("0.00") + ": " + message, context);
        }

        [Conditional("DEBUG_LOG")]
        public static void LogError(object message, Color color, UnityEngine.Object context = null) {
            message = GetLogWithColor(message, color);
            UnityEngine.Debug.LogError(Time.realtimeSinceStartup.ToString("0.00") + ": " + message, context);
        }
#endregion // Log with Color

#region Log with Tag
        [Conditional("DEBUG_LOG")]
        public static void Log(LogTag tag, object message, UnityEngine.Object context = null) {
            if (!ShouldLog(tag)) return;
            message = GetLogWithColor(tag, message);
            UnityEngine.Debug.Log(Time.realtimeSinceStartup.ToString("0.00") + ": " + message, context);
        }

        [Conditional("DEBUG_LOG")]
        internal static void LogIf(LogTag tag, bool condition, object message, UnityEngine.Object context = null) {
            if (condition) Log(tag, message, context);
        }

        [Conditional("DEBUG_LOG")]
        public static void LogWarning(LogTag tag, object message, UnityEngine.Object context = null) {
            if (!ShouldLog(tag)) return;
            message = GetLogWithColor(tag, message);
            UnityEngine.Debug.LogWarning(Time.realtimeSinceStartup.ToString("0.00") + ": " + message, context);
        }

        [Conditional("DEBUG_LOG")]
        public static void LogError(LogTag tag, object message, UnityEngine.Object context = null) {
            if (!ShouldLog(tag)) return;
            message = GetLogWithColor(tag, message);
            UnityEngine.Debug.LogError(Time.realtimeSinceStartup.ToString("0.00") + ": " + message, context);
        }

        private static bool ShouldLog(LogTag tag) {
    #if UNITY_EDITOR
            // Hack to get the DebugLog to always load.
            if (activeTags == null) {
                var filter = "t:DebugLog";
                var guids = UnityEditor.AssetDatabase.FindAssets(filter);
                if (guids.Length == 0) return true;
                var guid = guids[0];
                var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<DebugLog>(path);
                if (asset == null) return true;
                asset.OnEnable();
            }
    #endif
            if (logEverything || activeTags == null) return true;
            foreach (var activeTag in activeTags) {
                if (tag == activeTag.tag) return activeTag.show;
            }
            return false;
        }

        private static Color GetLogColor(LogTag tag) {
#if UNITY_EDITOR
            if (activeTags == null) return Color.white;
            foreach (var activeTag in activeTags) {
                if (tag == activeTag.tag) return activeTag.color;
            }
#endif
            return Color.white;
        }

        private static object GetLogWithColor(LogTag tag, object message) {
            var color = GetLogColor(tag);
            message = GetLogWithColor(message, color);
#if UNITY_EDITOR
            if (tag == LogTag.LowPriority) {
                // Add a linebreak so it will remove the first stack trace line from the console.
                message += "\n";
            }
#endif
            return message;
        }

        private static object GetLogWithColor(object message, Color color) {
            if (color != Color.white) {
                var colorHex = color.ToHex();
                message = string.Format(COLOR_TAG_FORMAT, colorHex, message);
            }
            return message;
        }
#endregion // Log with Tag

#region Log on Thread
        /// <summary>
        /// This is needed if we want to log messages from a thread that is not the
        /// main thread (for example, callbacks from native code).
        /// </summary>
        [Conditional("DEBUG_LOG")]
        public static void LogOnThread(object message) {
            TimeSpan span = DateTime.Now - LogStartTime;
            UnityEngine.Debug.Log(span.TotalSeconds.ToString("0.00") + ": " + message);
        }

        /// <summary>
        /// This is needed if we want to log messages from a thread that is not the
        /// main thread (for example, callbacks from native code).
        /// </summary>
        [Conditional("DEBUG_LOG")]
        public static void LogErrorOnThread(object message) {
            TimeSpan span = DateTime.Now - LogStartTime;
            UnityEngine.Debug.LogError(span.TotalSeconds.ToString("0.00") + ": " + message);
        }
#endregion // Log on Thread
    }
}
