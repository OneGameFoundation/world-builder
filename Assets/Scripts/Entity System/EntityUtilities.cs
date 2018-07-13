using System;
using System.Security.Cryptography;

namespace OneGame
{
    internal static class EntityUtilities {
		private static object lockObj = new object ();

		/// <summary>
		/// Generates a unique 6-character string id
		/// </summary>
		internal static string GetUniqueId () {
			lock (lockObj) {
				var cryptoService = new RNGCryptoServiceProvider ();
				var bytes = new byte[3];
				cryptoService.GetBytes (bytes);
				return BitConverter.ToString (bytes).Replace ("-", string.Empty).ToLower();
			}
		}
	}
}