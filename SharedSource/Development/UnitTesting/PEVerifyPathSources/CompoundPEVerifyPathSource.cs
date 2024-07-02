// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0
using System;
using System.Linq;
using Remotion.Utilities;

#nullable enable
// ReSharper disable once CheckNamespace
namespace Remotion.Development.UnitTesting.PEVerifyPathSources
{
  partial class CompoundPEVerifyPathSource : IPEVerifyPathSource
  {
    private readonly IPEVerifyPathSource[] _sources;

    public CompoundPEVerifyPathSource (params IPEVerifyPathSource[] sources)
    {
      ArgumentUtility.CheckNotNullOrEmpty("sources", sources);
      _sources = sources;
    }

    public string? GetPEVerifyPath (PEVerifyVersion version)
    {
      return _sources.Select(source => source.GetPEVerifyPath(version)).FirstOrDefault(path => path != null);
    }

    public string GetLookupDiagnostics (PEVerifyVersion version)
    {
      return string.Join(Environment.NewLine, _sources.Select(source => source.GetLookupDiagnostics(version)));
    }
  }
}
