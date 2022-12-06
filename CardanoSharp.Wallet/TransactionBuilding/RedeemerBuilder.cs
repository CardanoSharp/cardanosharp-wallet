using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Models.Transactions.TransactionWitness.PlutusScripts;

namespace CardanoSharp.Wallet.TransactionBuilding
{
    public interface IRedeemerBuilder
    {
        IRedeemerBuilder SetTag(RedeemerTag redeemerTag);
        IRedeemerBuilder SetIndex(uint index);
        IRedeemerBuilder SetPlutusData(IPlutusData plutusData);
        IRedeemerBuilder SetExUnits(ExUnits exUnits);
    }

    public class RedeemerBuilder : ABuilder<Redeemer>, IRedeemerBuilder
    {
        private RedeemerBuilder()
        {
            _model = new Redeemer();
        }

        private RedeemerBuilder(Redeemer model)
        {
            _model = model;
        }

        public static RedeemerBuilder Create
        {
            get => new RedeemerBuilder();
        }

        public IRedeemerBuilder SetTag(RedeemerTag redeemerTag) 
        {
            _model.Tag = redeemerTag;
            return this;
        }

        public IRedeemerBuilder SetIndex(uint index) 
        {
            _model.Index = index;
            return this;
        }

        public IRedeemerBuilder SetPlutusData(IPlutusData plutusData)
        {
            _model.PlutusData = plutusData;
            return this;
        }

        public IRedeemerBuilder SetExUnits(ExUnits exUnits)
        {
            _model.ExUnits = exUnits;
            return this;
        }
    }
}
