using CardanoSharp.Wallet.Models.Transactions.TransactionWitness.PlutusScripts;
using PeterO.Cbor2;

namespace CardanoSharp.Wallet.Extensions.Models.Transactions
{
    public static partial class ScriptReferenceExtensions
    {
        public static CBORObject Serialize(this ScriptReference scriptReference)
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
    }
}