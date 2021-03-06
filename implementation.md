# Wilderness Labs Meadow.TestSuite

Below are details on the implementation.

## Architecture

Broadly speaking, TestSuite consists of two pieces.

Right now architecture is very, very fluid. Once this solidify a bit more, a diagram of the architecture will be put here.  

Goals:
- Support Serial transport
- Support TCP transport
- Provide a `Director` CLI
- Provide a `Director` REST interface
- Provide a UI to allow control of the Director
- Provide `Director` storage to keep history of test runs and results
- `Director` can:
  - Send Test Assemblies from `Director` to `Worker`
  - Get a list of test assemblies on the `Worker`
  - Get a list of Test methods on a given test assemly
  - Request the `Worker` run a specific test or set of tests
  - Get a list of known test results from the `Worker`
  - Get result data for a given test run

### Director

The `Director` is a .NET Core app that runs on a PC. The `Director` provides an API  (probably even a REST one in the future) and a CLI that allows a user to control testing.  

The `Director` communicates with 1:n `Workers` via an `ITestTransport` layer to send commands and files as well as retrieve results.  

An `ICommandSerializer` is used to abstract the serializaation mechanism for the commands and command results.

### Worker

The `Worker` is an application that runs on the Meadow.  It uses an `ITestTransport` that corresponds to that beng used by the `Director` to receive commands and command data. The `Worker` uses a matching `ICommandSerializer` to deserialize commands received from the `Director`.

### ITestTransport

#### Serial

Currently the only tested mechanism of data transport has been on the COM4 USB serial interface.  There are significant limitations on the transport layer due to a variety of factors. Current max throughput has been measured to be ~970 bytes/sec.

- Do not use transfer speeds of > 9600bps.  Anything faster seems to flood the handler on the Meadow causing buffer overruns and file corruption.
- Do not use transfer packet sizes > 255 bytes.  It appears that anything beyond 255 bytes coming into the Meadow get thrown on the floor with no error or warning, causing file corruption.
- Sometimes a delay is needed between packet transfers.  This has not been fully profiled and is not fully understood.  Anecdotally, the first file transfer requires ~100ms between packets but once that transfer is done, no delay is required.  Further test and understanding is required

### ICommandSerializer

Currently there are several candidate command serialization schemes, all of which are different states of implementation and test.  Below is a list of each and notes on state of development and requirements known.

**Deserialization Speed Results**
Testing a single command to uplink a 28-byte file.

| Serialization | First Run Deserialization | Second Run Seserialization |
| --- | --- | --- |
| protobuf-net | untested | untested |
| JSON.NET | 24062ms | 13ms |
| System.Text.Json | 8317ms | 214ms |
| SimpleJson | 3234ms | 33ms|



#### Protocol Buffers (`protobuf-net`)
This is stubbed out, but has not been tested beyond getting Missing Assembly exceptions when run.  Needs further testing and documentation on what BCL libraries are required to make it run.

BCL assemblies required to run:
- TBD


#### JSON (`System.Text.Json`)

*This is the default serialization library* used by `TestSuite` and is confirmed working.

The following files were deployed, but is likely far more than necessary.  This list needs to be trimmed to those actually needed.

```
/meadow0/Meadow.dll
/meadow0/mscorlib.dll
/meadow0/System.dll
/meadow0/App.exe
/meadow0/Meadow.TestSuite.Core.dll
/meadow0/System.Core.dll
/meadow0/System.Memory.dll
/meadow0/System.Runtime.CompilerServices.Unsafe.dll
/meadow0/Microsoft.Bcl.AsyncInterfaces.dll
/meadow0/System.Buffers.dll
/meadow0/System.Text.Encodings.Web.dll
/meadow0/System.Threading.Tasks.Extensions.dll
/meadow0/System.ValueTuple.dll
/meadow0/System.Text.Json.dll
/meadow0/System.Numerics.Vectors.dll
/meadow0/Mono.Security.dll
/meadow0/System.Runtime.Serialization.dll
/meadow0/System.ServiceModel.Internals.dll
/meadow0/System.Numerics.dll
/meadow0/System.Transactions.dll
/meadow0/System.EnterpriseServices.dll
/meadow0/System.Data.dll
/meadow0/System.Configuration.dll
/meadow0/System.Security.dll
/meadow0/System.Xml.Linq.dll
/meadow0/System.Xml.dll
```

#### JSON (Newtonsoft `JSON.NET`)
Basic testing of serialization is confirmed working.  `JSON.NET` has a large memory footprint and often causes the Medow to run out of memory, especially when transferring an assembly.

It is **not currently recommended** that you use `JSON.NET.

JSON.NET is very slow on the first call while all of the infrastucture is interpreted

The following files were deployed, but is likely far more than necessary.  This list needs to be trimmed to those actually needed.

```
// TODO: fill this out
```

#### JSON (`SimpleJson`)
This has been somewhat tested.  Currently small files (100 bytes) transfer just fine.  Transferring a 6k file transferred all bytes successfully, but it was unable to deserialize to object.  It may be that the library is limited in its capacity to handle large strings (the file is Base64-encoded into a field).  Specific limits have not been narrowed.

SimpleJson, not surprisingly, is the fastest deserializer tested so far.

Basic testing of transferring a a small file has been confirmed. larger files and full usage has not been tested.

BCL assemblies required to run:
- `System.Memory.dll`
- `System.Runtime.CompilerServices.Unsafe.dll`
