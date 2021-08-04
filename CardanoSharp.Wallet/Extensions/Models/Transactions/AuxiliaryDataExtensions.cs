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
    }
}
