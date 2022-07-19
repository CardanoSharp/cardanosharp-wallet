using CardanoSharp.Wallet.CIPs.CIP8.Models;
using CardanoSharp.Wallet.Extensions.Models;
using PeterO.Cbor2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CardanoSharp.Wallet.CIPs.CIP8.Extensions
{
    public static class CborSerialisationExtensions
    {
        public static ProtectedHeaderMap DecodeProtectedHeaderMap(this CBORObject protectedHeaderMapCbor)
        {
            if (protectedHeaderMapCbor == null)
            {
                throw new ArgumentNullException(nameof(protectedHeaderMapCbor));
            }
            return new ProtectedHeaderMap(protectedHeaderMapCbor.GetByteString());
        }

        public static HeaderMap DecodeHeaderMap(this CBORObject headerMapCbor)
        {
            if (headerMapCbor is null)
            {
                throw new ArgumentNullException(nameof(headerMapCbor));
            }
            // Common aka Generic headers https://datatracker.ietf.org/doc/html/rfc8152#section-3.1
            AlgorithmId? algorithmId = null;
            IList<object> criticalHeaders = null;
            object contentType = null;
            byte[] keyId = null;
            byte[] initVector = null;
            byte[] partialInitVector = null;
            IList<CoseSignature> counterSignatures = null;
            byte[] address = null;
            // All other headers
            var otherHeaders = new Dictionary<object, object>();
            foreach (var key in headerMapCbor.Keys)
            {
                if (key.Type == CBORType.Integer)
                {
                    var intLabel = key.AsNumber().ToInt32Unchecked();
                    var cborObjectAtLabel = headerMapCbor[key];
                    if (intLabel == CoseCommonHeaderLabels.AlgorithmId)
                    {
                        algorithmId = DecodeAlgorithmId(cborObjectAtLabel);
                    }
                    else if (intLabel == CoseCommonHeaderLabels.CriticalHeaders)
                    {
                        criticalHeaders = DecodeCriticalHeaders(cborObjectAtLabel);
                    }
                    else if (intLabel == CoseCommonHeaderLabels.ContentType)
                    {
                        contentType = DecodeLabel(cborObjectAtLabel);
                    }
                    else if (intLabel == CoseCommonHeaderLabels.KeyId)
                    {
                        keyId = DecodeBytesFromByteString(cborObjectAtLabel);
                    }
                    else if (intLabel == CoseCommonHeaderLabels.InitVector)
                    {
                        initVector = DecodeBytesFromByteString(cborObjectAtLabel);
                    }
                    else if (intLabel == CoseCommonHeaderLabels.PartialInitVector)
                    {
                        partialInitVector = DecodeBytesFromByteString(cborObjectAtLabel);
                    }
                    else if (intLabel == CoseCommonHeaderLabels.CounterSignature)
                    {
                        counterSignatures = DecodeCounterSignatures(cborObjectAtLabel);
                    }
                    else
                    {
                        otherHeaders.Add(key.AsInt32(), headerMapCbor[key].DecodeValueByCborType());
                    }
                }
                else if (key.Type == CBORType.TextString)
                {
                    if (key.AsString() == CoseCommonHeaderLabels.Address)
                    {
                        address = headerMapCbor[key].GetByteString();
                    }
                    else
                    {
                        otherHeaders.Add(key.AsString(), headerMapCbor[key].DecodeValueByCborType());
                    }
                }
                else
                {
                    throw new CBORException("The label in a COSE Header map must be a string or an integer");
                }
            }
            return new HeaderMap(
                algorithmId: algorithmId,
                criticalHeaders: criticalHeaders,
                contentType: contentType,
                keyId: keyId,
                initVector: initVector,
                partialInitVector: partialInitVector,
                counterSignature: counterSignatures,
                address: address,
                otherHeaders: otherHeaders);
        }

        public static CoseSignature DecodeCoseSignature(this CBORObject coseSigCbor)
        {
            if (coseSigCbor.Type != CBORType.Array)
            {
                throw new CBORException("Each counter signature must be an array type");
            }
            var protectedHeader = coseSigCbor[0].DecodeProtectedHeaderMap();
            var unprotectedHeader = coseSigCbor[1].DecodeHeaderMap();
            var signatureBytes = coseSigCbor[2].GetByteString();
            return new CoseSignature(new Headers(protectedHeader, unprotectedHeader), signatureBytes);
        }

        private static AlgorithmId? DecodeAlgorithmId(this CBORObject cborObjectForKey)
        {
            AlgorithmId? algorithmId;
            if (cborObjectForKey.Type != CBORType.Integer)
            {
                throw new CBORException("Only integer algorithmId types are supported");
            }
            var algorithmIdInt = cborObjectForKey.AsNumber().ToInt32Unchecked();
            if (!Enum.IsDefined(typeof(AlgorithmId), algorithmIdInt))
            {
                throw new CBORException($"Unsupported algorithmId value {algorithmIdInt}");
            }
            algorithmId = (AlgorithmId)algorithmIdInt;
            return algorithmId;
        }

        private static IList<object> DecodeCriticalHeaders(this CBORObject cborObjectForKey)
        {
            if (cborObjectForKey.Type != CBORType.Array)
            {
                throw new CBORException("Critical Headers must be an array type");
            }
            var criticalHeaders = new List<object>();
            foreach (var criticalHeaderCbor in cborObjectForKey.Values)
            {
                criticalHeaders.Add(criticalHeaderCbor.DecodeLabel());
            }
            return criticalHeaders;
        }

        private static object DecodeLabel(this CBORObject cborObjectForKey) => cborObjectForKey.Type switch
        {
            CBORType.TextString => cborObjectForKey.AsString(),
            CBORType.Integer => cborObjectForKey.AsNumber().ToUInt32Checked(),
            _ => throw new CBORException($"Unexpected CBOR type {cborObjectForKey.Type}, expecting TextString or Integer")
        };

        private static byte[] DecodeBytesFromByteString(this CBORObject cborObjectForKey)
        {
            if (cborObjectForKey.Type != CBORType.ByteString)
            {
                throw new CBORException($"Unexpected CBOR type {cborObjectForKey.Type} but expecting ByteString");
            }
            return cborObjectForKey.GetByteString();
        }

        private static IList<CoseSignature> DecodeCounterSignatures(this CBORObject cborObjectForKey)
        {
            if (cborObjectForKey.Type != CBORType.Array)
            {
                throw new CBORException("Counter signatures must be an array type");
            }
            var counterSignatures = new List<CoseSignature>();
            foreach (var counterSigCbor in cborObjectForKey.Values)
            {
                counterSignatures.Add(counterSigCbor.DecodeCoseSignature());
            }
            return counterSignatures;
        }
    }
}
