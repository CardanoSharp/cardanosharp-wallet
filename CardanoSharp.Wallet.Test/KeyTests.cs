using CardanoSharp.Wallet.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace CardanoSharp.Wallet.Test
{
    public class KeyTests
    {
        private readonly IKeyService _keyService;

        private string _restoreTest_Entropy = "475083b81730de275969b1f18db34b7fb4ef79c66aa8efdd7742f1bcfe204097";
        private string _restoreTest_RootKeyWithChainCode = "68f71a097ad8c9baed0798313233b2e6a557a0493e533d4a39f984b0b14935493c2bbecb4fa565d18d51164e0ff45f4f1c5a2be252ca9b4939140aff3f75389f";
        
        public KeyTests()
        {
            _keyService = new KeyService();
        }

        [Fact]
        public void RestoreTest()
        {
            //arrange
            var mnemonic = "elder lottery unlock common assume beauty grant curtain various horn spot youth exclude rude boost fence used two spawn toddler soup awake across use";

            //act
            var entropy =  _keyService.Restore(mnemonic);

            //assert
            Assert.Equal(entropy.ToStringHex(), _restoreTest_Entropy);
        }
    }
}
