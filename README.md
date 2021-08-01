# CardanoSharp.Wallet [![Build status](https://ci.appveyor.com/api/projects/status/knh87k86mf7gbxyo?svg=true)](https://ci.appveyor.com/project/nothingalike/cardanosharp-wallet) [![Test status](https://img.shields.io/appveyor/tests/nothingalike/cardanosharp-wallet)](https://ci.appveyor.com/project/nothingalike/cardanosharp-wallet) [![NuGet Version](http://img.shields.io/nuget/v/CardanoSharp.Wallet.svg?style=flat)](https://www.nuget.org/packages/CardanoSharp.Wallet/) ![NuGet Downloads](https://img.shields.io/nuget/dt/CardanoSharp.Wallet.svg)

CardanoSharp.Wallet is a Cardano Serialization library for .NET applications. 

## Features

 * Generate Mnemonics
 * Create Private and Public Keys
 * Create Addresses
 * Build Transactions
 * Sign Transactions

## Getting Started

CardanoSharp.Wallet is installed from NuGet. 

```
Install-Package CardanoSharp.Wallet
```

## Generate a Mnemonic
```csharp
using CardanoSharp.Wallet;

class Program
{
    static void Main(string[] args)
    {
        // The KeyServices exposes functions for Mnemonics and Deriving Keys
        var keyService = new KeyService();
        // The AddressService allows us to create Addresses from our Public Keys
        var addressService = new AddressService();
        // 24 represents the number of words we want for our Mnemonic
        int size = 24;

        // This will generate a 24 English Mnemonic
        Mnemonic mnemonic = keyService.Generate(24, WordLists.English);
        string mnemonicWords = mnemonic.Words;
    }
}
```

## Create Private and Public Keys
```csharp
// Here we can get the entropy from our mnemonic
byte[] entropy = mnemonic.Entropy;

// The masterKey is a PrivateKey made of up of the 
//  - byte[] Key
//  - byte[] Chaincode
PrivateKey masterKey = mnemonic.GetRootKey();

// This path will give us our Payment Key on index 0
string paymentPath = $"m/1852'/1815'/0'/0/0";
// The paymentPrv is another Tuple with the Private Key and Chain Code
PrivateKey paymentPrv = masterKey.Derive(paymentPath);
// Get the Public Key from the Payment Private Key
PublicKey paymentPub = paymentPrv.GetPublicKey(false);

// This path will give us our Stake Key on index 0
var stakePath = $"m/1852'/1815'/0'/2/0";
// The stakePrv is another Tuple with the Private Key and Chain Code
var stakePrv = masterKey.Derive(stakePath);
// Get the Public Key from the Stake Private Key
var stakePub = stakePrv.GetPublicKey(false);
```

## Create Addresses

```csharp
// Creating Addresses require the Public Payment and Stake Keys
var baseAddr = addressService.GetAddress(
    paymentPub, 
    stakePub, 
    NetworkType.Testnet, 
    AddressType.Base);
```

### NetworkType

```csharp
namespace CardanoSharp.Wallet.Enums
{
    public enum NetworkType
    {
        Testnet,
        Mainnet
    }
}
```

### AddressType

```csharp
namespace CardanoSharp.Wallet.Enums
{
    public enum AddressType
    {
        Base,
        Enterprise,
        Reward
    }
}
```

## Building and Sign Transactions
This is just an example of how to start. You will need to Calculate Fees, compare with Protocol Parameters and re-serialize. 
```csharp
// The Transaction Builder allows us to contruct and serialize our Transaction
var transactionBuilder = new TransactionBuilder();

// Create the Transaction Body
//  The Transaction Body:
//      Supported
//       - Transaction Inputs
//       - Transaction Outputs
//       - Fee
//       - TTL
//       - Certificates
//       - Metadata Hash
//      Coming Soon
//       - Transaction Start Interval
//       - Withdrawls
//       - Mint
var transactionBody = new TransactionBody()
{
    TransactionInputs = new List<TransactionInput>()
    {
        new TransactionInput()
        {
            TransactionIndex = 0,
            TransactionId = someTxHash
        }
    },
    TransactionOutputs = new List<TransactionOutput>()
    {
        new TransactionOutput()
        {
            Address = addressService.GetAddressBytes(baseAddr),
            Value = new TransactionOutputValue()
            {
                Coin = 1
            }
        }
    },
    Ttl = 10,
    Fee = 0
};

// Add our witness(es)
//  Currently we only support VKey Witnesses
var witnesses = new TransactionWitnessSet()
{
    VKeyWitnesses = new List<VKeyWitness>()
    {
        new VKeyWitness()
        {
            VKey = paymentPub.Key,
            SKey = paymentPrv.Key
        }
    }
};

// Create Transaction
var transaction = new Transaction()
{
    TransactionBody = transactionBody,
    TransactionWitnessSet = witnesses
};

// Serialize Transaction with Body and Witnesses
//  This results in a Signed Transaction
var signedTx = transactionBuilder.SerializeTransaction(transaction);
```

### Calculate Fees
```csharp
var fee = transactionBuilder.CalculateFee(signedTx);
```

### Adding Metadata
```csharp
var auxData = new AuxiliaryData()
{
    Metadata = new Dictionary<int, object>()
    {
        { 1234, new { name = "simple message" } }
    }
};

var transaction = new Transaction()
{
    TransactionBody = transactionBody,
    TransactionWitnessSet = witnesses,
    AuxiliaryData = auxData
};
```

### More Examples
Please see the [Transaction Tests](https://github.com/CardanoSharp/cardanosharp-wallet/blob/main/CardanoSharp.Wallet.Test/TransactionTests.cs)