using CardanoSharp.Wallet.Common;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Models.Keys;
using CardanoSharp.Wallet.Models.Segments;
using CardanoSharp.Wallet.Utilities;
using System;
using System.Security.Cryptography;

namespace CardanoSharp.Wallet.Models.Derivations
{
    public interface IPathDerivation
    {
        ISegment Segment { get; }
        PrivateKey PrivateKey { get; }
        PublicKey PublicKey { get; }
    }

    public abstract class AKeyDerivation : IPathDerivation
    {
        protected AKeyDerivation(ISegment segment)
        {
            Segment = segment;
        }

        public ISegment Segment { get; }
        public PrivateKey PrivateKey { get; protected set; }
        public PublicKey PublicKey { get; protected set; }
    }

    public abstract class AChildKeyDerivation : AKeyDerivation, IPathDerivation
    {
        static uint MinHardIndex = 0x80000000;


        protected AChildKeyDerivation(PrivateKey key, ISegment segment) : base(segment)
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var index = Convert.ToUInt32(segment.Value);
            if (segment.IsHardened) index += MinHardIndex;
            PrivateKey = GetChildKeyDerivation(key, index);
            PublicKey = PrivateKey.GetPublicKey(withZeroByte: false);
        }

        protected AChildKeyDerivation(PublicKey key, ISegment segment) : base(segment)
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var index = Convert.ToUInt32(segment.Value);
            if (segment.IsHardened) throw new Exception("Public Keys cannot derive hardened paths");
            PrivateKey = null;
            PublicKey = GetChildKeyDerivation(key, index);
        }

        private static PublicKey GetChildKeyDerivation(PublicKey pKey, uint index)
        {
            var kl = new byte[32];
            Buffer.BlockCopy(pKey.Key, 0, kl, 0, 32);
            //var kr = new byte[32];
            //Buffer.BlockCopy(pKey.Key, 32, kr, 0, 32);

            var z = new byte[64];
            var zl = new byte[32];
            var zr = new byte[32];
            var i = new byte[64];
            var seri = Bip32Utility.Le32(index);

            BigEndianBuffer zBuffer = new BigEndianBuffer();
            BigEndianBuffer iBuffer = new BigEndianBuffer();

            zBuffer.Write(new byte[] { 0x02 }); //constant or enum?
            zBuffer.Write(pKey.Key);
            zBuffer.Write(seri);

            iBuffer.Write(new byte[] { 0x03 }); //constant or enum?
            iBuffer.Write(pKey.Key);
            iBuffer.Write(seri);

            using (HMACSHA512 hmacSha512 = new HMACSHA512(pKey.Chaincode))
            {
                z = hmacSha512.ComputeHash(zBuffer.ToArray());
                zl = z.Slice(0, 32);
                zr = z.Slice(32);
            }

            // left = kl + 8 * trunc28(zl)
            var left = Bip32Utility.Add28Mul8(kl, zl);
            // right = zr + kr
            //var right = Bip32Utility.Add256Bits(kr, zr);

            //var key = new byte[left.Length + right.Length];
            //Buffer.BlockCopy(left, 0, key, 0, left.Length);
            //Buffer.BlockCopy(right, 0, key, left.Length, right.Length);
            var key = new byte[left.Length];
            Buffer.BlockCopy(left, 0, key, 0, left.Length);
            //chaincode

            byte[] cc;
            using (HMACSHA512 hmacSha512 = new HMACSHA512(pKey.Chaincode))
            {
                i = hmacSha512.ComputeHash(iBuffer.ToArray());
                cc = i.Slice(32);
            }

            return new PublicKey(key, cc);
        }

        private static PrivateKey GetChildKeyDerivation(PrivateKey privateKey, uint index)
        {
            var kl = new byte[32];
            Buffer.BlockCopy(privateKey.Key, 0, kl, 0, 32);
            var kr = new byte[32];
            Buffer.BlockCopy(privateKey.Key, 32, kr, 0, 32);

            var z = new byte[64];
            var zl = new byte[32];
            var zr = new byte[32];
            var i = new byte[64];
            var seri = Bip32Utility.Le32(index);

            BigEndianBuffer zBuffer = new BigEndianBuffer();
            BigEndianBuffer iBuffer = new BigEndianBuffer();
            if (Bip32Utility.FromIndex(index) == DerivationType.HARD)
            {
                zBuffer.Write(new byte[] { 0x00 }); //constant or enum?
                zBuffer.Write(privateKey.Key);
                zBuffer.Write(seri);

                iBuffer.Write(new byte[] { 0x01 }); //constant or enum?
                iBuffer.Write(privateKey.Key);
                iBuffer.Write(seri);
            }
            else
            {
                var pk = privateKey.GetPublicKey(false);
                zBuffer.Write(new byte[] { 0x02 }); //constant or enum?
                zBuffer.Write(pk.Key);
                zBuffer.Write(seri);

                iBuffer.Write(new byte[] { 0x03 }); //constant or enum?
                iBuffer.Write(pk.Key);
                iBuffer.Write(seri);
            }


            using (HMACSHA512 hmacSha512 = new HMACSHA512(privateKey.Chaincode))
            {
                z = hmacSha512.ComputeHash(zBuffer.ToArray());
                zl = z.Slice(0, 32);
                zr = z.Slice(32);
            }

            // left = kl + 8 * trunc28(zl)
            var left = Bip32Utility.Add28Mul8(kl, zl);
            // right = zr + kr
            var right = Bip32Utility.Add256Bits(kr, zr);

            var key = new byte[left.Length + right.Length];
            Buffer.BlockCopy(left, 0, key, 0, left.Length);
            Buffer.BlockCopy(right, 0, key, left.Length, right.Length);

            //chaincode

            byte[] cc;
            using (HMACSHA512 hmacSha512 = new HMACSHA512(privateKey.Chaincode))
            {
                i = hmacSha512.ComputeHash(iBuffer.ToArray());
                cc = i.Slice(32);
            }

            return new PrivateKey(key, cc);
        }
    }
}
