namespace CardanoSharp.Wallet.Enums
{

    /// <summary>
    /// Additional resources
    /// https://cips.cardano.org/cips/CIP-0003
    /// https://cips.cardano.org/cips/cip1852/
    /// https://github.com/input-output-hk/technical-docs/blob/main/cardano-components/adrestia/doc/key-concepts/hierarchical-deterministic-wallets.md#path-levels
    /// </summary>
    public enum RoleType
    {
        /// <summary>
        /// Same as defined in <see href="https://github.com/bitcoin/bips/blob/master/bip-0044.mediawiki">BIP44</see>
        /// </summary>
        ExternalChain = 0,
        /// <summary>
        /// Same as defined in <see href="https://github.com/bitcoin/bips/blob/master/bip-0044.mediawiki">BIP44</see>
        /// </summary>
        InternalChain = 1,
        /// <summary>
        /// See <see href="https://cips.cardano.org/cips/CIP-0011">CIP11</see>
        /// </summary>
        Staking = 2
    }
}
