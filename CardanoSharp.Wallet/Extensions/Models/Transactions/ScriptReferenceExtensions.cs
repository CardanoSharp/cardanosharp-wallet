using System;
using CardanoSharp.Wallet.Models.Transactions.TransactionWitness;
using CardanoSharp.Wallet.Models.Transactions.TransactionWitness.NativeScripts;
using CardanoSharp.Wallet.Models.Transactions.TransactionWitness.PlutusScripts;
using PeterO.Cbor2;

namespace CardanoSharp.Wallet.Extensions.Models.Transactions
{
    public static partial class ScriptReferenceExtensions
    {
        public static CBORObject GetCBOR(this ScriptReference scriptReference)
        {
            CBORObject cborScriptReference = CBORObject.NewArray();

            if (scriptReference.NativeScript is not null)
            {
                cborScriptReference.Add(0);
                cborScriptReference.Add(scriptReference.NativeScript.GetCBOR2());
            }

            if (scriptReference.PlutusV1Script is not null)
            {
                cborScriptReference.Add(1);
                cborScriptReference.Add(scriptReference.PlutusV1Script.script);
            }

            if (scriptReference.PlutusV2Script is not null)
            {
                cborScriptReference.Add(2);
                cborScriptReference.Add(scriptReference.PlutusV2Script.script);
            }
            
            return CBORObject.FromObject(cborScriptReference.EncodeToBytes()).WithTag(24);
        }

        public static ScriptReference GetScriptReference(this CBORObject scriptReferenceCborWithTag)
        {
            if (scriptReferenceCborWithTag == null)
            {
                throw new ArgumentNullException(nameof(scriptReferenceCborWithTag));
            }

            if (scriptReferenceCborWithTag.Type != CBORType.Array)
            {
                throw new ArgumentException("scriptReferenceCbor is not expected type CBORType.Array");
            }

            if (scriptReferenceCborWithTag.Count != 2)
            {
                throw new ArgumentException("scriptReferenceCbor has unexpected number of elements (expected 2)");
            }

            if (!scriptReferenceCborWithTag.HasMostOuterTag(24))
            {
                throw new ArgumentException("scriptReferenceCbor expected tag 24");
            }
            
            ScriptReference scriptReference = new ScriptReference();
            var scriptReferenceCbor = scriptReferenceCborWithTag.Untag();

            // Must decode the object after removing the tag
            var decodedScriptReferenceCbor = CBORObject.DecodeFromBytes(
                ((string)scriptReferenceCbor.DecodeValueByCborType()).HexToByteArray()
            );
            var scriptKey = decodedScriptReferenceCbor[0].DecodeValueToUInt32();
            if (scriptKey == 0)
            {
                // Is this correct for native script?
                NativeScript nativeScript = (NativeScript)
                    decodedScriptReferenceCbor[1].DecodeValueByCborType();
                scriptReference.NativeScript = nativeScript;
            }
            else if (scriptKey == 1)
            {
                byte[] plutusV1ScriptBytes = (
                    (string)scriptReferenceCbor[1].DecodeValueByCborType()
                ).HexToByteArray();
                PlutusV1Script plutusV1Script = new PlutusV1Script
                {
                    script = plutusV1ScriptBytes
                };
                scriptReference.PlutusV1Script = plutusV1Script;
            }
            else if (scriptKey == 2)
            {
                byte[] plutusV2ScriptBytes = (
                    (string)decodedScriptReferenceCbor[1].DecodeValueByCborType()
                ).HexToByteArray();
                PlutusV2Script plutusV2Script = new PlutusV2Script
                {
                    script = plutusV2ScriptBytes
                };
                scriptReference.PlutusV2Script = plutusV2Script;
            }

            return scriptReference;            
        }
    }
}