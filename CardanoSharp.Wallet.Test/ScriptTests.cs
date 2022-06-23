using CardanoSharp.Wallet.Extensions;
using Xunit;
using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.TransactionBuilding;
using CardanoSharp.Wallet.Utilities;

namespace CardanoSharp.Wallet.Test
{
    public class ScriptTests
    {
        public ScriptTests()
        {
        }

        [Fact]
        public void SingleScriptAll()
        {
            //JSON Example from cardano-cli
            //{
            //  "type": "all",
            //  "scripts":
            //  [
            //    {
            //      "type": "sig",
            //      "keyHash": "0a2c76fca4bce93da9ea7c11095e17244b3772e0077c47a12640c89f"
            //    }
            //  ]
            //}
            var script = ScriptAllBuilder.Create
                .SetScript( 
                    NativeScriptBuilder.Create
                        .SetKeyHash("0a2c76fca4bce93da9ea7c11095e17244b3772e0077c47a12640c89f".HexToByteArray()))
                .Build();

            var actual = script.GetPolicyId().ToStringHex();
            var expected = "f7932212f88d81cbea1f5e50b02180b0f5fc8a91ad6e0afe21308442";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void KeyHashTest()
        {
            var policyVkey = "848AC717B552FCD1F2DCB4933E4A8198187E7E424693B51E1B8B16250F3CADFE".HexToByteArray();
            var policyKeyHash = HashUtility.Blake2b224(policyVkey);

            Assert.Equal("5cd719e99fdd80fd889e82cf012e64d5da7ab35364bb02163bc93974", policyKeyHash.ToStringHex());
        }
    }
}
