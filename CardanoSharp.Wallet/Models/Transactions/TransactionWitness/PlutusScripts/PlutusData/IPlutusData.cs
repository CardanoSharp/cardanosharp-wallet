using System;
using System.Collections.Generic;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Utilities;
using PeterO.Cbor2;

namespace CardanoSharp.Wallet.Models.Transactions.TransactionWitness.PlutusScripts
{
    // Plutus Data HS: https://github.com/input-output-hk/plutus/blob/1f31e640e8a258185db01fa899da63f9018c0e85/plutus-core/plutus-core/src/PlutusCore/Data.hs#L61
    // plutus_data =
    //     constr<plutus_data> ?
    //     / { * plutus_data => plutus_data }
    //     / [ * plutus_data ]
    //     / big_int
    //     / bounded_bytes
    public interface IPlutusData
    {
        public CBORObject GetCBOR();
        public byte[] Serialize();
    }    

    public static partial class PlutusDataExtensions {
        public static IPlutusData GetPlutusData(this CBORObject dataCbor)
        {
            if (dataCbor == null)
            {
                throw new ArgumentNullException(nameof(dataCbor));
            }

            if (dataCbor.Type == CBORType.Integer)
            {
                return dataCbor.GetPlutusDataBigInt();
            }
            else if (dataCbor.Type == CBORType.ByteString)
            {
                return dataCbor.GetPlutusDataBytes();
            }
            else if (dataCbor.Type == CBORType.Array)
            {                
                // If the dataCbor has a tag, it is a PlutusDataConstr
                if (dataCbor.IsTagged)
                {
                    return dataCbor.GetPlutusDataConstr();
                }
                return dataCbor.GetPlutusDataArray();
            }
            else if (dataCbor.Type == CBORType.Map)
            {
                return dataCbor.GetPlutusDataMap();
            }

            throw new ArgumentException("Cbor deserialization failed. Invalid type. " + dataCbor.Type.ToString());
        }
    }
}
