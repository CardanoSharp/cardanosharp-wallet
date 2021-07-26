using CardanoSharp.Wallet.Models.Transactions;
using CardanoSharp.Wallet.Models.Transactions.Scripts;
using System;
using System.Collections.Generic;
using CardanoSharp.Wallet.Extensions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using CardanoSharp.Wallet.Extensions.Models;

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
            var script = new ScriptAll()
            {
                NativeScripts = new List<NativeScript>()
                {
                    new NativeScript()
                    {
                        ScriptPubKey = new ScriptPubKey()
                        {
                            KeyHash = "0a2c76fca4bce93da9ea7c11095e17244b3772e0077c47a12640c89f".HexToByteArray()
                        }
                    }
                }
            };

            var actual = script.GetPolicyId().ToStringHex();
            var expected = "f7932212f88d81cbea1f5e50b02180b0f5fc8a91ad6e0afe21308442";

            Assert.Equal(expected, actual);
        }
    }
}
