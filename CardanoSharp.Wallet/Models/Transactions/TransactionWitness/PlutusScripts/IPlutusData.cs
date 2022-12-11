using System;
using System.Collections.Generic;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Utilities;
using PeterO.Cbor2;

namespace CardanoSharp.Wallet.Models.Transactions.TransactionWitness.PlutusScripts
{
    // plutus_data =
    //     constr<plutus_data> ?
    //     / { * plutus_data => plutus_data }
    //     / [ * plutus_data ]
    //     / big_int
    //     / bounded_bytes
    public interface IPlutusData
    {
        CBORObject GetCBOR();
        byte[] Serialize();
    }

    //constr<plutus_data>
    //     constr<a> =
    //     #6.121([* a])
    //   / #6.122([* a])
    //   / #6.123([* a])
    //   / #6.124([* a])
    //   / #6.125([* a])
    //   / #6.126([* a])
    //   / #6.127([* a])
    //   ; similarly for tag range: 6.1280 .. 6.1400 inclusive
    //   / #6.102([uint, [* a]])
    public class PlutusDataConstr : IPlutusData
    {
        public int Constructor { get; set; } = 0;
        public PlutusDataArray Value { get; set; }

        // TODO support #6.102([uint, [* a]])
        public int GetTag()
        {
            return Constructor + 121;
        }

        public CBORObject GetCBOR()
        {
            return CBORObject.FromObject(
                CBORObject.FromObject(Value.GetCBOR().WithTag(GetTag())).EncodeToBytes()
            );
        }

        public byte[] Serialize()
        {
            return GetCBOR().EncodeToBytes();
        }
    }

    // { * plutus_data => plutus_data }
    public class PlutusDataMap : IPlutusData
    {
        public Dictionary<IPlutusData, IPlutusData> Value { get; set; }

        public CBORObject GetCBOR()
        {
            var cborDatum = CBORObject.NewMap();
            foreach (var dataPair in Value)
            {
                cborDatum.Add(dataPair.Key.GetCBOR(), dataPair.Value.GetCBOR());
            }
            return cborDatum;
        }

        public byte[] Serialize()
        {
            return GetCBOR().EncodeToBytes();
        }
    }

    // [ * plutus_data ]
    public class PlutusDataArray : IPlutusData
    {
        public IPlutusData[] Value { get; set; }

        public CBORObject GetCBOR()
        {
            var cborDatum = CBORObject.NewArray();
            foreach (var data in Value)
            {
                cborDatum.Add(data.GetCBOR());
            }
            return cborDatum;
        }

        public byte[] Serialize()
        {
            return GetCBOR().EncodeToBytes();
        }
    }

    // big_int = int / big_uint / big_nint
    // big_uint = #6.2(bounded_bytes)
    // big_nint = #6.3(bounded_bytes)

    // int
    public class PlutusDataInt : IPlutusData
    {
        public int Value { get; set; }

        public CBORObject GetCBOR()
        {
            return CBORObject.FromObject(Value);
        }

        public byte[] Serialize()
        {
            return GetCBOR().EncodeToBytes();
        }
    }

    // big_uint = #6.2(bounded_bytes)
    public class PlutusDataUInt : IPlutusData
    {
        public byte[] Value { get; set; }

        public CBORObject GetCBOR()
        {
            return CBORObject.FromObject(Value);
        }

        public byte[] Serialize()
        {
            return GetCBOR().EncodeToBytes();
        }
    }

    // big_nint = #6.3(bounded_bytes)
    public class PlutusDataNInt : IPlutusData
    {
        public byte[] Value { get; set; }

        public CBORObject GetCBOR()
        {
            return CBORObject.FromObject(Value);
        }

        public byte[] Serialize()
        {
            return GetCBOR().EncodeToBytes();
        }
    }

    // bounded_bytes
    public class PlutusDataBytes : IPlutusData
    {
        public byte[] Value { get; set; }

        public void SetValue(string value)
        {
            Value = value.ToBytes();
        }

        public CBORObject GetCBOR()
        {
            return CBORObject.FromObject(Value);
        }

        public byte[] Serialize()
        {
            return GetCBOR().EncodeToBytes();
        }
    }
}
