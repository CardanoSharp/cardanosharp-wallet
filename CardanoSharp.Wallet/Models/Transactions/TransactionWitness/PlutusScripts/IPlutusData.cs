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
    }

    // big_int = int / big_uint / big_nint
    // big_uint = #6.2(bounded_bytes)
    // big_nint = #6.3(bounded_bytes)
    
    // int
    public class PlutusDataInt : IPlutusData
    {
        public uint Value { get; set; }
        public CBORObject GetCBOR()
        {
            return CBORObject.FromObject(Value);
        }
    }
    
    // big_uint = #6.2(bounded_bytes)
    public class PlutusDataUInt: IPlutusData
    {
        public byte[] Value { get; set; }
        public CBORObject GetCBOR()
        {
            return CBORObject.FromObject(
                CBORObject.FromObject(Value)
                    .EncodeToBytes()
            ).WithTag(24);
        }
    }
    
    // big_nint = #6.3(bounded_bytes)
    public class PlutusDataNInt: IPlutusData
    {
        public byte[] Value { get; set; }
        public CBORObject GetCBOR()
        {
            return CBORObject.FromObject(
                CBORObject.FromObject(Value)
                    .EncodeToBytes()
            ).WithTag(24);
        }
    }
    
    // bounded_bytes
    public class PlutusDataBytes: IPlutusData
    {
        public byte[] Value { get; set; }
        public CBORObject GetCBOR()
        {
            return CBORObject.FromObject(
                CBORObject.FromObject(Value)
                    .EncodeToBytes()
            ).WithTag(24);
        }
    }
    
    // { * plutus_data => plutus_data }
    public class PlutusDataMap: IPlutusData
    {
        public Dictionary<IPlutusData, IPlutusData> Value { get; set; }
        public CBORObject GetCBOR()
        {
            return CBORObject.FromObject(
                CBORObject.FromObject(Value)
                    .EncodeToBytes()
            ).WithTag(24);
        }
    }
    
    // [ * plutus_data ]
    public class PlutusDataArray: IPlutusData
    {
        public IPlutusData[] Value { get; set; }
        public CBORObject GetCBOR()
        {
            return CBORObject.FromObject(
                CBORObject.FromObject(Value)
                    .EncodeToBytes()
            ).WithTag(24);
        }
    }
}