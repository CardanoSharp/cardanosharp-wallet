using CardanoSharp.Wallet.Models.Keys;
using System;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Extensions.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CardanoSharp.Wallet.Test
{
    public class KeyPairTests
    {

        [Theory]
        [InlineData("a simple message")]
        public void GenerationTest(string message)
        {
            var keyPair = KeyPair.GenerateKeyPair();
            var messageByte = message.HexToByteArray();

            var signature = keyPair.PrivateKey.Sign(messageByte);
            var verified = keyPair.PublicKey.Verify(messageByte, signature);

            Assert.True(verified);
        }
    }
}
