using CardanoSharp.Wallet.CIPs.CIP30.Models;
using CardanoSharp.Wallet.CIPs.CIP8;
using CardanoSharp.Wallet.CIPs.CIP8.Models;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Models.Keys;

namespace CardanoSharp.Wallet.Utilities
{
    public static class SignDataUtility
    {
        /// <summary>
        /// Sign Payload
        /// https://developers.cardano.org/docs/governance/cardano-improvement-proposals/cip-0008/
        /// </summary>
        /// <param name="coseSigner">An instance of CoseSigner</param>
        /// <param name="payload">String value</param>
        /// <param name="stake">Stake public address</param>
        /// <param name="payment">Payment address private key</param>
        /// <param name="networkType">Cardano network.</param>
        /// <param name="hash">Hash payload</param>
        /// <returns></returns>
        public static DataSignature SignData(ICoseSigner coseSigner, string payload,
            PublicKey stake, PrivateKey payment,
            NetworkType networkType = NetworkType.Mainnet,
            bool hash = false)
        {
            var aPub = payment.GetPublicKey(false);
            var address = AddressUtility.GetBaseAddress(aPub, stake, networkType);

            var payloadBytes = payload.ToBytes();
            if (hash)
            {
                // reference: https://github.com/gitmachtl/cardano-signer/blob/e792768944c8f8d5244a418472cc14028e95aa27/src/cardano-signer.js#L605
                payloadBytes = HashUtility.Blake2b224(payloadBytes);
            }
            
            var coseSign1 = coseSigner.BuildCoseSign1(
                payloadBytes,
                payment,
                address: address.GetBytes(),
                hashed: hash);

            var key = new CIPs.CIP8.Models.CoseKey(
                KeyType.Okp,
                AlgorithmId.EdDsa,
                CurveType.Ed25519,
                verificationKey: aPub.Key);

            return new()
            {
                Signature = coseSign1.GetCbor().EncodeToBytes().ToStringHex(),
                Key = key.GetCbor().EncodeToBytes().ToStringHex()
            };
        }

        /// <summary>
        /// Sign Payload
        /// https://developers.cardano.org/docs/governance/cardano-improvement-proposals/cip-0008/
        /// </summary>
        /// <param name="coseSigner">An instance of CoseSigner</param>
        /// <param name="payload">String value</param>
        /// <param name="privateKey">Private key. eg: root or account private key.</param>
        /// <param name="stakePath">Derivation path to resolve stake keys</param>
        /// <param name="paymentPath">Derivation path to resolve signing payment keys</param>
        /// <param name="networkType">Cardano network.</param>
        /// <param name="hash">Hash payload</param>
        /// <returns></returns>
        public static DataSignature SignData(ICoseSigner coseSigner, string payload,
            PrivateKey privateKey,
            string stakePath = "m/1852'/1815'/0'/2/0",
            string paymentPath = "m/1852'/1815'/0'/0/0",
            NetworkType networkType = NetworkType.Mainnet,
            bool hash = false)
        {
            // stake 
            var (_, sPub) = privateKey.GetKeyPairFromPath(stakePath, false);

            // payment
            var (aPri, _) = privateKey.GetKeyPairFromPath(paymentPath, false);

            return SignData(coseSigner, payload, sPub, aPri, networkType, hash);
        }
    }
}
