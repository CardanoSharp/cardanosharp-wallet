using CardanoSharp.Wallet.Models.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CardanoSharp.Wallet.Extensions
{
    public static class NativeAssetCollectionExtension
    {
        public static ulong CalculateMinUtxoLovelace(this Dictionary<byte[], NativeAsset> tokens,
            int lovelacePerUtxoWord = 34482, // utxoCostPerWord in protocol params (could change in the future)
            int policyIdSizeBytes = 28, // 224 bit policyID (won't change in forseeable future)
            bool hasDataHash = false) // for UTxOs with a smart contract datum
        {
            const int fixedUtxoPrefixWords = 6;
            const int fixedUtxoEntryWithoutValueSizeWords = 27; // The static parts of a UTxO: 6 + 7 + 14 words
            const int fixedPerTokenCost = 12;
            const int byteRoundUpAddition = 7;
            const int bytesPerWord = 8; // One "word" is 8 bytes (64-bit)
            const int fixedDataHashSizeWords = 10;

            // Get distinct policyIDs and assetNames
            var policyIds = new HashSet<string>();
            var assetNameHexadecimals = new HashSet<string>();

            foreach (var asset in tokens)
            {
                policyIds.Add(asset.Key.ToStringHex());
                foreach (var token in asset.Value.Token)
                {
                    assetNameHexadecimals.Add(token.Key.ToStringHex());
                }
            }

            // Calculate (prefix + (numDistinctPids * 28(policyIdSizeBytes) + numTokens * 12(fixedPerTokenCost) + tokensNameLen + 7) ~/8)
            var tokensNameLen = assetNameHexadecimals.Sum(an => an.Length) / 2; // 2 hexadecimal chars = 1 Byte
            var valueSizeWords = fixedUtxoPrefixWords + (
                (policyIds.Count * policyIdSizeBytes)
                + (assetNameHexadecimals.Count * fixedPerTokenCost)
                + tokensNameLen + byteRoundUpAddition) / bytesPerWord;
            var dataHashSizeWords = hasDataHash ? fixedDataHashSizeWords : 0;

            var minUtxoLovelace = Convert.ToUInt64(lovelacePerUtxoWord
                * (fixedUtxoEntryWithoutValueSizeWords + valueSizeWords + dataHashSizeWords));

            return minUtxoLovelace;
        }
    }
}
