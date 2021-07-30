using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Extensions;
using Chaos.NaCl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

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
    }
}
