using CardanoSharp.Wallet.Models.Transactions;
using CardanoSharp.Wallet.Models.Transactions.Scripts;
using CardanoSharp.Wallet.Models.Transactions.TransactionWitness.Scripts;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.TransactionBuilding
{
    public class NativeScriptBuilder: ABuilder<NativeScript>
    {
        public NativeScriptBuilder()
        {
            _model = new NativeScript();
        }

        public NativeScriptBuilder WithScriptPubKey(ScriptPubKey scriptPubKey)
        {
            _model.ScriptPubKey = scriptPubKey;
            return this;
        }

        public NativeScriptBuilder WithScriptAll(ScriptAll scriptAll)
        {
            _model.ScriptAll = scriptAll;
            return this;
        }

        public NativeScriptBuilder WithScriptAny(ScriptAny scriptAny)
        {
            _model.ScriptAny = scriptAny;
            return this;
        }

        public NativeScriptBuilder WithScriptNofK(ScriptNofK scriptNofK)
        {
            _model.ScriptNofK = scriptNofK;
            return this;
        }

        public NativeScriptBuilder WithScriptInvalidAfter(ScriptInvalidAfter invalidAfter)
        {
            _model.InvalidAfter = invalidAfter;
            return this;
        }

        public NativeScriptBuilder WithScriptInvalidBefore(ScriptInvalidBefore invalidBefore)
        {
            _model.InvalidBefore = invalidBefore;
            return this;
        }
    }
}
