using CardanoSharp.Wallet.Models.Transactions;
using CardanoSharp.Wallet.Models.Transactions.Scripts;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.TransactionBuilding
{
    public class ScriptNofKBuilder : ABuilder<ScriptNofK>
    {
        public ScriptNofKBuilder()
        {
            _model = new ScriptNofK();
        }

        public ScriptNofKBuilder WithNativeScripts(List<NativeScript> nativeScripts)
        {
            _model.NativeScripts = nativeScripts;
            return this;
        }
    }
}
