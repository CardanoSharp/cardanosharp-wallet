using CardanoSharp.Wallet.Models.Keys;
using CardanoSharp.Wallet.Models.Transactions;
using System;
using System.Collections.Generic;

namespace CardanoSharp.Wallet.TransactionBuilding
{
    public interface ITransactionWitnessSetBuilder: IABuilder<TransactionWitnessSet>
    {
        ITransactionWitnessSetBuilder AddVKeyWitness(PublicKey vKey, PrivateKey sKey);
        ITransactionWitnessSetBuilder MockVKeyWitness(int count = 1);
        ITransactionWitnessSetBuilder AddNativeScript(INativeScriptBuilder nativeScriptBuilder);
        [Obsolete("Will be deprecated. Please use SetScriptAllNativeScript() instead")]
        ITransactionWitnessSetBuilder SetNativeScript(IScriptAllBuilder scriptAllBuilder);
        ITransactionWitnessSetBuilder SetScriptAllNativeScript(IScriptAllBuilder scriptAllBuilder);
        ITransactionWitnessSetBuilder SetScriptAnyNativeScript(IScriptAnyBuilder scriptAnyBuilder);
        ITransactionWitnessSetBuilder SetScriptNofKNativeScript(IScriptNofKBuilder scriptNofKBuilder);
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

        public ITransactionWitnessSetBuilder AddVKeyWitness(PublicKey vKey, PrivateKey sKey)
        {
            _model.VKeyWitnesses.Add(new VKeyWitness()
            {
                VKey = vKey,
                SKey = sKey,
                IsMock = false
            });
            return this;
        }

        public ITransactionWitnessSetBuilder MockVKeyWitness(int count = 1)
        {
            for (var x = 0; x < count; x++)
            {
                _model.VKeyWitnesses.Add(new VKeyWitness()
                {
                   VKey = new PublicKey(getMockKeyId(32), null),
                   Signature = getMockKeyId(64),
                   IsMock = true
                });
            }

            return this;
        }

        public ITransactionWitnessSetBuilder AddNativeScript(INativeScriptBuilder nativeScriptBuilder)
        {
            _model.NativeScripts.Add(nativeScriptBuilder.Build());
            return this;
        }

        [Obsolete("Will be deprecated. Please use SetScriptAllNativeScript() instead")]
        public ITransactionWitnessSetBuilder SetNativeScript(IScriptAllBuilder scriptAllBuilder)
        {
            _model.NativeScripts = new List<NativeScript>() { 
                new NativeScript() {
                    ScriptAll = scriptAllBuilder.Build()
                }
            };
            return this;
        }

        public ITransactionWitnessSetBuilder SetScriptAllNativeScript(IScriptAllBuilder scriptAllBuilder)
        {
            _model.NativeScripts = new List<NativeScript>() {
                new NativeScript() {
                    ScriptAll = scriptAllBuilder.Build()
                }
            };
            return this;
        }

        public ITransactionWitnessSetBuilder SetScriptAnyNativeScript(IScriptAnyBuilder scriptAnyBuilder)
        {
            _model.NativeScripts = new List<NativeScript>() {
                new NativeScript() {
                    ScriptAny = scriptAnyBuilder.Build()
                }
            };
            return this;
        }

        public ITransactionWitnessSetBuilder SetScriptNofKNativeScript(IScriptNofKBuilder scriptNofKBuilder)
        {
            _model.NativeScripts = new List<NativeScript>() {
                new NativeScript() {
                    ScriptNofK = scriptNofKBuilder.Build()
                }
            };
            return this;
        }
        
        private byte[] getMockKeyId(int length)
        {
            var hash = new byte[length];
            for (var i = 0; i < hash.Length; i++)
            {
                hash[i] = 0x00;
            }
            return hash;
        }
    }
}
