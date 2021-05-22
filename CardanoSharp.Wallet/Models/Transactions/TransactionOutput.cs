namespace CardanoSharp.Wallet.Models.Transactions
{

    //transaction_output = [address, amount : value]
    /*
     * value = ...from Rust SDK
     * pub struct Value {
            coin: Coin,
            multiasset: Option<MultiAsset>,
        }
        Coin = ulong
        MultiAsset = Rust Type of BTreeMap<PolicyID, Assets>
        https://doc.rust-lang.org/std/collections/struct.BTreeMap.html
        BTreeMap might essentially be a Dictionary

        PolicyID = byte[] (length 28)
        Assets = BTreeMap<AssetName, ulong>
        AssetName = byte[] (length 28)
     */
    public partial class TransactionOutput
    {
        public byte[] Id { get; set; }
    }
}