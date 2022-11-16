using CardanoSharp.Wallet.CIPs.CIP30.Models;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Extensions.Models;
using PeterO.Cbor2;
using System;

namespace CardanoSharp.Wallet.CIPs.CIP30.Extensions.Models
{
	public static class COSEKeyExtensions
	{
		public static COSEKey GetCOSEKey(this CBORObject COSEKeyCbor)
		{
			//validation
			if (COSEKeyCbor == null)
			{
				throw new ArgumentNullException(nameof(COSEKeyCbor));
			}
			if (COSEKeyCbor.Type != CBORType.Map)
			{
				throw new ArgumentException("COSEKeyCbor is not expected type CBORType.Map");
			}
			if (COSEKeyCbor.Keys.Count < 4)
			{
				throw new ArgumentException("COSEKeyCbor does not contain expected at least 4 keys");
			}
			if (COSEKeyCbor.Keys.Count > 5)
			{
				throw new ArgumentException("COSEKeyCbor does not contain expected at most 5 keys");
			}
			if (!COSEKeyCbor.ContainsKey(1))
			{
				throw new ArgumentException("COSEKeyCbor does not contain '1' key (kty)");
			}
			if (!COSEKeyCbor.ContainsKey(3))
			{
				throw new ArgumentException("COSEKeyCbor does not contain '3' key (alg)");
			}
			if (!COSEKeyCbor.ContainsKey(-1))
			{
				throw new ArgumentException("COSEKeyCbor does not contain '-1' key (crv)");
			}
			if (!COSEKeyCbor.ContainsKey(-2))
			{
				throw new ArgumentException("COSEKeyCbor does not contain -2 key (pub key index)");
			}
			if (COSEKeyCbor[1].DecodeValueToInt32() != 1)
			{
				throw new ArgumentException("COSEKeyCbor[1] does not match expected value '1' (OKP)");
			}
			if (COSEKeyCbor[3].DecodeValueToInt32() != -8)
			{
				throw new ArgumentException("COSEKeyCbor[3] does not match expected value '-8' (EdDSA)");
			}
			if (COSEKeyCbor[-1].DecodeValueToInt32() != 6)
			{
				throw new ArgumentException("COSEKeyCbor[-1] does not match expected value '6' (Ed25519)");
			}

			var key = new COSEKey()
			{
				Key = ((string)COSEKeyCbor[-2].DecodeValueByCborType()).HexToByteArray(),
			};

			if (COSEKeyCbor.ContainsKey(2))
				key.Kid = ((string)COSEKeyCbor[2].DecodeValueByCborType()).HexToByteArray();

			return key;
		}
	}
}