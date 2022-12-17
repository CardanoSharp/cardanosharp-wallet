using System;
using System.Collections.Generic;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Models;
using CardanoSharp.Wallet.Models.Transactions;
using CardanoSharp.Wallet.Models.Transactions.TransactionWitness.PlutusScripts;
using CardanoSharp.Wallet.TransactionBuilding;

namespace CardanoSharp.Wallet.TransactionBuilding
{
    public interface IRedeemerBuilder
    {
        IRedeemerBuilder SetTag(RedeemerTag redeemerTag);
        IRedeemerBuilder SetIndex(uint index);
        IRedeemerBuilder SetIndex(Transaction transaction, TransactionInput scriptTransactionInput);
        IRedeemerBuilder SetIndex(Transaction transaction, Utxo utxo);
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


        public IRedeemerBuilder SetIndex(Transaction transaction, TransactionInput scriptTransactionInput)
        {
            List<TransactionInput> transactionInputs = new List<TransactionInput>();
            transactionInputs.AddRange((List<TransactionInput>)transaction.TransactionBody.TransactionInputs);

            //https://github.com/bloxbean/cardano-client-lib/blob/7322b16030d8fa3ac5417d5dc58c92df401855ad/function/src/main/java/com/bloxbean/cardano/client/function/helper/RedeemerUtil.java
            //https://cardano.stackexchange.com/questions/7969/meaning-of-index-of-redeemer-in-serialization-lib-10-4
            // Sort transaction inputs to determine redeemer index
            transactionInputs.Sort((a, b) => $"{a.TransactionId.ToStringHex()}#{a.TransactionIndex}".CompareTo($"{b.TransactionId.ToStringHex()}#{b.TransactionIndex}"));

            uint index = (uint)transactionInputs.IndexOf(scriptTransactionInput);
            _model.Index = index;
            return this;
        }

        public IRedeemerBuilder SetIndex(Transaction transaction, Utxo utxo)
        {
            List<TransactionInput> transactionInputs = new List<TransactionInput>();
            transactionInputs.AddRange((List<TransactionInput>)transaction.TransactionBody.TransactionInputs);

            //https://github.com/bloxbean/cardano-client-lib/blob/7322b16030d8fa3ac5417d5dc58c92df401855ad/function/src/main/java/com/bloxbean/cardano/client/function/helper/RedeemerUtil.java
            //https://cardano.stackexchange.com/questions/7969/meaning-of-index-of-redeemer-in-serialization-lib-10-4
            // Sort transaction inputs to determine redeemer index
            transactionInputs.Sort((a, b) => $"{a.TransactionId.ToStringHex()}#{a.TransactionIndex}".CompareTo($"{b.TransactionId.ToStringHex()}#{b.TransactionIndex}"));

            uint index = (uint)transactionInputs.FindIndex(t => t.TransactionId.ToStringHex() == utxo.TxHash && t.TransactionIndex == utxo.TxIndex);
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
