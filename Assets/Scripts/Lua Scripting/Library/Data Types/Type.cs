using System.Collections;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using UnityEngine;

namespace OneGame.Lua {
	/// <summary>
	/// A Lua representation of a System.Type
	/// </summary>
	[MoonSharpUserData]
	public struct Type {
		public System.Type type;
	}
}