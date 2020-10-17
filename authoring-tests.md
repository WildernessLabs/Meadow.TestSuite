# Authoring Wilderness Labs Meadow.TestSuite tests

`TestSuite` tests are designed to feel like xUnit.  Simply reference the `Meadow.TestSuite.Core` assembly and include the namespace `Munit`.

`mUnit` is a small subset of the xUnit API.  It currently supports:

- `FactAttribute`
- `Assert`
  - `True(bool condition)`
  - `False(bool condition)`
  - `NotNull(object o)`
  - `Assert.Null(object o)`
  - `Assert.Equal(object expected, object actual)`

## `Device` Access

Many mUnit tests will want access to Meadow device hardware.  This is achieved using property injection at test runtime.

To use the Device, simply expose a public, settable property of the type `IIODevice` (this requires a reference to `Meadow.Core`).  The `TestSuite` Worker will inject an instance of the Device at test run time.

## Example

Below is a simple example of an mUnit test.  `Console.WriteLine` calls will output through the Meadow's console UART, *not* back tot he Director.

Note the use of the `IIODevice` property for hardware access.

> Note: Since `mUnit` tests only have an interface access to the IIODevice, they do not have direct access to read-only F7Micro Pin Properties like `Device.Pins.D01`.  Instead pins must be retrieved by Name or ID.  
> For most IPins, this is just a simple string representation.  For example `Device.Pins.D02` would translate to an mUnit call of `Device.GetPin("D02")`.

```
using Munit;

public class MyTests
{
    public IIODevice Device { get; set; }

    [Fact]
    public void TestA()
    {
        Console.WriteLine("Starting LedTest");

        var green = Device.GetPin("OnboardLedGreen");

        Assert.NotNull(green);

        var led = Device.CreateDigitalOutputPort(green);
        led.State = true;
        Thread.Sleep(1000);
        led.State = false;

        Console.WriteLine("LedTest Complete");
    }
}
```
