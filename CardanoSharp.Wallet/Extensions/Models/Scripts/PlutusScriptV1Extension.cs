using CardanoSharp.Wallet.Common;
using CardanoSharp.Wallet.Models.Transactions.TransactionWitness;
using CardanoSharp.Wallet.Models.Transactions.TransactionWitness.PlutusScripts;
using CardanoSharp.Wallet.Utilities;
using PeterO.Cbor2;

namespace CardanoSharp.Wallet.Extensions.Models
{
    public static class PlutusScriptV1Extension
    {
        // IS THIS CORRECT??
        //https://cardano.stackexchange.com/questions/4573/how-to-generate-the-address-of-a-plutus-script-using-cardano-serialization-lib/8820#8820
        public static byte[] GetPolicyId(this PlutusV1Script plutusScriptV1)
        {
            BigEndianBuffer buffer = new BigEndianBuffer();
            buffer.Write(new byte[] { 0x01 });
            buffer.Write(plutusScriptV1.bytes);
            return HashUtility.Blake2b224(buffer.ToArray());
        }

        public static byte[] Serialize(this PlutusV1Script plutusScriptV1)
        {
            return plutusScriptV1.bytes;
        }

        public static PlutusV1Script Deserialize(this byte[] bytes)
        {
            return new PlutusV1Script { bytes = bytes };
        }
    }
}
