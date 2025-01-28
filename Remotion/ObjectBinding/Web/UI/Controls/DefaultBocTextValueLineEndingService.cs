// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  [ImplementationFor(typeof(IBocTextValueLineEndingService), Lifetime = LifetimeKind.Singleton)]
  public class DefaultBocTextValueLineEndingService : IBocTextValueLineEndingService
  {
    public string GetLineEnding (BocTextValueBase bocTextValueBase)
    {
      ArgumentUtility.CheckNotNull(nameof(bocTextValueBase), bocTextValueBase);

      return Environment.NewLine;
    }
  }
}
