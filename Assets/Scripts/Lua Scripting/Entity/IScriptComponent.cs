using MoonSharp.Interpreter;

namespace OneGame.Lua {
	/// <summary>
	/// Interface describing the script interface on an entity
	/// </summary>
	public interface IScriptComponent {

		void InvokeLuaMethod (string methodName);
		void InvokeLuaMethod (string methodName, DynValue value);
	}
}