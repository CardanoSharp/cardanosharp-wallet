using CardanoSharp.Wallet.Common;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Models.Keys;
using CardanoSharp.Wallet.Utilities;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace CardanoSharp.Wallet.Extensions.Models
{
    public static class PublicKeyExtensions
    {
        public static PublicKey Derive(this PublicKey publicKey, string path)
        {
            if (!Bip32Utility.IsValidPath(path))
                throw new FormatException("Invalid derivation path");

            var segments = path
                .Split('/');

            //if (segments[0] == "m") segments = segments.Slice(1);

            PublicKey newpublicKey = new PublicKey(publicKey.Key, publicKey.Chaincode);
            foreach (var segment in segments)
            {
                if (segment.Contains("'"))
                    throw new Exception("Public Keys cannot derive hardened paths");

                var index = Convert.ToUInt32(segment);

                newpublicKey = GetChildKeyDerivation(newpublicKey, index);
            }

            return newpublicKey;
        }

        private static PublicKey GetChildKeyDerivation(PublicKey publicKey, uint index)
        {
            var kl = new byte[32];
            Buffer.BlockCopy(publicKey.Key, 0, kl, 0, 32);
            var kr = new byte[32];
            Buffer.BlockCopy(publicKey.Key, 32, kr, 0, 32);

            var z = new byte[64];
            var zl = new byte[32];
            var zr = new byte[32];
            var i = new byte[64];
            var seri = Bip32Utility.Le32(index);

            BigEndianBuffer zBuffer = new BigEndianBuffer();
            BigEndianBuffer iBuffer = new BigEndianBuffer();

            zBuffer.Write(new byte[] { 0x02 }); //constant or enum?
            zBuffer.Write(publicKey.Key);
            zBuffer.Write(seri);

            iBuffer.Write(new byte[] { 0x03 }); //constant or enum?
            iBuffer.Write(publicKey.Key);
            iBuffer.Write(seri);
            


            using (HMACSHA512 hmacSha512 = new HMACSHA512(publicKey.Chaincode))
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
            using (HMACSHA512 hmacSha512 = new HMACSHA512(publicKey.Chaincode))
            {
                i = hmacSha512.ComputeHash(iBuffer.ToArray());
                cc = i.Slice(32);
            }

            return new PublicKey(key, cc);
        }
    }
}
