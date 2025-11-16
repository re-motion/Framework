// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System.Web.UI;
using Remotion.Web.UI;

namespace Remotion.Web.ContentSecurityPolicy;

/// <summary>
/// Provides an interface for retrieving default values for CSP related settings for a <see cref="Page"/>.
/// </summary>
/// <threadsafety static="true" instance="true" />
public interface ICspDefaultsProvider
{
  /// <summary>
  /// Returns the default value for CSP being enabled on the specified <paramref name="page"/>.
  /// </summary>
  bool GetDefaultIsCspEnabledForPage (IPage page);

  /// <summary>
  /// Returns the default value for CSP report-only being enabled on the specified <paramref name="page"/>.
  /// </summary>
  bool GetDefaultIsCspReportOnlyEnabledForPage (IPage page);

  /// <summary>
  /// Returns the default value of <see cref="SmartPage"/>.<see cref="SmartPage.GetCspHeader"/> for the specified <paramref name="page"/>.
  /// </summary>
  CspHeader GetDefaultCspHeaderForPage (IPage page);

  /// <summary>
  /// Returns the default value of <see cref="SmartPage"/>.<see cref="SmartPage.GetCspReportOnlyHeader"/> for the specified <paramref name="page"/>.
  /// </summary>
  CspHeader GetDefaultCspReportOnlyHeaderForPage (IPage page);
}
