// All Options from https://github.com/Cysharp/csbindgen/blob/11c68c818dafeb55843e95f64ad0f44f5a1c042a/csbindgen/src/builder.rs
/*
pub struct BindgenOptions {
    pub input_bindgen_files: Vec<PathBuf>,
    pub input_extern_files: Vec<PathBuf>,
    pub method_filter: fn(method_name: String) -> bool,
    pub rust_method_type_path: String,
    pub rust_method_prefix: String,
    pub rust_file_header: String,
    pub csharp_namespace: String,
    pub csharp_class_name: String,
    pub csharp_dll_name: String,
    pub csharp_disable_emit_dll_name: bool,
    pub csharp_class_accessibility: String,
    pub csharp_entry_point_prefix: String,
    pub csharp_method_prefix: String,
    pub csharp_if_symbol: String,
    pub csharp_if_dll_name: String,
    pub csharp_use_function_pointer: bool,
}
*/

fn main() {

    csbindgen::Builder::default()
        .input_extern_file("src/lib.rs")
        .csharp_dll_name("./UPLC/target/debug/cardanosharp_wallet_uplc.dll")
        .csharp_class_accessibility("public")
        //.csharp_use_function_pointer (false) // Uncomment this for Unity callback function support when running "cargo build" to get the dll
        .generate_csharp_file("./dotnet/UPLCNativeMethods.g.cs")        
        .unwrap();
}


