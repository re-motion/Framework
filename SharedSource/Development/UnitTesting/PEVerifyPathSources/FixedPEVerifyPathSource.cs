// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0
using System;
using Remotion.Utilities;

#nullable enable
// ReSharper disable once CheckNamespace
namespace Remotion.Development.UnitTesting.PEVerifyPathSources
{
  partial class FixedPEVerifyPathSource : IPEVerifyPathSource
  {
    private readonly string _path;

    public FixedPEVerifyPathSource (string path)
    {
      ArgumentUtility.CheckNotNullOrEmpty("path", path);
      _path = path;
    }

    public string GetPEVerifyPath (PEVerifyVersion version)
    {
      return  _path;
    }

    public string GetLookupDiagnostics (PEVerifyVersion version)
    {
      return "Path: " + _path;
    }
  }
}
