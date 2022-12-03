using CardanoSharp.Wallet.CIPs.CIP30.Models;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Extensions.Models;
using PeterO.Cbor2;
using System;

namespace CardanoSharp.Wallet.CIPs.CIP30.Extensions.Models
{
	public static class CoseKeyExtensions
	{
		public static CoseKey GetCoseKey(this CBORObject coseKeyCbor)
		{
			//validation
			if (coseKeyCbor == null)
			{
				throw new ArgumentNullException(nameof(coseKeyCbor));
			}
			if (coseKeyCbor.Type != CBORType.Map)
			{
				throw new ArgumentException($"{nameof(coseKeyCbor)} is not expected type CBORType.Map");
			}
			if (coseKeyCbor.Keys.Count < 4)
			{
				throw new ArgumentException($"{nameof(coseKeyCbor)} does not contain expected at least 4 keys");
			}
			if (coseKeyCbor.Keys.Count > 5)
			{
				throw new ArgumentException($"{nameof(coseKeyCbor)} does not contain expected at most 5 keys");
			}
			if (!coseKeyCbor.ContainsKey(1))
			{
				throw new ArgumentException($"{nameof(coseKeyCbor)} does not contain '1' key (kty)");
			}
			if (!coseKeyCbor.ContainsKey(3))
			{
				throw new ArgumentException($"{nameof(coseKeyCbor)} does not contain '3' key (alg)");
			}
			if (!coseKeyCbor.ContainsKey(-1))
			{
				throw new ArgumentException($"{nameof(coseKeyCbor)} does not contain '-1' key (crv)");
			}
			if (!coseKeyCbor.ContainsKey(-2))
			{
				throw new ArgumentException($"{nameof(coseKeyCbor)} does not contain -2 key (pub key index)");
			}
			if (coseKeyCbor[1].DecodeValueToInt32() != 1)
			{
				throw new ArgumentException($"{nameof(coseKeyCbor)}[1] does not match expected value '1' (OKP)");
			}
			if (coseKeyCbor[3].DecodeValueToInt32() != -8)
			{
				throw new ArgumentException($"{nameof(coseKeyCbor)}[3] does not match expected value '-8' (EdDSA)");
			}
			if (coseKeyCbor[-1].DecodeValueToInt32() != 6)
			{
				throw new ArgumentException($"{nameof(coseKeyCbor)}[-1] does not match expected value '6' (Ed25519)");
			}

			var key = new CoseKey()
			{
				Key = ((string)coseKeyCbor[-2].DecodeValueByCborType()).HexToByteArray(),
			};

			if (coseKeyCbor.ContainsKey(2))
				key.Kid = ((string)coseKeyCbor[2].DecodeValueByCborType()).HexToByteArray();

			return key;
		}
	}
}