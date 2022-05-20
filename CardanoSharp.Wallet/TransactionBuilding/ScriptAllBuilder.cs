using CardanoSharp.Wallet.Models.Transactions;
using CardanoSharp.Wallet.Models.Transactions.Scripts;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.TransactionBuilding
{
    public interface IScriptAllBuilder: IABuilder<ScriptAll>
    {
        IScriptAllBuilder SetScript(INativeScriptBuilder nativeScriptBuilder);
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
    }
}
