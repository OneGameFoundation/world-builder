using System;

namespace OneGame {
	/// <summary>
	/// A utility class that extends the save data object for convenience
	/// </summary>
	public static class SaveExtensions {

		/// <summary>
		/// Gets a string value from the save data
		/// </summary>
		/// <param name="data">The data to read from</param>
		/// <param name="name">The name of the string value</param>
		/// <param name="defaultValue">The default value to return</param>
		public static bool GetBoolValue (this SaveData data, string name, bool defaultValue = false) {
			var names = data.boolNames;
			var values = data.boolValues;

			for (var i = 0; i < names.Length; ++i) {
				if (names[i] == name) {
					return values[i];
				}
			}

			return defaultValue;
		}

		/// <summary>
		/// Gets a number value from the save data
		/// </summary>
		/// <param name="data">The data to read from</param>
		/// <param name="name">The name of the number value</param>
		/// <param name="defaultValue">The default value to return</param>
		public static double GetNumberValue (this SaveData data, string name, double defaultValue = 0) {
			var names = data.numberNames;
			var values = data.numberValues;

			for (var i = 0; i < names.Length; ++i) {
				if (names[i] == name) {
					return values[i];
				}
			}

			return defaultValue;
		}

		/// <summary>
		/// Gets a string value from the save data
		/// </summary>
		/// <param name="data">The data to read from</param>
		/// <param name="name">The name of the string value</param>
		/// <param name="defaultValue">The default value to return</param>
		/// <returns></returns>
		public static string GetStringValue (this SaveData data, string name, string defaultValue = "") {
			var names = data.stringNames;
			var values = data.stringValues;

			for (var i = 0; i < names.Length; ++i) {
				if (names[i] == name) {
					return values[i];
				}
			}

			return defaultValue;
		}

		/// <summary>
		/// Sets a boolean value with a mapped name
		/// </summary>
		/// <param name="data">The current save data</param>
		/// <param name="name">The name of the value</param>
		/// <param name="value">The value to set</param>
		public static void SetBoolValue (ref SaveData data, string name, bool value) {
			var nameArray = data.boolNames;
			var valueArray = data.boolValues;
			var index = GetIndexOf (name, nameArray);

			if (index > -1) {
				valueArray[index] = value;
				data.boolValues = valueArray;
			} else {
				var length = nameArray.Length + 1;

				Array.Resize (ref nameArray, length);
				nameArray[length - 1] = name;

				Array.Resize (ref valueArray, length);
				valueArray[length - 1] = value;
			}

			data.boolNames = nameArray;
			data.boolValues = valueArray;
		}

		/// <summary>
		/// Sets a number value with a mapped name
		/// </summary>
		/// <param name="data">The current save data</param>
		/// <param name="name">The name of the value</param>
		/// <param name="value">The value to set</param>
		public static void SetNumberValue (ref SaveData data, string name, double value) {
			var nameArray = data.numberNames;
			var valueArray = data.numberValues;
			var index = GetIndexOf (name, nameArray);

			if (index > -1) {
				valueArray[index] = value;
				data.numberValues = valueArray;
			} else {
				var length = nameArray.Length + 1;

				Array.Resize (ref nameArray, length);
				nameArray[length - 1] = name;

				Array.Resize (ref valueArray, length);
				valueArray[length - 1] = value;
			}

			data.numberNames = nameArray;
			data.numberValues = valueArray;
		}

		/// <summary>
		/// Sets a string value with a mapped name
		/// </summary>
		/// <param name="data">The current save data</param>
		/// <param name="name">The name of the value</param>
		/// <param name="value">The value to set</param>
		public static void SetStringValue (ref SaveData data, string name, string value) {
			var nameArray = data.stringNames;
			var valueArray = data.stringValues;
			var index = GetIndexOf (name, nameArray);

			if (index > -1) {
				valueArray[index] = value;
				data.stringValues = valueArray;
			} else {
				var length = nameArray.Length + 1;

				Array.Resize (ref nameArray, length);
				nameArray[length - 1] = name;

				Array.Resize (ref valueArray, length);
				valueArray[length - 1] = value;
			}

			data.stringNames = nameArray;
			data.stringValues = valueArray;
		}

		/// <summary>
		/// Creates a new extended array with an appended element
		/// </summary>
		/// <param name="currentArray">The existing array</param>
		/// <param name="value">The value to append</param>
		/// <typeparam name="T">The type of eleent in the array</typeparam>
		/// <returns>An array one element larger</returns>
		private static T[] ExtendArray<T> (T[] currentArray, T value) {
			var array = new T[currentArray.Length + 1];
			currentArray.CopyTo (array, 0);

			array[array.Length - 1] = value;

			return array;
		}

		/// <summary>
		/// Finds the first index of an element with a matching name
		/// </summary>
		/// <param name="name">The name of the element</param>
		/// <param name="values">The array to query in</param>
		/// <returns>-1 if no element is foudn</returns>
		private static int GetIndexOf (string name, string[] values) {
			for (var i = 0; i < values.Length; ++i) {
				if (values[i] == name) {
					return i;
				}
			}

			return -1;
		}

	}
}