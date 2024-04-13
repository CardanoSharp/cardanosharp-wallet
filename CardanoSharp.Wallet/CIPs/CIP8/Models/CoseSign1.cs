using PeterO.Cbor2;
using System;

namespace CardanoSharp.Wallet.CIPs.CIP8.Models
{
    /// <summary>
    /// https://datatracker.ietf.org/doc/html/rfc8152#section-4.2
    /// COSE_Sign = [
    ///    Headers,
    ///    payload : bstr / nil,
    ///    signature : bstr
    /// ]
    /// </summary>
    public class CoseSign1 : ICoseMessage
    {
        public Headers Headers { get; }
        public byte[] Payload { get; } 
        public byte[] Signature { get; }

        public CoseSign1(Headers headers, byte[] payload, byte[] signature)
        {
            Headers = headers ?? throw new ArgumentNullException(nameof(headers));
            Payload = payload ?? throw new ArgumentNullException(nameof(payload));
            Signature = signature ?? throw new ArgumentNullException(nameof(signature));
        }

        public CoseSign1(CBORObject coseSignCbor)
        {
            if (coseSignCbor == null)
                throw new ArgumentNullException(nameof(coseSignCbor));
            if (coseSignCbor.Type != CBORType.Array)
                throw new ArgumentException($"{nameof(coseSignCbor)} must be an array");

            var protectedHeader = new ProtectedHeaderMap(coseSignCbor[0]);
            var unprotectedHeader = new HeaderMap(coseSignCbor[1]);
            Headers = new Headers(protectedHeader, unprotectedHeader);
            Payload = coseSignCbor[2].GetByteString();
            Signature = coseSignCbor[3].GetByteString();
        }

        public CBORObject GetCbor()
        {
            var cbor = CBORObject.NewArray();
            foreach (var headerItem in Headers.GetCbor())
            {
                cbor.Add(headerItem);
            }
            cbor.Add(Payload is { Length: > 0 } ? CBORObject.FromObject(Payload) : CBORObject.Null);
            cbor.Add(Signature is { Length: > 0 }? CBORObject.FromObject(Signature) : CBORObject.FromObject(Array.Empty<byte>()));
            return cbor;
        }
    }
}