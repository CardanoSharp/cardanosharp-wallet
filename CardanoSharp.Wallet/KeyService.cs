using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Security.Cryptography;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Extensions;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text.RegularExpressions;
using CardanoSharp.Wallet.Common;
using Chaos.NaCl;

namespace CardanoSharp.Wallet
{
    public interface IKeyService
    {
        string Generate(int size, WordLists wl = WordLists.English);
        byte[] Restore(string mnemonic, WordLists wl = WordLists.English);
        (byte[], byte[]) GetRootKey(byte[] entropy, string password = "");
        byte[] GetPublicKey(byte[] privateKey, bool withZeroByte = true);
        (byte[], byte[]) DerivePath(string path, byte[] key, byte[] chainCode);
    }

    public class KeyService : IKeyService
    {
        #region BIP39
        private readonly int[] allowedEntropyLengths = { 12, 16, 20, 24, 28, 32 };
        private static readonly int[] allowedWordLengths = { 9, 12, 15, 18, 21, 24 };
        private uint[] wordIndexes;
        private string[] allWords;

        public string Generate(int wordSize, WordLists wl = WordLists.English)
        {
            if (!allowedWordLengths.Contains(wordSize))
                throw new ArgumentOutOfRangeException(nameof(wordSize), $"{nameof(wordSize)} must be one of the following values ({string.Join(", ", allowedWordLengths)})");

            var entropySize = allowedEntropyLengths[Array.FindIndex(allowedWordLengths, x => x == wordSize)];
            if (!allowedEntropyLengths.Contains(entropySize))
                throw new ArgumentOutOfRangeException(nameof(entropySize), $"Derived entropy {entropySize} is not within the allowed values ({string.Join(", ", allowedEntropyLengths)})");

            var rng = new RNGCryptoServiceProvider();
            if (rng is null)
                throw new ArgumentNullException(nameof(rng), "Random number generator cannot be null.");

            allWords = GetAllWords(wl);

            var entropy = new byte[entropySize];
            rng.GetBytes(entropy);
            SetWordsFromEntropy(entropy);
            return GetMnemonic();
        }

        public byte[] Restore(string mnemonic, WordLists wl = WordLists.English)
        {
            if (string.IsNullOrWhiteSpace(mnemonic))
                throw new ArgumentNullException(nameof(mnemonic), "Seed can not be null or empty!");
            allWords = GetAllWords(wl);

            string[] words = mnemonic.Normalize(NormalizationForm.FormKD)
                                     .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (!words.All(x => allWords.Contains(x)))
            {
                throw new ArgumentException(nameof(mnemonic), "Seed has invalid words.");
            }
            if (!allowedWordLengths.Contains(words.Length))
            {
                throw new FormatException($"Invalid seed length. It must be one of the following values ({string.Join(", ", allowedWordLengths)})");
            }

            wordIndexes = new uint[words.Length];
            for (int i = 0; i < words.Length; i++)
            {
                wordIndexes[i] = (uint)Array.IndexOf(allWords, words[i]);
            }

            // Compute and check checksum
            int MS = words.Length;
            int ENTCS = MS * 11;
            int CS = ENTCS % 32;
            int ENT = ENTCS - CS;

            var entropy = new byte[ENT / 8];

            int itemIndex = 0;
            int bitIndex = 0;
            // Number of bits in a word
            int toTake = 8;
            // Indexes are held in a UInt32 but they are only 11 bits
            int maxBits = 11;
            for (int i = 0; i < entropy.Length; i++)
            {
                if (bitIndex + toTake <= maxBits)
                {
                    // All 8 bits are in one item

                    // To take 8 bits (*) out of 00000000 00000000 00000xx* *******x:
                    // 1. Shift right to get rid of extra bits on right, then cast to byte to get rid of the rest
                    // >> maxBits - toTake - bitIndex
                    entropy[i] = (byte)(wordIndexes[itemIndex] >> (3 - bitIndex));
                }
                else
                {
                    // Only a part of 8 bits are in this item, the rest is in the next.
                    // Since items are only 32 bits there is no other possibility (8<32)

                    // To take 8 bits(*) out of [00000000 00000000 00000xxx xxxx****] [00000000 00000000 00000*** *xxxxxxx]:
                    // Take first item at itemIndex [00000000 00000000 00000xxx xxxx****]: 
                    //    * At most 7 bits and at least 1 bit should be taken
                    // 1. Shift left [00000000 00000000 0xxxxxxx ****0000] (<< 8 - (maxBits - bitIndex)) 8-max+bi
                    // 2. Zero the rest of the bits (& (00000000 00000000 00000000 11111111))

                    // Take next item at itemIndex+1 [00000000 00000000 00000*** *xxxxxxx]
                    // 3. Shift right [00000000 00000000 00000000 0000****]
                    // number of bits already taken = maxBits - bitIndex
                    // nuber of bits to take = toTake - (maxBits - bitIndex)
                    // Number of bits on the right to get rid of= maxBits - (toTake - (maxBits - bitIndex))
                    // 4. Add two values to each other using bitwise OR [****0000] | [0000****]
                    entropy[i] = (byte)(((wordIndexes[itemIndex] << (bitIndex - 3)) & 0xff) |
                                         (wordIndexes[itemIndex + 1] >> (14 - bitIndex)));
                }

                bitIndex += toTake;
                if (bitIndex >= maxBits)
                {
                    bitIndex -= maxBits;
                    itemIndex++;
                }
            }

            // Compute and compare checksum:
            // CS is at most 8 bits and it is the remaining bits from the loop above and it is only from last item
            // [00000000 00000000 00000xxx xxxx****]
            // We already know the number of bits here: CS
            // A simple & does the work
            uint mask = (1U << CS) - 1;
            byte expectedChecksum = (byte)(wordIndexes[itemIndex] & mask);

            // Checksum is the "first" CS bits of hash: [****xxxx]

            using SHA256 hash = SHA256.Create();
            byte[] hashOfEntropy = hash.ComputeHash(entropy);
            byte actualChecksum = (byte)(hashOfEntropy[0] >> (8 - CS));

            if (expectedChecksum != actualChecksum)
            {
                Array.Clear(wordIndexes, 0, wordIndexes.Length);
                wordIndexes = null;

                throw new FormatException("Wrong checksum.");
            }

            return entropy;
        }

