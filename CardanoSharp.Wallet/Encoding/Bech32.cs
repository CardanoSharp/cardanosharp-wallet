using CardanoSharp.Wallet.Extensions;
using System;
using System.Linq;

namespace CardanoSharp.Wallet.Encoding
{
    public static partial class Bech32
    {
        /// <summary>
        /// Maximum length of the whole Bech32 string (hrp + separator + data)
        /// </summary>
        private const int CheckSumSize = 6;
        private const int HrpMinLength = 1;
        private const int HrpMaxLength = 83;
        private const int HrpMinValue = 33;
        private const int HrpMaxValue = 126;
        private const char Separator = '1';
        private const string B32Chars = "qpzry9x8gf2tvdw0s3jn54khce6mua7l";
        private static uint[] _generator = { 0x3b6a57b2u, 0x26508e6du, 0x1ea119fau, 0x3d4233ddu, 0x2a1462b3u };

        /// <summary>
        /// Checks to see if a given string is a valid bech-32 encoded string with a valid checksum.
        /// </summary>
        /// <param name="bech32EncodedString">Input string to check</param>
        /// <returns>True if input was a valid bech-32 encoded string with checksum, false if otherwise.</returns>
        public static bool IsValid(string bech32EncodedString)
        {
            if (!HasValidChars(bech32EncodedString))
            {
                return false;
            }

            byte[] b32Arr = Bech32Decode(bech32EncodedString, out string hrp);
            if (b32Arr.Length < CheckSumSize)
            {
                return false;
            }

            return VerifyChecksum(hrp, b32Arr);
        }

        /// <summary>
        /// Checks to see if a given string is a valid bech-32 encoded string.
        /// <para/>* Doesn't verify checksum.
        /// <para/>* Doesn't verify data length (since it requires decoding first).
        /// </summary>
        /// <param name="bech32EncodedString">Input string to check.</param>
        /// <returns>True if input was a valid bech-32 encoded string (without verifying checksum).</returns>
        public static bool HasValidChars(string bech32EncodedString)
        {
            if (string.IsNullOrEmpty(bech32EncodedString))
            {
                return false;
            }

            // Reject mixed upper and lower characters.
            if (bech32EncodedString.ToLower() != bech32EncodedString && bech32EncodedString.ToUpper() != bech32EncodedString)
            {
                return false;
            }

            // Check if it has a separator
            int sepIndex = bech32EncodedString.LastIndexOf(Separator);
            if (sepIndex == -1)
            {
                return false;
            }

            // Validate human readable part
            string hrp = bech32EncodedString.Substring(0, sepIndex);
            if (!IsValidHrp(hrp))
            {
                return false;
            }

            // Validate data part
            string data = bech32EncodedString.Substring(sepIndex + 1);
            if (data.Length < CheckSumSize || !data.All(x => B32Chars.Contains(char.ToLower(x))))
            {
                return false;
            }

            return true;
        }

        private static bool IsValidHrp(string hrp)
        {
            return !string.IsNullOrWhiteSpace(hrp) &&
                    hrp.Length >= HrpMinLength &&
                    hrp.Length <= HrpMaxLength &&
                    hrp.All(x => (byte)x >= HrpMinValue && (byte)x <= HrpMaxValue);
        }


        private static uint Polymod(byte[] data)
        {
            uint chk = 1;
            foreach (byte b in data)
            {
                uint temp = chk >> 25;
                chk = ((chk & 0x1ffffff) << 5) ^ b;
                for (int i = 0; i < 5; i++)
                {
                    if (((temp >> i) & 1) == 1)
                    {
                        chk ^= _generator[i];
                    }
                }
            }
            return chk;
        }

        private static byte[] ExpandHrp(string hrp)
        {
            byte[] result = new byte[(2 * hrp.Length) + 1];
            for (int i = 0; i < hrp.Length; i++)
            {
                result[i] = (byte)(hrp[i] >> 5);
                result[i + hrp.Length + 1] = (byte)(hrp[i] & 0b0001_1111 /*=31*/);
            }
            return result;
        }

        private static bool VerifyChecksum(string hrp, byte[] data)
        {
            byte[] temp = ExpandHrp(hrp).ConcatFast(data);
            return Polymod(temp) == 1;
        }
    }
}