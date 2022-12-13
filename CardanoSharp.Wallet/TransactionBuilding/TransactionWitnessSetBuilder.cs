﻿using System;
using System.Collections.Generic;
using System.Linq;
using CardanoSharp.Wallet.Models.Keys;
using CardanoSharp.Wallet.Models.Transactions;
using CardanoSharp.Wallet.Models.Transactions.TransactionWitness;
using CardanoSharp.Wallet.Models.Transactions.TransactionWitness.PlutusScripts;
using CardanoSharp.Wallet.Extensions.Models.Transactions.TransactionWitnesses;

namespace CardanoSharp.Wallet.TransactionBuilding
{
    public interface ITransactionWitnessSetBuilder : IABuilder<TransactionWitnessSet>
    {
        ITransactionWitnessSetBuilder AddVKeyWitness(PublicKey vKey, PrivateKey sKey);
        ITransactionWitnessSetBuilder MockVKeyWitness(int count = 1);
        ITransactionWitnessSetBuilder ClearMocks();
        ITransactionWitnessSetBuilder AddNativeScript(INativeScriptBuilder nativeScriptBuilder);

        [Obsolete("Will be deprecated. Please use SetScriptAllNativeScript() instead")]
        ITransactionWitnessSetBuilder SetNativeScript(IScriptAllBuilder scriptAllBuilder);
        ITransactionWitnessSetBuilder SetScriptAllNativeScript(IScriptAllBuilder scriptAllBuilder);
        ITransactionWitnessSetBuilder SetScriptAnyNativeScript(IScriptAnyBuilder scriptAnyBuilder);
        ITransactionWitnessSetBuilder SetScriptNofKNativeScript(
            IScriptNofKBuilder scriptNofKBuilder
        );
        ITransactionWitnessSetBuilder AddPlutusV1Script(
            PlutusV1ScriptBuilder plutusV1ScriptBuilder
        );
        ITransactionWitnessSetBuilder AddPlutusData(IPlutusData plutusData);
        ITransactionWitnessSetBuilder AddRedeemer(RedeemerBuilder redeemerBuilder);
        ITransactionWitnessSetBuilder AddRedeemer(Redeemer redeemer);
        ITransactionWitnessSetBuilder AddPlutusV2Script(
            PlutusV2ScriptBuilder plutusV2ScriptBuilder
        );
    }

    public class TransactionWitnessSetBuilder
        : ABuilder<TransactionWitnessSet>,
            ITransactionWitnessSetBuilder
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
            _model.VKeyWitnesses.Add(
                new VKeyWitness()
                {
                    VKey = vKey,
                    SKey = sKey,
                    IsMock = false
                }
            );
            return this;
        }

        public ITransactionWitnessSetBuilder MockVKeyWitness(int count = 1)
        {
            _model.VKeyWitnesses.CreateMocks(count);
            return this;
        }

        public ITransactionWitnessSetBuilder ClearMocks()
        {
            // Hold non-Mocked VKeys
            var realVkeys = _model.VKeyWitnesses.Where(x => !x.IsMock);

            // Reset VKey list
            _model.VKeyWitnesses = new HashSet<VKeyWitness>();

            // Add non-Mocked VKeys
            foreach (var vkey in realVkeys)
            {
                _model.VKeyWitnesses.Add(vkey);
            }

            return this;
        }

        public ITransactionWitnessSetBuilder AddNativeScript(
            INativeScriptBuilder nativeScriptBuilder
        )
        {
            _model.NativeScripts.Add(nativeScriptBuilder.Build());
            return this;
        }

        [Obsolete("Will be deprecated. Please use SetScriptAllNativeScript() instead")]
        public ITransactionWitnessSetBuilder SetNativeScript(IScriptAllBuilder scriptAllBuilder)
        {
            _model.NativeScripts = new List<NativeScript>()
            {
                new NativeScript() { ScriptAll = scriptAllBuilder.Build() }
            };
            return this;
        }

        public ITransactionWitnessSetBuilder SetScriptAllNativeScript(
            IScriptAllBuilder scriptAllBuilder
        )
        {
            _model.NativeScripts = new List<NativeScript>()
            {
                new NativeScript() { ScriptAll = scriptAllBuilder.Build() }
            };
            return this;
        }

        public ITransactionWitnessSetBuilder SetScriptAnyNativeScript(
            IScriptAnyBuilder scriptAnyBuilder
        )
        {
            _model.NativeScripts = new List<NativeScript>()
            {
                new NativeScript() { ScriptAny = scriptAnyBuilder.Build() }
            };
            return this;
        }

        public ITransactionWitnessSetBuilder SetScriptNofKNativeScript(
            IScriptNofKBuilder scriptNofKBuilder
        )
        {
            _model.NativeScripts = new List<NativeScript>()
            {
                new NativeScript() { ScriptNofK = scriptNofKBuilder.Build() }
            };
            return this;
        }

        public ITransactionWitnessSetBuilder AddPlutusV1Script(
            PlutusV1ScriptBuilder plutusV1ScriptBuilder
        )
        {
            _model.PlutusV1Scripts.Add(plutusV1ScriptBuilder.Build());
            return this;
        }

        public ITransactionWitnessSetBuilder AddPlutusData(IPlutusData plutusData)
        {
            _model.PlutusDatas.Add(plutusData);
            return this;
        }

        public ITransactionWitnessSetBuilder AddRedeemer(RedeemerBuilder redeemerBuilder)
        {
            _model.Redeemers.Add(redeemerBuilder.Build());
            return this;
        }

        public ITransactionWitnessSetBuilder AddRedeemer(Redeemer redeemer)
        {
            _model.Redeemers.Add(redeemer);
            return this;
        }

        public ITransactionWitnessSetBuilder AddPlutusV2Script(
            PlutusV2ScriptBuilder plutusV2ScriptBuilder
        )
        {
            _model.PlutusV2Scripts.Add(plutusV2ScriptBuilder.Build());
            return this;
        }
    }
}
