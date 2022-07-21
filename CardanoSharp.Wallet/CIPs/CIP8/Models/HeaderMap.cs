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
        public byte[] Address { get; }
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

        public CBORObject GetCBOR()
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
                    map.Add(CoseCommonHeaderLabels.CounterSignature, CounterSignature.First().GetCBOR());
                }
                else
                {
                    var counterSignatureCbors = CBORObject.NewArray();
                    foreach (var coseSignature in CounterSignature)
                    {
                        counterSignatureCbors.Add(coseSignature.GetCBOR());
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
    }
}