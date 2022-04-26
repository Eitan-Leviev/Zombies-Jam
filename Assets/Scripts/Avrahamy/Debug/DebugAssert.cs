// Define the following scripting define symbols to enable debug functionality:
// DEBUG_LOG
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Avrahamy {
    public class DebugAssert {
        [Conditional("DEBUG_LOG")]
        public static void Assert(bool condition, string message = null) {
            if (!condition) throw new Exception(message);
        }

        [Conditional("DEBUG_LOG")]
        public static void AssertArrayIndex<T>(ICollection<T> array, int index, string arrayName = null) {
            if (index < 0 || index >= array.Count) {
                throw new Exception($"Got index {index} to array of length {array.Count} {arrayName}");
            }
        }

        [Conditional("DEBUG_LOG")]
        public static void AssertFormat(bool condition, string format, params object[] args) {
            if (!condition) throw new Exception(string.Format(format, args));
        }

        /// <summary>
        /// Logs a warning if the assert fails - does not raise an exception.
        /// </summary>
        [Conditional("DEBUG_LOG")]
        public static void WarningAssert(bool condition, string message, UnityEngine.Object context = null) {
            if (!condition) DebugLog.LogWarning(message, context);
        }
    }
}
