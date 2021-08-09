using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.TransactionBuilding
{
    public abstract class ABuilder<T>
    {
        protected T _model;

        public T Build()
        {
            return _model;
        }
    }
}
