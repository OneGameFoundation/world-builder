using System.Collections;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using UnityEngine;

namespace OneGame.Lua {
	public class LuaSettings : MonoBehaviour {

		private void Awake () {
			Script.DefaultOptions.DebugPrint += HandleDebugPrintEvent;

			UserData.RegisterAssembly ();
			UserData.RegisterType<Player> ();
			UserData.RegisterType<WeaponType> ();
			UserData.RegisterType<EquipType> ();
            UserData.RegisterType<BodyPart> ();
        }

		private void OnDisable () {
			Script.DefaultOptions.DebugPrint -= HandleDebugPrintEvent;
		}

		private void HandleDebugPrintEvent (string s) {
			Debug.Log (s);
		}
	}
}