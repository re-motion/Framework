// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using Remotion.ServiceLocation;
using Remotion.Web.ContentSecurityPolicy;
using Remotion.Web.UI;

namespace Remotion.Web.Test.Shared.ContentSecurityPolicy;

[ImplementationFor(typeof(ICspDefaultsProvider), Lifetime = LifetimeKind.Singleton, Position = CspDefaultsProvider.Position -1)]
public class TestCspDefaultsProvider : ICspDefaultsProvider
{
  public bool GetDefaultIsCspEnabledForPage (IPage page)
  {
    return true;
  }

  public bool GetDefaultIsCspReportOnlyEnabledForPage (IPage page)
  {
    return false;
  }

  public CspHeader GetDefaultCspHeaderForPage (IPage page)
  {
    return CspDefaultsProvider.DefaultCspHeader.AddDirectiveValue(CspDirectives.ScriptSrc, "'nonce-testsite'");
  }
  public CspHeader GetDefaultCspReportOnlyHeaderForPage (IPage page)
  {
    return CspDefaultsProvider.DefaultCspHeader.AddDirectiveValue(CspDirectives.ScriptSrc, "'nonce-testsite'");
  }
}
