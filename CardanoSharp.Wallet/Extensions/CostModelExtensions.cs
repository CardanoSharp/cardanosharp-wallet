using System;
using System.Collections.Generic;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Common;
using CardanoSharp.Wallet.Models;
using CardanoSharp.Wallet.Models.Transactions.TransactionWitness;
using CardanoSharp.Wallet.Models.Transactions.TransactionWitness.PlutusScripts;
using CardanoSharp.Wallet.Utilities;
using PeterO.Cbor2;

namespace CardanoSharp.Wallet.Extensions.Models
{
    public static class CostModelExtensions
    {
        public static CBORObject GetCBOR(this CostModel costModel)
        {
            // We only support V2. V1 will need to be added if one wishes to support it

            /*
            costmdls =
            { ? 0 : [ 166*166 int ] ; Plutus v1
            , ? 1 : [ 175*175 int ] ; Plutus v2
            }
            */
            var costModelCostsCbor = CBORObject.NewArray();
            foreach (long cost in costModel.Costs)
            {
                costModelCostsCbor.Add((uint)cost);
            }

            var costModelCbor = CBORObject.NewMap();
            costModelCbor.Add(1, costModelCostsCbor);
            return costModelCbor;
        }

        public static CostModel GetCostModel(this CBORObject costModelCbor)
        {
            if (costModelCbor == null)
            {
                throw new ArgumentNullException(nameof(costModelCbor));
            }

            if (costModelCbor.Type != CBORType.Map)
            {
                throw new ArgumentException("costModelCbor is not expected type CBORType.ByteString");
            }

            if (costModelCbor.Count <= 0 || costModelCbor.Count > 2)
            {
                throw new ArgumentException("costModelCbor has unexpected number of elements (expected at least 1 and at most 2)");
            }
            
            var costModel = new CostModel();
            costModel.Language = (Language)costModelCbor[0].DecodeValueToInt32();
            costModel.Costs = (List<long>)costModelCbor[1].DecodeValueByCborType();
            return costModel;          
        }

        public static byte[] Serialize(this CostModel costModel)
        {
            return costModel.GetCBOR().EncodeToBytes();
        }

        public static CostModel Deserialize(this byte[] bytes)
        {
            return CBORObject.DecodeFromBytes(bytes).GetCostModel();
        }
    }
}
