using CardanoSharp.Wallet.Encoding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using CardanoSharp.Wallet.Extensions;

namespace CardanoSharp.Wallet.Test
{
    /// <summary>
    /// Bech32 Test vectors from BIP-0173
    /// https://github.com/bitcoin/bips/blob/master/bip-0173.mediawiki#test-vectors
    /// </summary>
    public class Bech32Tests
    {
        /// <summary>
        /// 
        /// https://github.com/bitcoin/bips/blob/master/bip-0173.mediawiki#test-vectors
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="expectedHex"></param>
        /// <param name="expectedVer"></param>
        /// <param name="expectedHrp"></param>
        [Theory]
        [InlineData("A12UEL5L", "", 0x00, "")] //@TODO verify
        [InlineData("a12uel5l", "", 0x00, "")] //@TODO verify
        [InlineData("an83characterlonghumanreadablepartthatcontainsthenumber1andtheexcludedcharactersbio1tt5tgs", "", 0x00, "")]
        [InlineData("abcdef1qpzry9x8gf2tvdw0s3jn54khce6mua7lmqqqxw", "00443214c74254b635cf84653a56d7c675be77df", 0x00, "abcdef")]
        [InlineData("11qqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqc8247j", "000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000", 0x00, "1")]
        [InlineData("split1checkupstagehandshakeupstreamerranterredcaperred2y9e3w", "c5f38b70305f5fb6cf03058f3dde463ecd7918f2dc743918f2d", 0x00, "split")]
        [InlineData("?1ezyfcl", "", 0x00, "")]
        [InlineData("BC1QW508D6QEJXTDG4Y5R3ZARVARY0C5XW7KV8F3T4", "0014751e76e8199196d454941c45d1b3a323f1433bd6", 0x00, "BC")]
        [InlineData("tb1qrp33g0q5c5txsp9arysrx4k6zdkfs4nce4xj0gdcccefvpysxf3q0sl5k7", "00201863143c14c5166804bd19203356da136c985678cd4d27a1b8c6329604903262", 0x00, "tb")]
        [InlineData("bc1pw508d6qejxtdg4y5r3zarvary0c5xw7kw508d6qejxtdg4y5r3zarvary0c5xw7k7grplx", "5128751e76e8199196d454941c45d1b3a323f1433bd6751e76e8199196d454941c45d1b3a323f1433bd6", 0x00, "tb")]
        [InlineData("BC1SW50QA3JX3S", "6002751e", 0x00, "tb")]
        [InlineData("bc1zw508d6qejxtdg4y5r3zarvaryvg6kdaj", "5210751e76e8199196d454941c45d1b3a323", 0x00, "tb")]
        [InlineData("tb1qqqqqp399et2xygdj5xreqhjjvcmzhxw4aywxecjdzew6hylgvsesrxh6hy", "0020000000c4a5cad46221b2a187905e5266362b99d5e91c6ce24d165dab93e86433", 0x00, "tb")]
        public void TestValidInputs(string addr, string expectedHex, byte expectedVer, string expectedHrp)
        {
            // Act
            var decoded = Bech32.Decode(addr, out var actualVer, out var actualHrp);
            var actualHex = decoded.ToStringHex();

            // Assert
            Assert.Equal(expectedVer, actualVer);
            Assert.Equal(expectedHrp, actualHrp);
            Assert.Equal(expectedHex, actualHex);
        }

        /// <summary>
        /// 
        /// https://github.com/bitcoin/bips/blob/master/bip-0173.mediawiki#test-vectors
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="expectedError"></param>
        [Theory]
        [InlineData("1nwldj5", "HRP character out of range")]
        [InlineData("1axkwrx", "HRP character out of range")]
        [InlineData("1eym55h", "HRP character out of range")]
        [InlineData("an84characterslonghumanreadablepartthatcontainsthenumber1andtheexcludedcharactersbio1569pvx", "overall max length exceeded")]
        [InlineData("pzry9x0s0muk", "no separator")]
        [InlineData("1pzry9x0s0muk", "empty hrp")]
        [InlineData("x1b4n0q5v", "Invalid data format.")]
        [InlineData("li1dgmt3", "too short checksum")]
        [InlineData("de1lg7wt", "Invalid character in checksum.")]
        [InlineData("A1G7SGD8", "Invalid checksum.")]
        [InlineData("A1G7SGD8", "empty HRP")]
        [InlineData("1qzzfhee", "empty HRP")]
        public void TestInvalidInputs(string addr, string expectedError)
        {
            // arrange
            if(addr == "delig7wt")
            {
                addr += 0xff;
            }
            if (addr == "1nwldj5")
            {
                addr = 0x20 + addr;
            }
            if (addr == "1axkwrx")
            {
                addr = 0x7f + addr;
            }
            if (addr == "1eym55h")
            {
                addr = 0x80 + addr;
            }

            bool raised = false;
            try
            {
                var decoded = Bech32.Decode(addr, out var actualVer, out var actualHrp);
                var hex = decoded.ToStringHex();
            }
            catch (Exception ex)
            {
                raised = true;
                Console.Error.WriteLine(ex);
            }

            Assert.True(raised);
        }
    }
}
