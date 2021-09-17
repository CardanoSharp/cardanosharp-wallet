using CardanoSharp.Wallet.Models.Keys;
using CardanoSharp.Wallet.Models.Transactions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.TransactionBuilding
{
    public class VKeyWitnessBuilder: ABuilder<VKeyWitness>
    {
        public VKeyWitnessBuilder()
        {
            _model = new VKeyWitness();
        }

        public VKeyWitnessBuilder WithVKey(PublicKey vKey)
        {
            _model.VKey = vKey;
            return this;
        }

        public VKeyWitnessBuilder WithSKey(PrivateKey sKey)
        {
            _model.SKey = sKey;
            return this;
        }

        public VKeyWitnessBuilder WithSignature(byte[] signature)
        {
            _model.Signature = signature;
            return this;
        }
    }
}
