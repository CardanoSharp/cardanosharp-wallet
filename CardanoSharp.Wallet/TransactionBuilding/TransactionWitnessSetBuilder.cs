using CardanoSharp.Wallet.Models.Keys;
using CardanoSharp.Wallet.Models.Transactions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.TransactionBuilding
{
    public interface ITransactionWitnessSetBuilder: IABuilder<TransactionWitnessSet>
    {
        ITransactionWitnessSetBuilder AddVKeyWitness(PublicKey vKey, PrivateKey sKey);
        ITransactionWitnessSetBuilder AddNativeScript(INativeScriptBuilder nativeScriptBuilder);
    }

    public class TransactionWitnessSetBuilder: ABuilder<TransactionWitnessSet>, ITransactionWitnessSetBuilder
    {
        public TransactionWitnessSetBuilder()
        {
            _model = new TransactionWitnessSet();
        }

        public static ITransactionWitnessSetBuilder Create
        {
            get => new TransactionWitnessSetBuilder();
        }

        public ITransactionWitnessSetBuilder AddNativeScript(INativeScriptBuilder nativeScriptBuilder)
        {
            _model.NativeScripts.Add(nativeScriptBuilder.Build());
            return this;
        }

        public ITransactionWitnessSetBuilder AddVKeyWitness(PublicKey vKey, PrivateKey sKey)
        {
            _model.VKeyWitnesses.Add(new VKeyWitness()
            {
                VKey = vKey,
                SKey = sKey
            });
            return this;
        }
    }
}
