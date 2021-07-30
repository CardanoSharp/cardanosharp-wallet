﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.Models.Keys
{
    public class PublicKey
    {
        public PublicKey(byte[] key, byte[] chaincode)
        {
            Key = key;
            Chaincode = chaincode;
        }

        public byte[] Key { get; set; }
        public byte[] Chaincode { get; set; }
    }
}