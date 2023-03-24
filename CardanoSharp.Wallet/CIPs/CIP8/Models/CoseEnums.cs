namespace CardanoSharp.Wallet.CIPs.CIP8.Models
{
    /// <summary>
    /// Translated from https://github.com/Emurgo/message-signing/blob/master/rust/src/builders.rs
    /// </summary>
    public enum AlgorithmId
    {
        /// EdDSA (Pure EdDSA, not HashedEdDSA) - the algorithm used for Cardano addresses
        EdDsa = -8,
        /// ChaCha20/Poly1305 w/ 256-bit key, 128-bit tag
        ChaCha20Poly1305 = 24,
    }

    public enum KeyType
    {
        /// octet key pair
        Okp = 1,
        /// 2-coord EC
        Ec2 = 2,
        Symmetric = 4,
    }

    public enum EcKey
    {
        // EC identifier
        Crv = -1,
        // x coord (OKP) / pubkey (EC2)
        X = -2,
        // y coord (only for EC2 - not present in OKP)
        Y = -3,
        // private key (optional)
        D = -4,
    }

    public enum CurveType
    {
        P256 = 1,
        P384 = 2,
        P521 = 3,
        X25519 = 4,
        X448 = 5,
        Ed25519 = 6, // the EdDSA variant used for cardano addresses
        Ed448 = 7,
    }

    public enum KeyOperations
    {
        // The key is used to create signatures. Requires private key fields
        Sign = 1,
        // The key is used for verification of signatures.
        Verify = 2,
        // The key is used for key transport encryption. 
        Encrypt = 3,
        // The key is used for key transport decryption. Requires private key fields.
        Decrypt = 4,
        // The key is used for key wrap encryption.
        WrapKey = 5,
        // The key is used for key wrap decryption. Requires private key fields.
        UnwrapKey = 6,
        // The key is used for deriving keys. Requires private key fields
        DeriveKey = 7,
        // The key is used for deriving bits not to be used as a key. Requires private key fields
        DeriveBits = 8,
    }
}
