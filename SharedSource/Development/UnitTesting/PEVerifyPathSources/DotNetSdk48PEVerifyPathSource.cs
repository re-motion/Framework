// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0
using System;
using System.IO;
using Microsoft.Win32;

#nullable enable
// ReSharper disable once CheckNamespace
namespace Remotion.Development.UnitTesting.PEVerifyPathSources
{
  partial class DotNetSdk48PEVerifyPathSource : PotentialPEVerifyPathSourceBase
  {
    public const string WindowsSdkRegistryKey40 = @"SOFTWARE\Microsoft\Microsoft SDKs\NETFXSDK\4.8\WinSDK-NetFx40Tools";
    public const string WindowsSdkRegistryInstallationFolderValue = "InstallationFolder";

    public override string GetLookupDiagnostics (PEVerifyVersion version)
    {
      switch (version)
      {
        case PEVerifyVersion.DotNet4:
          return string.Format(
              ".NET SDK 4.8: Registry: HKEY_LOCAL_MACHINE\\{0}\\{1}\\PEVerify.exe",
              WindowsSdkRegistryKey40,
              WindowsSdkRegistryInstallationFolderValue);

        default:
          return ".NET SDK 4.8: n/a";
      }
    }

    protected override string? GetPotentialPEVerifyPath (PEVerifyVersion version)
    {
      switch (version)
      {
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
