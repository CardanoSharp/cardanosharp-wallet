using PeterO.Cbor2;
using System;

namespace CardanoSharp.Wallet.CIPs.CIP8.Models
{
    /// <summary>
    /// empty_or_serialized_map = bstr.cbor header_map / bstr.size 0
    /// 
    /// Headers = (
    ///    protected : empty_or_serialized_map,
    ///    unprotected : header_map
    /// )
    /// </summary>
    public class Headers
    {
        public ProtectedHeaderMap Protected { get; }
        public HeaderMap Unprotected { get; }

        public Headers(ProtectedHeaderMap @protected, HeaderMap unprotected = null)
        {
            Protected = @protected;
            Unprotected = unprotected ?? new HeaderMap();
        }

        public CBORObject[] GetCbor()
        {
            var cbor = new CBORObject[2];
            if (Protected != null)
            {
                cbor[0] = Protected.GetCbor();
            }
            else
            {
                cbor[0] = CBORObject.FromObject(Array.Empty<byte>());
            }

            if (Unprotected != null)
            {
                cbor[1] = Unprotected.GetCbor();
            }
            else
            {
                cbor[1] = CBORObject.NewMap();
            }
            return cbor;
        }
    }
}