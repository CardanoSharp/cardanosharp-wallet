using CardanoSharp.Wallet.Models.Transactions.TransactionWitness.Scripts;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.TransactionBuilding
{
    public class ScriptInvalidBeforeBuilder: ABuilder<ScriptInvalidBefore>
    {
        public ScriptInvalidBeforeBuilder()
        {
            _model = new ScriptInvalidBefore();
        }

        public ScriptInvalidBeforeBuilder WithBefore(uint before)
        {
            _model.Before = before;
            return this;
        }
    }
}
