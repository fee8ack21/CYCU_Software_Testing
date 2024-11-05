using System.Security.Cryptography;
using System.Text;

namespace App.Common.Utilities
{
	public static class HashUtility
	{
		public static string GenerateSha256Hash(string value, string salt)
		{
			var _value = Encoding.UTF8.GetBytes(value);
			var _salt = Encoding.UTF8.GetBytes(salt);

			var salted = _value.Concat(_salt).ToArray();
			var hash = SHA256.HashData(salted);
			return BitConverter.ToString(hash).Replace("-", string.Empty);
		}

		public static bool VerifySha256Hash(string value, string salt, string comparedHash)
		{
			var hash = GenerateSha256Hash(value, salt);
			return string.Equals(hash, comparedHash);
		}
	}
}
