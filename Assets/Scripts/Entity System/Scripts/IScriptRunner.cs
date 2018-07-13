using MoonSharp.Interpreter;

namespace OneGame {
	/// <summary>
	/// An interface describing the ability to run lua-based scripts
	/// </summary>
	public interface IScriptRunner {
		/// <summary>
		/// Ivokes a method in custom lua scripts
		/// </summary>
		void InvokeLuaMethod (string name);

		/// <summary>
		/// Invokes a method in custom lua scripts with a single parameter
		/// </summary>
		void InvokeLuaMethod (string name, DynValue value);
	}
}