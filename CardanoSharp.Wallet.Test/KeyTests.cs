using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Models.Keys;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace CardanoSharp.Wallet.Test
{
    public class KeyTests
    {
        private const string _password = "password";
        private readonly IKeyService _keyService;

        public KeyTests()
        {
            _keyService = new KeyService();
        }

        [Theory]
        [InlineData(9)]
        [InlineData(12)]
        [InlineData(15)]
        [InlineData(18)]
        [InlineData(21)]
        [InlineData(24)]
        public void Generates_Mnemonic_Of_Correct_Word_Count(int wordCount)
        {
            var mnemonic = _keyService.Generate(wordCount);

            Assert.Equal(wordCount, mnemonic.Words.Split(' ').Length);
        }

        [Theory]
        [InlineData(
            "elder lottery unlock common assume beauty grant curtain various horn spot youth exclude rude boost fence used two spawn toddler soup awake across use", 
            "475083b81730de275969b1f18db34b7fb4ef79c66aa8efdd7742f1bcfe204097")]
        public void Restores_Mnemonic_Of_Correct_Entropy(string words, string expectedEntropyHexString)
        {
            var mnemonic = _keyService.Restore(words);

            Assert.Equal(mnemonic.Entropy.ToStringHex(), expectedEntropyHexString);
        }

        [Theory]
        [InlineData(
            "art forum devote street sure rather head chuckle guard poverty release quote oak craft enemy",
            "b8f2bece9bdfe2b0282f5bad705562ac996efb6af96b648f4445ec44f47ad95c10e3d72f26ed075422a36ed8585c745a0e1150bcceba2357d058636991f38a3791e248de509c070d812ab2fda57860ac876bc489192c1ef4ce253c197ee219a4")]
        public void RestoreTest_PrivateKey_withChainCode(string words, string expectedPrivateKey)
        {
            var mnemonic = _keyService.Restore(words);
            var rootKey = mnemonic.GetRootKey();

            Assert.Equal(rootKey.Key.ToStringHex(), expectedPrivateKey.Substring(0, 128));
            Assert.Equal(rootKey.Chaincode.ToStringHex(), expectedPrivateKey.Substring(128));
        }

        [Theory]
        [InlineData(
            "art forum devote street sure rather head chuckle guard poverty release quote oak craft enemy",
            "b8f2bece9bdfe2b0282f5bad705562ac996efb6af96b648f4445ec44f47ad95c10e3d72f26ed075422a36ed8585c745a0e1150bcceba2357d058636991f38a3791e248de509c070d812ab2fda57860ac876bc489192c1ef4ce253c197ee219a4")]
        public void Enrypt_PrivateKey_withChainCode(string words, string expectedPrivateKey)
        {
            var password = _password;
            var mnemonic = _keyService.Restore(words);
            var rootKey = mnemonic.GetRootKey();

            var json = JsonSerializer.Serialize(rootKey.Encrypt(password));
            var load = JsonSerializer.Deserialize<PrivateKey>(json);

            var loadKey = load.Decrypt(password);

            Assert.Equal(rootKey.Key, loadKey.Key);
            Assert.Equal(rootKey.Chaincode, loadKey.Chaincode);

            Assert.Equal(loadKey.Key.ToStringHex(), expectedPrivateKey.Substring(0, 128));
            Assert.Equal(loadKey.Chaincode.ToStringHex(), expectedPrivateKey.Substring(128));
        }

        [Fact]
        public async Task Handles_Concurrency_For_A_Shared_KeyService_Instance()
        {
            var random = new Random();
            var mnemonicWordLengths = new[] { 9, 12, 15, 18, 21, 24 };
            var mnemonicWords = new[]
            {
                "elder lottery unlock common assume beauty grant curtain various horn spot youth exclude rude boost fence used two spawn toddler soup awake across use",
                "art forum devote street sure rather head chuckle guard poverty release quote oak craft enemy",
            };

            (int ExpectedWordLength, int ResultWordLength) GenerateRandomMnemonic()
            {
                var wordLength = mnemonicWordLengths[random.Next(mnemonicWordLengths.Length)];
                var mnemonic = _keyService.Generate(wordLength, Enums.WordLists.Spanish);
                return (ExpectedWordLength: wordLength, ResultWordLength: mnemonic.Words.Split(' ').Length);
            }

            (int ExpectedWordLength, int ResultWordLength) RestoreMnemonic()
            {
                var words = mnemonicWords[random.Next(mnemonicWords.Length)];
                var mnemonic = _keyService.Restore(words);
                return (ExpectedWordLength: words.Split(' ').Length, ResultWordLength: mnemonic.Words.Split(' ').Length);
            }

            var concurrentTasks = new List<Task<(int ExpectedWordLength, int ResultWordLength)>>();
            for (var i = 0; i < 15; i++)
            {
                concurrentTasks.Add(Task.Run(RestoreMnemonic));
                concurrentTasks.Add(Task.Run(GenerateRandomMnemonic));
            }

            var completedTasks = await Task.WhenAll(concurrentTasks);
            foreach(var completedTask in completedTasks)
            {
                Assert.Equal(completedTask.ExpectedWordLength, completedTask.ResultWordLength);
            }
        }
    }
}
