using System.Collections;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using UnityEngine;

namespace OneGame.Lua {
    /// <summary>
    /// A c# bridge for keeping track of scores
    /// </summary>
    [MoonSharpUserData]
    public class Score {

        public float this[string scoreName] {
            get {
                var score = 0f;
                scoreBank.TryGetValue (scoreName, out score);
                return score;
            }
            set {
                if (scoreBank.ContainsKey (scoreName)) {
                    scoreBank[scoreName] = value;
                } else {
                    scoreBank.Add (scoreName, value);
                }
            }
        }

        private Dictionary<string, float> scoreBank;

        /// <summary>
        /// The current score instance
        /// </summary>
        [MoonSharpHidden]
        public static Score Instance {
            get {
                if (localInstance == null) {
                    localInstance = new Score ();
                }

                return localInstance;
            }
        }

        private static Score localInstance;

        [MoonSharpHidden]
        public Score () {
            scoreBank = new Dictionary<string, float> ();
        }

        /// <summary>
        /// Clears all scores
        /// </summary>
        public void Clear () {
            scoreBank.Clear ();
        }

        /// <summary>
        /// Removes a particular score
        /// </summary>
        /// <param name="scoreName">The name of the score</param>
        public void Remove (string scoreName) {
            if (scoreBank.ContainsKey (scoreName)) {
                scoreBank.Remove (scoreName);
            }
        }
    }
}