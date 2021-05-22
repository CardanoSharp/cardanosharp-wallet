using CardanoSharp.Wallet.Models.Transactions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet
{
    public class TransactionBuilder
    {
        private readonly TransactionBody _transactionBody;

        public TransactionBuilder()
        {
            //new up TransactionBody
            _transactionBody = new TransactionBody();
        }

        public void AddInput(byte[] transactionId, uint index)
        {
            //add a new TransactionInput to the TransactionBody
            _transactionBody.TransactionInputs.Add(new TransactionInput()
            {
                Id = transactionId,
                TransactionInputIndex = index
            });
        }

        public void AddOutput(string transactionHash, int index)
        {
            //add a new TransactionOutput to the TransactionBody
            _transactionBody.TransactionOutputs.Add(new TransactionOutput()
            {
                d
            });
        }

        public void AddWithdrawals(byte[] key, uint value)
        {
            //key = reward_account
            //value = coin/amount
            //add a new Withdrawal to the TransactionBody
        }

        public void AddCertificates()
        {
            /*
            certificate =
            [stake_registration
            // stake_deregistration
            // stake_delegation
            // pool_registration
            // pool_retirement
            // genesis_key_delegation
            // move_instantaneous_rewards_cert
            ]

            stake_registration = (0, stake_credential)
            stake_deregistration = (1, stake_credential)
            stake_delegation = (2, stake_credential, pool_keyhash)
            pool_registration = (3, pool_params)
            pool_retirement = (4, pool_keyhash, epoch)
            genesis_key_delegation = (5, genesishash, genesis_delegate_hash, vrf_keyhash)
            move_instantaneous_rewards_cert = (6, move_instantaneous_reward)*/
            //Certificates are byte[] but we may need to denote type...
            //add a new Certificate
        }

        public void SetFee(int fee)
        {
            //set the fee on TransactionBody
        }

        public void SetTtl(int ttl)
        {
            //set the ttl on TransactionBody
        }
    }
}
