using CardanoSharp.Wallet.Models.Transactions.TransactionWitness.Scripts;

namespace CardanoSharp.Wallet.TransactionBuilding
{
    public interface IScriptInvalidBeforeBuilder: IABuilder<ScriptInvalidBefore>
    {
        IScriptInvalidBeforeBuilder WithBefore(uint before);
    }

    public class ScriptInvalidBeforeBuilder: ABuilder<ScriptInvalidBefore>, IScriptInvalidBeforeBuilder
    {
        public ScriptInvalidBeforeBuilder()
        {
            _model = new ScriptInvalidBefore();
        }

        private ScriptInvalidBeforeBuilder(ScriptInvalidBefore model)
        {
            _model = model;
        }

        public static IScriptInvalidBeforeBuilder GetBuilder(ScriptInvalidBefore model)
        {
            if (model == null)
            {
                return new ScriptInvalidBeforeBuilder();
            }
            return new ScriptInvalidBeforeBuilder(model);
        }

        public IScriptInvalidBeforeBuilder WithBefore(uint before)
        {
            _model.Before = before;
            return this;
        }
    }
}
