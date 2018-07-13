using UnityEngine;

namespace OneGame.Lua {
	/// <summary>
	/// An in-house component that is accessible via Lua
	/// </summary>
	public abstract class NativeComponent : MonoBehaviour {

		protected IEntityContainer entity;

		protected virtual void Awake () {
			entity = GetComponent<IEntityContainer> ();
		}

		/// <summary>
		/// Processes the metadata for customization e.g. wepaon damage
		/// </summary>
		/// <param name="data">The data to process</param>
		public virtual void ProcessMetadata (IMetadata data) { }
	}
}