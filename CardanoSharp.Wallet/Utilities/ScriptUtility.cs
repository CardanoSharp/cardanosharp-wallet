using System;
using System.Collections.Generic;
using System.Linq;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Models.Transactions.TransactionWitness.PlutusScripts;
using PeterO.Cbor2;
namespace CardanoSharp.Wallet.Utilities
{
    public static class ScriptUtility
    {
        public static byte[] GenerateScriptDataHash(List<Redeemer> redeemers, List<IPlutusData> datums, byte[] languageViews)
        {
            byte[] encodedBytes;

            /**
            ; script data format:
            ; [ redeemers | datums | language views ]
            ; The redeemers are exactly the data present in the transaction witness set.
            ; Similarly for the datums, if present. If no datums are provided, the middle
            ; field is an empty string.
            **/
            
            byte[] plutusDataBytes = new byte[0];
            if (datums != null && datums.Count > 0)
            {
                var cborPlutusDatas = CBORObject.NewArray();
                foreach (var plutusData in datums) 
                {
                    cborPlutusDatas.Add(plutusData.GetCBOR());
                }
                plutusDataBytes = cborPlutusDatas.EncodeToBytes();
            }
            
            byte[] redeemerBytes = new byte[0];
            if (redeemers != null && redeemers.Count > 0)
            {
                var cborRedeemers = CBORObject.NewArray();
                foreach (var redeemer in redeemers) 
                {
                    cborRedeemers.Add(redeemer.GetCBOR());
                }
                redeemerBytes = cborRedeemers.EncodeToBytes();
            }
            else {
                /**
                ; Finally, note that in the case that a transaction includes datums but does not
                ; include any redeemers, the script data format becomes (in hex):
                ; [ 80 | datums | A0 ]
                ; corresponding to a CBOR empty list and an empty map.
                **/

                redeemerBytes = "80".HexToByteArray();
                languageViews = "A0".HexToByteArray();
            }

            encodedBytes = new byte[redeemerBytes.Length + plutusDataBytes.Length + languageViews.Length];
            Buffer.BlockCopy(redeemerBytes, 0, encodedBytes, 0, redeemerBytes.Length);
            Buffer.BlockCopy(plutusDataBytes, 0, encodedBytes, redeemerBytes.Length, plutusDataBytes.Length);
            Buffer.BlockCopy(languageViews, 0, encodedBytes, redeemerBytes.Length + plutusDataBytes.Length, languageViews.Length);   
            return HashUtility.Blake2b256(encodedBytes);
        }
    }
}