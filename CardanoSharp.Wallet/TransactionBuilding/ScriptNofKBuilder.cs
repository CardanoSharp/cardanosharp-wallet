using CardanoSharp.Wallet.Models.Transactions;
using CardanoSharp.Wallet.Models.Transactions.Scripts;
using System.Collections.Generic;

namespace CardanoSharp.Wallet.TransactionBuilding
{
    public interface IScriptNofKBuilder: IABuilder<ScriptNofK>
    {
        IScriptNofKBuilder SetN(uint n);
        IScriptNofKBuilder SetScript(INativeScriptBuilder nativeScriptBuilder);
        IScriptNofKBuilder WithNativeScripts(List<NativeScript> nativeScripts);
    }

    public class ScriptNofKBuilder : ABuilder<ScriptNofK>, IScriptNofKBuilder
    {
        public ScriptNofKBuilder()
        {
            _model = new ScriptNofK();
        }

        private ScriptNofKBuilder(ScriptNofK model)
        {
            _model = model;
        }

        public static IScriptNofKBuilder GetBuilder(ScriptNofK model)
        {
            if (model == null)
            {
                return new ScriptNofKBuilder();
            }
            return new ScriptNofKBuilder(model);
        }

        public static IScriptNofKBuilder Create
        {
            get => new ScriptNofKBuilder();
        }

        public IScriptNofKBuilder SetN(uint n)
        {
            _model.N = n;
            return this;
        }

        public IScriptNofKBuilder SetScript(INativeScriptBuilder nativeScriptBuilder)
        {
            _model.NativeScripts.Add(nativeScriptBuilder.Build());
            return this;
        }

        public IScriptNofKBuilder WithNativeScripts(List<NativeScript> nativeScripts)
        {
            _model.NativeScripts = nativeScripts;
            return this;
        }
    }
}
