using PeterO.Cbor2;
using System;
using System.Collections.Generic;


namespace CardanoSharp.Wallet.CIPs.CIP8.Models
{
    /// <summary>
    /// Modeled to specifically handle https://cips.cardano.org/cips/cip30/#apisigndataaddraddresspayloadbytespromisedatasignature
    /// </summary>
    public class CoseKey
    {
        public KeyType KeyType { get; }
        public byte[] KeyId { get; }
        public AlgorithmId AlgorithmId { get; }
        public CurveType CurveType { get; }
        public byte[] VerificationKey { get; }
        public IDictionary<object, object> OtherHeaders { get; }

        public CoseKey(
            KeyType keyType, 
            AlgorithmId algorithmId, 
            CurveType curveType,
            byte[] keyId = null,
            byte[] verificationKey = null)
        {
            KeyType = keyType;
            KeyId = keyId;
            AlgorithmId = algorithmId;
            CurveType = curveType;
            VerificationKey = verificationKey;
            OtherHeaders = new Dictionary<object, object>();
        }

        public void AddOtherHeader(object key, object value)
        {
            // https://datatracker.ietf.org/doc/html/rfc8152#section-1.4
            if (key is not null && !(key is string || key is int))
                throw new NotSupportedException($"value for {nameof(key)} must be of type string or int");
            OtherHeaders.Add(key, value);
        }

        public CBORObject GetCBOR()
        {
            var map = CBORObject.NewMap();
            map.Add(1, (int)KeyType);
            if (KeyId is not null && KeyId.Length > 0)
            {
                map.Add(2, KeyId);
            }
            map.Add(3, (int)AlgorithmId);
            map.Add((int)EcKey.CRV, (int)CurveType);
            map.Add((int)EcKey.X, VerificationKey);
            return map;
        }
    }
}