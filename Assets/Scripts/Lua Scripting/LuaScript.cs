using System;
using MoonSharp.Interpreter;

namespace OneGame.Lua {
	/// <summary>
	/// A c# interface for a lua script running on an entity
	/// </summary>
	[Serializable]
	[LitJson.JsonIgnoreMember (new string[] { "script", "Empty" })]
	public struct LuaScript {
		/// <summary>
		/// The id that this script belongs to
		/// </summary>
		public uint id;

		/// <summary>
		/// The script's code
		/// </summary>
		public string code;

		/// <summary>
		/// The running script
		/// </summary>
		public Script script;

		/// <summary>
		/// A collection of editable script variables
		/// </summary>
		public ScriptProperty[] properties;

		public static LuaScript Empty {
			get {
				return new LuaScript {
					id = 0,
						code = string.Empty,
						properties = new ScriptProperty[0]
				};
			}
		}

		public LuaScript CreateCopy () {
			if (this.properties.Length > 0) {
				var properties = new ScriptProperty[this.properties.Length];
				this.properties.CopyTo (properties, 0);
			}
			return new LuaScript {
				id = id,
					code = code,
					script = script,
					properties = properties
			};
		}
	}
}