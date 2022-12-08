using PeterO.Cbor2;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Models.Transactions.TransactionWitness.PlutusScripts;
using CardanoSharp.Wallet.Extensions.Models;

namespace CardanoSharp.Wallet.TransactionBuilding
{
    public interface IPlutusScriptBuilder
    {
        IPlutusScriptBuilder SetScript(byte[] script);
        IPlutusScriptBuilder SetScript(string scriptCBOR);
    }

    public class PlutusV1ScriptBuilder : ABuilder<PlutusV1Script>, IPlutusScriptBuilder
    {
        private PlutusV1ScriptBuilder()
        {
            _model = new PlutusV1Script();
        }

        private PlutusV1ScriptBuilder(PlutusV1Script model)
        {
            _model = model;
        }

        public static PlutusV1ScriptBuilder Create
        {
            get => new PlutusV1ScriptBuilder();
        }

        public IPlutusScriptBuilder SetScript(byte[] script) 
        {
            _model.script = script;
            return this;
        }

        public IPlutusScriptBuilder SetScript(string scriptCBOR)
        {
            _model.script = ((string)CBORObject.DecodeFromBytes(scriptCBOR.HexToByteArray()).DecodeValueByCborType()).HexToByteArray();
            return this;
        }
    }

    public class PlutusV2ScriptBuilder : ABuilder<PlutusV2Script>, IPlutusScriptBuilder
    {
         private PlutusV2ScriptBuilder()
        {
            _model = new PlutusV2Script();
        }

        private PlutusV2ScriptBuilder(PlutusV2Script model)
        {
            _model = model;
        }

        public static PlutusV2ScriptBuilder Create
        {
            get => new PlutusV2ScriptBuilder();
        }

        public IPlutusScriptBuilder SetScript(byte[] script) 
        {
            // This must be the decoded CBOR byte array
            _model.script = script;
            return this;
        }

        public IPlutusScriptBuilder SetScript(string scriptCBOR)
        {
            _model.script = ((string)CBORObject.DecodeFromBytes(scriptCBOR.HexToByteArray()).DecodeValueByCborType()).HexToByteArray();
            return this;
        }
    }
}
