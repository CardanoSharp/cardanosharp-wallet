using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CardanoSharp.Wallet.Test
{
    public class Bech32Tests
    {
        /// <summary>
        /// Check if 1111 0111 1011 1101
        /// </summary>
        /// <param name="data"></param>
        /// <param name="result"></param>
        [Theory]
        [InlineData(new byte[] { 0xf7, 0xbd }, new byte[] { 0x1e, 0x1e, 0x1e, 0x10 })] 
        [InlineData(new byte[] { 0xf7, 0xbd, 0xe0 }, new byte[] { 0x1e, 0x1e, 0x1e, 0x1e, 0x0 })]
        [InlineData(new byte[] { 0xf7, 0xbd, 0xef, 0x7b, 0xde }, new byte[] { 0x1e, 0x1e, 0x1e, 0x1e, 0x1e, 0x1e, 0x1e, 0x1e })]
        public void TestConvert(byte[] data, byte[] result)
        {
            //arrange
            int fromBits = 8;
            int toBits = 5;
            var view = data.Select(b => Convert.ToString(b, 2));
            //act
            var convert = ConvertBitsFast(data, fromBits, toBits, true);

            //assert
            Assert.Equal(result, convert);
        }
    
    static byte[] ConvertBitsFast(ReadOnlySpan<byte> data, int fromBits, int toBits, bool pad = true)
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
