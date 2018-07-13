using MoonSharp.Interpreter;
using OneGame.Lua;
using UnityEngine;

namespace OneGame {
	/// <summary>
	/// A mini game engine that controls how the game should start and end
	/// </summary>
	public class MicroGameEngine : ScriptableObject, IEntityContainer, IScriptRunner {
		public Entity Entity { get; private set; }
		public GameObject GameObject { get { return null; } }
		public Transform Transform { get { return null; } }
		public LuaScript Script { get { return luaScript; } set { luaScript = value; } }
		public NativeComponent[] ActiveComponents { get { return new NativeComponent[0]; } }

		public string AssetId { get { return "00000000"; } }
		public string UniqueId { get { return "000001"; } }

		[SerializeField]
		private ScriptManager scriptManager;
		private LuaScript luaScript;

		private void OnEnable () {
			luaScript = new LuaScript {
				id = 1,
					code = string.Empty,
					script = new Script (),
					properties = new ScriptProperty[0]
			};
		}

		/// <summary>
		/// Calls 'onGameInit' on the engine to initialize the game
		/// </summary>
		public void InitializeGame () {
			var keys = luaScript.script.Globals.Keys;
			luaScript.script.ApplyDefaultValues ();
			luaScript.script.CallFunction ("onGameInit");
		}

		/// <summary>
		/// A dummy method that adds a component to the entity
		/// </summary>
		/// <param name="type">The type of component</param>
		/// <param name="name">The name to assign the component to</param>
		public void AddComponent (Type type, string name) {	}

		/// <summary>
		/// Invokes a lua method
		/// </summary>
		/// <param name="methodName">The method to call</param>
		public void InvokeLuaMethod (string methodName) {
			luaScript.script.CallFunction (methodName);
		}

		/// <summary>
		/// Invokes a lua method with a given Dynamic Value
		/// </summary>
		/// <param name="methodName">The method to call</param>
		/// <param name="value">The value to pass</param>
		public void InvokeLuaMethod (string methodName, DynValue value) {
			luaScript.script.CallFunction (methodName, value);
		}
	}
}