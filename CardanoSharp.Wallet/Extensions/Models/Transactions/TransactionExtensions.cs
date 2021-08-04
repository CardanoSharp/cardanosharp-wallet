using CardanoSharp.Wallet.Extensions.Models.Transactions.TransactionWitnesses;
using CardanoSharp.Wallet.Models.Transactions;
using PeterO.Cbor2;
using System;
using System.Collections.Generic;
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
            cborTransaction.Add(transaction.AuxiliaryData.GetCBOR());

            //return serialized cbor
            return cborTransaction;
        }
    }
}
