using CardanoSharp.Wallet.Models.Transactions;
using CardanoSharp.Wallet.Models.Transactions.Scripts;
using System.Collections.Generic;

namespace CardanoSharp.Wallet.TransactionBuilding
{
    public interface IScriptAnyBuilder: IABuilder<ScriptAny>
    {
        IScriptAnyBuilder SetScript(INativeScriptBuilder nativeScriptBuilder);
        IScriptAnyBuilder WithNativeScripts(List<NativeScript> nativeScripts);
    }

    public class ScriptAnyBuilder: ABuilder<ScriptAny>, IScriptAnyBuilder
    {
        public ScriptAnyBuilder()
        {
            _model = new ScriptAny();
        }

        private ScriptAnyBuilder(ScriptAny model)
        {
            _model = model;
        }

        public static IScriptAnyBuilder GetBuilder(ScriptAny model)
        {
            if (model == null)
            {
                return new ScriptAnyBuilder();
            }
            return new ScriptAnyBuilder(model);
        }

        public static IScriptAnyBuilder Create
        {
            get => new ScriptAnyBuilder();
        }

        public IScriptAnyBuilder SetScript(INativeScriptBuilder nativeScriptBuilder)
        {
            _model.NativeScripts.Add(nativeScriptBuilder.Build());
            return this;
        }

        public IScriptAnyBuilder WithNativeScripts(List<NativeScript> nativeScripts)
        {
            _model.NativeScripts = nativeScripts;
            return this;
        }
    }
}
