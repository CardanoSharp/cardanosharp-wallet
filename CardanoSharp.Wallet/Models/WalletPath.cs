using CardanoSharp.Wallet.Enums;
using System;

namespace CardanoSharp.Wallet.Models
{
    /// <summary>
    /// Path used in Hierarchical Deterministic (HD) Wallets for Cardano 
    /// See <see href="https://cips.cardano.org/cips/cip1852/">CIP1852</see> for context
    /// </summary>
    public class WalletPath
    {
        private readonly DerivationType[] _derivations = new DerivationType[]
        {
            DerivationType.SOFT, // m
            DerivationType.HARD, // purpose
            DerivationType.HARD, // coin
            DerivationType.HARD, // account
            DerivationType.SOFT, // role
            DerivationType.SOFT  // index
        };

        private static char __master = 'm';
        private static char __separator = '/';

        private readonly string[] _segments = new string[6];

        /// <summary>
        /// zero based path depth
        /// </summary>
        public bool IsValid { get; }
        public bool IsPartial { get; }
        public bool IsFull => !IsPartial;
        public bool IsRoot { get; }

        public char MasterNode => __master;
        public PurposeType Purpose { get; }
        public CoinType Coin { get; }
        public int AccountIndex { get; }
        public RoleType Role { get; }
        public int Index { get; }

        private WalletPath(params object[] segments) : this(FormatPath(segments))
        {
        }

        public WalletPath(PurposeType purpose, CoinType coin, int accountIx, RoleType role, int index)
            : this(__master, (int)purpose, (int)coin, accountIx, (int)role, index)
        {
        }


        /// <summary>
        /// partial derivation only works for un-hardened paths supported paths are 
        /// "m/HARD'/HARD'/HARD'" and "SOFT/SOFT" 
        /// other paths are rejected
        /// </summary>
        /// <param name="path"></param>
        public WalletPath(string path)
        {
            var seg = path.Split(__separator);
            var depth = seg.Length - 1;
            var isPartial = seg.Length != _segments.Length;
            var isRoot = seg[0] == "m";

            if (depth < 1 || depth > 5) throw new InvalidOperationException("Invalid path");
            if (isRoot && depth <= 2) throw new InvalidOperationException("Invalid path");

            var dt = new DerivationType[seg.Length];
            for(int i = 0; i< seg.Length; i++)
            {
                dt[i] = seg[i].EndsWith("'") ? DerivationType.HARD : DerivationType.SOFT;
            }

            if(isRoot)
            {
                InitAccountPath(seg, dt);

                if (!TryParseSegment(1, out PurposeType purposeType))
                {
                    //throw new InvalidOperationException($"{typeof(PurposeType).Name} '{_segments[1]}' not supported.");
                }

                if (!TryParseSegment(2, out CoinType coinType))
                {
                    //throw new InvalidOperationException($"{typeof(CoinType).Name} '{_segments[2]}' not supported.");
                }

                if (!TryParseSegment(3, out uint accountIx))
                {
                    //throw new InvalidOperationException("Invalid accountIx.");
                }

                Purpose = purposeType;
                Coin = coinType;
                AccountIndex = (int)accountIx;
            }

            if (!isPartial || (!isRoot && isPartial))
            {
                InitPaymentPath(seg, dt);

                if (!TryParseSegment(4, out RoleType role))
                {
                    //throw new InvalidOperationException($"{typeof(RoleType).Name} '{_segments[4]}' not supported.");
                }

                if (!TryParseSegment(5, out uint index))
                {
                    //throw new InvalidOperationException("Invalid index.");
                }

                Role = role;
                Index = (int)index;
            }

            IsValid = true;
        }

        public override string ToString()
        {
            // TODO filter empty segments
            return FormatPath(_segments);     //empty index
        }


        private string ReadSegment(int id, bool trim = true)
        {
            return trim ? _segments[id].TrimEnd('\'') : _segments[id];
        }

        private bool TryParseSegment<T>(int id, out T value) where T : struct
        {
            if (typeof(T).IsEnum) return Enum.TryParse(ReadSegment(id).TrimEnd('\''), out value);
            value = default;
            return false;
        }

        private bool TryParseSegment(int id, out uint value)
        {
            return uint.TryParse(ReadSegment(id), out value);
        }

        /// <summary>
        /// Checks if the last two elements of seg match the _derivations array
        /// </summary>
        /// <param name="seg"></param>
        /// <param name="dt"></param>
        private void InitPaymentPath(string[] seg, DerivationType[] dt)
        {
            var segEnd = seg.Length - 1;
            var j = _derivations.Length - 1;

            for (int i = segEnd; i > segEnd - 2; i--)
            {
                if (dt[i] != _derivations[j--])
                {
                    throw new InvalidOperationException("Invalid path.");
                }
            }
            Array.Copy(seg, segEnd - 1, _segments, 4, 2);
        }

        /// <summary>
        /// Checks if the first four elements in seg match the _derivations array
        /// </summary>
        /// <param name="seg"></param>
        /// <param name="dt"></param>
        private void InitAccountPath(string[] seg, DerivationType[] dt)
        {
            for (int i = 0; i < 4; i++)
            {
                if (dt[i] != _derivations[i])
                {
                    throw new InvalidOperationException("Invalid path.");
                }
            }

            Array.Copy(seg, _segments, 4);
        }


        public static WalletPath Parse(string path)
        {
            return new WalletPath(path.Trim().ToLower());
        }

        public static bool TryParse(string s, out WalletPath path)
        {
            try
            {
                path = Parse(s);
                return true;
            }
            catch (Exception ex)
            {
                path = null;
                return false;
            }
        }

        /// <summary>
        /// Apostrophe in the path indicates that BIP32 hardened derivation is used.
        /// </summary>
        /// <param name="segments"></param>
        /// <returns></returns>
        private static string FormatPath(object[] segments)
        {
            string format;
            if (segments.GetType() == typeof(string[]))
            {
                format = string.Join("/", segments);
            } else
            {
                format = string.Format("{0}/{1}'/{2}'/{3}'/{4}/{5}", segments);
            }

            return format
                .Replace("/'/'/'/", "") //empty account
                .Replace("//", "");
        }
    }
}
