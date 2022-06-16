using CardanoSharp.Wallet.Extensions;
using System;
using System.Text;

namespace CardanoSharp.Wallet.Encoding
{
    public static partial class Bech32
    {
        /// <summary>
        /// Converts the given byte array to its equivalent string representation that is encoded with bech-32 digits,
        /// with 6 byte appended checksum.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        /// <param name="data">Byte array to encode</param>
        /// <param name="hrp">Human readable part</param>
        /// <returns>The string representation in bech-32 with a checksum.</returns>
        public static string Encode(byte[] data, string hrp)
        {
            if (data == null || data.Length == 0)
                throw new ArgumentNullException(nameof(data), "Data can not be null or empty.");
            if (!IsValidHrp(hrp))
                throw new FormatException("Invalid HRP.");

            byte[] b32Arr = ConvertBits(data, 8, 5, true);
            byte[] checksum = CalculateCheckSum(hrp, b32Arr);
            b32Arr = b32Arr.ConcatFast(checksum);
            StringBuilder result = new StringBuilder(b32Arr.Length + 1 + hrp.Length);
            result.Append($"{hrp}{Separator}");
            foreach (var b in b32Arr)
            {
                result.Append(B32Chars[b]);
            }

            return result.ToString();
        }

        private static byte[] CalculateCheckSum(string hrp, byte[] data)
        {
            // expand hrp, append data to it, and then add 6 zero bytes at the end.
            byte[] bytes = ExpandHrp(hrp).ConcatFast(data).ConcatFast(new byte[CheckSumSize]);

            // get polymod of the whole data and then flip the least significant bit.
            uint pm = Polymod(bytes) ^ 1;

            byte[] result = new byte[6];
            for (int i = 0; i < 6; i++)
            {
                result[i] = (byte)((pm >> 5 * (5 - i)) & 0b0001_1111 /*=31*/);
            }
            return result;
        }
    }
}
