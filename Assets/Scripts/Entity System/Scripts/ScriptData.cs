namespace OneGame {
	/// <summary>
	/// A container of a script and its metadata
	/// </summary>
	[System.Serializable]
	public struct ScriptData {
		/// <summary>
		/// The script's id
		/// </summary>
		public uint id;

		/// <summary>
		/// The name of the script
		/// </summary>
		public string name;

		/// <summary>
		/// The script's lua code
		/// </summary>
		public string script;
	}
}