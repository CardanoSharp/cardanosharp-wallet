﻿namespace CardanoSharp.Wallet.Common
{
    public static class FeeStructure
    {
        // Constants taken from protocol params as of mainnet epoch 345
        public const uint Coefficient = 44;
        public const uint Constant = 155381;
        public const double PriceMem = 0.0577;
        public const double PriceStep = 0.0000721;
    }
}
