using System;

namespace OneGame {
	[Serializable]
	public struct ScriptProperty {
		public string name;
		public string value;
		public PropertyType type;
	}
}