using System.Collections.Generic;

namespace CardanoSharp.Wallet.CIPs.CIP8.Models
{
    public class CoseRecipient
    {
        public Headers Headers { get; set; }
        public byte[] CipherText { get; set; }
    }

    public class CoseEncrypt
    {
        public Headers Headers { get; set; }
        public byte[] CipherText { get; set; }
        public IList<CoseRecipient> Signatures { get; set; }
    }

    public class CoseEncrypt0
    {
        public Headers Headers { get; set; }
        public byte[] CipherText { get; set; }
    }

    public class PasswordEncryption
    {
        public CoseEncrypt0 CoseEncrypt0 { get; set; }
    }

    public class PubKeyEncryption
    {
        public CoseEncrypt CoseEncrypt { get; set; }
    }
}