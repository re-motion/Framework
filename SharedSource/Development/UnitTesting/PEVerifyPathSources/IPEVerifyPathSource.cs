// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0

// ReSharper disable once CheckNamespace

using System;

#nullable enable
namespace Remotion.Development.UnitTesting.PEVerifyPathSources
{
  partial interface IPEVerifyPathSource
  {
    // Returns a valid path or null
    string? GetPEVerifyPath (PEVerifyVersion version);
    string GetLookupDiagnostics (PEVerifyVersion version);
  }
}
