using UnityEngine;

namespace OneGame {
    /// <summary>
    /// A utility script that "warms" scriptable objects so that
    /// they are ready to use by other scripts
    /// </summary>
    public class ScriptableObjectWarmer : MonoBehaviour {

        [SerializeField]
        private ScriptableObject[] objectsToWarm;

        private void Start () {
            // Perform a simple operaton on each scriptable
            var idValue = 0;
            for (var i = 0; i < objectsToWarm.Length; ++i) {
                idValue += objectsToWarm[i].GetInstanceID ();
            }
        }
    }
}