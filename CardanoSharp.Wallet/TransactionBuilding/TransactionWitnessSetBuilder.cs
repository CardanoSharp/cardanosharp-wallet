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
        ITransactionWitnessSetBuilder SetNativeScript(IScriptAllBuilder scriptAllBuilder);
    }

    public class TransactionWitnessSetBuilder: ABuilder<TransactionWitnessSet>, ITransactionWitnessSetBuilder
    {
        public TransactionWitnessSetBuilder()
        {
            _model = new TransactionWitnessSet();
        }

        private TransactionWitnessSetBuilder(TransactionWitnessSet model)
        {
            _model = model;
        }

        public static ITransactionWitnessSetBuilder GetBuilder(TransactionWitnessSet model)
        {
            if (model == null)
            {
                return new TransactionWitnessSetBuilder();
            }
            return new TransactionWitnessSetBuilder(model);
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

        public ITransactionWitnessSetBuilder SetNativeScript(IScriptAllBuilder scriptAllBuilder)
        {
            _model.NativeScripts = new List<NativeScript>() { 
                new NativeScript() {
                    ScriptAll = scriptAllBuilder.Build()
                }
            };
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
