# `Tests.Meadow.Core` Test Assembly

This test assembly is for testing the classes and features of the `Meadow.Core` Library (i.e. `Meadow.dll`).

## GPIO and Interrupt Tests 

> NOTE: Since tese tests are loopbacks, they can only test an *even* number of pins.  meadow has an odd number, so these tests do not test the `SCK` pin at all.
> Note2: D00 and D01 on the DUT are used for the serial communication from the ATE and are therefore also not tested.
 
These tests are designed to use a fixture with 0-ohm loopbacks on the following GPIOs:

```
A00 <--> A01
A02 <--> A03
A04 <--> A05
SCK <-- [NC]
MOSI <--> MISO
D00 <-- [ATE Serial]
D01 <-- [ATE Serial]
D03 <--> D04
D05 <--> D06
D07 <--> D08
D09 <--> D10
D11 <--> D12
D13 <--> D44
D15 <--> D02
```