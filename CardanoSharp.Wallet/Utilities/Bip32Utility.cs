using CardanoSharp.Wallet.Common;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Models.Keys;
using Chaos.NaCl;
using System;
using System.Linq;
using System.Security.Cryptography;

namespace CardanoSharp.Wallet.Utilities
{
    //this might need a better name
    public static class Bip32Utility
    {
        public static byte[] Add28Mul8(byte[] x, byte[] y)
        {
            if (x.Length != 32) throw new Exception("x is incorrect length");
            if (y.Length != 32) throw new Exception("y is incorrect length");

            ushort carry = 0;
            var res = new byte[32];

            for (var i = 0; i < 28; i++)
            {
                var r = (ushort)x[i] + (((ushort)y[i]) << 3) + carry;
                res[i] = (byte)(r & 0xff);
                carry = (ushort)(r >> 8);
            }

            for (var j = 28; j < 32; j++)
            {
                var r = (ushort)x[j] + carry;
                res[j] = (byte)(r & 0xff);
                carry = (ushort)(r >> 8);
            }

            return res;
        }

        public static byte[] Add256Bits(byte[] x, byte[] y)
        {
            if (x.Length != 32) throw new Exception("x is incorrect length");
            if (y.Length != 32) throw new Exception("y is incorrect length");

            ushort carry = 0;
            var res = new byte[32];

            for (var i = 0; i < 32; i++)
            {
                var r = (ushort)x[i] + (ushort)y[i] + carry;
                res[i] = (byte)(r);
                carry = (ushort)(r >> 8);
            }

            return res;
        }

        public static byte[] PointOfTrunc28Mul8(byte[] sk)
        {
            var kl = new byte[32];
            var copy = Bip32Utility.Add28Mul8(kl, sk);
            return Ed25519.GetPublicKey(copy);
        }

        public static byte[] Le32(uint i) =>
            new byte[] 
            { 
                (byte)i, 
                (byte)(i >> 8), 
                (byte)(i >> 16), 
                (byte)(i >> 24) 
            };

        public static bool IsValidPath(string path) => 
            !(path.Split('/')
                .Slice(1)
                .Select(a => a.Replace("'", ""))
                .Any(a => !Int32.TryParse(a, out _)));

        public static DerivationType FromIndex(uint index) =>
            index >= 0x80000000
                ? DerivationType.HARD
                : DerivationType.SOFT;

        public static PrivateKey GetChildKeyDerivation(PrivateKey privateKey, uint index)
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

        public static PublicKey GetChildKeyDerivation(PublicKey pKey, uint index)
        {
            var z = new byte[64];
            var zl = new byte[32];
            var zr = new byte[32];
            var i = new byte[64];
            var seri = Bip32Utility.Le32(index);

            BigEndianBuffer zBuffer = new BigEndianBuffer();
            BigEndianBuffer iBuffer = new BigEndianBuffer();

            zBuffer.Write(new byte[] { 0x02 });
            zBuffer.Write(pKey.Key);
            zBuffer.Write(seri);

            iBuffer.Write(new byte[] { 0x03 });
            iBuffer.Write(pKey.Key);
            iBuffer.Write(seri);


            using (HMACSHA512 hmacSha512 = new HMACSHA512(pKey.Chaincode))
            {
                z = hmacSha512.ComputeHash(zBuffer.ToArray());
                zl = z.Slice(0, 32);
                zr = z.Slice(32);
            }

            // left = kl + 8 * trunc28(zl)
            var key = Ed25519.PointPlus(pKey.Key, Bip32Utility.PointOfTrunc28Mul8(zl));

            byte[] cc;
            using (HMACSHA512 hmacSha512 = new HMACSHA512(pKey.Chaincode))
            {
                i = hmacSha512.ComputeHash(iBuffer.ToArray());
                cc = i.Slice(32);
            }

            return new PublicKey(key, cc);
        }
    }
}
