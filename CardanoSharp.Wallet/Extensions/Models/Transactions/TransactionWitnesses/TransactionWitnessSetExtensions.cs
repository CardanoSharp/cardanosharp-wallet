using CardanoSharp.Wallet.Models.Transactions;
using PeterO.Cbor2;
using System;
using System.Collections.Generic;
using System.Linq;
using CardanoSharp.Wallet.Models.Transactions.TransactionWitness;
using CardanoSharp.Wallet.Models.Transactions.TransactionWitness.PlutusScripts;

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

            if (transactionWitnessSet.PlutusV1Scripts.Any())
            {
                var cborPlutusV1Scripts = CBORObject.NewArray();
                foreach (var plutusV1Script in transactionWitnessSet.PlutusV1Scripts) 
                {
                    cborPlutusV1Scripts.Add(plutusV1Script.GetCBOR());
                }
                cborWitnessSet.Add(3, cborPlutusV1Scripts);
            }

            if (transactionWitnessSet.PlutusDatas.Any())
            {
                var cborPlutusDatas = CBORObject.NewArray();
                foreach (var plutusData in transactionWitnessSet.PlutusDatas) 
                {
                    cborPlutusDatas.Add(plutusData.GetCBOR());
                }
                cborWitnessSet.Add(4, cborPlutusDatas);
            }

            if (transactionWitnessSet.Redeemers.Any())
            {
                var cborRedeemers = CBORObject.NewArray();
                foreach (var redeemer in transactionWitnessSet.Redeemers) 
                {
                    cborRedeemers.Add(redeemer.GetCBOR());
                }
                cborWitnessSet.Add(5, cborRedeemers);
            }
            
            if (transactionWitnessSet.PlutusV2Scripts.Any())
            {
                var cborPlutusV1Scripts = CBORObject.NewArray();
                foreach (var plutusV1Script in transactionWitnessSet.PlutusV1Scripts) 
                {
                    cborPlutusV1Scripts.Add(plutusV1Script.GetCBOR());
                }
                cborWitnessSet.Add(6, cborPlutusV1Scripts);
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

            if (transactionWitnessSetCbor.ContainsKey(3))
            {
                var plutusV1ScriptsCbor = transactionWitnessSetCbor[3];
                foreach (var plutusV1ScriptCbor in plutusV1ScriptsCbor.Values)
                {
                    transactionWitnessSet.PlutusV1Scripts.Add(plutusV1ScriptCbor.GetPlutusV1Script());
                }
            }

            if (transactionWitnessSetCbor.ContainsKey(4))
            {
                var plutusDatasCbor = transactionWitnessSetCbor[4];
                foreach (var plutusData in plutusDatasCbor.Values) 
                {
                    transactionWitnessSet.PlutusDatas.Add((IPlutusData)plutusData.DecodeValueByCborType());
                }
            }

            if (transactionWitnessSetCbor.ContainsKey(5))
            {
                var redeemersCbor = transactionWitnessSetCbor[5];
                foreach (var redeemerCbor in redeemersCbor.Values)
                {
                    transactionWitnessSet.Redeemers.Add(redeemerCbor.GetRedeemer());
                }
            }

            if (transactionWitnessSetCbor.ContainsKey(6))
            {
                var plutusV2ScriptsCbor = transactionWitnessSetCbor[6];
                foreach (var plutusV2ScriptCbor in plutusV2ScriptsCbor.Values)
                {
                    transactionWitnessSet.PlutusV2Scripts.Add(plutusV2ScriptCbor.GetPlutusV2Script());
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
