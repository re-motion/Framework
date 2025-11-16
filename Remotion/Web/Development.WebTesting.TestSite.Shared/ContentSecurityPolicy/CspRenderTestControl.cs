// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System.Web.UI;

namespace Remotion.Web.Development.WebTesting.TestSite.Shared.ContentSecurityPolicy;

public class CspRenderTestControl : Control
{
  protected override void Render (HtmlTextWriter writer)
  {
    writer.RenderBeginTag(HtmlTextWriterTag.B);
    {
      writer.AddAttribute(HtmlTextWriterAttribute.Src, "");
      writer.AddAttribute(HtmlTextWriterAttribute.Id, "myTestLink");
      writer.AddAttribute(HtmlTextWriterAttribute.Href, "#");
      writer.AddAttribute("onclick", "document.getElementById('output').append('; CONTROL INLINE ATTRIBUTE');", false);
      writer.RenderBeginTag("a");
      writer.WriteEncodedText("LINK");
      writer.RenderEndTag();

      writer.WriteEncodedText("Render test control");
    }
    writer.RenderEndTag();

    writer.RenderBeginTag("script");
    writer.Write("document.getElementById('output').append('; CONTROL INLINE SCRIPT');");
    writer.RenderEndTag();
    base.Render(writer);
  }
}
