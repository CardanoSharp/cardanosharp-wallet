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
        public static DataSignature SignData(ICoseSigner coseSigner, PrivateKey rootKey, string payload,
            int addressIndex, NetworkType networkType = NetworkType.Mainnet)
        {
            // stake 
            var (_, sPub) = rootKey.GetKeyPairFromPath($"m/1852'/1815'/0'/2/0", false);

            // payment address at index
            var (aPri, aPub) = rootKey.GetKeyPairFromPath($"m/1852'/1815'/0'/0/{addressIndex}", false);

            var address = AddressUtility.GetBaseAddress(aPub, sPub, networkType);

            var coseSign1 = coseSigner.BuildCoseSign1(payload.ToBytes(), aPri, address: address.GetBytes());

            var key = new CIPs.CIP8.Models
                    .CoseKey(
                        KeyType.Okp,
                        AlgorithmId.EdDsa,
                        CurveType.Ed25519,
                        verificationKey: aPub.Key)
                .GetCbor()
                .EncodeToBytes();


            return new()
            {
                Signature = coseSign1.GetCbor().EncodeToBytes().ToStringHex(),
                Key = key.ToStringHex()
            };
        }
    }
}
