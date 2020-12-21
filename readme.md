# Wilderness Labs Meadow.TestSuite (Beta)

Meadow.TestSuite is intended to provide a remote-controllable test infrastructure for the Wilderness Labs Meadow.  It provides a mechanism to push test assemblies to a device, enumerate assemblies and test methods, selectively run test methods, and retrieve test results.

A goal is to provide a API that at least feels like xUnit, or a subset of xUnit, to facilitate easier test creation.  Direct use of xUnit, at least right now, is not a goal since the tests must be run on-device and control has to be handled by a meadow-specific transport layer.

[Hardware Setup](setup.md)

[Using TestSuite](usage.md)

[Authoring TestSuite Tests](authoring-tests.md)

[TestSuite Implementation Details](implementation.md)

