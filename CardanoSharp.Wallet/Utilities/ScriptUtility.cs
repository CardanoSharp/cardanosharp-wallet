using System;
using System.Collections.Generic;
using System.Linq;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Models.Transactions.TransactionWitness.PlutusScripts;


namespace CardanoSharp.Wallet.Utilities
{
    public static class ScriptUtility
    {
        public static byte[] GenerateScriptHash(List<Redeemer> redeemers, List<IPlutusData> datums, byte[] languageViews)
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
                plutusDataBytes = new byte[datums.Sum(datum => datum.Serialize().Length)];
                int offset = 0; 
                foreach (var datum in datums)
                {
                    var datumBytes = datum.Serialize();
                    Buffer.BlockCopy(datumBytes, 0, plutusDataBytes, offset, datumBytes.Length);
                    offset += datumBytes.Length;
                }
            }
            
            byte[] redeemerBytes = new byte[0];
            if (redeemers != null && redeemers.Count > 0)
            {
                redeemerBytes = new byte[redeemers.Sum(redeemer => redeemer.Serialize().Length)];
                int offset = 0; 
                foreach (var redeemer in redeemers)
                {
                    var redeemerSerializedBytes = redeemer.Serialize();
                    Buffer.BlockCopy(redeemerSerializedBytes, 0, redeemerBytes, offset, redeemerSerializedBytes.Length);
                    offset += redeemerSerializedBytes.Length;
                }
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