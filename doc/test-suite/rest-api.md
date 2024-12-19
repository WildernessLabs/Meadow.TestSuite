# Test Suite Worker REST API

The Test Suite Worker exposes a REST API using the [Wilderness Labs Maple Web Server](https://github.com/WildernessLabs/Meadow.Foundation/tree/main/Source/Meadow.Foundation.Libraries_and_Frameworks/Web.Maple).

A [Postman Collection is available](TestSuite.postman_collection.json) that provides examples of all REST endpoints.

| Path | Verb | Description |
| --- | --- | --- |
| `/` | GET | Gets version information about Test Suite.  Intended to be a simple test point to see if the Worker is running. |
| `/time` | GET | Gets the current UTC time of the Meadow running the Worker |
| `/time` | PUT | Sets the current UTC time of the Meadow running the Worker |
| `/assemblies` | GET | Gets a list of all loaded test assemblies |
| `/assemblies/{name}` | PUT | Uploads a test assembly with file name `name` to the configured test assembly folder |
| `/tests` | GET | Gets a list of all loaded tests IDs |
| `/tests/{testID}` | GET | Gets information about the specified Test ID |
| `/tests/{testID}` | POST | Starts a new run of the specified Test ID |
| `/results` | GET | Gets a list of all known Test Results |
| `/results/{guid}` | GET | Gets the results for the specified Result ID |
| `/results/{testID}` | GET | Gets all known results for the specified Test ID |
