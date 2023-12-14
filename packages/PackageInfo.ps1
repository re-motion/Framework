$licenses = @{
  "http://www.apache.org/licenses/LICENSE-2.0.html"=@{
    url=$null
    license="Apache License 2.0"};
  "http://www.apache.org/licenses/LICENSE-2.0"=@{
    url=$null
    license="Apache License 2.0"};
  "https://licenses.nuget.org/Apache-2.0"=@{
    url=$null
    license="Apache License 2.0"};
  "http://fluentvalidation.codeplex.com/license"=@{
    url=$null
    license="Apache License 2.0"};
  "http://logging.apache.org/log4net/license.html"=@{
    url=$null
    license="Apache License 2.0"};
  "https://raw.githubusercontent.com/NuGet/NuGet.Client/dev/LICENSE.txt"=@{
    url=$null
    license="Apache License 2.0"};
  "http://ironpython.codeplex.com/license"=@{
    url=$null
    license="Apache License 2.0"};

  "http://opensource.org/licenses/mit-license.php"=@{
    url=$null
    license="MIT License"};
  "https://licenses.nuget.org/MIT"=@{
    url=$null
    license="MIT License"};
  "https://raw.githubusercontent.com/JetBrains/ExternalAnnotations/master/LICENSE.md"=@{
    url=$null
    license="MIT License"};
  "https://github.com/DefinitelyTyped/NugetAutomation/blob/master/LICENSE.MIT"=@{
    url=$null
    license="MIT License"};
  "https://github.com/dotnet/corefx/blob/master/LICENSE.TXT"=@{
    url=$null
    license="MIT License"};
  "https://github.com/dotnet/standard/blob/master/LICENSE.TXT"=@{
    url=$null
    license="MIT License"};
  "http://msbuildextensionpack.codeplex.com/license"=@{
    url=$null
    license="MIT License"};
  "http://nunit.org/nuget/nunit3-license.txt"=@{
    url=$null
    license="MIT License"};
  "NUnit"=@{
    url="https://github.com/nunit/nunit/blob/master/LICENSE.txt"
    license="MIT License"};
  "NUnit.ConsoleRunner"=@{
    url="https://github.com/nunit/nunit-console/blob/main/LICENSE.txt"
    license="MIT License"};
  "NUnitTestAdapter"=@{
    url="https://github.com/nunit/nunit-vs-adapter/blob/master/LICENSE.txt"
    license="MIT License"};

  "SharpZipLib"=@{
    url="https://github.com/re-motion/IO/tree/develop/license/SharpZipLib-0.86"
    license="GPL, with exceptions for Non-GPL-licenses"};

  "http://www.microsoft.com/web/webpi/eula/nuget_release_eula.htm"=@{
    url=$null
    license="Microsoft Software License"};

  "https://opensource.org/licenses/MS-PL"=@{
    url=$null
    license="Microsoft Public License"};
  "http://commonservicelocator.codeplex.com/license"=@{
    url=$null
    license="Microsoft Public License"; }
  "http://nuget4msbuild.codeplex.com/license"=@{
    url=$null
    license="Microsoft Public License"};

  "http://nunit.org/nuget/license.html"=@{
    url=$null
    license="NUnit License"};

  "http://opensource.org/licenses/BSD-3-Clause"=@{
    url=$null
    license="3-Clause BSD License"};
  "https://licenses.nuget.org/BSD-3-Clause"=@{
    url=$null
    license="3-Clause BSD License"};
  "https://raw.githubusercontent.com/moq/moq4/master/License.txt"=@{
    url=$null
    license="3-Clause BSD License"};

  "CoreForms.Web"=@{
    url=$null
    license="Closed source, RUBICON IT GmbH"};
  "DependDB.BuildProcessor"=@{
    url=$null
    license="Closed source, RUBICON IT GmbH"};
  "DependDB.BuildProcessor.NuGetPreProcessor"=@{
    url=$null
    license="Closed source, RUBICON IT GmbH"};
  "Microsoft.NET.Test.Sdk"=@{
    url=$null
    license="Closed source, Microsoft Corporation"};
  "Microsoft.TypeScript.MSBuild"=@{
    url=$null
    license="Closed source, Microsoft Corporation"};
}

$global:packageIds = New-Object System.Collections.Generic.List[System.Object]

function Get-RegistrationsBaseUrl ()
{
  $resources = (Invoke-WebRequest -Uri "https://api.nuget.org/v3/index.json" | ConvertFrom-Json).resources
  foreach ($resource in $resources)
  {
    if ($resource.'@type' -eq 'RegistrationsBaseUrl')
    {
      return $resource.'@id'
    }
  }
}

