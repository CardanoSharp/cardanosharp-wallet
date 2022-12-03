using CardanoSharp.Wallet.CIPs.CIP30.Models;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Utilities;
using PeterO.Cbor2;
using System;

namespace CardanoSharp.Wallet.CIPs.CIP30.Extensions.Models
{
	public static class CoseSign1Extensions
	{
		public static CoseSign1 GetCoseSign1(this CBORObject coseSign1Cbor)
		{
			//validation
			if (coseSign1Cbor == null)
			{
				throw new ArgumentNullException(nameof(coseSign1Cbor));
			}
			if (coseSign1Cbor.Type != CBORType.Array)
			{
				throw new ArgumentException($"{nameof(coseSign1Cbor)} is not expected type CBORType.Array");
			}
			if (coseSign1Cbor.Count != 4)
			{
				throw new ArgumentException($"{nameof(coseSign1Cbor)} does not contain expected 4 keys");
			}
			if (coseSign1Cbor[1].Type != CBORType.Map)
			{
				throw new ArgumentException($"{nameof(coseSign1Cbor)}[1] 'headers' is not expected type CBORType.Map");
			}
			if (!coseSign1Cbor[1].ContainsKey("hashed"))
			{
				throw new ArgumentException($"{nameof(coseSign1Cbor)}[1] 'headers' does not contain mandatory header 'hashed'");
			}

			var payload = (string)coseSign1Cbor[2].DecodeValueByCborType();
			var sig = (string)coseSign1Cbor[3].DecodeValueByCborType();
			bool hashed = (bool)coseSign1Cbor[1]["hashed"].DecodeValueByCborType();
			string? hashedPayload = null;
			if (hashed)
				hashedPayload = HashUtility.Blake2b224(payload.HexToByteArray()).ToStringHex();
			var headers = coseSign1Cbor[0].ToObject<byte[]>();

			return new CoseSign1()
			{
				Signature = sig.HexToByteArray(),
				Payload = payload.HexToByteArray(),
				Hashed = hashed,
				PayloadHash = hashedPayload?.HexToByteArray(),
				Headers = headers
			};
		}

		public static byte[] GetSigStructure(this CoseSign1 coseSign1)
		{
			return CBORObject.NewArray()
				.Add("Signature1")
				.Add(coseSign1.Headers)
				.Add(Array.Empty<byte>())
				.Add(coseSign1.GetPayload())
				.EncodeToBytes();
		}
	}
}