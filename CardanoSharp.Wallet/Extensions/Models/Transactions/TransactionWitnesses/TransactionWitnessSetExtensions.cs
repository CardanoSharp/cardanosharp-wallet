using CardanoSharp.Wallet.Models.Transactions;
using PeterO.Cbor2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CardanoSharp.Wallet.Extensions.Models.Transactions.TransactionWitnesses
{
    public static class TransactionWitnessSetExtensions
    {
        public static CBORObject GetCBOR(this TransactionWitnessSet transactionWitnessSet, TransactionBody transactionBody, AuxiliaryData auxiliaryData)
        {
            var cborWitnessSet = CBORObject.NewMap();

            if (transactionWitnessSet.VKeyWitnesses.Any())
            {
                var cborVKeyWitnesses = CBORObject.NewArray();
                foreach (var vkeyWitness in transactionWitnessSet.VKeyWitnesses)
                {

                    cborVKeyWitnesses.Add(vkeyWitness.GetCBOR(transactionBody, auxiliaryData));
                }

                cborWitnessSet.Add(0, cborVKeyWitnesses);
            }

            if (transactionWitnessSet.NativeScripts.Any())
            {
                var cborNativeScriptWitnesses = CBORObject.NewArray();
                foreach (var nativeScript in transactionWitnessSet.NativeScripts)
                {
                    cborNativeScriptWitnesses.Add(nativeScript.GetCBOR());
                }

                cborWitnessSet.Add(1, cborNativeScriptWitnesses);
            }

            return cborWitnessSet;
        }

        public static byte[] Serialize(this TransactionWitnessSet transactionWitnessSet, TransactionBody transactionBody, AuxiliaryData auxiliaryData)
        {
            return transactionWitnessSet.GetCBOR(transactionBody, auxiliaryData).EncodeToBytes();
        }
    }
}
