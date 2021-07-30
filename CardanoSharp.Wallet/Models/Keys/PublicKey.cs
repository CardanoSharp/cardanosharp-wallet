using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.Models.Keys
{
    public record PublicKey
    {
        public byte[] Key { get; set; }
        public byte[] ChainCode { get; set; }

        public PublicKey(byte[] key, byte[] chaincode)
        {
            Key = key;
            ChainCode = chaincode;
        }
    }
}
