namespace OneGame {
    /// <summary>
    /// A collection of math-related utilities
    /// </summary>
	public static class MathUtility {
        /// <summary>
        /// Converts an unsigned integer into a string
        /// </summary>
        public static string ToHexString (this uint id) {
            return id.ToString ("x8");
        }

        /// <summary>
        /// Converts a string to a unsigned integer
        /// </summary>
		public static uint ToUInt (this string id) {
            uint num;

            if (uint.TryParse (id, System.Globalization.NumberStyles.HexNumber, null, out num)) {
                return num;
            }

            return default (uint);
        }
    }
}