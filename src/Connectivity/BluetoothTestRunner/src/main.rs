use core::{panic, str};
use regex::Regex;
use std::thread;
use std::time::Duration;
use lazy_static::lazy_static;
use blurz::{
    BluetoothSession,
    BluetoothAdapter,
    BluetoothDiscoverySession,
    BluetoothDevice,
    BluetoothGATTService,
    BluetoothGATTCharacteristic,
    // BluetoothGATTDescriptor
};

/// Regular expression to match a UUID.
const UUID_REGEX: &str = r"([0-9a-f]{8})-(?:[0-9a-f]{4}-){3}[0-9a-f]{12}";

/// UUID of the on/off characteristic.
const ON_OFF_UUID: &str = "016e99d6-8a61-11eb-8dcd-0242ac1300ff";

/// UUID of the boolean characteristic.
const BOOLEAN_UUID: &str = "017e99d6-8a61-11eb-8dcd-0242ac1300aa";

/// UUID of the number characteristic.
const NUMBER_UUID: &str = "018e99d6-8a61-11eb-8dcd-0242ac1300bb";

/// UUID of the text characteristic.
const TEXT_UUID: &str = "019e99d6-8a61-11eb-8dcd-0242ac1300cc";

/// GAT Service ID
const GAT_SERVICE_ID: u16 = 0xfd;

/// Number of times each test should be run.
const NUMBER_OF_ITERATIONS: u32 = 1000;

/// Delay between writes and read operations.
const MILLISECONDS_DELAY: u64 = 500;

/// Test the On/Off characteristic.
fn test_on_off_characteristic(characteristic: &BluetoothGATTCharacteristic) {
    println!("On/Off characteristic (UUID: {:?}) test started", ON_OFF_UUID);

    let on: u32 = 1;
    let off: u32 = 0;
    let mut counter: u32 = 0;
    while counter < NUMBER_OF_ITERATIONS {
        characteristic.write_value(Vec::from(on.to_le_bytes()), None).unwrap();
        thread::sleep(Duration::from_millis(MILLISECONDS_DELAY));
        let value = characteristic.read_value(None).unwrap();
        let value = u32::from_le_bytes(value.try_into().unwrap());
        if value != on {
            panic!("Failed to write value: {on}, read {value}");
        }
        characteristic.write_value(Vec::from(off.to_le_bytes()), None).unwrap();
        thread::sleep(Duration::from_millis(250));
        let value = characteristic.read_value(None).unwrap();
        let value = u32::from_le_bytes(value.try_into().unwrap());
        if value != off {
            panic!("Failed to write value: {off}, read {value}");
        }
        counter += 1;
    }

    println!(" - PASS");
}

/// Test setting a boolean characteristic.  Note that the managed application has this set to read-only.
fn test_boolean_characteristic(characteristic: &BluetoothGATTCharacteristic) {
    println!("Boolean characteristic (UUID: {:?}) test started", ON_OFF_UUID);

    let on: u32 = 1;
    let mut counter: u32 = 0;
    while counter < NUMBER_OF_ITERATIONS {
        match characteristic.write_value(Vec::from(on.to_le_bytes()), None) {
            Err(_e) => counter += 1,
            _ => panic!("Failed managed to write read-only boolean value")
        };
    }

    println!(" - PASS");
}

/// Test the number characteristic by writing a value and reading it back multiple times.
fn test_number_characteristic(characteristic: &BluetoothGATTCharacteristic) {
    println!("Number characteristic (UUID: {:?}) test started", NUMBER_UUID);

    let mut counter: u32 = 0;
    while counter < NUMBER_OF_ITERATIONS {
        characteristic.write_value(Vec::from(counter.to_le_bytes()), None).unwrap();
        thread::sleep(Duration::from_millis(MILLISECONDS_DELAY));
        let value = characteristic.read_value(None).unwrap();
        let value = u32::from_le_bytes(value.try_into().unwrap());
        if value != counter {
            panic!("Failed to write value: {counter}");
        }
        counter += 1;
    }

    println!(" - PASS");
}

