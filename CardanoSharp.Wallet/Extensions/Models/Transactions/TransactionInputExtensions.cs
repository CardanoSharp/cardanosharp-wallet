using CardanoSharp.Wallet.Models.Transactions;
using PeterO.Cbor2;
using System;

namespace CardanoSharp.Wallet.Extensions.Models.Transactions
{
    public static class TransactionInputExtensions
    {
        public static CBORObject GetCBOR(this TransactionInput transactionInput)
        {
            return CBORObject.NewArray()
                .Add(transactionInput.TransactionId)
                .Add(transactionInput.TransactionIndex);
        }

        public static TransactionInput GetTransactionInput(this CBORObject transactionInputCbor)
        {
            //validation
            if (transactionInputCbor == null)
            {
                throw new ArgumentNullException(nameof(transactionInputCbor));
            }
            if (transactionInputCbor.Type != CBORType.Array) 
            {
                throw new ArgumentException("transactionInputCbor is not expected type CBORType.Map");
            }
            if (transactionInputCbor.Count != 2)
            {
                throw new ArgumentException("transactionInputCbor unexpected number elements (expected 2)");
            }
            if (transactionInputCbor[0].Type != CBORType.ByteString)
            {
                throw new ArgumentException("transactionInputCbor first element unexpected type (expected ByteString)");
            }
            if (transactionInputCbor[1].Type != CBORType.Integer)
            {
                throw new ArgumentException("transactionInputCbor second element unexpected type (expected Integer)");
            }

            //get data
            var transactionInput = new TransactionInput();
            transactionInput.TransactionId = ((string)transactionInputCbor[0].DecodeValueByCborType()).HexToByteArray();
            transactionInput.TransactionIndex = transactionInputCbor[1].DecodeValueToUInt32();

            //return
            return transactionInput;
        }

        public static byte[] Serialize(this TransactionInput transactionInput)
        {
            return transactionInput.GetCBOR().EncodeToBytes();
        }

        public static TransactionInput DeserializeTransactionInput(this byte[] bytes)
        {
            return CBORObject.DecodeFromBytes(bytes).GetTransactionInput();
        }
    }
}
