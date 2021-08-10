using CardanoSharp.Wallet.Models.Transactions.TransactionWitness.Scripts;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.TransactionBuilding
{
    public class ScriptInvalidAfterBuilder: ABuilder<ScriptInvalidAfter>
    {
        public ScriptInvalidAfterBuilder()
        {
            _model = new ScriptInvalidAfter();
        }

        public ScriptInvalidAfterBuilder WithAfter(uint after)
        {
            _model.After = after;
            return this;
        }
    }
}
