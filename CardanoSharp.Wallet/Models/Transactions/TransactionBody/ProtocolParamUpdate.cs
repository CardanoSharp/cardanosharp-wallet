namespace CardanoSharp.Wallet.Models.Transactions
{

//    pub struct ProtocolParamUpdate
//    {
//        minfee_a: Option<Coin>,
//    minfee_b: Option<Coin>,
//    max_block_body_size: Option<u32>,
//    max_tx_size: Option<u32>,
//    max_block_header_size: Option<u32>,
//    key_deposit: Option<Coin>,
//    pool_deposit: Option<Coin>,
//    max_epoch: Option<Epoch>,
//    // desired number of stake pools
//    n_opt: Option<u32>,
//    pool_pledge_influence: Option<Rational>,
//    expansion_rate: Option<UnitInterval>,
//    treasury_growth_rate: Option<UnitInterval>,
//    // decentralization constant
//    d: Option<UnitInterval>,
//    extra_entropy: Option<Nonce>,
//    protocol_version: Option<ProtocolVersions>,
//    min_utxo_value: Option<Coin>,
//}

    /// <summary>
    /// This type is not needed as of right now. There is not intention to update network protocols
    /// so as of this time this will stay empty until we need it, but stays to ensure we match accurately 
    /// the Cardano transaction mapping
    /// </summary>
    public partial class ProtocolParamUpdate
    {

    }
}