        private string GetMnemonic()
        {
            StringBuilder sb = new StringBuilder(wordIndexes.Length * 8);
            for (int i = 0; i < wordIndexes.Length; i++)
            {
                sb.Append($"{allWords[wordIndexes[i]]} ");
            }

            // no space at the end.
            sb.Length--;
            return sb.ToString();
        }

        private static string[] GetAllWords(WordLists wl)
        {
            if (!Enum.IsDefined(typeof(WordLists), wl))
                throw new ArgumentException("Given word list is not defined.");

            string path = $"CardanoSharp.Wallet.Words.{wl}.txt";
            Assembly asm = Assembly.GetExecutingAssembly();
            using Stream stream = asm.GetManifestResourceStream(path);
            if (stream != null)
            {
                using StreamReader reader = new StreamReader(stream);
                int i = 0;
                const int allWordsLength = 2048;
                string[] result = new string[allWordsLength];
                while (!reader.EndOfStream)
                {
                    result[i++] = reader.ReadLine();
                }
                if (i != 2048)
                {
                    throw new ArgumentException($"Embedded word list has {i} words instead of {allWordsLength}.");
                }

                return result;
            }
            else
            {
                throw new ArgumentException("Word list was not found.");
            }
        }

        private void SetWordsFromEntropy(byte[] entropy)
        {
            using SHA256 hash = SHA256.Create();
            byte[] hashOfEntropy = hash.ComputeHash(entropy);

            int ENT = entropy.Length * 8;
            int CS = ENT / 32;
            int ENTCS = ENT + CS;
            int MS = ENTCS / 11;

            // To convert a given entropy to mnemonic (word list) it must be converted to binary and then
            // split into 11-bit chunks each representing an index inside the list of all words (2048 total).
            // Here we use a UInt32 array to hold the bits and select each 11 bits from that array

            // To make the entropy length divisible by 11 it needs to be padded with a checksum of CS bits first
            // Extra bytes are added to make conversion and selection easier and the extra bits will be ignored in final
            // selection step.
            int arrSize = (int)Math.Ceiling((double)ENTCS / 32);
            int fillingBytes = (arrSize * 4) - entropy.Length;
            byte[] ba = entropy.ConcatFast(hashOfEntropy.SubArray(0, fillingBytes));

            uint[] bits = new uint[arrSize];
            for (int i = 0, j = 0; i < ba.Length; i += 4, j++)
            {
                bits[j] = (uint)(ba[i + 3] | (ba[i + 2] << 8) | (ba[i + 1] << 16) | (ba[i] << 24));
            }

            int itemIndex = 0;
            int bitIndex = 0;
            // Number of bits in a word
            int toTake = 11;
            // UInt32 is 32 bit!
            int maxBits = 32;
            wordIndexes = new uint[MS];
            for (int i = 0; i < MS; i++)
            {
                if (bitIndex + toTake <= maxBits)
                {
                    // All 11 bits are in one item

                    // To take astrix out of xx***xxx:
                    // 1. Shift left bitIndex times to get rid of values on the left: ***xxx00 (<< bitIndex)
                    // 2. Shift right the same amount to put bits back where they were: 00***xxx (>> bitIndex)
                    // 3. Shift right to get rid of the extra values on the right: 00000*** (>> maxBits - (bitIndex + toTake))
                    // 2+3= bitIndex + maxBits - bitIndex - toTake
                    wordIndexes[i] = (bits[itemIndex] << bitIndex) >> (maxBits - toTake);
                }
                else
                {
                    // Only a part of 11 bits are in this item, the rest is in the next.
                    // Since items are only 32 bits there is no other possibility (11<32)

                    // To take astrix out of [xxxxxx**] [***xxxxx]:
                    // Take first item at itemIndex [xxxxxx**]: 
                    // 1. Shift left bitIndex times to to get rid of values on the right: **000000 (<< bitIndex)
                    // 2. Shift right the same amount to put bits back where they were: 000000** (>> bitIndex)
                    // 3. Shift left to open up room for remaining bits: 000**000 (<< toTake - (maxBits - bitIndex))
                    // 2+3= bitIndex - toTake + maxBits - bitIndex

                    // Take next item at itemIndex+1 [***xxxxx]:
                    // 4. Shift right to get rid of extra values: 00000***
                    // Number of bits already taken= maxBits - bitIndex
                    // Number of bits to take = toTake - (maxBits - bitIndex)
                    // Number of bits on the right to get rid of= maxBits - (toTake - (maxBits - bitIndex))

                    // 5. Add two values to each other using bitwise OR (000**000 | 00000*** = 000*****)
                    wordIndexes[i] = ((bits[itemIndex] << bitIndex) >> (maxBits - toTake)) |
                                     (bits[itemIndex + 1] >> (maxBits - toTake + maxBits - bitIndex));
                }

                bitIndex += toTake;
                if (bitIndex >= maxBits)
                {
                    bitIndex -= maxBits;
                    itemIndex++;
                }
            }
        }
        #endregion

