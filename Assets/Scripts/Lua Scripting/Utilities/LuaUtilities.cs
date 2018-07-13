using MoonSharp.Interpreter;

namespace OneGame.Lua {
    /// <summary>
    /// A collection of convenience methods for calling lua scripts
    /// </summary>
    public static class LuaUtilities {
        public static void ApplyDefaultValues (this Script script) {
            script.Globals["vec3"] = typeof (Vec3);
            script.Globals["spawner"] = typeof (SpawnManager);
            script.Globals["database"] = typeof (Database);
            script.Globals["timer"] = typeof (Timer);
            script.Globals["input"] = typeof (Lua.Input);
            script.Globals["player"] = Player.Instance;
            script.Globals["inventory"] = Player.Instance.inventory;
            script.Globals["game"] = typeof (Game);
            script.Globals["score"] = Score.Instance;
            script.Globals["weaponType"] = UserData.CreateStatic<WeaponType> ();
            script.Globals["equipType"] = UserData.CreateStatic<EquipType> ();
            script.Globals["bodyPart"] = UserData.CreateStatic<BodyPart> ();
        }

        /// <summary>
        /// Calls a function on a script instance
        /// </summary>
        /// <param name="script">The script to call on</param>
        /// <param name="function">The function to call</param>
        public static void CallFunction (this Script script, string function) {
            var func = script.Globals.Get (function);

            if (func.IsNotNil ()) {
                script.Call (func);
            }
        }

        /// <summary>
        /// Calls a function on a script instance
        /// </summary>
        /// <param name="script">The script to call on</param>
        /// <param name="function">The function to call</param>
        /// <param name="value">The value to pass</param>
        public static void CallFunction (this Script script, string function, DynValue value) {
            var func = script.Globals.Get (function);

            if (func.IsNotNil ()) {
                script.Call (func, value);
            }
        }
    }
}