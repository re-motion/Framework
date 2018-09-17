$thirdPartyPackages = @( Get-Project -All | ? { $_.ProjectName } | % { Get-Package -ProjectName $_.ProjectName } ) | Where-Object { !$_.Id.StartsWith('Remotion')  } | Sort -Unique

$output = $thirdPartyPackages | % { 
"$($_.Id)
ProjectUrl: $($_.ProjectUrl)
LicenseUrl: $($_.LicenseUrl)
Description: $($_.Description)
"
}

$output +=
"Remotion and Remotion.* (addendum for license)
This software embeds code based on 'JetBrains Annotations'.
Copyright (c) 2016 JetBrains http://www.jetbrains.com
Original software available via ReSharper -> Options -> Code Annotations -> Copy to Clipboard.
LicenseUrl: https://raw.githubusercontent.com/JetBrains/ExternalAnnotations/master/LICENSE.md (MIT License)
"

$output +=
"Remotion.TypePipe (addendum for license)
This software embeds code based on 'Microsoft Dynamic Language Runtime'.
Copyright (c) Microsoft Corporation
ProjectUrl: https://dlr.codeplex.com
LicenseUrl: https://dlr.codeplex.com/license (Apache License 2.0)
Relevant code (assembly 'Remotion.TypePipe.dll', namespace 'Remotion.TypePipe.Dlr') contains a copy of '/DLR_Main/Runtime/Microsoft.Scripting.Core', changeset #54115
"

$output +=
"Remotion.TypePipe (addendum for license)
This software embeds code based on 'JetBrains Annotations'.
Copyright (c) 2016 JetBrains http://www.jetbrains.com
Original software available via ReSharper -> Options -> Code Annotations -> Copy to Clipboard.
LicenseUrl: https://raw.githubusercontent.com/JetBrains/ExternalAnnotations/master/LICENSE.md (MIT License)
"

$output +=
"Remotion.ObjectBinding.Web (addendum for license)
This software embeds code based on 'Autocomplete - jQuery plugin'.
Copyright (c) 2007 Dylan Verheul, Dan G. Switzer, Anjesh Tuladhar, Jörn Zaefferer
ProjectUrl: http://bassistance.de/jquery-plugins/jquery-plugin-autocomplete/
LicenseUrl: http://www.opensource.org/licenses/mit-license.php
"

$output +=
"Remotion.Web (addendum for license)
This software embeds a copy of 'jQuery JavaScript Library'.
Copyright JS Foundation and other contributors, https://js.foundation/
ProjectUrl: http://jquery.com/
LicenseUrl: https://jquery.org/license (MIT License)
"

$output +=
"Remotion.Web (addendum for license)
This software embeds code based on 'jquery.bgiframe'.
Copyright (c) 2006 Brandon Aaron (http://brandonaaron.net)
ProjectUrl: https://github.com/brandonaaron/bgiframe
LicenseUrl: https://raw.githubusercontent.com/brandonaaron/bgiframe/master/LICENSE.txt (MIT License)
"

$output +=
"RhinoMocks
ProjectUrl: http://hibernatingrhinos.com/oss/rhino-mocks
LicenseUrl: http://opensource.org/licenses/bsd-license.php
Description: Mocking Framework
"

$output +=
"SharpZipLib (addendum for license)
License: GPL, with exceptions for Non-GPL-licenses
LicenseUrl: https://github.com/re-motion/IO/tree/develop/license/SharpZipLib-0.86
"

$output +=
"MSBuild Community Tasks
ProjectUrl: https://github.com/loresoft/msbuildtasks/
LicenseUrl: https://github.com/loresoft/msbuildtasks/blob/master/LICENSE
Description: The MSBuild Community Tasks project is a collection of open source tasks for MSBuild.
"

$output +=
"MSBuild.Extension.Pack
ProjectUrl: https://github.com/mikefourie/MSBuildExtensionPack
LicenseUrl: http://msbuildextensionpack.codeplex.com/license
Description: The MSBuild Extension Pack provides a collection of over 480 MSBuild Tasks, MSBuild Loggers and MSBuild TaskFactories.
"

$output +=
"NuGet.CommandLine
ProjectUrl: http://nuget.codeplex.com/
LicenseUrl: http://www.microsoft.com/web/webpi/eula/nuget_release_eula.htm
Description: NuGet Command Line Tool
"

$output +=
"NUnit.Runners
ProjectUrl: http://nunit.org/
LicenseUrl: http://nunit.org/nuget/license.html
Description: Test runner for NUnit
"

$output +=
"SandcastleHelpFileBuilder
ProjectUrl: https://github.com/EWSoftware/SHFB 
LicenseUrl: http://shfb.codeplex.com/license
Description: This package allows you to deploy the Sandcastle Help File Builder tools inside of a project to build help files without installing the tools manually such as on a build server. Some limitations apply.
"

$output +=
"Selenium Core
ProjectUrl: http://www.seleniumhq.org/projects/ide/
LicenseUrl: https://github.com/SeleniumHQ/selenium/blob/master/LICENSE
Description: Javascript-based testrunner web clients
"

$output +=
"Subversion Client
ProjectUrl: https://subversion.apache.org/
LicenseUrl: http://www.apache.org/licenses/LICENSE-2.0
Description: Source Control Client
"

$output +=
"Used only in development / build tooling for re-motion and is not distributed with re-motion:
* MSBuild
* NuGet
* NUnit
* RhinoMocks
* SandcastleHelpFileBuilder
* Selenium Core
* Subversion Client
"

$output > re-motion_3rdParty.txt

write-host Written to re-motion_3rdParty.txt