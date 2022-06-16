using CardanoSharp.Wallet.Models.Transactions;

namespace CardanoSharp.Wallet.TransactionBuilding
{
    public interface IBootStrapWitnessBuilder: IABuilder<BootStrapWitness>
    {
        IBootStrapWitnessBuilder WithSignature(byte[] signature);

        IBootStrapWitnessBuilder WithChainNode(byte[] chainNode);

        IBootStrapWitnessBuilder WithAttributes(byte[] attributes);
    }

    public class BootStrapWitnessBuilder: ABuilder<BootStrapWitness>, IBootStrapWitnessBuilder
    {
        public BootStrapWitnessBuilder()
        {
            _model = new BootStrapWitness();
        }

        private BootStrapWitnessBuilder(BootStrapWitness model)
        {
            _model = model;
        }

        public static IBootStrapWitnessBuilder GetBuilder(BootStrapWitness model)
        {
            if (model == null)
            {
                return new BootStrapWitnessBuilder();
            }
            return new BootStrapWitnessBuilder(model);
        }

        public IBootStrapWitnessBuilder WithSignature(byte[] signature)
        {
            _model.Signature = signature;
            return this;
        }

        public IBootStrapWitnessBuilder WithChainNode(byte[] chainNode)
        {
            _model.ChainNode = chainNode;
            return this;
        }

        public IBootStrapWitnessBuilder WithAttributes(byte[] attributes)
        {
            _model.Attributes = attributes;
            return this;
        }
    }
}
