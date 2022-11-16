using CardanoSharp.Wallet.CIPs.CIP30.Models;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Utilities;
using PeterO.Cbor2;
using System;

namespace CardanoSharp.Wallet.CIPs.CIP30.Extensions.Models
{
	public static class COSESign1Extensions
	{
		public static COSESign1 GetCOSESign1(this CBORObject COSESign1Cbor)
		{
			//validation
			if (COSESign1Cbor == null)
			{
				throw new ArgumentNullException(nameof(COSESign1Cbor));
			}
			if (COSESign1Cbor.Type != CBORType.Array)
			{
				throw new ArgumentException("COSESign1Cbor is not expected type CBORType.Array");
			}
			if (COSESign1Cbor.Count != 4)
			{
				throw new ArgumentException("COSESign1Cbor does not contain expected 4 keys");
			}
			if (COSESign1Cbor[1].Type != CBORType.Map)
			{
				throw new ArgumentException("COSESign1Cbor[1] 'headers' is not expected type CBORType.Map");
			}
			if (!COSESign1Cbor[1].ContainsKey("hashed"))
			{
				throw new ArgumentException("COSESign1Cbor[1] 'headers' does not contain mandatory header 'hashed'");
			}

			var payload = (string)COSESign1Cbor[2].DecodeValueByCborType();
			var sig = (string)COSESign1Cbor[3].DecodeValueByCborType();
			bool hashed = (bool)COSESign1Cbor[1]["hashed"].DecodeValueByCborType();
			string? hashedPayload = null;
			if (hashed)
				hashedPayload = HashUtility.Blake2b224(payload.HexToByteArray()).ToStringHex();
			var headers = COSESign1Cbor[0].ToObject<byte[]>();

			return new COSESign1()
			{
				Signature = sig.HexToByteArray(),
				Payload = payload.HexToByteArray(),
				Hashed = hashed,
				PayloadHash = hashedPayload?.HexToByteArray(),
				Headers = headers
			};
		}

		public static byte[] GetSigStructure(this COSESign1 coseSign1)
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