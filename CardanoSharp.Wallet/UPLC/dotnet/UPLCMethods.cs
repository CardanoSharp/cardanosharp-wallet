using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using CardanoSharp.Wallet.Common;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Extensions.Models.Transactions;
using CardanoSharp.Wallet.Utilities;
using CardanoSharp.Wallet.Models;
using CardanoSharp.Wallet.Models.Transactions;
using CardanoSharp.Wallet.Models.Transactions.TransactionWitness.PlutusScripts;
using CardanoSharp.Wallet.Extensions.Models;

namespace CsBindgen
{
    public static class UPLCMethods
    {
        public static string ApplyParamsToPlutusScript(PlutusDataArray parameters, string plutusScriptCbor) {
            
            byte[] paramsArray = parameters.Serialize();
            byte[] plutusScriptBytes = plutusScriptCbor.HexToByteArray();
            nuint paramsLength = (nuint)paramsArray.Length;
            nuint plutusScriptLength = (nuint)plutusScriptBytes.Length;

            PlutusScriptResult result;
            string scriptHex;
            unsafe {
                fixed (byte* paramsPtr = &paramsArray[0])
                fixed (byte* plutusScriptPtr = &plutusScriptBytes[0])
                
                result = UPLCNativeMethods.apply_params_to_plutus_script(paramsPtr, plutusScriptPtr, paramsLength, plutusScriptLength);
                if (!result.success)
                    return null!;
                
                byte[] byteArray = new byte[result.length];
                Marshal.Copy((IntPtr)result.value, byteArray, 0, (int)result.length);

                scriptHex = byteArray.ToStringHex();
            };
            return scriptHex;
        }

        // This function's Rust code needs to be debugged before it can be used
        /*
        public static List<Redeemer> GetExUnits(Transaction transaction, NetworkType networkType) 
        {
            byte[] txBytes = transaction.Serialize();

            List<byte[]> inputsList = new List<byte[]>();
            List<nuint> inputsLengthList = new List<nuint>();
            foreach (TransactionInput input in transaction.TransactionBody.TransactionInputs)
            {
                byte[] inputBytes = input.Serialize();
                inputsList.Add(inputBytes); 
                inputsLengthList.Add((nuint)inputBytes.Length);               
            }
            byte[][] inputs = inputsList.ToArray();
            nuint[] inputsLength = inputsLengthList.ToArray();

            List<byte[]> outputsList = new List<byte[]>();
            List<nuint> oyutputsLengthList = new List<nuint>();
            foreach (TransactionOutput output in transaction.TransactionBody.TransactionOutputs)
            {
                byte[] outputBytes = output.Serialize();
                outputsList.Add(outputBytes);   
                oyutputsLengthList.Add((nuint)outputBytes.Length);             
            }
            byte[][] outputs = outputsList.ToArray();
            nuint[] outputsLength = oyutputsLengthList.ToArray();

            byte[] costMdls = new byte[CostModelUtility.PlutusV2CostModel.Costs.Length * sizeof(long)];
            Buffer.BlockCopy(CostModelUtility.PlutusV2CostModel.Costs, 0, costMdls, 0, costMdls.Length);

            ulong initialBudgetMem = FeeStructure.MaxTxExMem;
            ulong initialBudgetStep = FeeStructure.MaxTxExSteps;

            SlotNetworkConfig slotNetworkConfig = SlotUtility.GetSlotNetworkConfig(networkType);
            ulong slotConfigZeroTime = (ulong)slotNetworkConfig.ZeroTime;
            ulong slotConfigZeroSlot = (ulong)slotNetworkConfig.ZeroSlot;
            uint slotConfigSlotLength = (uint)slotNetworkConfig.SlotLength;

            nuint txLength = (nuint)txBytes.Length;
            nuint length = (nuint)inputs.Length;
            nuint costMdlsLength = (nuint)costMdls.Length;
            
            ExUnitsResult result;
            byte[][] redeemersByteArray;
            unsafe {
                byte** inputsPtr = ConvertByteArrayToByteArrayPointer(inputs);
                byte** outputsPtr = ConvertByteArrayToByteArrayPointer(outputs);                
                fixed (byte* txPtr = &txBytes[0])
                fixed (byte* costMdlsPtr = &costMdls[0])
                fixed (nuint* inputsLengthPtr = &inputsLength[0])
                fixed (nuint* outputsLengthPtr = &outputsLength[0])

                result = UPLCNativeMethods.get_ex_units(txPtr, inputsPtr, outputsPtr, costMdlsPtr, initialBudgetMem, initialBudgetStep, slotConfigZeroTime, slotConfigZeroSlot, slotConfigSlotLength, txLength, length, inputsLengthPtr, outputsLengthPtr, costMdlsLength);
                if (!result.success)
                    return null!;
                redeemersByteArray = ConvertByteArrayPointerToByteArray(result.value, result.length, result.length_value);
            };

            List<Redeemer> redeemers = new List<Redeemer>();
            foreach (byte[] redeemerByteArray in redeemersByteArray)
            {
                Redeemer redeemer = RedeemerExtensions.Deserialize(redeemerByteArray);
                redeemers.Add(redeemer);
            }

            return redeemers;
        }
        */

        public unsafe static byte** ConvertByteArrayToByteArrayPointer(byte[][] bytes)
        {
            int numRows = bytes.Length;
            IntPtr[] pointers = new IntPtr[numRows];

            for (int i = 0; i < numRows; i++)
            {
                pointers[i] = Marshal.AllocHGlobal(bytes[i].Length);
                Marshal.Copy(bytes[i], 0, pointers[i], bytes[i].Length);
            }

            byte** bytePtr = (byte**)Marshal.AllocHGlobal(numRows * sizeof(IntPtr));

            for (int i = 0; i < numRows; i++)
            {
                ((IntPtr*)bytePtr)[i] = pointers[i];
            }

            return bytePtr;
        }

        public unsafe static byte[][] ConvertByteArrayPointerToByteArray(byte** value, nuint length, nuint* length_value)
        {
            byte[][] result = new byte[length][];
            for (nuint i = 0; i < length; i++)
            {
                int rowLength = (int)length_value[i];
                result[i] = new byte[rowLength];
                Marshal.Copy((IntPtr)value[i], result[i], 0, rowLength);
            }

            return result;
        }
    }
}
