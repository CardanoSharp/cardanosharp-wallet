using PeterO.Cbor2;
using System;

namespace CardanoSharp.Wallet.CIPs.CIP8.Models
{
    /// <summary>
    /// The CBOR object that carries the body and information about the body is called the
    /// COSE_Sign structure.The CBOR object that carries the signature and information about 
    /// the signature is called the COSE_Signature structure.
    /// 
    /// COSE_Signature =  [
    ///    Headers,
    ///    signature : bstr
    /// ]
    /// </summary>
    public class CoseSignature
    {
        public Headers Headers { get; }
        public byte[] Signature { get; }

        public CoseSignature(Headers headers, byte[] signature)
        {
            Headers = headers ?? throw new ArgumentNullException(nameof(headers));
            Signature = signature ?? throw new ArgumentNullException(nameof(signature));
        }

        public CBORObject GetCBOR()
        {
            var cbor = CBORObject.NewArray();
            foreach (var headerItem in Headers.GetCBOR())
            {
                cbor.Add(headerItem);
            }
            cbor.Add(CBORObject.FromObject(Signature));
            return cbor;
        }
    }
}