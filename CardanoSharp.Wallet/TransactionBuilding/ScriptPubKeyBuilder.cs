using CardanoSharp.Wallet.Models.Transactions.Scripts;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.TransactionBuilding
{
    public class ScriptPubKeyBuilder: ABuilder<ScriptPubKey>
    {
        public ScriptPubKeyBuilder()
        {
            _model = new ScriptPubKey();
        }

        public ScriptPubKeyBuilder WithKeyHash(byte[] keyHash)
        {
            _model.KeyHash = keyHash;
            return this;
        }
    }
}
