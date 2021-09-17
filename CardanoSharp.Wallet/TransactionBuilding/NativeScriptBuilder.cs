using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Models.Transactions;
using CardanoSharp.Wallet.Models.Transactions.Scripts;
using CardanoSharp.Wallet.Models.Transactions.TransactionWitness.Scripts;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.TransactionBuilding
{
    public interface INativeScriptBuilder: IABuilder<NativeScript>
    {
        INativeScriptBuilder SetKeyHash(byte[] keyHash);
        INativeScriptBuilder SetScript(NativeScriptType type, INativeScriptBuilder nativeScriptBuilder);
        INativeScriptBuilder SetInvalidAfter(uint after);
        INativeScriptBuilder SetInvalidBefore(uint before);
    }

    public class NativeScriptBuilder: ABuilder<NativeScript>, INativeScriptBuilder
    {
        private NativeScriptBuilder()
        {
            _model = new NativeScript();
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
