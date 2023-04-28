#![feature(vec_into_raw_parts)]

// lib.rs, simple FFI code
#[repr(C)]
pub struct PlutusScriptResult {
    success: bool,
    value: *const u8,
    length: usize,
}

#[no_mangle]
pub extern "C" fn apply_params_to_plutus_script(
    params: *const u8,        //&PlutusList,
    plutus_script: *const u8, //PlutusScript,
    params_length: usize,
    plutus_script_length: usize,
) -> PlutusScriptResult {
    unsafe {
        let params_bytes: &[u8] = std::slice::from_raw_parts(params, params_length);
        let plutus_script_bytes: &[u8] = std::slice::from_raw_parts(plutus_script, plutus_script_length);        
        match uplc::tx::apply_params_to_script(params_bytes, plutus_script_bytes) {
            Ok(script) => {                
                let (script_ptr, script_len, _cap) = script.into_raw_parts();
                PlutusScriptResult {
                    success: true,
                    value: script_ptr,
                    length: script_len,
                }
            }
            Err(e) => PlutusScriptResult {
                success: false,
                value: e.to_string().as_bytes().as_ptr(),
                length: e.to_string().as_bytes().len(),
            },
        }
    }
}

#[repr(C)]
pub struct ExUnitsResult {
    success: bool,
    value: *const *const u8,
    length: usize,
    length_value: *const usize
}

#[no_mangle]
pub extern "C" fn get_ex_units(
    tx: *const u8, //&Transaction,
    inputs: *const *const u8, //&TransactionUnspentOutputs,
    outputs: *const *const u8, //&TransactionUnspentOutputs,
    cost_mdls: *const u8, //&CostModels,
    initial_budget_mem: u64, //&ExUnits,
    initial_budget_step: u64, //&ExUnits,
    slot_config_zero_time: u64, // (BigNum, BigNum, u32),
    slot_config_zero_slot: u64, // (BigNum, BigNum, u32),
    slot_config_slot_length: u32, // (BigNum, BigNum, u32),    
    tx_length: usize,
    inputs_outputs_length: usize, 
    inputs_length: *const usize,
    outputs_length: *const usize,
    cost_mdls_length: usize,
) -> ExUnitsResult {
    unsafe {
        let tx_bytes: &[u8] = std::slice::from_raw_parts(tx, tx_length);
        let converted_inputs_outputs: Vec<(Vec<u8>, Vec<u8>)> = convert_inputs_outputs(inputs, outputs, inputs_outputs_length, inputs_length, outputs_length);
        let converted_inputs_outputs_slice: &[(Vec<u8>, Vec<u8>)] = &converted_inputs_outputs;        
        let cost_mdls_bytes: &[u8] = std::slice::from_raw_parts(cost_mdls, cost_mdls_length);
        let initial_budget_tuple = (initial_budget_mem, initial_budget_step);
        let slot_config_tuple = (slot_config_zero_time, slot_config_zero_slot, slot_config_slot_length);

        let result = uplc::tx::eval_phase_two_raw(
            tx_bytes,
            converted_inputs_outputs_slice,
            cost_mdls_bytes,
            initial_budget_tuple,
            slot_config_tuple,
            false,
            |_| (),
        );

        match result {
            Ok(redeemers_bytes) => {
                let mut redeemers_ptrs: Vec<*const u8> = Vec::with_capacity(redeemers_bytes.len());
                let mut inner_lengths: Vec<usize> = Vec::with_capacity(redeemers_bytes.len());

                for inner_vec in redeemers_bytes {
                    let (ptr, len, _cap) = inner_vec.into_raw_parts();
                    redeemers_ptrs.push(ptr as *const u8);
                    inner_lengths.push(len);
                }

                // Convert the Vec<*const u8> into raw parts (redeemers_ptrs_ptr)
                let (redeemers_ptrs_ptr, redeemers_ptrs_len, _) = redeemers_ptrs.into_raw_parts();
                
                // Convert the Vec<usize> into raw parts (length_value)
                let (length_value, _, _) = inner_lengths.into_raw_parts();                
                ExUnitsResult {
                    success: true,
                    value: redeemers_ptrs_ptr,
                    length: redeemers_ptrs_len,
                    length_value: length_value
                }
            }
            Err(e) => {
                let error_string = e.to_string();
                let error_bytes = error_string.as_bytes();
                let error_ptr = error_bytes.as_ptr() as *const u8;

                // Wrap the error pointer in a Box to ensure its memory is deallocated when
                // the struct is dropped
                let boxed_error_ptr = Box::new(error_ptr);

                // Get the raw pointer from the Box
                let raw_error_ptr = Box::into_raw(boxed_error_ptr);

                ExUnitsResult {
                    success: false,
                    value: raw_error_ptr as *const *const u8,
                    length: error_bytes.len(),
                    length_value: std::ptr::null(),
                }
            }
        }
    }
}

unsafe fn convert_inputs_outputs(
    inputs: *const *const u8,
    outputs: *const *const u8,
    length: usize,
    inputs_length: *const usize,
    outputs_length: *const usize

) -> Vec<(Vec<u8>, Vec<u8>)> {
    // Convert the raw pointers to slices of raw pointers
    let inputs_ptrs: &[*const u8] = unsafe { std::slice::from_raw_parts(inputs, length) };
    let outputs_ptrs: &[*const u8] = unsafe { std::slice::from_raw_parts(outputs, length) };
    let inputs_lengths: &[usize] = unsafe { std::slice::from_raw_parts(inputs_length, length) };
    let outputs_lengths: &[usize] = unsafe { std::slice::from_raw_parts(outputs_length, length) };

    // Convert the slices of raw pointers to slices of Vec<u8>
    let inputs_vecs: Vec<Vec<u8>> = inputs_ptrs
        .iter()
        .zip(inputs_lengths)
        .map(|(&ptr, &len)| unsafe { Vec::from_raw_parts(ptr as *mut u8, len, len) })
        .collect();

    let outputs_vecs: Vec<Vec<u8>> = outputs_ptrs
        .iter()
        .zip(outputs_lengths)
        .map(|(&ptr, &len)| unsafe { Vec::from_raw_parts(ptr as *mut u8, len, len) })
        .collect();

    // Combine the slices of Vec<u8> into a single slice of tuples
    let combined_utxos: Vec<(Vec<u8>, Vec<u8>)> = inputs_vecs.into_iter().zip(outputs_vecs).collect();
    combined_utxos
}

