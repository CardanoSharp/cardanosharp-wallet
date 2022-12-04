using CardanoSharp.Wallet.Common;
using CardanoSharp.Wallet.Models.Transactions.TransactionWitness.PlutusScripts;
using CardanoSharp.Wallet.Utilities;

namespace CardanoSharp.Wallet.Extensions.Models
{
    public static class PlutusScriptV2Extension
    {

        // IS THIS CORRECT??
        //https://cardano.stackexchange.com/questions/4573/how-to-generate-the-address-of-a-plutus-script-using-cardano-serialization-lib/8820#8820
        public static byte[] GetPolicyId(this PlutusV2Script plutusV2Script)
        {
            BigEndianBuffer buffer = new BigEndianBuffer();
            buffer.Write(new byte[] { 0x02 });
            buffer.Write(plutusV2Script.script);
            return HashUtility.Blake2b224(buffer.ToArray());
        }

        public static byte[] Serialize(this PlutusV2Script plutusV2Script)
        {
            return plutusV2Script.script;
        }

        public static PlutusV2Script Deserialize(this byte[] bytes)
        {
            return new PlutusV2Script { script = bytes };
        }
    }
}
