using CardanoSharp.Wallet.Models.Transactions;
using CardanoSharp.Wallet.Models.Transactions.Scripts;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.TransactionBuilding
{
    public interface IScriptNofKBuilder: IABuilder<ScriptNofK>
    {
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
            return new ScriptNofKBuilder(model);
        }

        public IScriptNofKBuilder WithNativeScripts(List<NativeScript> nativeScripts)
        {
            _model.NativeScripts = nativeScripts;
            return this;
        }
    }
}
