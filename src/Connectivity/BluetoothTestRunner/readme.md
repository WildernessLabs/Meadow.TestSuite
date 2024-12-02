# Bluetooth Test Runner

The Bluetooth test runner is designed to run on a Raspberry Pi computer.  The application will interrogate the Bluetooth adapter on the Meadow board and read / write characteristics on the board.  Each test will run multiple times to verify memory stability.

The test application is written in Rust and requires the installation of the `blurz` library.

## Prerequisites

### Install Rust

Log on to the Raspberry Pi and follow the instructions on the [Install Rust](https://www.rust-lang.org/tools/install) web page.

### Install Support Libraries

Log on to the Raspberry Pi and execute the following commands:

```bash
sudo apt install libdbus-1-1-dev pkg-config
cargo install bluer-tools
. "$HOME/.cargo/env"
```

## Execute the Test Application

A quick of the system means that the Rust application cannot detect devices correctly unless `blumon` is running.

* Open a `ssh` console to the Raspberry Pi and run `blumon` and wait for `Meadow F7` to appear in the device list.
* Open a second `ssh` console to the Raspberry Pi and execute the test application by running the command `cargo run -- <test to run>` from the directory holding this Readme.

The <tests to run> option will control which of the Bluetooth characteristics to be exercised.  Valid options are:

* `number`
* `onoff`
* `text`
* `boolean`
* `all` - runs all of the above.

So `cargo run -- number` will run the number characteristic tests.

### Service Cannot be Found

This message may be generated the first time the Rust application is run.  This can be fixed by re-running the application.

### Large Number of Writes Fail

The Rust application currently fails after ~950 writes to the test characteristic.
