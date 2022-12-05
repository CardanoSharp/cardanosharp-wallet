using System;
using CardanoSharp.Wallet.Common;
using CardanoSharp.Wallet.Models.Transactions.TransactionWitness;
using CardanoSharp.Wallet.Models.Transactions.TransactionWitness.PlutusScripts;
using CardanoSharp.Wallet.Utilities;
using PeterO.Cbor2;

namespace CardanoSharp.Wallet.Extensions.Models
{
    public static class ExUnitsExtensions
    {
        public static CBORObject GetCBOR(this ExUnits exUnits)
        {
            //ex_units = [mem: uint, steps: uint]
            var exUnitsCbor = CBORObject.NewArray();
            exUnitsCbor.Add(exUnits.Mem);
            exUnitsCbor.Add(exUnits.Steps);
            return exUnitsCbor;
        }

        public static ExUnits GetExUnits(this CBORObject exUnitsCbor)
        {
            if (exUnitsCbor == null)
            {
                throw new ArgumentNullException(nameof(exUnitsCbor));
            }

            if (exUnitsCbor.Type != CBORType.Array)
            {
                throw new ArgumentException("exUnitsCbor is not expected type CBORType.ByteString");
            }

            if (exUnitsCbor.Count != 2)
            {
                throw new ArgumentException("exUnitsCbor has unexpected number of elements (expected 2)");
            }
            
            var exUnits = new ExUnits();
            exUnits.Mem = (uint)exUnitsCbor[0].DecodeValueToInt32();
            exUnits.Steps = (uint)exUnitsCbor[1].DecodeValueToInt32();
            return exUnits;          
        }
    }
}
