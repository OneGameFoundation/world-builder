using System;
using System.Collections.Generic;
using UnityEngine;

namespace OneGame.Lua {
	using Object = UnityEngine.Object;

	/// <summary>
	/// A collection of default assets to fall back to if the asset was not
	/// defined by the player
	/// </summary>
	public class WeaponAssetDefaults : ScriptableObject {
		/// <summary>
		/// The current bank instance
		/// </summary>
		public static WeaponAssetDefaults Instance { get; private set; }

		[SerializeField]
		private AssetNameMap[] assets;

		private Dictionary<string, Object> assetBank;

		/// <summary>
		/// A one-to-one relationship between a name and an object
		/// </summary>
		[Serializable]
		public struct AssetNameMap {
			public string name;
			public Object asset;
		}

		private void OnEnable () {
			assetBank = new Dictionary<string, Object> (assets.Length);
			for (var i = 0; i < assets.Length; ++i) {
				var asset = assets[i];
				assetBank.Add (asset.name, asset.asset);
			}

			Instance = this;
		}

		private void OnDisable () {
			assetBank.Clear ();
		}

		/// <summary>
		/// Gets an asset with a matching name association
		/// </summary>
		/// <param name="name">The name of the asset</param>
		public T GetAsset<T> (string name) where T : Object {
			Object asset;

			if (assetBank.TryGetValue (name, out asset)) {
				return asset as T;
			}

			return default (T);
		}
	}
}