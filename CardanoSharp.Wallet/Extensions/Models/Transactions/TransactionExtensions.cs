using CardanoSharp.Wallet.Common;
using CardanoSharp.Wallet.Extensions.Models.Transactions.TransactionWitnesses;
using CardanoSharp.Wallet.Models.Transactions;
using PeterO.Cbor2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CardanoSharp.Wallet.Extensions.Models.Transactions
{
    public static class TransactionExtensions
    {
        public static CBORObject GetCBOR(this Transaction transaction)
        {
            //create Transaction CBOR Object
            var cborTransaction = CBORObject.NewArray();

            //if we have a transaction body, lets build Body CBOR and add to Transaction Array
            if (transaction.TransactionBody != null)
            {
                cborTransaction.Add(transaction.TransactionBody.GetCBOR(transaction.AuxiliaryData));
            }

            //if we have a transaction witness set, lets build Witness Set CBOR and add to Transaction Array
            if (transaction.TransactionWitnessSet != null)
            {
                cborTransaction.Add(transaction.TransactionWitnessSet.GetCBOR(
                    transaction.TransactionBody, transaction.AuxiliaryData));
            }
            else
            {
                cborTransaction.Add(CBORObject.NewArray());
            }

            //add metadata
            cborTransaction.Add(transaction.AuxiliaryData != null
                ? transaction.AuxiliaryData.GetCBOR()
                : null);

            //return serialized cbor
            return cborTransaction;
        }

        public static Transaction GetTransaction(this CBORObject transactionCbor)
        {
            //validation
            if (transactionCbor == null)
            {
                throw new NullReferenceException("Transaction CBOR is null");
            }
            if (transactionCbor.Type != CBORType.Array)
            {
                throw new InvalidOperationException("Transaction CBOR is not Array type");
            }
            if (transactionCbor.Count < 2)
            {
                throw new InvalidOperationException("Transaction does not contain at least 2 elements (body & witness set)");
            }

            //get data
            var transactionBodyCbor = transactionCbor.Values.First();
            var transactionWitnessSetCbor = transactionCbor.Values.Skip(1).First();
            CBORObject auxiliaryDataCbor = null;
            if (transactionCbor.Count > 2)
            {
                auxiliaryDataCbor = transactionCbor.Values.Skip(2).FirstOrDefault();
            }

            //populate
            var transaction = new Transaction();
            transaction.TransactionBody = transactionBodyCbor.GetTransactionBody();
            if (transactionWitnessSetCbor != null && transactionWitnessSetCbor.Count > 0)
            {
                transaction.TransactionWitnessSet = transactionWitnessSetCbor.GetTransactionWitnessSet();
            }
            if (auxiliaryDataCbor != null)
            {
                transaction.AuxiliaryData = auxiliaryDataCbor.GetAuxiliaryData();
            }

            //return
            return transaction;
        }

        public static uint CalculateFee(this Transaction transaction, uint? a = null, uint? b = null)
        {
            if (!a.HasValue) a = FeeStructure.Coefficient;
            if (!b.HasValue) b = FeeStructure.Constant;

            return ((uint)transaction.Serialize().ToStringHex().Length * a.Value) + b.Value;
        }

        public static byte[] Serialize(this Transaction transaction)
        {
            return transaction.GetCBOR().EncodeToBytes();
        }

        //name includes Transaction to avoid ambiguity as byte[] could be serialized anything
        public static Transaction DeserializeTransaction(this byte[] bytes)
        {
            return CBORObject.DecodeFromBytes(bytes).GetTransaction();
        }
    }
}
