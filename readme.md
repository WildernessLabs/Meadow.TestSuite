# Wilderness Labs Meadow.TestSuite (Beta)

Meadow.TestSuite is intended to provide a remote-controllable test infrastructure for the Wilderness Labs Meadow.  It provides a mechanism to push test assemblies to a device, enumerate assemblies and test methods, selectively run test methods, and retrieve test results.

A goal is to provide a API that at least feels like xUnit, or a subset of xUnit, to facilitate easier test creation.  Direct use of xUnit, at least right now, is not a goal since the tests must be run on-device and control has to be handled by a meadow-specific transport layer.

[Using a Raspberry Pi as the Test Director](doc/raspi.md)

[Running the TestDirector from a Container](deploy/readme.md)

[Hardware Setup](doc/setup.md)

[Using TestSuite](doc/usage.md)

[Authoring TestSuite Tests](doc/authoring-tests.md)

[TestSuite Implementation Details](doc/implementation.md)

## Beta Notes

Currently the Beta source has references to local projects for the full source of Meadow Core and Meadow Foundation since we're using it to find and fix bugs in both and it makes things a bit easier internally. If you don't have the source for those, however, it leads to compile errors like this:

```
Error	NU1104	Unable to find project '...\Meadow.Foundation\Source\Meadow.Foundation.Core\Meadow.Foundation.Core.csproj'. Check that the project reference is valid and that the project file exists.	Tests.Meadow.Foundation	{My Repo Folder}\Source\Repos\Meadow.TestSuite\Tests.Meadow.Foundation\Tests.Meadow.Foundation.csproj
```

This is solved by changing from the local project reference to using the Nuget packages instead.

### Update `Meadow.TestSuite.Worker.csproj`

Old:
```
<Project Sdk="Meadow.Sdk/1.1.0">
  ...
  <ItemGroup>
    <ProjectReference Include="..\..\Meadow.Core\source\Meadow.Core\Meadow.Core.csproj" />
    <ProjectReference Include="..\..\Meadow.Foundation\Source\Meadow.Foundation.Core\Meadow.Foundation.Core.csproj" />
    <ProjectReference Include="..\Meadow.TestSuite.Core\Meadow.TestSuite.Core.csproj" />
  </ItemGroup>
</Project>
```
New:
```
<Project Sdk="Meadow.Sdk/1.1.0">
  ...
  <ItemGroup>
    <PackageReference Include="Meadow" Version="0.18.0" />
    <PackageReference Include="Meadow.Foundation" Version="0.20.0" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\Meadow.TestSuite.Core\Meadow.TestSuite.Core.csproj" />
  </ItemGroup>
</Project>
```

### Update `Tests.Meadow.Core.csproj` and `Tests.Meadow.Foundation.csproj`

Old (similar to):
```
<Project Sdk="Meadow.Sdk/1.1.0">
  ...
  <ItemGroup>
    <PackageReference Include="Meadow.Foundation" Version="0.*" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\..\Meadow.Core\source\Meadow.Core\Meadow.Core.csproj" />
    <ProjectReference Include="..\Meadow.TestSuite.Core\Meadow.TestSuite.Core.csproj" />
  </ItemGroup>
</Project>
```
New:
```
<Project Sdk="Meadow.Sdk/1.1.0">
  ...
  <ItemGroup>
    <PackageReference Include="Meadow" Version="0.18.0" />
    <PackageReference Include="Meadow.Foundation" Version="0.*.0" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\Meadow.TestSuite.Core\Meadow.TestSuite.Core.csproj" />
  </ItemGroup>
</Project>
```
