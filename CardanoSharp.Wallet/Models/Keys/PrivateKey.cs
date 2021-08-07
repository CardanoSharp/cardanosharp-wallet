using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.Models.Keys
{
    public class PrivateKey
    {
        public byte[] Key { get; }
        public byte[] Chaincode { get; }

        public PrivateKey(byte[] key)
        {
            Key = key;
        }

        public PrivateKey(byte[] key, byte[] chaincode)
        {
            Key = key;
            Chaincode = chaincode;
        }
    }
}
