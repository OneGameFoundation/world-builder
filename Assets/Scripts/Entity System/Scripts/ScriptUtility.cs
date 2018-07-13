using System.Collections;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using UnityEngine;

namespace OneGame {
	/// <summary>
	/// A utility class used for producing script properties
	/// </summary>
	public static class ScriptUtility {

		private static HashSet<string> ignoreSet = new HashSet<string> (new string[] { "_VERSION" });

		/// <summary>
		/// Applies all properties on the script
		/// </summary>
		/// <param name="script">The script to apply</param>
		/// <param name="properties">The properties to set values with</param>
		public static void ApplyProperties (Script script, ScriptProperty[] properties) {
			var globals = script.Globals;

			for (var i = 0; i < properties.Length; ++i) {
				var property = properties[i];

				switch (property.type) {
					case PropertyType.Boolean:
						globals[property.name] = DynValue.NewBoolean (property.value == "True");
						break;

					case PropertyType.Number:
						double num;

						if (double.TryParse (property.value, out num)) {
							globals[property.name] = DynValue.NewNumber (num);
						}
						break;

					case PropertyType.String:
						globals[property.name] = DynValue.NewString (property.value);
						break;
				}
			}
		}

		/// <summary>
		/// Extracts the properties from a running script
		/// </summary>
		/// <param name="script">The script to extract from</param>
		public static ScriptProperty[] ExtractProperties (Script script) {
			var globals = script.Globals;
			var keys = script.Globals.Keys;
			var properties = new List<ScriptProperty> ();

			foreach (var key in keys) {
				var value = globals.Get (key);
				var name = key.String.Replace ("\"", "");

				if (ignoreSet.Contains (name)) { continue; }

				var property = new ScriptProperty ();
				property.name = name;

				var isValidProperty = true;

				switch (value.Type) {
					case DataType.Boolean:
						property.value = value.Boolean.ToString ();
						property.type = PropertyType.Boolean;
						break;

					case DataType.Number:
						property.value = value.Number.ToString ();
						property.type = PropertyType.Number;
						break;

					case DataType.String:
						property.value = value.String;
						property.type = PropertyType.String;
						break;

					default:
						isValidProperty = false;
						break;
				}

				if (isValidProperty) {
					properties.Add (property);
				}
			}

			return properties.ToArray ();
		}
	}
}