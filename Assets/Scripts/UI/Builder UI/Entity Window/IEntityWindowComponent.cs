using System.Collections;
using System.Collections.Generic;
using OneGame.Lua;
using UnityEngine;

namespace OneGame.UI {
	/// <summary>
	/// An enum describing the entity's subwindow types
	/// </summary>
	public enum EntityWindowComponentType { Component, Script }

	/// <summary>
	/// An interface describing a submenu in the entity window
	/// </summary>
	public interface IEntityWindowComponent {
		IEntityContainer Entity { get; set; }
		EntityWindowComponentType Type { get; }

		void Close ();
		void Open ();
	}
}