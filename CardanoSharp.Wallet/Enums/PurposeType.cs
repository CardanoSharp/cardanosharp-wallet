namespace CardanoSharp.Wallet.Enums
{
    /// <summary>
    /// 
    /// </summary>
    public enum PurposeType
    {
        /// <summary>
        /// Represents a 'Byron-era' wallet. 44 was chosen because Cardano implements BIP44-Ed25519 (not BIP44)
        /// </summary>
        Byron = 44,
        /// <summary>
        /// Represents a 'Shelley-era' wallet. 1852 was chosen as it is the year of death of Ada Lovelace
        /// </summary>
        Shelley = 1852,
        /// <summary>
        /// Represents a 'MultiSig' wallet. https://cips.cardano.org/cips/cip1854/
        /// </summary>
        MultiSig = 1854,
        /// <summary>
        /// Represents a 'Policy Keys' wallet. https://cips.cardano.org/cips/cip1855/
        /// </summary>
        PolicyKeys = 1855
    }
}
