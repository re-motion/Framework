# TypeScript build infrastructure README

This document outlines the build infrastructure used in re-motion to build TypeScript.

There are currently two projects that use the TS build infrastructure:

- `Remotion.Web.ClientScript`
- `Remotion.ObjectBinding.Web.ClientScript` (references `Remotion.Web.ClientScript`)

## General Setup

To simplify the integration with the C# build, TS projects are also modelled as C# projects but their C# output is not used. Such projects have the `JavaScriptLibrary` project type.

TypeScript compilation is enabled using the `Microsoft.TypeScript.MSBuild` Nuget package. While it is possible to configure TypeScript properties using MSBuild properties, we opted to use `tsconfig.json` files instead. The `Microsoft.TypeScript.MSBuild` Nuget package will build any `tsconfig.json` file that is added to MSBuild item group `Content`.

The `Scripts` folder contains the TypeScript files that are compiled as part of the project. There are regular TypeScript files `*.ts` and TypeScript definition files `*.d.ts`, with the definition files being located in the `typings` subfolder. `common.d.ts` defines common types and type helpers that we created, while further subfolders contains typings for certain libraries (e.g. microsoft-ajax).

## Debug vs. Release

The build setup is made more complex by the fact that different Debug/Release configurations need to be supported. This is not supported by TypeScript by default, and thus requires some trickery. There is a `tsconfig.json` file in the project root (Build action: `None`) which will not be used for the compilation but only acts as config file for IDEs. It acts as a "base configuration" but on its own will not emit any outputs as `noEmit` is set.

Instead, there are two additional `tsconfig.json` files located in `~/tsconfig/debug` and `~/tsconfig/release`. These inherit from the `tsconfig.json` in the root directory and contain configuration specific settings. Their output is emitted in the `bin/Debug/dist` or `bin/Release/dist` folders. This convoluted folder setup is necessary as the `Microsoft.TypeScript.MSBuild` Nuget package searches for files named `tsconfig.json`, which makes it impossible to have different configurations for Debug/Release in the same folder. `JavaScriptLibrary.props` adds the correct `tsconfig.json` file as `Content` depending on the current build configuration.

## Output files

There are different kinds of output files generated in the output folder `bin/<Configuration>/dist`. Only the `*.js` files are actually necessary for downstream projects but the other files provide additional functionality:

- `*.js.map` files provide source mapping support for the generated JavaScript files. Serving these with the JavaScript files will allow browsers to display the TypeScript code in the debugger instead of the generated JavaScript files.
- `*.d.ts` files provide type definitions for the generated files. Downstream projects can reference these if they themselves have a TypeScript build setup and need to reference functions/types from the generated JavaScript files.
- `*.tsbuildinfo` files are only used for project references (see section below) and do not need to be shipped.

## Project references

References between different TypeScript projects are done using TypeScript's project references. The `Remotion.ObjectBinding.Web.ClientScript` project, for example, has a project reference on `Remotion.Web.ClientScript`, which can be found in all three `tsconfig.json` files. The Debug/Release files reference their respective `tsconfig.json` in the other project. The root "IDE" `tsconfig.json` references the debug version in the other project. This is necessary as it is not possible to reference the other project's "IDE" `tsconfig.json` as it does not emit any output.

There are requirements for the referenced project and the referencing project for the project reference to function. The referenced project must have `"composite": true` and must have a `baseUrl` set. The referencing project on the other hand has the `"references": { "path": "..." }` entry and must also have the MSBuild property `<TypeScriptBuildMode>true</TypeScriptBuildMode>` set.

## Using the JS output in other projects

To use the JS output in the `dist` file in another project, they need to be included. For example:

```xml
<ItemGroup>
  <Content Include="..\ClientScript\bin\$(Configuration)\dist\*" LinkBase="Res.ClientScript\HTML" CopyToOutputDirectory="Never">
    <Pack>true</Pack>
    <PackagePath>res/HTML</PackagePath>
  </Content>
</ItemGroup>
```

