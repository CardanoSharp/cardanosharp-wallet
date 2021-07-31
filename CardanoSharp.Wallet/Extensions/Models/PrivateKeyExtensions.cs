using CardanoSharp.Wallet.Common;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Models.Keys;
using CardanoSharp.Wallet.Utilities;
using Chaos.NaCl;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace CardanoSharp.Wallet.Extensions.Models
{
    public static class PrivateKeyExtensions
    {
        static UInt32 MinHardIndex = 0x80000000;

        public static PrivateKey Encrypt(this PrivateKey skey, string password)
        {
            return new PrivateKey(skey.Key.Encrypt(password), skey.Chaincode.Encrypt(password));
        }

        public static PrivateKey Decrypt(this PrivateKey key, string password)
        {
            return new PrivateKey(key.Key.Decrypt(password), key.Chaincode.Decrypt(password));
        }       
        
        public static PublicKey GetPublicKey(this PrivateKey privateKey, bool withZeroByte = true)
        {
            var sk = new byte[privateKey.Key.Length];
            Buffer.BlockCopy(privateKey.Key, 0, sk, 0, privateKey.Key.Length);
            var pk = Ed25519.GetPublicKey(sk);

            var zero = new byte[] { 0 };

            var buffer = new BigEndianBuffer();
            if (withZeroByte)
                buffer.Write(zero);

            buffer.Write(pk);

            return new PublicKey(
                buffer.ToArray(),
                privateKey.Chaincode);
        }

        public static PrivateKey Derive(this PrivateKey privateKey, string path)
        {
            if (!Bip32Utility.IsValidPath(path))
                throw new FormatException("Invalid derivation path");

            var segments = path
                .Split('/');

            if (segments[0] == "m") segments = segments.Slice(1);

            PrivateKey newPrivateKey = new PrivateKey(privateKey.Key, privateKey.Chaincode);
            foreach (var segment in segments)
            {
                var isHardened = segment.Contains("'");
                var index = Convert.ToUInt32(segment.Replace("'", ""));

                if (isHardened) index += MinHardIndex;

                newPrivateKey = GetChildKeyDerivation(newPrivateKey, index);
            }

            return newPrivateKey;
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
