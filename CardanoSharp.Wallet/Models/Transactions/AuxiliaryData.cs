using System.Collections.Generic;

namespace CardanoSharp.Wallet.Models.Transactions
{
    //   auxiliary_data =
    //{ * transaction_metadatum_label => transaction_metadatum }
    /// [transaction_metadata: { *transaction_metadatum_label => transaction_metadatum }
    //, auxiliary_scripts:[ *native_script ]
    //; other types of metadata...
    //]
    //transaction_metadatum_label = uint

    public partial class AuxiliaryData
    {
        public AuxiliaryData()
        {
            Metadata = new Dictionary<int, object>();
            List = new List<object>();
        }

        public Dictionary<int, object> Metadata { get; set; }
        public List<object> List { get; set; }
    }
}
