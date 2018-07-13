using System;
using MoonSharp.Interpreter;

namespace OneGame.Lua {
    /// <summary>
    /// A c# bridge for lua commands involving game states
    /// </summary>
    [MoonSharpUserData]
    public static class Game {

        /// <summary>
        /// The current state of the game
        /// </summary>
        public static string currentState { get; private set; }

        [MoonSharpHidden]
        public static event Action<string, float> OnAlert;
        [MoonSharpHidden]
        public static event Action<string> OnGameStateChange;
        [MoonSharpHidden]
        public static event Action<string> OnEventCall;
        [MoonSharpHidden]
        public static event Action<string, DynValue> OnEventCallData;
        [MoonSharpHidden]
        public static event Action<string, float> OnEventCallDelayed;
        [MoonSharpHidden]
        public static event Action<string> OnMessage;


        /// <summary>
        /// Alerts the player with a message
        /// </summary>
        public static void Alert (string message) {
            UnityEngine.Debug.Log (message);

            OnAlert?.Invoke (message, 0f);
        }

        /// <summary>
        /// Alerts the player with a message for a set duration
        /// </summary>
        /// <param name="message">The message to alert with</param>
        /// <param name="duration">How long should the message stay on the screen</param>
        public static void Alert (string message, float duration) {
            UnityEngine.Debug.Log (message);

            OnAlert?.Invoke (message, duration);
        }

        /// <summary>
        /// Changes the state of the game
        /// </summary>
        public static void ChangeState (string state) {
            currentState = state;

            if (OnGameStateChange != null) {
                OnGameStateChange (state);
            }
        }

        /// <summary>
        /// Propagates an event call to all lua scripts
        /// </summary>
        public static void FireEvent (string eventName) {
            if (OnEventCall != null) {
                OnEventCall (eventName);
            }
        }

        /// <summary>
        /// Propagates an event to call to all other lua scripts in the game
        /// </summary>
        /// <param name="eventName">The name of the event</param>
        /// <param name="value">The value to pass</param>
        public static void FireEvent (string eventName, DynValue value) {
            OnEventCallData?.Invoke (eventName, value);
        }

        /// <summary>
        /// Propagates an event to call to all other lua scripts, with a delay
        /// </summary>
        /// <param name="eventName">The name of the event</param>
        /// <param name="delay">The value to pass</param>
        public static void FireEventDelayed (string eventName, float delay) {
            OnEventCallDelayed?.Invoke (eventName, delay);
        }

        /// <summary>
        /// Outputs a message on the message logger
        /// </summary>
        /// <param name="message">The message to log</param>
        public static void Message (string message) {
            UnityEngine.Debug.Log (message);

            OnMessage?.Invoke (message);
        }
    }
}