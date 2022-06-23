using System;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Models.Keys;
using CardanoSharp.Wallet.Words;

namespace CardanoSharp.Wallet
{
    public interface IMnemonicService
    {
        Mnemonic Generate(int size, WordLists wl = WordLists.English);
        Mnemonic Restore(string mnemonic, WordLists wl = WordLists.English);
    }

    public class MnemonicService : IMnemonicService
    {
        #region BIP39
        private readonly int[] allowedEntropyLengths = { 12, 16, 20, 24, 28, 32 };
        private static readonly int[] allowedWordLengths = { 9, 12, 15, 18, 21, 24 };
        private const int allWordsLength = 2048;

        public Mnemonic Generate(int wordSize, WordLists wl = WordLists.English)
        {
            if (!allowedWordLengths.Contains(wordSize))
                throw new ArgumentOutOfRangeException(nameof(wordSize), $"{nameof(wordSize)} must be one of the following values ({string.Join(", ", allowedWordLengths)})");

            var entropySize = allowedEntropyLengths[Array.FindIndex(allowedWordLengths, x => x == wordSize)];
            if (!allowedEntropyLengths.Contains(entropySize))
                throw new ArgumentOutOfRangeException(nameof(entropySize), $"Derived entropy {entropySize} is not within the allowed values ({string.Join(", ", allowedEntropyLengths)})");

            var allWords = GetAllWords(wl);

            var entropy = new byte[entropySize];
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(entropy);
            return CreateMnemonicFromEntropy(entropy, allWords);
        }

        public Mnemonic Restore(string words, WordLists wl = WordLists.English)
        {
            if (string.IsNullOrWhiteSpace(words))
                throw new ArgumentNullException(nameof(words), "Seed can not be null or empty!");
            var allWords = GetAllWords(wl);

            string[] wordArr = words.Normalize(NormalizationForm.FormKD)
                                     .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (!wordArr.All(x => allWords.Contains(x)))
            {
                throw new ArgumentException(nameof(wordArr), "Seed has invalid words.");
            }
            if (!allowedWordLengths.Contains(wordArr.Length))
            {
                throw new FormatException($"Invalid seed length. It must be one of the following values ({string.Join(", ", allowedWordLengths)})");
            }

            var wordIndexes = new uint[wordArr.Length];
            for (int i = 0; i < wordArr.Length; i++)
            {
                wordIndexes[i] = (uint)Array.IndexOf(allWords, wordArr[i]);
            }

            // Compute and check checksum
            int MS = wordArr.Length;
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

            return new Mnemonic(words, entropy);
        }

        private static string[] GetAllWords(WordLists wl) =>
            wl switch
            {
                WordLists.ChineseSimplified => ChineseSimplified.Words,
                WordLists.ChineseTraditional => ChineseTraditional.Words,
                WordLists.Czech => Czech.Words,
                WordLists.English => English.Words,
                WordLists.French => French.Words,
                WordLists.German => German.Words,
                WordLists.Italian => Italian.Words,
                WordLists.Japanese => Japanese.Words,
                WordLists.Korean => Korean.Words,
                WordLists.Portuguese => Portuguese.Words,
                WordLists.Spanish => Spanish.Words,
                _ => throw new NotImplementedException($"Invalid wordlist {wl}"),
            };

        private Mnemonic CreateMnemonicFromEntropy(byte[] entropy, string[] allWords)
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
            var wordIndexes = new uint[MS];
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

            StringBuilder sb = new StringBuilder(wordIndexes.Length * 8);
            for (int i = 0; i < wordIndexes.Length; i++)
            {
                sb.Append($"{allWords[wordIndexes[i]]} ");
            }

            // no space at the end.
            sb.Length--;
            return new Mnemonic(sb.ToString(), entropy);
        }
        #endregion
    }
}