/// Test writing to a long text characteristic.
fn test_text_characteristic(characteristic: &BluetoothGATTCharacteristic) {
    println!("Text characteristic (UUID: {:?}) test started", TEXT_UUID);

    let mut counter = 0;
    while counter < NUMBER_OF_ITERATIONS {
        let value  = format!("Hello from the Wilderness Labs Bluetooth test suite, iteration: {counter}");
        characteristic.write_value(Vec::from(value.as_bytes()), None).unwrap();
        thread::sleep(Duration::from_millis(MILLISECONDS_DELAY));
        let read_value = characteristic.read_value(None).unwrap();
        let read_value = match String::from_utf8(read_value) {
            Ok(v) => v,
            Err(_e) => panic!("Error reading string characteristic")
        };
        let length = value.len() - 1;
        if read_value[0..length] != value[0..length] {
            panic!("Text does not match, '{value}' and '{read_value}'.");
        }
        counter += 1;
    }

    println!(" - PASS");
}

/// Find the characteristics for the service with the GAT_SERVICE_ID and execute the tests.
fn execute_tests(session: &BluetoothSession, characteristics: Vec<String>) {
    for characteristic_path in characteristics {
        let characteristic = BluetoothGATTCharacteristic::new(session, characteristic_path.clone());
        lazy_static! {
            static ref RE: Regex = Regex::new(UUID_REGEX).unwrap();
        }
        let uuid = characteristic.get_uuid().unwrap();
        match uuid.as_str() {
            ON_OFF_UUID => test_on_off_characteristic(&characteristic),
            BOOLEAN_UUID => test_boolean_characteristic(&characteristic),
            NUMBER_UUID => test_number_characteristic(&characteristic),
            TEXT_UUID => test_text_characteristic(&characteristic),
            _ => ()
            // _ => panic!("Unknown characteristic: {:?}", uuid)
        }
    }
}

/// Find the characteristics for service with the GAT_SERVICE_ID
fn find_characteristics(bt_session: &BluetoothSession, device_path: String) -> Vec<String> {
    let device = BluetoothDevice::new(&bt_session, device_path);
    if let Err(e) = device.connect(10000) {
        panic!("Failed to connect {:?}: {:?}", device.get_id(), e);
    }

    lazy_static! {
        static ref RE: Regex = Regex::new(UUID_REGEX).unwrap();
    }

    let mut counter = 0;
    while counter < 240 {
        let services_list = match device.get_gatt_services() {
            Ok(services) => services,
            Err(e) => {
                panic!("Failed to get services: {:?}", e);
            }
        };
        
        for service_path in services_list {
            let service = BluetoothGATTService::new(&bt_session, service_path.clone());
            let uuid = service.get_uuid().unwrap();
            let assigned_number = RE.captures(&uuid).unwrap().get(1).map_or("", |m| m.as_str());
    
            let service_id = u16::from_str_radix(assigned_number, 16).unwrap();
            if GAT_SERVICE_ID == service_id {
                match service.get_gatt_characteristics() {
                    Ok(characteristics) => return characteristics,
                    Err(e) => {
                        panic!("Failed to get characteristics: {:?}", e);
                    }
                };
            }
        }
        counter += 1;
    }
    panic!("Service not found");
}

/// Find the Meadow device with the name "Meadow F7" and return the BluetoothSession and device path
fn find_meadow() -> (BluetoothSession, String) {
    let mut counter = 0;

    let bt_session = BluetoothSession::create_session(None).unwrap();
    let adapter: BluetoothAdapter = BluetoothAdapter::init(&bt_session).unwrap();
    if let Err(_error) = adapter.set_powered(true) {
        panic!("Failed to power adapter");
    }
    let discover_session = BluetoothDiscoverySession::create_session(&bt_session, adapter.get_id()).unwrap();
    while counter < 240 {
        if let Err(_error) = discover_session.start_discovery() {
            panic!("Failed to start discovery");
        }
        let device_list = adapter.get_device_list().unwrap();

        discover_session.stop_discovery().unwrap();
        for device_path in device_list {
            let device = BluetoothDevice::new(&bt_session, device_path.clone());
            if !device.get_name().ok().is_none() {
                let device_name = device.get_name().ok().unwrap();
                if device_name == "MeadowF7" {
                    return (bt_session, device_path);
                }
            }
        }
        thread::sleep(Duration::from_millis(500));
        counter += 1;
    }

    panic!("Meadow device not found");
}

/// Main program loop for testing the Bluetooth functionality on a Meadow F7 board.
fn main() {
    println!("Bluetooth Test Driver");
    let (bt_session, device_path) = find_meadow();

    let characteristics = find_characteristics(&bt_session, device_path.clone());
    execute_tests(&bt_session, characteristics);
}