using CardanoSharp.Wallet.Extensions;
using System;
using System.Collections.Generic;

namespace CardanoSharp.Wallet.Encoding
{
    public static partial class Bech32
    {
        /// <summary>
        /// Converts a bech-32 encoded string back to its byte array representation.
        /// </summary>
        /// <exception cref="FormatException"/>
        /// <param name="bech32EncodedString">Bech-32 encoded string.</param>
        /// <param name="witVer">Witness version</param>
        /// <param name="hrp">Human readable part</param>
        /// <returns>Byte array of the given string.</returns>
        public static byte[] Decode(string bech32EncodedString, out byte witVer, out string hrp)
        {
            byte[] b32Arr = Bech32Decode(bech32EncodedString, out hrp);
            if (b32Arr.Length < CheckSumSize)
            {
                throw new FormatException("Invalid data length.");
            }
            if (!VerifyChecksum(hrp, b32Arr))
            {
                throw new FormatException("Invalid checksum.");
            }

            byte[] data = b32Arr.SubArray(0, b32Arr.Length - CheckSumSize);
            byte[] b256Arr = ConvertBits(data, 5, 8, false);
            if (b256Arr == null)
            {
                throw new FormatException("Invalid data format.");
            }

            witVer = b32Arr[0];
            return b256Arr;
        }

        private static byte[] Bech32Decode(string bech32EncodedString, out string hrp)
        {
            bech32EncodedString = bech32EncodedString.ToLower();

            int sepIndex = bech32EncodedString.LastIndexOf(Separator);
            hrp = bech32EncodedString.Substring(0, sepIndex);
            string data = bech32EncodedString.Substring(sepIndex + 1);

            byte[] b32Arr = new byte[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                b32Arr[i] = (byte)B32Chars.IndexOf(data[i]);
            }

            return b32Arr;
        }

        private static byte[] ConvertBits(byte[] data, int fromBits, int toBits, bool pad = true)
        {
            // TODO: Optimize Looping
            // We can use a method similar to BIP39 here to avoid the nested loop, usage of List, increase the speed,
            // and shorten this function to 3 lines.
            // Or convert to ulong[], loop through it (3 times) take 5 bits at a time or 8 bits at a time...
            int acc = 0;
            int bits = 0;
            int maxv = (1 << toBits) - 1;
            int maxacc = (1 << (fromBits + toBits - 1)) - 1;

            List<byte> result = new List<byte>();
            foreach (var b in data)
            {
                // Speed doesn't matter for this class but we can skip this check for 8 to 5 conversion.
                if ((b >> fromBits) > 0)
                {
                    return null;
                }
                acc = ((acc << fromBits) | b) & maxacc;
                bits += fromBits;
                while (bits >= toBits)
                {
                    bits -= toBits;
                    result.Add((byte)((acc >> bits) & maxv));
                }
            }
            if (pad)
            {
                if (bits > 0)
                {
                    result.Add((byte)((acc << (toBits - bits)) & maxv));
                }
            }
            else if (bits >= fromBits || (byte)((acc << (toBits - bits)) & maxv) != 0)
            {
                return null;
            }
            return result.ToArray();
        }

    }
}
