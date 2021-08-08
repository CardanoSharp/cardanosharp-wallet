using CardanoSharp.Wallet.Models.Transactions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.TransactionBuilding
{
    public class TransactionWitnessSetBuilder: ABuilder<TransactionWitnessSet>
    {
        public TransactionWitnessSetBuilder()
        {
            _model = new TransactionWitnessSet();
        }

        public TransactionWitnessSetBuilder WithVKeyWitnesses(ICollection<VKeyWitness> vKeyWitnesses)
        {
            _model.VKeyWitnesses = vKeyWitnesses;
            return this;
        }

        public TransactionWitnessSetBuilder WithNativeScripts(ICollection<NativeScript> nativeScripts)
        {
            _model.NativeScripts = nativeScripts;
            return this;
        }

        public TransactionWitnessSetBuilder WithBootStrapWitnesses(ICollection<BootStrapWitness> bootStrapWitnesses)
        {
            _model.BootStrapWitnesses = bootStrapWitnesses;
            return this;
        }
    }
}
