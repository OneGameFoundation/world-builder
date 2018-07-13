using MoonSharp.Interpreter;

namespace OneGame.Lua {
    /// <summary>
    /// A simple data structure containing the item's id and name
    /// </summary>
    [MoonSharpUserData]
    public struct Item {
        public uint id;
        public string name;
    }
}