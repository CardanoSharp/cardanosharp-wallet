using CardanoSharp.Wallet.CIPs.CIP8.Models;
using CardanoSharp.Wallet.Models.Keys;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.CIPs.CIP8
{
    public interface IMessageSigner
    {
        CoseMessageSignature Sign(
            byte[] address,
            byte[] payload,
            PrivateKey signingKey);

        bool Verify(
            byte[] address,
            byte[] payload,
            PrivateKey signingKey);
    }
}
