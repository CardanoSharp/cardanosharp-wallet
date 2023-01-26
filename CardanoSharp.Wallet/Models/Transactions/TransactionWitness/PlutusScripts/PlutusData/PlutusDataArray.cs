using System;
using System.Collections.Generic;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Utilities;
using PeterO.Cbor2;

namespace CardanoSharp.Wallet.Models.Transactions.TransactionWitness.PlutusScripts
{
    // [ * plutus_data ]
    public class PlutusDataArray : IPlutusData
    {
        // May need to add a chunking system
        //https://github.com/bloxbean/cardano-client-lib/blob/7322b16030d8fa3ac5417d5dc58c92df401855ad/core/src/main/java/com/bloxbean/cardano/client/transaction/spec/ListPlutusData.java#L72
        public IPlutusData[] Value { get; set; }

        public CBORObject GetCBOR()
        {
            var cborDatum = CBORObject.NewArray();
            if (Value != null)
            {
                foreach (var data in Value)
                {
                    cborDatum.Add(data.GetCBOR());
                }
            }

            return cborDatum;
        }

        public byte[] Serialize()
        {
            return GetCBOR().EncodeToBytes();
        }
    }

    public static partial class PlutusDataExtensions
    {
        // TODO 64 character length check?
        //https://github.com/bloxbean/cardano-client-lib/blob/7322b16030d8fa3ac5417d5dc58c92df401855ad/core/src/main/java/com/bloxbean/cardano/client/transaction/spec/ListPlutusData.java
        public static PlutusDataArray GetPlutusDataArray(this CBORObject dataCbor)
        {
            if (dataCbor == null)
            {
                throw new ArgumentNullException(nameof(dataCbor));
            }

            if (dataCbor.Type != CBORType.Array)
            {
                throw new ArgumentException("dataCbor is not expected type CBORType.Array");
            }

            PlutusDataArray plutusDataArray = new PlutusDataArray();
            List<IPlutusData> plutusDatas = new List<IPlutusData>();
            foreach (var data in dataCbor.Values)
            {
                plutusDatas.Add(data.GetPlutusData());
            }
            plutusDataArray.Value = plutusDatas.ToArray();
            return plutusDataArray;
        }
    }
}
