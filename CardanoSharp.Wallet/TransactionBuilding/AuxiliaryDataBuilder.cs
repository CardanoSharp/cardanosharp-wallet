using CardanoSharp.Wallet.Models.Transactions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.TransactionBuilding
{
    public interface IAuxiliaryDataBuilder: IABuilder<AuxiliaryData>
    {
        IAuxiliaryDataBuilder AddMetadata(int index, object metadata);

        IAuxiliaryDataBuilder AddList(object item);
    }

    public class AuxiliaryDataBuilder: ABuilder<AuxiliaryData>, IAuxiliaryDataBuilder
    {
        private AuxiliaryDataBuilder()
        {
            _model = new AuxiliaryData();
        }

        public static IAuxiliaryDataBuilder Create
        {
            get => new AuxiliaryDataBuilder();
        }

        public IAuxiliaryDataBuilder AddMetadata(int index, object metadata)
        {
            _model.Metadata.Add(index, metadata);
            return this;
        }

        public IAuxiliaryDataBuilder AddList(object item)
        {
            _model.List.Add(item);
            return this;
        }
    }
}
