using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.Models.Keys
{
    public record PrivateKey
    {
        public byte[] Key { get; set; }
        public byte[] ChainCode { get; set; }

        public PrivateKey(byte[] key, byte[] chaincode)
        {
            Key = key;
            ChainCode = chaincode;
        }
    }
}
