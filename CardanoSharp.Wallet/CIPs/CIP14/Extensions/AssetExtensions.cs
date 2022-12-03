using CardanoSharp.Wallet.Encoding;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Models;
using CardanoSharp.Wallet.Utilities;
using System;

namespace CardanoSharp.Wallet.CIPs.CIP14.Extensions
{
    /// <summary>
    /// https://cips.cardano.org/cips/cip14/
    /// AssetId (Fingerprint) is a one way hash using a concatenation of PolicyId & AssetName
    /// </summary>
    public static class AssetExtensions
    {
        public const string FingerprintHrp = "asset";

        /// <summary>
        /// Gets the assetId fingerprint [one way hash]
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        public static string ToAssetFingerprint(this Asset asset)
        {
            var tokenTypeId = $"{asset.PolicyId}{asset.Name}";
            var hashed = HashUtility.Blake2b160(tokenTypeId.HexToByteArray());
            return Bech32.Encode(hashed, FingerprintHrp);
        }

        /// <summary>
        /// Turns a token type id into an asset
        /// </summary>
        /// <param name="tokenTypeId">concat of policy id hex & asset name hex</param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
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

        /// <summary>
        ///Gets the assetId fingerprint for a token type id (concat of policy id hex & asset name hex) [one way hash]
        /// </summary>
        /// <param name="tokenTypeId">concat of policy id hex & asset name hex</param>
        /// <returns></returns>
        public static string ToAssetFingerprint(this string tokenTypeId)
        {
            return Bech32.Encode(HashUtility.Blake2b160(tokenTypeId.HexToByteArray()), FingerprintHrp);
        }
    }
}