using CardanoSharp.Wallet.Models.Transactions;
using CardanoSharp.Wallet.Models.Transactions.Scripts;
using System.Collections.Generic;

namespace CardanoSharp.Wallet.TransactionBuilding
{
    public interface IScriptAllBuilder: IABuilder<ScriptAll>
    {
        IScriptAllBuilder SetScript(INativeScriptBuilder nativeScriptBuilder);
        IScriptAllBuilder WithNativeScripts(List<NativeScript> nativeScripts);
    }

    public class ScriptAllBuilder: ABuilder<ScriptAll>, IScriptAllBuilder
    {
        public ScriptAllBuilder()
        {
            _model = new ScriptAll();
        }

        private ScriptAllBuilder(ScriptAll model)
        {
            _model = model;
        }

        public static IScriptAllBuilder GetBuilder(ScriptAll model)
        {
            if (model == null)
            {
                return new ScriptAllBuilder();
            }
            return new ScriptAllBuilder(model);
        }

        public static IScriptAllBuilder Create
        {
            get => new ScriptAllBuilder();
        }

        public IScriptAllBuilder SetScript(INativeScriptBuilder nativeScriptBuilder)
        {
            _model.NativeScripts.Add(nativeScriptBuilder.Build());
            return this;
        }

        public IScriptAllBuilder WithNativeScripts(List<NativeScript> nativeScripts)
        {
            _model.NativeScripts = nativeScripts;
            return this;
        }
    }
}
