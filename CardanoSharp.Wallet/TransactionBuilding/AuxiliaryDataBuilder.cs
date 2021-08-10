using CardanoSharp.Wallet.Models.Transactions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.TransactionBuilding
{
    public class AuxiliaryDataBuilder: ABuilder<AuxiliaryData>
    {
        public AuxiliaryDataBuilder()
        {
            _model = new AuxiliaryData();
        }

        public AuxiliaryDataBuilder WithMetadata(Dictionary<int, object> metadata)
        {
            _model.Metadata = metadata;
            return this;
        }
    }
}
