using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Models.Transactions;
using CardanoSharp.Wallet.Models.Transactions.Scripts;
using CardanoSharp.Wallet.Models.Transactions.TransactionWitness.Scripts;
using System.Collections.Generic;

namespace CardanoSharp.Wallet.TransactionBuilding
{
    public interface INativeScriptBuilder: IABuilder<NativeScript>
    {
        INativeScriptBuilder SetKeyHash(byte[] keyHash);
        INativeScriptBuilder SetScript(NativeScriptType type, INativeScriptBuilder nativeScriptBuilder);
        INativeScriptBuilder SetScriptAll(IEnumerable<INativeScriptBuilder> nativeScriptBuilders);
        INativeScriptBuilder SetScriptAny(IEnumerable<INativeScriptBuilder> nativeScriptBuilders);
        INativeScriptBuilder SetScriptNofK(uint n, IEnumerable<INativeScriptBuilder> nativeScriptBuilders);
        INativeScriptBuilder SetInvalidAfter(uint after);
        INativeScriptBuilder SetInvalidBefore(uint before);
    }

    public class NativeScriptBuilder: ABuilder<NativeScript>, INativeScriptBuilder
    {
        private NativeScriptBuilder()
        {
            _model = new NativeScript();
        }

        private NativeScriptBuilder(NativeScript model)
        {
            _model = model;
        }

        public static INativeScriptBuilder GetBuilder(NativeScript model)
        {
            if (model == null)
            {
                return new NativeScriptBuilder();
            }
            return new NativeScriptBuilder(model);
        }

        public static INativeScriptBuilder Create
        {
            get => new NativeScriptBuilder();
        }

        public INativeScriptBuilder SetKeyHash(byte[] keyHash)
        {
            _model.ScriptPubKey = new ScriptPubKey()
            {
                KeyHash = keyHash
            };
            return this;
        }

        public INativeScriptBuilder SetScript(NativeScriptType type, INativeScriptBuilder nativeScriptBuilder)
        {
            switch(type)
            {
                case NativeScriptType.ScriptAll:
                    if (_model.ScriptAll == null) _model.ScriptAll = new ScriptAll();
                    _model.ScriptAll.NativeScripts.Add(nativeScriptBuilder.Build());
                    break;
                case NativeScriptType.ScriptAny:
                    if (_model.ScriptAny == null) _model.ScriptAny = new ScriptAny();
                    _model.ScriptAny.NativeScripts.Add(nativeScriptBuilder.Build());
                    break;
                case NativeScriptType.ScriptNofK:
                    if (_model.ScriptNofK == null) _model.ScriptNofK = new ScriptNofK();
                    _model.ScriptNofK.NativeScripts.Add(nativeScriptBuilder.Build());
                    break;
            }

            return this;
        }

        public INativeScriptBuilder SetScriptAll(IEnumerable<INativeScriptBuilder> nativeScriptBuilders)
        {
            if (_model.ScriptAll == null) _model.ScriptAll = new ScriptAll();
            foreach (var nativeScriptBuilder in nativeScriptBuilders)
            {
                _model.ScriptAll.NativeScripts.Add(nativeScriptBuilder.Build());
            }
            return this;
        }

        public INativeScriptBuilder SetScriptAny(IEnumerable<INativeScriptBuilder> nativeScriptBuilders)
        {
            if (_model.ScriptAny == null) _model.ScriptAny = new ScriptAny();
            foreach (var nativeScriptBuilder in nativeScriptBuilders)
            {
                _model.ScriptAny.NativeScripts.Add(nativeScriptBuilder.Build());
            }
            return this;
        }

        public INativeScriptBuilder SetScriptNofK(uint n, IEnumerable<INativeScriptBuilder> nativeScriptBuilders)
        {
            if (_model.ScriptNofK == null) _model.ScriptNofK = new ScriptNofK();
            _model.ScriptNofK.N = n;
            foreach (var nativeScriptBuilder in nativeScriptBuilders)
            {
                _model.ScriptNofK.NativeScripts.Add(nativeScriptBuilder.Build());
            }
            return this;
        }

        public INativeScriptBuilder SetInvalidAfter(uint after)
        {
            _model.InvalidAfter = new ScriptInvalidAfter()
            {
                After = after
            };
            return this;
        }

        public INativeScriptBuilder SetInvalidBefore(uint before)
        {
            _model.InvalidBefore = new ScriptInvalidBefore()
            {
                Before = before
            };
            return this;
        }
    }
}
