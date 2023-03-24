using System;
using System.Collections.Generic;

namespace CardanoSharp.Wallet.CIPs.CIP8.Models
{
    /// <summary>
    /// COSE_Sign = [
    ///    Headers,
    ///    payload : bstr / nil,
    ///    signatures : [+ COSE_Signature]
    /// ]
    /// </summary>
    public class CoseSign : ICoseMessage
    {
        public Headers Headers { get; }
        public byte[] Payload { get; }
        public IList<CoseSignature> Signatures { get; }

        public CoseSign(Headers headers, byte[] payload, IList<CoseSignature> signatures)
        {
            Headers = headers ?? throw new ArgumentNullException(nameof(headers));
            Payload = payload ?? throw new ArgumentNullException(nameof(payload));
            Signatures = signatures ?? throw new ArgumentNullException(nameof(signatures));
        }
    }
}