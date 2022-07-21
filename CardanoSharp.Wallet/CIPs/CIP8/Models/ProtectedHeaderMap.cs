using PeterO.Cbor2;

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
            Bytes = headerMap.GetCBOR().EncodeToBytes();
        }

        public CBORObject GetCBOR()
        {
            return CBORObject.FromObject(Bytes);
        }
    }
}