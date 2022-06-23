﻿using CardanoSharp.Wallet.Common;
using CardanoSharp.Wallet.Extensions.Models.Transactions.TransactionWitnesses;
using CardanoSharp.Wallet.Models.Transactions;
using PeterO.Cbor2;
using System;

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
                cborTransaction.Add(
                    transaction.TransactionWitnessSet.GetCBOR(transaction.TransactionBody, transaction.AuxiliaryData));
            }
            else
            {
                cborTransaction.Add(CBORObject.NewMap());
            }

            //add isValid
            cborTransaction.Add(transaction.IsValid.GetCBOR());

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
                throw new ArgumentNullException(nameof(transactionCbor));
            }
            if (transactionCbor.Type != CBORType.Array)
            {
                throw new ArgumentException("transactionCbor is not expected type CBORType.Array");
            }
            if (transactionCbor.Count < 2)
            {
                throw new ArgumentException("transactionCbor does not contain at least 2 elements (body & witness set)");
            }

            //get data
            var transactionBodyCbor = transactionCbor[0];
            var transactionWitnessSetCbor = transactionCbor[1];
            var isValidCbor = transactionCbor.Count > 2 ? transactionCbor[2] : null;
            var auxiliaryDataCbor = transactionCbor.Count > 3 ? transactionCbor[3] : null;

            //populate
            var transaction = new Transaction();
            transaction.TransactionBody = transactionBodyCbor.GetTransactionBody();
            if (transactionWitnessSetCbor != null && transactionWitnessSetCbor.Count > 0)
            {
                transaction.TransactionWitnessSet = transactionWitnessSetCbor.GetTransactionWitnessSet();
            }
            if (isValidCbor != null && !isValidCbor.IsNull)
            {
                transaction.IsValid = isValidCbor.GetIsValid();
            }
            if (auxiliaryDataCbor != null && !auxiliaryDataCbor.IsNull)
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

            return ((uint)transaction.Serialize().Length * a.Value) + b.Value;
        }

        public static byte[] Serialize(this Transaction transaction)
        {
            return transaction.GetCBOR().EncodeToBytes();
        }

        public static Transaction DeserializeTransaction(this byte[] bytes)
        {
            return CBORObject.DecodeFromBytes(bytes).GetTransaction();
        }
    }
}
