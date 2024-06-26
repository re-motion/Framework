// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0
using System;
using System.IO;

#nullable enable
// ReSharper disable once CheckNamespace
namespace Remotion.Development.UnitTesting.PEVerifyPathSources
{
  abstract partial class PotentialPEVerifyPathSourceBase : IPEVerifyPathSource
  {
    public string? GetPEVerifyPath (PEVerifyVersion version)
    {
      var potentialPath = GetPotentialPEVerifyPath(version);
      if (potentialPath != null && File.Exists(potentialPath))
        return potentialPath;

      return null;
    }

    public abstract string GetLookupDiagnostics (PEVerifyVersion version);

    protected abstract string? GetPotentialPEVerifyPath (PEVerifyVersion version);
  }
}
