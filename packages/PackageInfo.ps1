$licenses = @{
  "http://www.apache.org/licenses/LICENSE-2.0.html"=@{
    url=$null
    license="Apache License 2.0"};
  "http://www.apache.org/licenses/LICENSE-2.0"=@{
    url=$null
    license="Apache License 2.0"};
  "http://commonservicelocator.codeplex.com/license"=@{
    url=$null
    license="Microsoft Public License"; }
  "http://opensource.org/licenses/mit-license.php"=@{
    url=$null
    license="MIT License"};
  "DependDB.BuildProcessor"=@{
    url="---"
    license="Closed source, RUBICON IT GmbH"};
  "DependDB.BuildProcessor.NuGetPreProcessor"=@{
    url="---"
    license="Closed source, RUBICON IT GmbH"};
  "SharpZipLib"=@{
    url="https://github.com/re-motion/IO/tree/develop/license/SharpZipLib-0.86"
    license="GPL, with exceptions for Non-GPL-licenses"};
  "http://fluentvalidation.codeplex.com/license"=@{
    url=$null
    license="Apache License 2.0"};
  "https://raw.githubusercontent.com/JetBrains/ExternalAnnotations/master/LICENSE.md"=@{
    url=$null
    license="MIT License"};
  "http://logging.apache.org/log4net/license.html"=@{
    url=$null
    license="Apache License 2.0"};
  "http://msbuildextensionpack.codeplex.com/license"=@{
    url=$null
    license="MIT License"};
  "http://www.microsoft.com/web/webpi/eula/nuget_release_eula.htm"=@{
    url=$null
    license="Microsoft Software License"};
  "http://nuget4msbuild.codeplex.com/license"=@{
    url=$null
    license="Microsoft Public License"};
  "http://nunit.org/nuget/license.html"=@{
    url=$null
    license="NUnit License"};
  "http://opensource.org/licenses/BSD-3-Clause"=@{
    url=$null
    license="3-Clause BSD License"};
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
      return $item.catalogEntry.description
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
      return $item.catalogEntry.projectUrl
    }
  }
  return "---"
}

function Get-License-Info ($package)
{
  if ($licenses[$package.Id] -ne $null)
  {
    return $licenses[$package.Id]
  }

  if ($package.LicenseUrl -ne $null)
  {
    return $licenses[$package.LicenseUrl]
  }

  return "---"
}

function Is-Development-Dependency ($projectName)
{
  if ($projectName -eq $null)
  {
    return $false
  }
  elseif ($projectName.endswith(".UnitTests") -Or $projectName.endswith(".IntegrationTests") -Or $projectName.endswith(".Build"))
  {
    return $true
  }
  else
  {
    return $false;
  }
}
  
function CreateOutput ($showDevDependencies)
{
  $result = ""
  $packageLicenseUrl = ""
  $packages = @(Get-Project -All | ? { $_.ProjectName } | % { Get-Package -ProjectName $_.ProjectName } ) | Where-Object { !$_.Id.StartsWith('Remotion') -And !$global:packageIds.Contains($_.Id) -And $(Is-Development-Dependency($_.ProjectName)) -eq $showDevDependencies} | Sort -Property @{Expression = "Id"} -Unique
  
  foreach ($package in $packages)
  {  
    $version = [string]$package.Versions.Major + '.' + [string]$package.Versions.Minor + '.' + [string]$package.Versions.Patch
    if($package.Versions.Revision -ne 0)
    {
      $version += '.' + [string]$package.Versions.Revision
    }
    
    $licenseInfo = Get-License-Info -Package $package

    $packageLicenseUrl = $package.LicenseUrl
    if ($packageLicenseUrl -eq $null)
    {
      $packageLicenseUrl = "---"
    }
    if ($licenseInfo.url -ne $null)
    {
      $packageLicenseUrl = $licenseInfo.url
    }

    $global:packageIds.Add($package.id)

    $result += "
$($package.Id)
ProjectUrl: $(Get-PackageProjectUrl -ProjectName $package.Id.ToLower() -Version $version)
License: $($licenseInfo.license)
LicenseUrl: $($packageLicenseUrl)
Description: $(Get-PackageDescription -ProjectName $package.Id.ToLower() -Version $version)
"
  }
  
  return $result
} 

