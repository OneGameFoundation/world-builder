using UnityEngine;

namespace OneGame.UI {
	/// <summary>
	/// A one-to-one relationship between a component name and a sprite
	/// </summary>
	public class EntityComponentMap : ScriptableObject {
		[SerializeField]
		private Sprite defaultSprite;
		[SerializeField]
		private Map[] mappings;

		[System.Serializable]
		public struct Map {
			public string componentName;
			public Sprite icon;
		}

		public Sprite GetSprite (string name) {
			for (var i = 0; i < mappings.Length; ++i) {
				var map = mappings[i];

				if (map.componentName == name) {
					return map.icon;
				}
			}

			return defaultSprite;
		}
	}
}