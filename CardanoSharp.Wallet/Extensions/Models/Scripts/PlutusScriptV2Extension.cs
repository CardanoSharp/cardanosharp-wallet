using CardanoSharp.Wallet.Common;
using CardanoSharp.Wallet.Models.Transactions.TransactionWitness;
using CardanoSharp.Wallet.Models.Transactions.TransactionWitness.PlutusScripts;
using CardanoSharp.Wallet.Utilities;
using PeterO.Cbor2;

namespace CardanoSharp.Wallet.Extensions.Models
{
    public static class PlutusScriptV2Extension
    {

        // IS THIS CORRECT??
        //https://cardano.stackexchange.com/questions/4573/how-to-generate-the-address-of-a-plutus-script-using-cardano-serialization-lib/8820#8820
        public static byte[] GetPolicyId(this PlutusScriptV2 plutusScriptV2)
        {
            BigEndianBuffer buffer = new BigEndianBuffer();
            buffer.Write(new byte[] { 0x02 });
            buffer.Write(plutusScriptV2.bytes);
            return HashUtility.Blake2b224(buffer.ToArray());
        }

        public static byte[] Serialize(this PlutusScriptV2 plutusScriptV2)
        {
            return plutusScriptV2.bytes;
        }

        public static PlutusScriptV2 Deserialize(this byte[] bytes)
        {
            return new PlutusScriptV2 { bytes = bytes };
        }
    }
}
