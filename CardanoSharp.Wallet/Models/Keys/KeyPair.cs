using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.Models.Keys
{
    public record KeyPair
    {
        public PrivateKey PrivateKey { get; set; }
        public PublicKey PublicKey { get; set; }
    }
}
