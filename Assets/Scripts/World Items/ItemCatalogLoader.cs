using System.Collections;
using UnityEngine;

namespace OneGame {
	using LoadingStatus = ItemCatalog.LoadingStatus;

	/// <summary>
	/// A utility script that loads the ItemCatalog instance
	/// </summary>
	public class ItemCatalogLoader : MonoBehaviour {
		[SerializeField]
		private ItemCatalog catalog;

		private void Start () {
			catalog.LoadAssets ();
		}
	}
}