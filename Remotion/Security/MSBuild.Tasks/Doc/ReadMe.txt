Using the ExtractSecurityMetadata task in your project
======================================================

Add the following lines to your project file ("<project name>.csproj").

<UsingTask TaskName="Remotion.Security.MSBuild.Tasks.ExtractSecurityMetadata" 
           AssemblyFile="..\..\Security\MSBuild.Tasks\bin\Debug\Remotion.Security.MSBuild.Tasks.dll" />
           
<Target Name="AfterBuild">
  <ExtractSecurityMetadata Assemblies="$(TargetPath)" OutputFilename="$(TargetDir)security-metadata.xml" />
</Target>
