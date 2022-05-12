using CardanoSharp.Wallet.Models.Transactions;
using PeterO.Cbor2;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.Extensions.Models.Transactions
{
    public static class AuxiliaryDataExtensions
    {
        public static CBORObject GetCBOR(this AuxiliaryData auxiliaryData)
        {
            return CBORObject.NewArray()
                .Add(auxiliaryData.Metadata)
                .Add(auxiliaryData.List);
        }

        public static AuxiliaryData GetAuxiliaryData(this CBORObject auxiliaryDataCbor)
        {
            //validation

            //get data

            //populate
            var auxiliaryData = new AuxiliaryData();

            //return
            return auxiliaryData;
        }

        public static byte[] Serialize(this AuxiliaryData auxiliaryData)
        {
            return auxiliaryData.GetCBOR().EncodeToBytes();
        }
    }
}
