# Using Wilderness Labs Meadow.TestSuite

TestSuite supports two usage models, via CLI or API, both through the Director.

## CLI (`mtd`)

The Meadow TestSuite Director compiles to an executable named `mtd.exe` which accepts command-line parameters as follows:

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


*Examples*

```
> mtd test -p COM12 -l
> mtd test -p COM12 -e Tests.Meadow.Core.GpioTests.LedTest
> mtd test -p COM12 -e Tests.Meadow.Core.GpioTests.LedTest|Tests.Meadow.Core.OtherTetsts.RamTest
```

#### `result`<a name="result"></a>

[Not yet implemented]

### API

[TBD]
