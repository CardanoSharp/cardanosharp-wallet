using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.Models.Keys
{
    public class Mnemonic
    {
        public string Words { get; }
        public byte[] Entropy { get; }

        public Mnemonic(string words, byte[] entropy)
        {
            Words = words;
            Entropy = entropy;
        }
    }
}
