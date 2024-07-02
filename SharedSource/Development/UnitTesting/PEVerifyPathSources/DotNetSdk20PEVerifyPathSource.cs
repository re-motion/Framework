// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0
using System;
using System.IO;
using Microsoft.Win32;

#nullable enable
// ReSharper disable once CheckNamespace
namespace Remotion.Development.UnitTesting.PEVerifyPathSources
{
  partial class DotNetSdk20PEVerifyPathSource : PotentialPEVerifyPathSourceBase
  {
    public const string SdkRegistryKey = @"SOFTWARE\Microsoft\.NETFramework";
    public const string SdkRegistryValue = "sdkInstallRootv2.0";

    public override string GetLookupDiagnostics (PEVerifyVersion version)
    {
      if (version != PEVerifyVersion.DotNet2)
        return ".NET SDK 2.0: n/a";
      else
        return string.Format(".NET SDK 2.0: Registry: HKEY_LOCAL_MACHINE\\{0}\\{1}\\bin\\PEVerify.exe", SdkRegistryKey, SdkRegistryValue);
    }

    protected override string? GetPotentialPEVerifyPath (PEVerifyVersion version)
    {
      if (version != PEVerifyVersion.DotNet2)
        return null;

      var sdkPath = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32)
          .OpenSubKey(SdkRegistryKey, false)
          ?.GetValue(SdkRegistryValue) as string;

      if (sdkPath == null)
        return null;

      return Path.Combine(sdkPath, "bin", "PEVerify.exe");
    }
  }
}
