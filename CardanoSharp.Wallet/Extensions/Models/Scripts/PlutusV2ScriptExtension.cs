using System;
using CardanoSharp.Wallet.Common;
using CardanoSharp.Wallet.Models.Transactions.TransactionWitness;
using CardanoSharp.Wallet.Models.Transactions.TransactionWitness.PlutusScripts;
using CardanoSharp.Wallet.Utilities;
using PeterO.Cbor2;

namespace CardanoSharp.Wallet.Extensions.Models
{
    public static class PlutusV2ScriptExtension
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

        public static CBORObject GetCBOR(this PlutusV2Script plutusV2Script)
        {
            // plutus_v2_script = bytes
            var plutusV2ScriptCbor = CBORObject.DecodeFromBytes(plutusV2Script.script);
            return plutusV2ScriptCbor;
        }

        public static PlutusV2Script GetPlutusV2Script(this CBORObject plutusV2ScriptCbor)
        {
            if (plutusV2ScriptCbor == null)
            {
                throw new ArgumentNullException(nameof(plutusV2ScriptCbor));
            }

            if (plutusV2ScriptCbor.Type !=  CBORType.ByteString)
            {
                throw new ArgumentException("plutusV2ScriptCbor is not expected type CBORType.ByteString");
            }
            
            var plutusV2Script = new PlutusV2Script();
            plutusV2Script.script = ((string)plutusV2ScriptCbor.DecodeValueByCborType()).HexToByteArray();
            return plutusV2Script;          
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
