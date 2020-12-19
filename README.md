# Example .NET Project

This project showcases how to use the
[PleOps.Cake](https://github.com/pleonex/PleOps.Cake) pipeline with an example
.NET project.

## Setup

Follow the
[checklist in PleOps.Cake](https://github.com/pleonex/PleOps.Cake/blob/develop/docs/guides/Project%20setup.md)
to adapt this template to your project.

## Build

Requirements:

- .NET 5.0 SDK
- .NET Core 3.1 runtime

Steps:

```sh
# Run these command only the first time to get the build tools
dotnet tool restore

# Build, test and stage artifacts
dotnet cake

# Just for building and testing
dotnet cake --target=BuildTest
```
