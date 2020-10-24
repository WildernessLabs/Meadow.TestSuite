# Using Wilderness Labs Meadow.TestSuite

TestSuite supports two usage models, via CLI or API, both through the Director.

## CLI (`mtd`)

The Meadow TestSuite Director compiles to an executable named `mtd.exe` which accepts command-line parameters as follows:

Currently, `mtd` only supports the serial transport from the test PC to the Meadow. See the [Setup Readme](setup.md) for more details.

### Basic Commands

- [uplink](#uplink)
- [assembly](#assembly)
- [test](#test)
- [result](#result)

#### `uplink`<a name="uplink"></a>

```
> mtd uplink -p <SERIAL_PORT> -s <SOURCE_FILE_NAME> [-d <DEST_FILE_NAME>]
```

- `-p|--port <SERIAL_PORT>`

    Required. Serial port used to communicate with the Meadow.

- `-s|--source <SOURCE_FILE_NAME>`

    Required. Relative or absolute path to a local assembly to uplink to the Meadow.

- `[-d|--destination <DEST_FILE_NAME>]`

    Optional. Name of file to be written in the Meadow test directory.  When omitted the source file name is used.

*Examples*

```
mtd uplink -p COM12 -s "..\..\..\..\Tests.Meadow.Core\bin\Debug\net472\Tests.Meadow.Core.dll"
```

#### `assembly`<a name="assembly"></a>

```
> mtd assembly -p <SERIAL_PORT> -l
```

- `-p|--port <SERIAL_PORT>`

    Required. Serial port used to communicate with the Meadow.

- `-l|--list`

    Optional. Retrieves a list of all known test assemblies on the Meadow

*Examples*

```
> mtd assembly -p COM12 -l
```


#### `test`<a name="test"></a>

```
> mtd test -p <SERIAL_PORT> [-l] [-e <TEST_NAME_LIST>]
```

- `-p|--port <SERIAL_PORT>`

    Required. Serial port used to communicate with the Meadow.

- `-l|--list`

    Optional. Retrieves a list of all known tests on the Meadow

- `-e|--execute`

    Optional. A delimited list of test methods to execute.  Delimiters allowed: `,`, `;`, `|`  
    A *trailing* wildcard character (i.e. '*') is allowed.


*Examples*

```
> mtd test -p COM12 -l
> mtd test -p COM12 -e Tests.Meadow.Core.GpioTests.LedTest
> mtd test -p COM12 -e Tests.Meadow.Core.GpioTests.LedTest|Tests.Meadow.Core.OtherTetsts.RamTest
> mtd test -p COM12 -e Tests.Meadow.Core.LEDTests.*
> mtd test -p COM12 -e *
```

#### `result`<a name="result"></a>

```
> mtd result [-a|--all] [-r|--result-id] [-t|--test-id]
```

- `-p|--port <SERIAL_PORT>`

    Required. Serial port used to communicate with the Meadow.

- `-a|--all`

    Optional. Retrieves a list of all known test results on the Meadow

- `-r|--result-id`

    Optional. Retrieves the result with the provided result ID (GUID)

- `-t|--test-id`

    Optional. Retrieves a list of all results for the specified Test ID/name

*Examples*

```
> mtd result -p COM12 --all
> mtd result -p COM12 -t Tests.Meadow.Core.GpioTests.LedTest
> mtd result -p COM12 -r 69383647-df22-4c84-bbd9-677a1b6c19a9
```

## API

[TBD]

## Known Issues

1. Currently the Worker must interpret a fair bit of deserialization code on the first call. If your first call is to upload an assembly, the interpreter starves the processor and the serial port will overflow, causing the assemly upload to fail.  The Worker detects this condition and the corrupted assembly will not be written to the device but it means that the uplink must be done a second time.