function Get-PackageMetaData ($packageName)
{
  $registrationsBaseUrl = Get-RegistrationsBaseUrl
  $packageInfoUrl = $registrationsBaseUrl + $packageName + "/index.json"
  Try
  {
    $packageMetaData = Invoke-WebRequest -Uri $packageInfoUrl
    return $packageMetaData | ConvertFrom-Json
  }
  Catch
  {
    return "---"
  }
}

function Get-PackageDescription ($projectName, $version)
{
  $packageMetaData = Get-PackageMetaData ($projectName)
  foreach ($item in $packageMetaData.items.items)
  {
    if ($item.catalogEntry.version -eq $version)
    {
      return $item.catalogEntry.description.Replace("`r`n","<br>").Replace("`n","<br>")
    }
  }
  return "---"
}

function Get-PackageProjectUrl ($projectName, $version)
{
  $packageMetaData = Get-PackageMetaData ($projectName)
  foreach ($item in $packageMetaData.items.items)
  {
    if ($item.catalogEntry.version -eq $version)
    {
      $projectUrl = $item.catalogEntry.projectUrl
      if ($projectUrl -ne '')
      {
        return $projectUrl
      }
    }
  }
  return $null
}

function Get-License-Info ($package)
{
  if ($licenses[$package.Id] -ne $null)
  {
    return $licenses[$package.Id]
  }

  if ($package.LicenseUrl -ne $null -and $licenses[$package.LicenseUrl] -ne $null)
  {
    return $licenses[$package.LicenseUrl]
  }

  return @{
    url=$package.LicenseUrl
    license=$package.LicenseUrl};
}

function Is-Development-Dependency ($projectName)
{
  if ($projectName -eq $null)
  {
    return $false
  }
  elseif ($projectName.endswith(".UnitTests"))
  {
    return $true
  }
  elseif ($projectName.contains(".UnitTests."))
  {
    return $true
  }
  elseif ($projectName.endswith(".IntegrationTests"))
  {
    return $true
  }
  elseif ($projectName.contains(".IntegrationTests."))
  {
    return $true
  }
  elseif ($projectName.endswith(".PerformanceTests"))
  {
    return $true
  }
  elseif ($projectName.endswith(".Test"))
  {
    return $true
  }
  elseif ($projectName.startswith("SharedSource."))
  {
    return $true
  }
  elseif ($projectName.endswith(".Samples"))
  {
    return $true
  }
  elseif ($projectName.endswith(".NUnit2"))
  {
    return $true
  }
  elseif ($projectName.endswith(".RhinoMocks"))
  {
    return $true
  }
  elseif ($projectName.endswith(".Build"))
  {
    return $true
  }
  elseif ($projectName -eq "Build")
  {
    return $true
  }
  elseif ($projectName.endswith(".ClientScript"))
  {
    return $true
  }
  else
  {
    return $false
  }
}

