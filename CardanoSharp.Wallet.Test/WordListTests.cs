using CardanoSharp.Wallet.Words;
using System;
using System.Linq;
using Xunit;

namespace CardanoSharp.Wallet.Test
{
    public class WordListTests
    {
        [Fact]
        public void AllWordListsAreValidLength()
        {
            const int WordListLength = 2048;
            Assert.Equal(WordListLength, ChineseSimplified.Words.Length);
            Assert.Equal(WordListLength, ChineseTraditional.Words.Length);
            Assert.Equal(WordListLength, Czech.Words.Length);
            Assert.Equal(WordListLength, English.Words.Length);
            Assert.Equal(WordListLength, French.Words.Length);
            Assert.Equal(WordListLength, Italian.Words.Length);
            Assert.Equal(WordListLength, Japanese.Words.Length);
            Assert.Equal(WordListLength, Korean.Words.Length);
            Assert.Equal(WordListLength, Portuguese.Words.Length);
            Assert.Equal(WordListLength, Spanish.Words.Length);
        }

        [Fact]
        public void EnglishWordListContainsValidWords()
        {
            var wordsToCheck = new[] { "abandon", "lend", "zoo" };

            Assert.True(wordsToCheck.All(wordToCheck => English.Words.Contains(wordToCheck)));
        }

        [Fact]
        public void ChineseSimplifiedWordListContainsValidWords()
        {
            var wordsToCheck = new[] { "的", "竟", "歇" };

            Assert.True(wordsToCheck.All(wordToCheck => ChineseSimplified.Words.Contains(wordToCheck)));
        }

        [Fact]
        public void ChineseTraditionalWordListContainsValidWords()
        {
            var wordsToCheck = new[] { "的", "竟", "歇" };

            Assert.True(wordsToCheck.All(wordToCheck => ChineseTraditional.Words.Contains(wordToCheck)));
        }

        [Fact]
        public void CzechWordListContainsValidWords()
        {
            var wordsToCheck = new[] { "abdikace", "obsah", "zvyk" };

            Assert.True(wordsToCheck.All(wordToCheck => Czech.Words.Contains(wordToCheck)));
        }

        [Fact]
        public void FrenchWordListContainsValidWords()
        {
            var wordsToCheck = new[] { "abaisser", "incarner", "zoologie" };

            Assert.True(wordsToCheck.All(wordToCheck => French.Words.Contains(wordToCheck)));
        }

        [Fact]
        public void GermanWordListContainsValidWords()
        {
            var wordsToCheck = new[] { "abbau", "konkurs", "zyklus" };

            Assert.True(wordsToCheck.All(wordToCheck => German.Words.Contains(wordToCheck)));
        }

        [Fact]
        public void ItalianWordListContainsValidWords()
        {
            var wordsToCheck = new[] { "abaco", "mirino", "zuppa" };

            Assert.True(wordsToCheck.All(wordToCheck => Italian.Words.Contains(wordToCheck)));
        }

        [Fact]
        public void JapaneseWordListContainsValidWords()
        {
            var wordsToCheck = new[] { "あいこくしん", "そっけつ", "われる" };

            Assert.True(wordsToCheck.All(wordToCheck => Japanese.Words.Contains(wordToCheck)));
        }

        [Fact]
        public void KoreanWordListContainsValidWords()
        {
            var wordsToCheck = new[] { "가격", "실체", "힘껏" };

            Assert.True(wordsToCheck.All(wordToCheck => Korean.Words.Contains(wordToCheck)));
        }

        [Fact]
        public void PortugueseWordListContainsValidWords()
        {
            var wordsToCheck = new[] { "abacate", "implante", "zumbido" };

            Assert.True(wordsToCheck.All(wordToCheck => Portuguese.Words.Contains(wordToCheck)));
        }

        [Fact]
        public void SpanishWordListContainsValidWords()
        {
            var wordsToCheck = new[] { "ábaco", "limpio", "zurdo" };

            Assert.True(wordsToCheck.All(wordToCheck => Spanish.Words.Contains(wordToCheck)));
        }
    }
}