        #region BIP32
        static UInt32 MinHardIndex = 0x80000000;
        public (byte[], byte[]) GetRootKey(byte[] entropy, string password = "")
        {
            //GroupElementP3 A;
            var rootKey = KeyDerivation.Pbkdf2(password, entropy, KeyDerivationPrf.HMACSHA512, 4096, 96);
            rootKey[0] &= 248;
            rootKey[31] &= 31;
            rootKey[31] |= 64;

            return (rootKey.Slice(0, 64), rootKey.Slice(64));
        }

        public byte[] GetPublicKey(byte[] privateKey, bool withZeroByte = true)
        {
            var sk = new byte[privateKey.Length];
            Buffer.BlockCopy(privateKey, 0, sk, 0, privateKey.Length);
            var publicKey = Ed25519.GetPublicKey(sk);

            var zero = new byte[] { 0 };

            var buffer = new BigEndianBuffer();
            if (withZeroByte)
                buffer.Write(zero);

            buffer.Write(publicKey);

            return buffer.ToArray();
        }

        public (byte[], byte[]) DerivePath(string path, byte[] key, byte[] chainCode)
        {
            if (!IsValidPath(path))
                throw new FormatException("Invalid derivation path");

            var segments = path
                .Split('/');

            if (segments[0] == "m") segments = segments.Slice(1);

            foreach (var segment in segments)
            {
                var isHardened = segment.Contains("'");
                var index = Convert.ToUInt32(segment.Replace("'", ""));

                if (isHardened) index += MinHardIndex;

                byte[] z, cc;
                if (key.Length == 64)
                    (z, cc) = GetChildPrivateKeyDerivation(key, chainCode, index);
                else
                    (z, cc) = GetChildPublicKeyDerivation(key, chainCode, index);

                chainCode = cc;
                key = z;
            }

            return (key, chainCode);
        }

