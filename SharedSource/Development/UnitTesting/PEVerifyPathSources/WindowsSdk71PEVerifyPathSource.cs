// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 

using System;
using System.IO;
using Microsoft.Win32;

#nullable enable
// ReSharper disable once CheckNamespace
namespace Remotion.Development.UnitTesting.PEVerifyPathSources
{
  partial class WindowsSdk71PEVerifyPathSource : PotentialPEVerifyPathSourceBase
  {
    public const string WindowsSdkRegistryKey35 = @"SOFTWARE\Microsoft\Microsoft SDKs\Windows\v7.1\WinSDKNetFx35Tools";
    public const string WindowsSdkRegistryKey40 = @"SOFTWARE\Microsoft\Microsoft SDKs\Windows\v7.1\WinSDK-NetFx40Tools";
    public const string WindowsSdkRegistryInstallationFolderValue = "InstallationFolder";

    public override string GetLookupDiagnostics (PEVerifyVersion version)
    {
      switch (version)
      {
        case PEVerifyVersion.DotNet2:
          return string.Format(
              "Windows SDK 7.1: Registry: HKEY_LOCAL_MACHINE\\{0}\\{1}\\PEVerify.exe",
              WindowsSdkRegistryKey35,
              WindowsSdkRegistryInstallationFolderValue);

        case PEVerifyVersion.DotNet4:
          return string.Format(
              "Windows SDK 7.1: Registry: HKEY_LOCAL_MACHINE\\{0}\\{1}\\PEVerify.exe",
              WindowsSdkRegistryKey40,
              WindowsSdkRegistryInstallationFolderValue);

        default:
          return "Windows SDK 7.1: n/a";
      }
    }

    protected override string? GetPotentialPEVerifyPath (PEVerifyVersion version)
    {
      switch (version)
      {
        case PEVerifyVersion.DotNet2:
        {
          var sdkPath = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32)
              .OpenSubKey(WindowsSdkRegistryKey35, false)
              ?.GetValue(WindowsSdkRegistryInstallationFolderValue) as string;

          if (sdkPath == null)
            return null;

          return Path.Combine(sdkPath, "PEVerify.exe");
        }

        case PEVerifyVersion.DotNet4:
        {
          var sdkPath = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32)
              .OpenSubKey(WindowsSdkRegistryKey40, false)
              ?.GetValue(WindowsSdkRegistryInstallationFolderValue) as string;

          if (sdkPath == null)
            return null;

          return Path.Combine(sdkPath, "PEVerify.exe");
        }

        default:
        {
          return null;
        }
      }
    }

  }
}
