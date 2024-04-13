using PeterO.Cbor2;
using System;

namespace CardanoSharp.Wallet.CIPs.CIP8.Models
{
    /// CBOR ByteString Serialised HeaderMap included in crptographic computation
    public class ProtectedHeaderMap
    {
        public byte[] Bytes { get; }

        public ProtectedHeaderMap(byte[] bytes)
        {
            Bytes = bytes;
        }

        public ProtectedHeaderMap(HeaderMap headerMap)
        {
            Bytes = headerMap.GetCbor().EncodeToBytes();
        }

        public ProtectedHeaderMap(CBORObject protectedHeaderMapCbor)
        {
            if (protectedHeaderMapCbor == null)
            {
                throw new ArgumentNullException(nameof(protectedHeaderMapCbor));
            }
            Bytes = protectedHeaderMapCbor.GetByteString();
        }

        public CBORObject GetCbor()
        {
            return CBORObject.FromObject(Bytes);
        }
    }
}