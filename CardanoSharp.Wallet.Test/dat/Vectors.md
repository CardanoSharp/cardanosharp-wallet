# Test Vector Reference

## Overview

This file defines the CardanoSharp.Wallet Test Vectors

### Structure

```bash
┆
├╌┬╌ CardanoSharp.Wallet.Test/
┆ │
  ├╌┬╌╌ dat/
  │ ├╌┬ 01/
  │ │ ├╌╌ reference.cddl
  │ │ ├╌╌ reference.draft
  │ │ ├╌╌ tx.cddl
  │ │ └╌╌ tx.draft
  │ ├╌┬ 02/
  │ │ ├╌╌ ...
  │ │ └╌╌ ...
  ┆ ┆ 
  │ ├╌┬ keys/
  │ │ ├╌╌ payment1.addr
  │ │ ├╌╌ payment1.skey
  │ │ ├╌╌ payment1.vkey
  │ │ ├╌╌ payment2.addr
  │ │ ├╌╌ payment2.skey
  │ │ └╌╌ payment2.vkey
  │ │
  │ ├╌╌ protocol.json (protocol parameters)
  │ └╌╌ Vectors.md    (this file)  
  │
  ├╌ AddressTests.cs
  ├╌ Bech32Tests.cs
  ┆ 
```

### Keys

In order to ensure consitant test results we define the following keys and addresses to be used within tests.

|Key|Value|
|-|-|
|payment1.addr|`addr_test1vrgvgwfx4xyu3r2sf8nphh4l92y84jsslg5yhyr8xul29rczf3alu`|
|payment1.skey|`5820e260af648b5710bfabd978816b8e73720698926cec799ef9c0a5671b396b9202`|
|payment1.vkey|`58200bc7720123b3fd7bc8c0573bbd7df6577cef1a3385ab79959d1319d373f5ebe1`|
|payment2.addr|`addr_test1vqah2xrfp8qjp2tldu8wdq38q8c8tegnduae5zrqff3aeec7g467q`|
|payment2.skey|`5820b1c3ffe27415ed97870a0a399e62f2396185925700c7e8058f775bf63ba9caf4`|
|payment2.vkey|`58200b41b2f0cd92448ead8fe43e8a0b2b10e1ce6aceffcc6d5543479a5ffa52d149`|

## Test Vectors

* Vectors are numbered from 01 to XX.
* Each vector MUST define the means of how it was generated.
* Each vector MUST provide one or many `reference` files.

| Id        | Category        | Description                     | Reference(s) |
|-----------|:---------------:|:--------------------------------|-|
| [01](#01) | Transactions    | Serialize `Transaction` to CBOR ||
| [02](#02) | ...             | ...                             ||

### 01

#### Generate reference

```bash
$ pwd
cardanosharp-wallet/CardanoSharp.Wallet.Test/dat

$ cardano-cli version
cardano-cli 1.28.0 - linux-x86_64 - ghc-8.10
git rev e99393d10bb8f01ad43065627c21a33aa2a024c9

$ cardano-cli transaction build-raw \
--tx-in 98035740ab68cad12cb4d8281d10ce1112ef0933dc84920b8937c3e80d78d120#0 \
--tx-out $(cat keys/payment1.addr)+0 \
--tx-out $(cat keys/payment2.addr)+0 \
--fee 0 \
--out-file 01/reference.draft
```

#### JSON

```json
{
  "type": "TxBodyMary",
  "description": "",
  "cborHex": "83a3008182582098035740ab68cad12cb4d8281d10ce1112ef0933dc84920b8937c3e80d78d12000018282581d60d0c43926a989c88d5049e61bdebf2a887aca10fa284b9067373ea28f0082581d603b75186909c120a97f6f0ee6822701f075e5136f3b9a08604a63dce70002009ffff6"
}
```

#### CDDL

<http://cbor.me>

```cddl
[
    {
        0: [
            [h'98035740AB68CAD12CB4D8281D10CE1112EF0933DC84920B8937C3E80D78D120',
                0
            ]
        ],
        1: [
            [h'60D0C43926A989C88D5049E61BDEBF2A887ACA10FA284B9067373EA28F',
                0
            ],
            [h'603B75186909C120A97F6F0EE6822701F075E5136F3B9A08604A63DCE7',
                0
            ]
        ],
        2: 0
    },
    [],
    null
]
```

### 02

#### Generate reference

```bash
$ pwd
cardanosharp-wallet/CardanoSharp.Wallet.Test/dat

$ cardano-cli version
cardano-cli 1.28.0 - linux-x86_64 - ghc-8.10
git rev e99393d10bb8f01ad43065627c21a33aa2a024c9
```

#### JSON

```json

```

#### CDDL

```cddl

```