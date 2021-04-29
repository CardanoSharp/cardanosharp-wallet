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
        private string _restoreTest_PrivateKey_withChainCode = "b8f2bece9bdfe2b0282f5bad705562ac996efb6af96b648f4445ec44f47ad95c10e3d72f26ed075422a36ed8585c745a0e1150bcceba2357d058636991f38a3791e248de509c070d812ab2fda57860ac876bc489192c1ef4ce253c197ee219a4";
        
        public KeyTests()
        {
            _keyService = new KeyService();
        }

        [Fact]
        public void RestoreTest_Entropy()
        {
            //arrange
            var mnemonic = "elder lottery unlock common assume beauty grant curtain various horn spot youth exclude rude boost fence used two spawn toddler soup awake across use";

            //act
            var entropy = _keyService.Restore(mnemonic);
            var rootKey = _keyService.GetRootKey(entropy);

            //assert
            Assert.Equal(entropy.ToStringHex(), _restoreTest_Entropy);
        }

        [Fact]
        public void RestoreTest_PrivateKey_withChainCode()
        {
            //arrange
            var mnemonic = "art forum devote street sure rather head chuckle guard poverty release quote oak craft enemy";

            //act
            var entropy = _keyService.Restore(mnemonic);
            var rootKey = _keyService.GetRootKey(entropy);

            //assert
            Assert.Equal(rootKey.Item1.ToStringHex(), _restoreTest_PrivateKey_withChainCode.Substring(0, 128));
            Assert.Equal(rootKey.Item2.ToStringHex(), _restoreTest_PrivateKey_withChainCode.Substring(128));
        }
    }
}
