// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0
using System;
using System.IO;
using Microsoft.Win32;

#nullable enable
// ReSharper disable once CheckNamespace
namespace Remotion.Development.UnitTesting.PEVerifyPathSources
{
  partial class WindowsSdk6PEVerifyPathSource : PotentialPEVerifyPathSourceBase
  {
    public const string WindowsSdkRegistryKey = @"SOFTWARE\Microsoft\Microsoft SDKs\Windows";
    public const string WindowsSdkRegistryVersionValue = "CurrentVersion";
    public const string WindowsSdkRegistryInstallationFolderValue = "InstallationFolder";

    public override string GetLookupDiagnostics (PEVerifyVersion version)
    {
      if (version != PEVerifyVersion.DotNet2)
        return "Windows SDK 6: n/a";
      else
      {
        return string.Format(
            "Windows SDK 6: Registry: HKEY_LOCAL_MACHINE\\{0}\\[CurrentVersion]\\{1}\\bin\\PEVerify.exe",
            WindowsSdkRegistryKey,
            WindowsSdkRegistryInstallationFolderValue);
      }
    }

    protected override string? GetPotentialPEVerifyPath (PEVerifyVersion version)
    {
      if (version != PEVerifyVersion.DotNet2)
        return null;

      var windowsSdkVersion = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32)
          .OpenSubKey(WindowsSdkRegistryKey, false)
          ?.GetValue(WindowsSdkRegistryVersionValue) as string;

      if (windowsSdkVersion == null)
        return null;

      var sdkPath = Registry.LocalMachine.OpenSubKey(WindowsSdkRegistryKey + "\\" + windowsSdkVersion, false)
          ?.GetValue(WindowsSdkRegistryInstallationFolderValue) as string;

      if (sdkPath == null)
        return null;

      return Path.Combine(sdkPath, "bin", "PEVerify.exe");
    }
  }
}
