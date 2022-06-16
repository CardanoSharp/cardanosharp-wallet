using CardanoSharp.Wallet.Models.Keys;
using System;

namespace CardanoSharp.Wallet
{
    public interface IWalletService
    {
        object DiscoverAccounts(Mnemonic mnemonic);
    }

    public class WalletService : IWalletService
    {
        /// <summary>
        /// https://github.com/bitcoin/bips/blob/master/bip-0044.mediawiki#address-gap-limit
        /// </summary>
        private const int ADDRESS_GAP_LIMIT = 20;

        /// <summary>
        /// https://github.com/bitcoin/bips/blob/master/bip-0032.mediawiki#master-key-generation
        /// https://github.com/bitcoin/bips/blob/master/bip-0044.mediawiki#account-discovery
        /// </summary>
        /// <param name="entropy"></param>
        /// <returns></returns>
        public object DiscoverAccounts(Mnemonic mnemonic)
        {
            //1 derive the first account’s node (index = 0)
            //var rootKey = mnemonic.GetRootKey();
            //var path = rootKey.Derive(root: true).Derive(1582)

            //2 derive the external chain node of this account
            //3 scan addresses of the external chain; respect the gap limit described below
            //  if 20 addresses in a row, expect there are no used addresses beyond this point
            //  and stop searching the address chain. 
            //  > only scan external chains, because internal chains receive only coins
            //    that come from the associated external chains so we dont scan them
            //4 if no transactions are found on the external chain, stop discovery
            //5 if there are some transactions, increase the account index and go to step 1
            throw new NotImplementedException();
        }
    }
}
