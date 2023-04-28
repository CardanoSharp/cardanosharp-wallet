This folder contains instructions for how to include the Aiken UPLC functions in CardanoSharp

This is intended to give CardanoSharp access to the "apply_params_to_plutus_script" and "get_ex_units" aiken functions
so that we can obtain ex_units locally and apply parameters to plutus scripts.

These are extremely valuable features of Aiken. We are using the Rust -> C# Foreign Function Interface Library csbindgen to create the bindings: https://github.com/Cysharp/csbindgen/tree/main


To build we run this:

```
cargo build
```

Then when we run dotnet build at the top level of cardanosharp the dlls will be included in the build