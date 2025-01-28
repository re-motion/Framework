// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  public interface IBocTextValueLineEndingService
  {
    string GetLineEnding (BocTextValueBase bocTextValueBase);
  }
}
