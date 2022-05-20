using CardanoSharp.Wallet.Models.Keys;
using CardanoSharp.Wallet.Models.Transactions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.TransactionBuilding
{
    public interface IVKeyWitnessBuilder: IABuilder<VKeyWitness>
    {
        IVKeyWitnessBuilder WithVKey(PublicKey vKey);

        IVKeyWitnessBuilder WithSKey(PrivateKey sKey);

        IVKeyWitnessBuilder WithSignature(byte[] signature);
    }

    public class VKeyWitnessBuilder: ABuilder<VKeyWitness>, IVKeyWitnessBuilder
    {
        public VKeyWitnessBuilder()
        {
            _model = new VKeyWitness();
        }

        private VKeyWitnessBuilder(VKeyWitness model)
        {
            _model = model;
        }

        public static IVKeyWitnessBuilder GetBuilder(VKeyWitness model)
        {
            return new VKeyWitnessBuilder(model);
        }

        public IVKeyWitnessBuilder WithVKey(PublicKey vKey)
        {
            _model.VKey = vKey;
            return this;
        }

        public IVKeyWitnessBuilder WithSKey(PrivateKey sKey)
        {
            _model.SKey = sKey;
            return this;
        }

        public IVKeyWitnessBuilder WithSignature(byte[] signature)
        {
            _model.Signature = signature;
            return this;
        }
    }
}