function CreateOutput ($showDevDependencies)
{
  $result = ""
  $packageLicenseUrl = ""
  $packages = @(Get-Project -All `
    | ? { $_.ProjectName } | % { Get-Package -ProjectName $_.ProjectName } ) `
    | Where-Object { !$_.Id.StartsWith('Remotion') -And !$global:packageIds.Contains($_.Id) -And $(Is-Development-Dependency($_.ProjectName)) -eq $showDevDependencies} `
    | Sort -Property @{Expression = "Id"} -Unique

  foreach ($package in $packages)
  {  
    $version = [string]$package.Versions.Major + '.' + [string]$package.Versions.Minor + '.' + [string]$package.Versions.Patch
    if($package.Versions.Revision -ne 0)
    {
      $version += '.' + [string]$package.Versions.Revision
    }

    $packageNameAndUrl = $package.Id
    $packageUrl = Get-PackageProjectUrl -ProjectName $package.Id.ToLower() -Version $version
    if ($packageUrl -ne $null)
    {
      $packageNameAndUrl = '[' + $package.Id + '](' + $packageUrl + ')'
    }

    $licenseInfo = Get-License-Info -Package $package

    $packageLicenseUrl = $package.LicenseUrl
    if ($licenseInfo.url -ne $null)
    {
      $packageLicenseUrl = $licenseInfo.url
    }

    $global:packageIds.Add($package.id)

    $license = $licenseInfo.license
    if ($packageLicenseUrl -ne $null)
    {
      $license = '[' + $license + '](' + $packageLicenseUrl + ')'
    }

    $packageDescription = Get-PackageDescription -ProjectName $package.Id.ToLower() -Version $version

    $result += '| ' + $packageNameAndUrl + ' | ' + $package.Version + ' | ' + $license + ' | ' + $packageDescription + ' |'
    $result += "
"
  }

  return $result
}

$output = 
"Distributed Dependencies
========================

| Name | Version | License | Description |
|------|---------|---------|-------------|
"

$output += CreateOutput($false)

$output += "| JetBrains Annotations (Source Code) "
$output += "| 2017.2 "
$output += "| [MIT License](https://raw.githubusercontent.com/JetBrains/ExternalAnnotations/master/LICENSE.md) "
$output += "| This software (assemblies 'Remotion' and 'Remotion.*') embeds code based on 'JetBrains Annotations'. <br> Copyright (c) 2016 JetBrains http://www.jetbrains.com <br> Original software available via ReSharper -> Options -> Code Annotations -> Copy to Clipboard. "
$output += "|
"

$output += "| [Microsoft Dynamic Language Runtime](https://github.com/IronLanguages/main) (Source Code) "
$output += "| dated: 2012-02-27 "
$output += "| [Apache License 2.0](https://dlr.codeplex.com/license) "
$output += "| This software (assembly 'Remotion.TypePipe.dll', namespace 'Remotion.TypePipe.Dlr') embeds code from 'Microsoft Dynamic Language Runtime', now a part of 'IronPython'. <br> "
$output += "Copyright (c) Microsoft Corporation <br> Original software available from 'https://github.com/IronLanguages/main' or 'https://github.com/IronLanguages/ironpython2'. <br> "
$output += "Relevant code (/Core/Dlr in commit '3dfd952e94c66cdd7f2554602afc463053c1d146' from 2012-02-27) contains a copy of <br> "
$output += "* 'https://github.com/IronLanguages/main/tree/5846bd06372449247e62e22a374c3d41baf4b9fa/Runtime/Microsoft.Scripting.Core' or <br> "
$output += "* 'https://github.com/IronLanguages/ironpython2/tree/5846bd06372449247e62e22a374c3d41baf4b9fa/Runtime/Microsoft.Scripting.Core' <br> "
$output += "  and forked as <br> * 'https://github.com/re-motion/IronLanguages-main/tree/TypePipe-Source/Runtime/Microsoft.Scripting.Core' or <br> "
$output += "* 'https://github.com/re-motion/IronLanguages-ironpython2/tree/TypePipe-Source/Runtime/Microsoft.Scripting.Core', <br> "
$output += "originally hosted on 'https://dlr.codeplex.com', now archived as 'https://archive.codeplex.com/?p=dlr'. "
$output += "|
"

$output += "| [Autocomplete - jQuery plugin](http://bassistance.de/jquery-plugins/jquery-plugin-autocomplete/) (Source Code) "
$output += "| dated: 2008-07-12 "
$output += "| [MIT License](http://www.opensource.org/licenses/mit-license.php) "
$output += "| This software (assembly 'Remotion.ObjectBinding.Web') embeds code based on 'Autocomplete - jQuery plugin'. <br> Copyright (c) 2007 Dylan Verheul, Dan G. Switzer, Anjesh Tuladhar, Jörn Zaefferer <br> http://bassistance.de/jquery-plugins/jquery-plugin-autocomplete/ "
$output += "|
"

$output += "| [jQuery JavaScript Library](http://jquery.com/) (Source Code) "
$output += "| 1.6.4 "
$output += "| [MIT License](https://jquery.org/license) "
$output += "| This software (assembly 'Remotion.Web') embeds a copy of '[jQuery JavaScript Library](http://jquery.com/)'. <br> Copyright JS Foundation and other contributors, https://js.foundation/ "
$output += "|
"

$output += "| [jquery.bgiframe](https://github.com/brandonaaron/bgiframe) (Source Code) "
$output += "| dated: 2009-07-13 "
$output += "| [MIT License](https://raw.githubusercontent.com/brandonaaron/bgiframe/master/LICENSE.txt) "
$output += "| This software (assembly 'Remotion.Web') embeds code based on 'jquery.bgiframe'. <br> Copyright (c) 2006 Brandon Aaron (http://brandonaaron.net) "
$output += "|
"

$output += "
"


$output +=
"Development Dependencies
=========================

| Name | Version | License | Description |
|------|---------|---------|-------------|
"

$output += CreateOutput($true)

$output += "| [SandcastleHelpFileBuilder](https://github.com/EWSoftware/SHFB) "
$output += "| 1.9.5.0 "
$output += "| [Microsoft Public License](https://github.com/EWSoftware/SHFB/blob/master/LICENSE) "
$output += "| This package allows you to deploy the Sandcastle Help File Builder tools inside of a project to build help files without installing the tools manually such as on a build server. Some limitations apply. "
$output += "|
"

$output > re-motion_3rdParty.md

write-host Written to re-motion_3rdParty.md