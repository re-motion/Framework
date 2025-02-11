// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System.Web.UI;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.UI;

namespace Remotion.Web.ContentSecurityPolicy;

/// <summary>
/// Implements <see cref="ICspDefaultsProvider"/> to provide a default service with opt-in CSP support with a default header.
/// Register a custom instance to enable CSP and set a custom CSP header.
/// </summary>
/// <threadsafety static="true" instance="true" />
[ImplementationFor(typeof(ICspDefaultsProvider), Lifetime = LifetimeKind.Singleton)]
public class CspDefaultsProvider : ICspDefaultsProvider
{
  public static readonly CspHeader DefaultCspHeader = CspHeader.Empty
      .SetDirective(CspDirectives.DefaultSrc, "'self'")
      .SetDirective(CspDirectives.ScriptSrc, "'self'") // Explicit script-src is necessary to ensure that we can add nonces without restricting the CSP
      .SetDirective(CspDirectives.StyleSrc, "'self' 'unsafe-inline'")
      .SetDirective(CspDirectives.FrameAncestors, "'self'");

  private readonly bool _isCspEnabledDefault;
  private readonly CspHeader _cspHeaderDefault;
  private readonly bool _isCspReportOnlyEnabledDefault;
  private readonly CspHeader _cspReportOnlyHeaderDefault;

  public CspDefaultsProvider ()
    : this(false, DefaultCspHeader, false, DefaultCspHeader)
  {
  }

  private CspDefaultsProvider (
      bool isCspEnabledDefault,
      CspHeader cspHeaderDefault,
      bool isCspReportOnlyEnabledDefault,
      CspHeader cspReportOnlyHeaderDefault)
  {
    _isCspEnabledDefault = isCspEnabledDefault;
    _cspHeaderDefault = cspHeaderDefault;
    _isCspReportOnlyEnabledDefault = isCspReportOnlyEnabledDefault;
    _cspReportOnlyHeaderDefault = cspReportOnlyHeaderDefault;
  }

  /// <inheritdoc />
  public bool GetDefaultIsCspEnabledForPage (IPage page) => _isCspEnabledDefault;

  /// <inheritdoc />
  public bool GetDefaultIsCspReportOnlyEnabledForPage (IPage page) => _isCspReportOnlyEnabledDefault;

  /// <inheritdoc />
  public CspHeader GetDefaultCspHeaderForPage (IPage page) => _cspHeaderDefault;

  /// <inheritdoc />
  public CspHeader GetDefaultCspReportOnlyHeaderForPage (IPage page) => _cspReportOnlyHeaderDefault;
}
