using CardanoSharp.Wallet.Models.Transactions;
using PeterO.Cbor2;
using System;

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
            if (auxiliaryDataCbor == null)
            {
                throw new ArgumentNullException(nameof(auxiliaryDataCbor));
            }

            //get data
            var auxiliaryData = new AuxiliaryData();

            if (auxiliaryDataCbor.Type == CBORType.Array)
            {
                var metadata = auxiliaryDataCbor[0];
                if (metadata != null && metadata.Keys.Count > 0)
                {
                    foreach (var key in metadata.Keys)
                    {
                        var intKey = key.DecodeValueToInt32();
                        auxiliaryData.Metadata[intKey] = metadata[key].DecodeValueByCborType();
                    }
                }
                var list = auxiliaryDataCbor[1];
                if (list != null && list.Count > 0)
                {
                    foreach (var item in list.Values)
                    {
                        auxiliaryData.List.Add(item.DecodeValueByCborType());
                    }
                }
            }else if(auxiliaryDataCbor.Type == CBORType.Map)
            {
                foreach (var key in auxiliaryDataCbor.Keys)
                {
                    var intKey = key.DecodeValueToInt32();
                    auxiliaryData.Metadata[intKey] = auxiliaryDataCbor[key].DecodeValueByCborType();
                }
            }            

            //return
            return auxiliaryData;
        }

        public static byte[] Serialize(this AuxiliaryData auxiliaryData)
        {
            return auxiliaryData.GetCBOR().EncodeToBytes();
        }

        public static AuxiliaryData DeserializeAuxiliaryData(this byte[] bytes)
        {
            return CBORObject.DecodeFromBytes(bytes).GetAuxiliaryData();
        }
    }
}
