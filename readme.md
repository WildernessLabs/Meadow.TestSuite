# Meadow Test and Validation

(updated 19 Dec 2024)

This repository contains all of the Meadow Test and Validation tools, libraries, etc.  These can be broken down into several categories.

## Workbench

`Meadow.Workbench` is a cross-platform (Avalonia) app that provides a UI around the Meadow CLI features. Among other things, it provides the ability to manage the local firmware builds, flash firmware files, manage a connected device's file system, and view device output logging.

## TestSuite

TestSuite is envisioned to be a hardware-in-the-loop test infrastructure.  

[Much more detail can be found in the TestSuite ReadMe.](doc/test-suite-readme.md)

It consists of 2 parts:

### Test Director

The Director manages running tests, getting results, etc.  It would run on a development or host machine and it's job it to determine what tests are available, what should be run, what results exist, etc.

The Director communicates with a Worker over either Serial or Ethernet.

### Test Worker

The Worker runs on target hardware - the device under test (DUT).  The Worker can load an assembly that contains tests.  It can enumerate those tests back to the Director.  It can also execute tests and managed the results.

Test Assemblies look and feel like xUnit tests (they are called "mUnit").

Work stalled on TestSuite because Mono at the time had serious problems with loading test assemblies via reflection, and while the plumbing all seemed to work, getting actual tests to run was painful.

## Validation

This folder is a mess and contains things that were part of a more "product-like" thing called `Meadow.Validation` and just ad-hoc validation apps.

### Meadow.Validation
Since TestSuite was not working well due to Reflection issues on device, `Meadow.Validation` was created.  Validation tests are designed as applications that simply run right on the DUT, with no overall coordination.

Meadow.Validation has some structure to allow you to define a `MeadowTestDevice` and then define `ITest` implementations for that hardware.

In here you will find some tests for broader coverage as well as device-specific things such as:

| name | purpose |
| --- | --- |
| F7Feather_v2 | This contains a set of automated tests for the broader OS capabilities.  File system, WiFi, SQLite, Bluetooth, etc.  It's a reasonable example of how to author tests for Validation |
| DiscreteLoopback_v2 | These app is designed to plug into a breadboard (with the idea tha specific hardware would be created for just this test).  The breadboard has loopback jumpers for almost all pins (the pin count is odd, so one is missing).  It then runs automated tests for inputs, outputs, interrupts, PushButtons, toggle speed, etc.  It's a very complete and robust test for I/O on the Feather. |

### LongTermMapleServiceTest

This is an app that, like the name suggests, is designed to do long-run testing of Maple to help test some 1.x issues we had with memory and reliability

### Meadow.Validation.ProjectLab

This is another stand-alone application that is an example of how you might create an extensible, ever-growing set of tests for a DUT.

`Meadow.Validation.ProjectLab` allow you to follow an interface and create new tests that plug into a test framework.  These tests are generally designed to be **user interactive**.  For example, it might ask the user to "press the UP button" and it will validate that the action occurred as expected.

One newer feature of `Meadow.Validation.ProjectLab` is that upon completing a run of tests, it provides the option of publishing the test results to Meadow.Cloud to allow a record of test run history to be generated.  The cloud endpoint and a simple web interface to browse results already exists.

### Network

The network foder contains a lot of long-running soak tests for validation issues that we've run across in the past