using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CardanoSharp.Wallet.Test
{
    public class AddressTests
    {
        private readonly IAddressService _keyService;

        private string _restoreTest_Entropy = "475083b81730de275969b1f18db34b7fb4ef79c66aa8efdd7742f1bcfe204097";
        private string _restoreTest_RootKeyWithChainCode = "68f71a097ad8c9baed0798313233b2e6a557a0493e533d4a39f984b0b14935493c2bbecb4fa565d18d51164e0ff45f4f1c5a2be252ca9b4939140aff3f75389f";

        public AddressTests()
        {
            _keyService = new AddressService();
        }

        [Fact]
        public void RestoreTest()
        {
            //arrange

            //act

            //assert

        }
    }
}
