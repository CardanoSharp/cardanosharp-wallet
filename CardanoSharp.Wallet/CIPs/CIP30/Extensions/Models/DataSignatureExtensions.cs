using CardanoSharp.Wallet.CIPs.CIP30.Models;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Models.Keys;
using PeterO.Cbor2;

namespace CardanoSharp.Wallet.CIPs.CIP30.Extensions.Models
{
	public static class DataSignatureExtensions
	{
		public static bool Verify(this DataSignature dataSignature)
		{
			var coseKeyCbor = CBORObject.DecodeFromBytes(dataSignature.Key.HexToByteArray());
			var coseSign1Cbor = CBORObject.DecodeFromBytes(dataSignature.Signature.HexToByteArray());

			var coseKey = coseKeyCbor.GetCOSEKey();
			var coseSign1 = coseSign1Cbor.GetCOSESign1();

			var pubKey = new PublicKey(coseKey.Key, null);

			var verified = pubKey.Verify(coseSign1.GetSigStructure(), coseSign1.Signature);
			return verified;
		}

		public static CoseKey GetCOSEKey(this DataSignature dataSignature)
		{
			var coseKeyCbor = CBORObject.DecodeFromBytes(dataSignature.Key.HexToByteArray());
			var coseKey = coseKeyCbor.GetCOSEKey();
			return coseKey;
		}

		public static CoseSign1 GetCOSESign1(this DataSignature dataSignature)
		{
			var coseSign1Cbor = CBORObject.DecodeFromBytes(dataSignature.Signature.HexToByteArray());
			var coseSign1 = coseSign1Cbor.GetCOSESign1();
			return coseSign1;
		}
	}
}