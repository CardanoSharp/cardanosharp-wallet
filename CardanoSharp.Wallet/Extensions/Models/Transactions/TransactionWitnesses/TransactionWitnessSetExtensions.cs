using CardanoSharp.Wallet.Models.Transactions;
using PeterO.Cbor2;
using System;
using System.Collections.Generic;
using System.Linq;

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
                    cborNativeScriptWitnesses.Add(nativeScript.GetCBOR2());
                }

                cborWitnessSet.Add(1, cborNativeScriptWitnesses);
            }

            return cborWitnessSet;
        }

        public static TransactionWitnessSet GetTransactionWitnessSet(this CBORObject transactionWitnessSetCbor)
        {
            //validation
            if (transactionWitnessSetCbor == null)
            {
                throw new ArgumentNullException(nameof(transactionWitnessSetCbor));
            }
            if (transactionWitnessSetCbor.Type != CBORType.Map)
            {
                throw new ArgumentException("transactionWitnessSetCbor is not expected type CBORType.Map");
            }

            //get data
            var transactionWitnessSet = new TransactionWitnessSet();
            if (transactionWitnessSetCbor.ContainsKey(0))
            {
                var vkeyWitnessesCbor = transactionWitnessSetCbor[0];
                if (vkeyWitnessesCbor.Type != CBORType.Array)
                {
                    throw new ArgumentException("vkeyWitnessesCbor is not expected type CBORType.Array");
                }
                var vKeyWitnesses = new HashSet<VKeyWitness>();
                foreach (var vkeyWitnessCbor in vkeyWitnessesCbor.Values)
                {
                    vKeyWitnesses.Add(vkeyWitnessCbor.GetVKeyWitness());
                }

                transactionWitnessSet.VKeyWitnesses = vKeyWitnesses;
            }

            if (transactionWitnessSetCbor.ContainsKey(1))
            {
                var nativeScriptsCbor = transactionWitnessSetCbor[1];
                foreach (var nativeScriptCbor in nativeScriptsCbor.Values)
                {
                    transactionWitnessSet.NativeScripts.Add(nativeScriptCbor.GetNativeScript());
                }
            }

            //return
            return transactionWitnessSet;
        }

        public static byte[] Serialize(this TransactionWitnessSet transactionWitnessSet, TransactionBody transactionBody, AuxiliaryData auxiliaryData)
        {
            return transactionWitnessSet.GetCBOR(transactionBody, auxiliaryData).EncodeToBytes();
        }

        public static TransactionWitnessSet DeserializeTransactionWitnessSet(this byte[] bytes)
        {
            return CBORObject.DecodeFromBytes(bytes).GetTransactionWitnessSet();
        }

        public static void RemoveMocks(this TransactionWitnessSet transactionWitnessSet)
        {
            //remove vkey witness mocks
            if (transactionWitnessSet.VKeyWitnesses is not null)
            {
                var mockedWitnesses = transactionWitnessSet.VKeyWitnesses.Where(x => x.IsMock);
                foreach (var mockedWitness in mockedWitnesses)
                {
                    transactionWitnessSet.VKeyWitnesses.Remove(mockedWitness);
                }
            }
        }
    }
}
