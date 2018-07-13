using UnityEngine;

namespace OneGame {
    /// <summary>
    /// A proxy for Debug calls in unity
    /// </summary>
    public static class Logger {

        public static void Log (string log) {
#if UNITY_EDITOR || ONEGAME_DEBUG
            Debug.Log (log);
#endif
        }

        /// <summary>
        /// Logs a formatted string into Debug.LogFormat()
        /// </summary>
        /// <param name="log">The debug string</param>
        /// <param name="args">The arguments for the formatted string</param>
        public static void LogFormat (string log, params object[] args) {
#if UNITY_EDITOR || ONEGAME_DEBUG
            Debug.LogFormat (log, args);
#endif
        }

        public static void LogWarning (string log) {
#if UNITY_EDITOR || ONEGAME_DEBUG
            Debug.LogWarning (log);
#endif
        }

        public static void LogWarningFormat (string log, params object[] args) {
#if UNITY_EDITOR || ONEGAME_DEBUG
            Debug.LogWarningFormat (log, args);
#endif
        }
    }
}