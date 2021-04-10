using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.Models
{
    public class PublicKey
    {
        public byte[] Key { get; set; }
        public byte[] Chaincode { get; set; }
    }
}
