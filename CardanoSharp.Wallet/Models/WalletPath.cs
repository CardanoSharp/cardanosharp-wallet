using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Models.Keys;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CardanoSharp.Wallet.Models
{

    

    /// <summary>
    /// Path used in Hierarchical Deterministic (HD) Wallets for Cardano 
    /// See <see href="https://cips.cardano.org/cips/cip1852/">CIP1852</see> for context
    /// </summary>
    public class WalletPath
    {
        private const char MASTER_NODE_IDENTIFIER = 'm';

        private readonly DerivationType[] _derivations = new DerivationType[]
        {
            DerivationType.SOFT, // m
            DerivationType.HARD, // purpose
            DerivationType.HARD, // coin
            DerivationType.HARD, // account
            DerivationType.SOFT, // role
            DerivationType.SOFT  // index
        };

        private readonly string[] _segments;
        
        private WalletPath(params object[] segments) : this(FormatPath(segments))
        {
        }

        public WalletPath(PurposeType purpose, CoinType coin, int accountIx, RoleType role, int index)
            : this(MASTER_NODE_IDENTIFIER, (int)purpose, (int)coin, accountIx, (int)role, index)
        {
        }

        public WalletPath(string path)
        {
            _segments = path.Split('/');

            if (_segments.Length != _derivations.Length)
            {
                throw new InvalidOperationException("Invalid path");
            }

            // resolve derivation scheme from path
            for (int i = 0; i < _segments.Length; i++)
            {
                //_derivations[i] = _segments[i].EndsWith("'") ? DerivationType.HARD : DerivationType.SOFT;
                _segments[i] = _segments[i].Replace("'", "");
            }

            if (!Enum.TryParse(_segments[1].TrimEnd('\''), out PurposeType purposeType))
            {
                throw new InvalidOperationException($"{typeof(PurposeType).Name} '{_segments[1]}' not supported.");
            }

            if (!Enum.TryParse(_segments[2].TrimEnd('\''), out CoinType coinType))
            {
                throw new InvalidOperationException($"{typeof(CoinType).Name} '{_segments[2]}' not supported.");
            }
            
            if (!uint.TryParse(_segments[3].TrimEnd('\''), out uint accountIx))
            {
                throw new InvalidOperationException("Invalid accountIx.");
            }

            if (!Enum.TryParse(_segments[4], out RoleType role))
            {
                throw new InvalidOperationException($"{typeof(RoleType).Name} '{_segments[4]}' not supported.");
            }

            if (!uint.TryParse(_segments[5], out uint index))
            {
                throw new InvalidOperationException("Invalid index.");
            }

            Purpose = purposeType;
            Coin = coinType;
            AccountIndex = (int)accountIx;
            Role = role;
            Index = (int)index;
        }

        public char MasterNode => MASTER_NODE_IDENTIFIER;
        public PurposeType Purpose { get; }
        public CoinType Coin { get;  }
        public int AccountIndex { get; }
        public RoleType Role { get; }
        public int Index { get; }


        public override string ToString()
        {
            return FormatPath(_segments);
        }

        /// <summary>
        /// Apostrophe in the path indicates that BIP32 hardened derivation is used.
        /// </summary>
        /// <param name="segments"></param>
        /// <returns></returns>
        private static string FormatPath(object[] segments)
        {
            return string.Format("{0}/{1}'/{2}'/{3}'/{4}/{5}", segments);
        }
    }
}
