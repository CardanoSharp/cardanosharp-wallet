using CardanoSharp.Wallet.Encoding;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Models;
using CardanoSharp.Wallet.Utilities;
using System;

namespace CardanoSharp.Wallet.CIPs.CIP14.Extensions
{
    public static class AssetExtensions
    {
        public static string GetFingerprint(this Asset asset)
        {
            var tokenTypeId = $"{asset.PolicyId}{asset.Name}";
            var hashed = HashUtility.Blake2b160(tokenTypeId.HexToByteArray());
            return Bech32.Encode(hashed, "asset");
        }

        public static Asset ToAsset(this string tokenTypeId, long quantity = 0)
        {
            if (tokenTypeId.Length < 56 || tokenTypeId.Length > 120)
                throw new ArgumentException("has to be between 56 and 120 character", nameof(tokenTypeId));
            return new Asset()
            {
                PolicyId = tokenTypeId.Substring(0, 56),
                Name = tokenTypeId.Substring(56),
                Quantity = quantity
            };
        }
    }
}