using CardanoSharp.Wallet.Models.Transactions.Scripts;

namespace CardanoSharp.Wallet.TransactionBuilding
{
    public interface IScriptPubKeyBuilder: IABuilder<ScriptPubKey>
    {
        IScriptPubKeyBuilder WithKeyHash(byte[] keyHash);
    }

    public class ScriptPubKeyBuilder: ABuilder<ScriptPubKey>, IScriptPubKeyBuilder
    {
        public ScriptPubKeyBuilder()
        {
            _model = new ScriptPubKey();
        }

        private ScriptPubKeyBuilder(ScriptPubKey model)
        {
            _model = model;
        }

        public static IScriptPubKeyBuilder GetBuilder(ScriptPubKey model)
        {
            if (model == null)
            {
                return new ScriptPubKeyBuilder();
            }
            return new ScriptPubKeyBuilder(model);
        }

        public IScriptPubKeyBuilder WithKeyHash(byte[] keyHash)
        {
            _model.KeyHash = keyHash;
            return this;
        }
    }
}
