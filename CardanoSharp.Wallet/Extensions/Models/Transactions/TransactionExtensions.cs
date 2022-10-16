using CardanoSharp.Wallet.Common;
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
                cborTransaction.Add(transaction.TransactionWitnessSet.GetCBOR(
                    transaction.TransactionBody, transaction.AuxiliaryData));
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
            // Required because zero value => smaller CBOR payload => fee lower than minimum
            transaction.TransactionBody.Fee = b.Value;
            return ((uint)transaction.Serialize().Length * a.Value) + b.Value;
        }

        /// <summary>
        /// This method will create mock witnesses, calculate fee, set the fee, and remove mocks
        /// </summary>
        /// <param name="transaction">This is the transaction we are calculating the fee for</param>
        /// <param name="a">This comes from the protocol parameters. Parameter MinFeeA</param>
        /// <param name="b">This comes from the protocol parameters. Parameter MinFeeB</param>
        /// <param name="numberOfVKeyWitnessesToMock">To correctly calculate the fee, we need to have the signatures of all required private keys. You can mock if you cannot currently sign with all. This will ensure the fee is correctly calculated while you gather the signatures</param>
        /// <returns></returns>
        public static uint CalculateAndSetFee(this Transaction transaction, uint? a = null, uint? b = null, int numberOfVKeyWitnessesToMock = 0)
        {
            if(numberOfVKeyWitnessesToMock > 0)
                transaction.TransactionWitnessSet.VKeyWitnesses.CreateMocks(numberOfVKeyWitnessesToMock);
            
            var fee = CalculateFee(transaction, a, b);
            transaction.TransactionBody.Fee = fee;

            if (transaction.TransactionWitnessSet is not null)
                transaction.TransactionWitnessSet.RemoveMocks();
            
            return fee;
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
