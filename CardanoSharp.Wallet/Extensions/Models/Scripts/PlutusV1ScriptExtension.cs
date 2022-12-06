using System;
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
        public static byte[] GetPolicyId(this PlutusV1Script plutusV1Script)
        {
            BigEndianBuffer buffer = new BigEndianBuffer();
            buffer.Write(new byte[] { 0x01 });
            buffer.Write(plutusV1Script.script);
            return HashUtility.Blake2b224(buffer.ToArray());
        }

        public static CBORObject GetCBOR(this PlutusV1Script plutusV1Script)
        {
            // plutus_v1_script = bytes
            var plutusV1ScriptCbor = CBORObject.DecodeFromBytes(plutusV1Script.script);
            return plutusV1ScriptCbor;
        }

        public static PlutusV1Script GetPlutusV1Script(this CBORObject plutusV1ScriptCbor)
        {
            if (plutusV1ScriptCbor == null)
            {
                throw new ArgumentNullException(nameof(plutusV1ScriptCbor));
            }

            if (plutusV1ScriptCbor.Type !=  CBORType.ByteString)
            {
                throw new ArgumentException("plutusV1ScriptCbor is not expected type CBORType.ByteString");
            }
            
            var plutusV1Script = new PlutusV1Script();
            plutusV1Script.script = ((string)plutusV1ScriptCbor.DecodeValueByCborType()).HexToByteArray();
            return plutusV1Script;          
        }

        public static byte[] Serialize(this PlutusV1Script plutusScriptV1)
        {
            return plutusScriptV1.script;
        }

        public static PlutusV1Script Deserialize(this byte[] bytes)
        {
            return new PlutusV1Script { script = bytes };
        }
    }
}
