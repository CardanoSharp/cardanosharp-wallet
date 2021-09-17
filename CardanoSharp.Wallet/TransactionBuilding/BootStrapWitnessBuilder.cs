using CardanoSharp.Wallet.Models.Transactions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.TransactionBuilding
{
    public class BootStrapWitnessBuilder: ABuilder<BootStrapWitness>
    {
        public BootStrapWitnessBuilder()
        {
            _model = new BootStrapWitness();
        }

        public BootStrapWitnessBuilder WithSignature(byte[] signature)
        {
            _model.Signature = signature;
            return this;
        }

        public BootStrapWitnessBuilder WithChainNode(byte[] chainNode)
        {
            _model.ChainNode = chainNode;
            return this;
        }

        public BootStrapWitnessBuilder WithAttributes(byte[] attributes)
        {
            _model.Attributes = attributes;
            return this;
        }
    }
}
