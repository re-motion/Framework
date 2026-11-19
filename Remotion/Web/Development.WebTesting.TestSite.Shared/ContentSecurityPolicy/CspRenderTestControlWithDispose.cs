// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Web.UI;

namespace Remotion.Web.Development.WebTesting.TestSite.Shared.ContentSecurityPolicy;

public class CspRenderTestControlWithDispose : Control
{
  protected override void OnPreRender (EventArgs e)
  {
    base.OnPreRender(e);

    ScriptManager.GetCurrent(Page).RegisterDispose(this, "document.getElementById('disposeLog').append('; DISPOSED')");
  }

  protected override void Render (HtmlTextWriter writer)
  {
    writer.RenderBeginTag(HtmlTextWriterTag.B);
    {
      writer.WriteEncodedText("Custom control with dispose script :)");
    }
    base.Render(writer);
  }
}
