using UnityEngine;

namespace OneGame.UI {
	/// <summary>
	/// An interface describing an entity element in the library
	/// </summary>
	public interface ILibraryElement {
		/// <summary>
		/// The current element
		/// </summary>
		ElementData Element { get; set; }

		/// <summary>
		/// The element's rect transform
		/// </summary>
		RectTransform Transform { get; }
	}
}