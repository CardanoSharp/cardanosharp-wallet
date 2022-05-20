using CardanoSharp.Wallet.Models.Transactions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.TransactionBuilding
{
    public interface IAuxiliaryDataBuilder: IABuilder<AuxiliaryData>
    {
        IAuxiliaryDataBuilder AddMetadata(int index, object metadata);
    }

    public class AuxiliaryDataBuilder: ABuilder<AuxiliaryData>, IAuxiliaryDataBuilder
    {
        private AuxiliaryDataBuilder()
        {
            _model = new AuxiliaryData();
        }

        private AuxiliaryDataBuilder(AuxiliaryData model)
        {
            _model = model;
        }

        public static IAuxiliaryDataBuilder GetBuilder(AuxiliaryData model)
        {
            return new AuxiliaryDataBuilder(model);
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
    }
}