$output = "Distributed Dependencies:
"

$output += CreateOutput($false)

$output +=
"
Addendum (Distributed Dependencies):

"

$output +=
"Remotion and Remotion.* (addendum for license)
This software embeds code based on 'JetBrains Annotations'.
Copyright (c) 2016 JetBrains http://www.jetbrains.com
Original software available via ReSharper -> Options -> Code Annotations -> Copy to Clipboard.
License: MIT License
LicenseUrl: https://raw.githubusercontent.com/JetBrains/ExternalAnnotations/master/LICENSE.md

"

$output +=
"Remotion.TypePipe (addendum for license)
This software embeds code based on 'Microsoft Dynamic Language Runtime'.
Copyright (c) Microsoft Corporation
ProjectUrl: https://dlr.codeplex.com
License: Apache License 2.0
LicenseUrl: https://dlr.codeplex.com/license
Relevant code (assembly 'Remotion.TypePipe.dll', namespace 'Remotion.TypePipe.Dlr') contains a copy of '/DLR_Main/Runtime/Microsoft.Scripting.Core', changeset #54115

"

$output +=
"Remotion.ObjectBinding.Web (addendum for license)
This software embeds code based on 'Autocomplete - jQuery plugin'.
Copyright (c) 2007 Dylan Verheul, Dan G. Switzer, Anjesh Tuladhar, Jörn Zaefferer
ProjectUrl: http://bassistance.de/jquery-plugins/jquery-plugin-autocomplete/
License: MIT License
LicenseUrl: http://www.opensource.org/licenses/mit-license.php

"

$output +=
"Remotion.Web (addendum for license)
This software embeds a copy of 'jQuery JavaScript Library'.
Copyright JS Foundation and other contributors, https://js.foundation/
ProjectUrl: http://jquery.com/
License: MIT License
LicenseUrl: https://jquery.org/license

"

$output +=
"Remotion.Web (addendum for license)
This software embeds code based on 'jquery.bgiframe'.
Copyright (c) 2006 Brandon Aaron (http://brandonaaron.net)
ProjectUrl: https://github.com/brandonaaron/bgiframe
License: MIT License
LicenseUrl: https://raw.githubusercontent.com/brandonaaron/bgiframe/master/LICENSE.txt
"

$output += "
###############################################################################

"


$output +=
"Development Dependencies:
"

$output += CreateOutput($true)

$output +=
"
Addendum (Development Dependencies):

"

$output +=
"RhinoMocks
ProjectUrl: http://hibernatingrhinos.com/oss/rhino-mocks
License: 2-Clause BSD License
LicenseUrl: http://opensource.org/licenses/bsd-license.php
Description: Mocking Framework

"

$output +=
"MSBuild Community Tasks
ProjectUrl: https://github.com/loresoft/msbuildtasks/
License: 2-Clause BSD License
LicenseUrl: http://opensource.org/licenses/bsd-license.php
Description: The MSBuild Community Tasks project is a collection of open source tasks for MSBuild.

"

$output +=
"SandcastleHelpFileBuilder
ProjectUrl: https://github.com/EWSoftware/SHFB
License: Microsoft Public License
LicenseUrl: http://shfb.codeplex.com/license
Description: This package allows you to deploy the Sandcastle Help File Builder tools inside of a project to build help files without installing the tools manually such as on a build server. Some limitations apply.

"

$output +=
"Selenium Core
ProjectUrl: http://www.seleniumhq.org/projects/ide/
License: Apache License 2.0
LicenseUrl: https://github.com/SeleniumHQ/selenium/blob/master/LICENSE
Description: Javascript-based testrunner web clients

"

$output +=
"Subversion Client
ProjectUrl: https://subversion.apache.org/
License: Apache License 2.0
LicenseUrl: http://www.apache.org/licenses/LICENSE-2.0
Description: Source Control Client"

$output > re-motion_3rdParty.txt

write-host Written to re-motion_3rdParty.txt