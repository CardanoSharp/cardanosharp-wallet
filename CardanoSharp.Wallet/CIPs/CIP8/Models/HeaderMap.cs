using CardanoSharp.Wallet.Extensions.Models;
using PeterO.Cbor2;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CardanoSharp.Wallet.CIPs.CIP8.Models
{
    /// <summary>
    /// A dictionary with a set of reserved int or string labels but also supporting open-ended values
    /// similar to HTTP request and response headers
    /// https://datatracker.ietf.org/doc/html/rfc8152#section-3.1
    /// 
    /// Generic_Headers = (
    ///    ? 1 => int / tstr,  ; algorithm identifier
    ///    ? 2 => [+label],    ; criticality
    ///    ? 3 => tstr / int,  ; content type
    ///    ? 4 => bstr,        ; key identifier
    ///    ? 5 => bstr,        ; IV
    ///    ? 6 => bstr,        ; Partial IV
    ///    ? 7 => COSE_Signature / [+COSE_Signature] ; Counter signature
    ///    ? adddress => bstr ; Address CIP8 and CIP30
    ///)
    /// 
    /// header_map = {
    ///    Generic_Headers,
    ///    * label => values
    ///   }
    ///   
    /// </summary>
    public class HeaderMap
    {
        public AlgorithmId? AlgorithmId { get; }
        public IList<object> CriticalHeaders { get; }
        public object ContentType { get; } // string or uint https://datatracker.ietf.org/doc/html/rfc8152#section-3.1
        public byte[] KeyId { get; }
        public byte[] InitVector { get; }
        public byte[] PartialInitVector { get; }
        public IList<CoseSignature> CounterSignature { get; }
        public byte[] Address { get; } // Cardano CIP8 specific 
        public IDictionary<object, object> OtherHeaders { get; }

        public HeaderMap(
            AlgorithmId? algorithmId = null,
            IList<object> criticalHeaders = null,
            object contentType = null,
            byte[] keyId = null,
            byte[] initVector = null,
            byte[] partialInitVector = null,
            IList<CoseSignature> counterSignature = null,
            byte[] address = null,
            IDictionary<object, object> otherHeaders = null)
        {
            // https://datatracker.ietf.org/doc/html/rfc8152#section-3.1 under content type
            if (contentType is not null && !(contentType is string || contentType is uint))
                throw new NotSupportedException($"value for {nameof(ContentType)} must be of type string or uint");
            AlgorithmId = algorithmId;
            CriticalHeaders = criticalHeaders ?? new List<object>();
            ContentType = contentType;
            KeyId = keyId;
            InitVector = initVector;
            PartialInitVector = partialInitVector;
            CounterSignature = counterSignature ?? new List<CoseSignature>();
            Address = address;
            OtherHeaders = otherHeaders ?? new Dictionary<object, object>();
        }

        public HeaderMap(CBORObject headerMapCbor)
        {
            if (headerMapCbor is null)
            {
                throw new ArgumentNullException(nameof(headerMapCbor));
            }

            OtherHeaders = new Dictionary<object, object>();
            foreach (var key in headerMapCbor.Keys)
            {
                switch (key.Type)
                {
                    case CBORType.Integer:
                        {
                            var intLabel = key.AsNumber().ToInt32Unchecked();
                            var cborObjectAtLabel = headerMapCbor[key];
                            switch (intLabel)
                            {
                                case CoseCommonHeaderLabels.AlgorithmId:
                                    AlgorithmId = DecodeAlgorithmId(cborObjectAtLabel);
                                    break;
                                case CoseCommonHeaderLabels.CriticalHeaders:
                                    CriticalHeaders = DecodeCriticalHeaders(cborObjectAtLabel);
                                    break;
                                case CoseCommonHeaderLabels.ContentType:
                                    ContentType = DecodeLabel(cborObjectAtLabel);
                                    break;
                                case CoseCommonHeaderLabels.KeyId:
                                    KeyId = DecodeBytesFromByteString(cborObjectAtLabel);
                                    break;
                                case CoseCommonHeaderLabels.InitVector:
                                    InitVector = DecodeBytesFromByteString(cborObjectAtLabel);
                                    break;
                                case CoseCommonHeaderLabels.PartialInitVector:
                                    PartialInitVector = DecodeBytesFromByteString(cborObjectAtLabel);
                                    break;
                                case CoseCommonHeaderLabels.CounterSignature:
                                    CounterSignature = DecodeCounterSignatures(cborObjectAtLabel);
                                    break;
                                default:
                                    OtherHeaders.Add(key.AsInt32(), cborObjectAtLabel.DecodeValueByCborType());
                                    break;
                            }
                            break;
                        }
                    case CBORType.TextString when key.AsString() == CoseCommonHeaderLabels.Address:
                        Address = headerMapCbor[key].GetByteString();
                        break;
                    case CBORType.TextString:
                        OtherHeaders.Add(key.AsString(), headerMapCbor[key].DecodeValueByCborType());
                        break;
                    default:
                        throw new CBORException("The label in a COSE Header map must be a string or an integer");
                }
            }
        }

        public CBORObject GetCbor()
        {
            var map = CBORObject.NewMap();

            if (AlgorithmId != null)
            {
                map.Add(CoseCommonHeaderLabels.AlgorithmId, CBORObject.FromObject(AlgorithmId));
            }

            if (CriticalHeaders.Any())
            {
                var critArray = CBORObject.NewArray();
                foreach (var crit in CriticalHeaders)
                {
                    critArray.Add(CBORObject.FromObject(crit));
                }
                map.Add(CoseCommonHeaderLabels.CriticalHeaders, critArray);
            }

            if (ContentType != null)
            {
                map.Add(CoseCommonHeaderLabels.ContentType, CBORObject.FromObject(ContentType));
            }

            if (KeyId != null && KeyId.Any())
            {
                map.Add(CoseCommonHeaderLabels.KeyId, CBORObject.FromObject(KeyId));
            }

            if (InitVector != null && InitVector.Any())
            {
                map.Add(CoseCommonHeaderLabels.InitVector, CBORObject.FromObject(InitVector));
            }

            if (PartialInitVector != null && PartialInitVector.Any())
            {
                map.Add(CoseCommonHeaderLabels.PartialInitVector, CBORObject.FromObject(PartialInitVector));
            }

            if (CounterSignature.Any())
            {
                if (CounterSignature.Count == 1)
                {
                    map.Add(CoseCommonHeaderLabels.CounterSignature, CounterSignature.First().GetCbor());
                }
                else
                {
                    var counterSignatureCbors = CBORObject.NewArray();
                    foreach (var coseSignature in CounterSignature)
                    {
                        counterSignatureCbors.Add(coseSignature.GetCbor());
                    }
                    map.Add(CoseCommonHeaderLabels.CounterSignature, counterSignatureCbors);
                }
            }

            if (Address is not null && Address.Length > 0)
            {
                map.Add(CoseCommonHeaderLabels.Address, Address);
            }

            foreach (var key in OtherHeaders.Keys)
            {
                map.Add(CBORObject.FromObject(key), CBORObject.FromObject(OtherHeaders[key]));
            }

            return map;
        }

        private static AlgorithmId? DecodeAlgorithmId(CBORObject cborObjectForKey)
        {
            if (cborObjectForKey.Type != CBORType.Integer)
            {
                throw new CBORException("Only integer algorithmId types are supported");
            }
            var algorithmIdInt = cborObjectForKey.AsNumber().ToInt32Unchecked();
            if (!Enum.IsDefined(typeof(AlgorithmId), algorithmIdInt))
            {
                throw new CBORException($"Unsupported algorithmId value {algorithmIdInt}");
            }
            return (AlgorithmId)algorithmIdInt;
        }

        private static IList<object> DecodeCriticalHeaders(CBORObject cborObjectForKey)
        {
            if (cborObjectForKey.Type != CBORType.Array)
            {
                throw new CBORException("Critical Headers must be an array type");
            }
            var criticalHeaders = new List<object>();
            foreach (var criticalHeaderCbor in cborObjectForKey.Values)
            {
                criticalHeaders.Add(DecodeLabel(criticalHeaderCbor));
            }
            return criticalHeaders;
        }

        private static object DecodeLabel(CBORObject cborObjectForKey) => cborObjectForKey.Type switch
        {
            CBORType.TextString => cborObjectForKey.AsString(),
            CBORType.Integer => cborObjectForKey.AsNumber().ToUInt32Checked(),
            _ => throw new CBORException($"Unexpected CBOR type {cborObjectForKey.Type} for label, expecting TextString or Integer")
        };

        private static byte[] DecodeBytesFromByteString(CBORObject cborObjectForKey)
        {
            if (cborObjectForKey.Type != CBORType.ByteString)
            {
                throw new CBORException($"Unexpected CBOR type {cborObjectForKey.Type} but expecting ByteString");
            }
            return cborObjectForKey.GetByteString();
        }

        private static IList<CoseSignature> DecodeCounterSignatures(CBORObject cborObjectForKey)
        {
            if (cborObjectForKey.Type != CBORType.Array)
            {
                throw new CBORException("Counter signatures must be an array type");
            }
            var counterSignatures = new List<CoseSignature>();
            foreach (var counterSigCbor in cborObjectForKey.Values)
            {
                counterSignatures.Add(DecodeCoseSignature(counterSigCbor));
            }
            return counterSignatures;
        }

        public static CoseSignature DecodeCoseSignature(CBORObject coseSigCbor)
        {
            if (coseSigCbor.Type != CBORType.Array)
            {
                throw new CBORException("Each counter signature must be an array type");
            }
            var protectedHeader = new ProtectedHeaderMap(coseSigCbor[0]);
            var unprotectedHeader = new HeaderMap(coseSigCbor[1]);
            var signatureBytes = coseSigCbor[2].GetByteString();
            return new CoseSignature(new Headers(protectedHeader, unprotectedHeader), signatureBytes);
        }

        public void AddCriticalHeader(object label)
        {
            // https://datatracker.ietf.org/doc/html/rfc8152#section-1.4
            if (label is not null && !(label is string || label is int))
                throw new NotSupportedException($"value for {nameof(label)} must be of type string or int");
            CriticalHeaders.Add(label);
        }

        public void AddCounterSignature(CoseSignature signature)
        {
            if (signature is null)
                throw new ArgumentNullException(nameof(signature));
            CounterSignature.Add(signature);
        }

        public void AddOtherHeader(object label, object value)
        {
            // https://datatracker.ietf.org/doc/html/rfc8152#section-1.4
            if (label is not null && !(label is string || label is int))
                throw new NotSupportedException($"value for {nameof(label)} must be of type string or int");
            OtherHeaders.Add(label, value);
        }
    }
}