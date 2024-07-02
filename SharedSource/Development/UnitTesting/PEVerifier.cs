// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0
using System;
using System.Diagnostics;
using System.Reflection;
using Remotion.Development.UnitTesting.PEVerifyPathSources;
using Remotion.Utilities;

#nullable enable
// ReSharper disable once CheckNamespace
namespace Remotion.Development.UnitTesting
{
  partial class PEVerifier
  {
    public static PEVerifier CreateDefault ()
    {
      return
          new PEVerifier(
              new CompoundPEVerifyPathSource(
                  new DotNetSdk48PEVerifyPathSource(),
                  new WindowsSdk81aPEVerifyPathSource(),
                  new WindowsSdk80aPEVerifyPathSource(),
                  new WindowsSdk71PEVerifyPathSource(),
                  new WindowsSdk70aPEVerifyPathSource(),
                  new WindowsSdk6PEVerifyPathSource(),
                  new DotNetSdk20PEVerifyPathSource()));
    }

    private readonly IPEVerifyPathSource _pathSource;

    public PEVerifier (IPEVerifyPathSource pathSource)
    {
      _pathSource = pathSource;
    }

    public string GetVerifierPath (PEVerifyVersion version)
    {
      string? verifierPath = _pathSource.GetPEVerifyPath(version);
      if (verifierPath == null)
      {
        var message = string.Format(
            "PEVerify for version '{0}' could not be found. Locations searched:\r\n{1}",
            version,
            _pathSource.GetLookupDiagnostics(version));
        throw new PEVerifyException(message);
      }
      return verifierPath;
    }

    public PEVerifyVersion GetDefaultVerifierVersion ()
    {
      return Environment.Version.Major == 4 ? PEVerifyVersion.DotNet4 : PEVerifyVersion.DotNet2;
    }


    public void VerifyPEFile (Assembly assembly)
    {
      ArgumentUtility.CheckNotNull("assembly", assembly);

      VerifyPEFile(assembly.ManifestModule.FullyQualifiedName);
    }

    public void VerifyPEFile (Assembly assembly, PEVerifyVersion version)
    {
      ArgumentUtility.CheckNotNull("assembly", assembly);

      VerifyPEFile(assembly.ManifestModule.FullyQualifiedName, version);
    }

    public void VerifyPEFile (string modulePath)
    {
      ArgumentUtility.CheckNotNull("modulePath", modulePath);

      var version = GetDefaultVerifierVersion();
      VerifyPEFile(modulePath, version);
    }

    public void VerifyPEFile (string modulePath, PEVerifyVersion version)
    {
      ArgumentUtility.CheckNotNullOrEmpty("modulePath", modulePath);

      var process = StartPEVerifyProcess(modulePath, version);

      string output = process.StandardOutput.ReadToEnd();
      process.WaitForExit();

      if (process.ExitCode != 0)
      {
        throw new PEVerifyException(process.ExitCode, output);
      }
    }

    private Process StartPEVerifyProcess (string modulePath, PEVerifyVersion version)
    {
      string verifierPath = GetVerifierPath(version);

      var process = new Process();
      process.StartInfo.CreateNoWindow = true;
      process.StartInfo.FileName = verifierPath;
      process.StartInfo.RedirectStandardOutput = true;
      process.StartInfo.UseShellExecute = false;
      process.StartInfo.WorkingDirectory = AppContext.BaseDirectory;
      process.StartInfo.Arguments = string.Format("/verbose \"{0}\"", modulePath);
      process.Start();
      return process;
    }
  }
}
