using CardanoSharp.Wallet.Extensions;
using Xunit;

namespace CardanoSharp.Wallet.Test.Extensions
{
    public class ByteArrayExtensionTests
    {
        [Fact]
        public void LastBits_Returns_LastNBits()
        {
            byte b = 0x83;
            int last = b.LastBits(4);
            Assert.Equal(0x3, last);
        }
    }
}
