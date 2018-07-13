using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OneGame {
	public class ScriptDatabaseLoader : MonoBehaviour {
		[SerializeField]
		private ScriptCatalog database;

		private void Start () {
			if (!database.IsReady) {
				database.LoadScripts ();
			}
		}
	}
}