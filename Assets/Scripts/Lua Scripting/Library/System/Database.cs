using System;
using MoonSharp.Interpreter;

namespace OneGame.Lua {
    /// <summary>
    /// A c# bridge for finding assets in the library
    /// </summary>
    [MoonSharpUserData]
    public class Database {
        [MoonSharpHidden]
        public static event Func<string, Item> OnItemFindRequest;
        [MoonSharpHidden]
        public static event Func<string, string> OnScriptFindRequest;
        [MoonSharpHidden]
        public static event Func<string, Type> OnComponentFindRequest;

        /// <summary>
        /// Finds a component with a given name
        /// </summary>
        /// <param name="name">The name of the component</param>
        public static Type FindComponent (string name) {
            if (OnComponentFindRequest != null) {
                return OnComponentFindRequest (name);
            }

            return default (Type);
        }

        /// <summary>
        /// Finds an item with a given id
        /// </summary>
        /// <param name="id">The unique id of the item</param>
        public static Item FindItem (uint id) {
            if (OnItemFindRequest != null) {
                OnItemFindRequest (id.ToString ("x8"));
            }

            return default (Item);
        }

        /// <summary>
        /// Finds an item with a matching id or name
        /// </summary>
        /// <param name="name">The name of the item</param>
        public static Item FindItem (string name) {
            if (OnItemFindRequest != null) {
                return OnItemFindRequest (name);
            }

            return default (Item);
        }

        /// <summary>
        /// Finds a lua template script with a matching id
        /// </summary>
        /// <param name="id">The id of the script</param>
        public static string FindScript (uint id) {
            if (OnScriptFindRequest != null) {
                OnScriptFindRequest (id.ToString ("x8"));
            }

            return string.Empty;
        }

        /// <summary>
        /// Finds a lua template script with a matching name
        /// </summary>
        /// <param name="name">The name of the script</param>
        public static string FindScript (string name) {
            if (OnScriptFindRequest != null) {
                return OnScriptFindRequest (name);
            }

            return string.Empty;
        }
    }
}