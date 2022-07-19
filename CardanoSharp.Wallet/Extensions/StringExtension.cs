using System;

namespace CardanoSharp.Wallet.Extensions
{
	public static class StringExtension
	{
		public static string ToStringHex(this byte[] bytes)
		{
			var hex = BitConverter
				.ToString(bytes)
				.Replace("-", "")
				.ToLower();

			return hex;
		}

		public static string GetString(this byte[] bytes)
		{
			return System.Text.Encoding.UTF8.GetString(bytes);
		}

		public static byte[] ToBytes(this string value)
		{
			return System.Text.Encoding.UTF8.GetBytes(value);
		}
	}
}