        private (byte[], byte[]) GetChildPublicKeyDerivation(byte[] pk, byte[] chainCode, uint index)
        {
            var z = new byte[64];
            var zl = new byte[32];
            var zr = new byte[32];
            var i = new byte[64];
            var seri = le32(index);

            BigEndianBuffer zBuffer = new BigEndianBuffer();
            BigEndianBuffer iBuffer = new BigEndianBuffer();
            if (fromIndex(index) == DerivationType.HARD)
            {
                throw new Exception("Hard derivation is now allowed");
            }
            else
            {
                zBuffer.Write(new byte[] { 0x02 });
                zBuffer.Write(pk);
                zBuffer.Write(seri);

                iBuffer.Write(new byte[] { 0x03 });
                iBuffer.Write(pk);
                iBuffer.Write(seri);
            }

            using (HMACSHA512 hmacSha512 = new HMACSHA512(chainCode))
            {
                z = hmacSha512.ComputeHash(zBuffer.ToArray());
                zl = z.Slice(0, 32);
                zr = z.Slice(32);
            }

            // left = kl + 8 * trunc28(zl)
            var key = Ed25519.PointPlus(pk, point_of_trunc28_mul8(zl));

            byte[] cc;
            using (HMACSHA512 hmacSha512 = new HMACSHA512(chainCode))
            {
                i = hmacSha512.ComputeHash(iBuffer.ToArray());
                cc = i.Slice(32);
            }

            return (key, cc);
        }

        private (byte[], byte[]) GetChildPrivateKeyDerivation(byte[] ekey, byte[] chainCode, uint index)
        {
            var kl = new byte[32];
            Buffer.BlockCopy(ekey, 0, kl, 0, 32);
            var kr = new byte[32];
            Buffer.BlockCopy(ekey, 32, kr, 0, 32);

            var z = new byte[64];
            var zl = new byte[32];
            var zr = new byte[32];
            var i = new byte[64];
            var seri = le32(index);

            BigEndianBuffer zBuffer = new BigEndianBuffer();
            BigEndianBuffer iBuffer = new BigEndianBuffer();
            if (fromIndex(index) == DerivationType.HARD)
            {
                zBuffer.Write(new byte[] { 0x00 });
                zBuffer.Write(ekey);
                zBuffer.Write(seri);

                iBuffer.Write(new byte[] { 0x01 });
                iBuffer.Write(ekey);
                iBuffer.Write(seri);
            }
            else
            {
                var pk = GetPublicKey(ekey, false);
                zBuffer.Write(new byte[] { 0x02 });
                zBuffer.Write(pk);
                zBuffer.Write(seri);

                iBuffer.Write(new byte[] { 0x03 });
                iBuffer.Write(pk);
                iBuffer.Write(seri);
            }


            using (HMACSHA512 hmacSha512 = new HMACSHA512(chainCode))
            {
                z = hmacSha512.ComputeHash(zBuffer.ToArray());
                zl = z.Slice(0, 32);
                zr = z.Slice(32);
            }

            // left = kl + 8 * trunc28(zl)
            var left = add_28_mul8(kl, zl);
            // right = zr + kr
            var right = add_256bits(kr, zr);

            var key = new byte[left.Length + right.Length];
            Buffer.BlockCopy(left, 0, key, 0, left.Length);
            Buffer.BlockCopy(right, 0, key, left.Length, right.Length);

            //chaincode

            byte[] cc;
            using (HMACSHA512 hmacSha512 = new HMACSHA512(chainCode))
            {
                i = hmacSha512.ComputeHash(iBuffer.ToArray());
                cc = i.Slice(32);
            }

            return (key, cc);
        }

        private bool IsValidPath(string path)
        {
            var regex = new Regex("^m(\\/[0-9]+')+$");

            // if (!regex.IsMatch(path))
            //     return false;

            var valid = !(path.Split('/')
                .Slice(1)
                .Select(a => a.Replace("'", ""))
                .Any(a => !Int32.TryParse(a, out _)));

            return valid;
        }

        private byte[] add_28_mul8(byte[] x, byte[] y)
        {
            if (x.Length != 32) throw new Exception("x is incorrect length");
            if (y.Length != 32) throw new Exception("y is incorrect length");

            ushort carry = 0;
            var res = new byte[32];

            for(var i = 0; i < 28; i++)
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

        private byte[] add_256bits(byte[] x, byte[] y)
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

        private byte[] point_of_trunc28_mul8(byte[] sk) {
            var kl = new byte[32];
            var copy = add_28_mul8(kl, sk);
            return Ed25519.GetPublicKey(copy);
        }

        private DerivationType fromIndex(uint index) =>
            index >= 0x80000000
                ? DerivationType.HARD 
                : DerivationType.SOFT;

        private byte[] le32(uint i) =>
            new byte[] { (byte)i, (byte)(i >> 8), (byte)(i >> 16), (byte)(i >> 24) };
        #endregion

    }
}
