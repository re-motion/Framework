// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it
// and/or modify it under the terms of the GNU Lesser General Public License
// as published by the Free Software Foundation; either version 2.1 of the
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
#if !NETFRAMEWORK
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Web.UI;
using JetBrains.Annotations;
using Remotion.Web.UI;

namespace Remotion.Web.ContentSecurityPolicy
{
  public class CspEnabledHtmlTextWriter : HtmlTextWriter
  {
    private static readonly Dictionary<string, string> s_supportedEvents =
        new(StringComparer.OrdinalIgnoreCase)
        {
            { "onclick", "click" },
            { "onchange", "change" },
            { "onmouseover", "mouseover" },
            { "onmouseout", "mouseout" },
            { "onkeyup", "keyup" },
            { "onkeydown", "keydown" },
            { "onkeypress", "keypress" }
        };

    private readonly ISmartPage _page;
    private readonly string _nonceValue;
    private readonly RandomNumberGenerator _randomNumberGenerator;

    public CspEnabledHtmlTextWriter (ISmartPage page, [NotNull] TextWriter writer, string nonceValue)
        : base(writer)
    {
      _page = page;
      _nonceValue = nonceValue;
      _randomNumberGenerator = RandomNumberGenerator.Create();
    }

    public override void RenderBeginTag (HtmlTextWriterTag tagKey)
    {
      if (tagKey == HtmlTextWriterTag.Script)
      {
        AddAttribute("nonce", _nonceValue);
      }

      base.RenderBeginTag(tagKey);
    }

    public override void AddAttribute (string name, string? value)
    {
      if (name.StartsWith("on", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(value))
      {
        if (s_supportedEvents.TryGetValue(name, out var eventType))
        {
          var eventID = GetRandomEventID();
          base.AddAttribute("data-inline-event-target", eventID);
          var script = $"SmartPageContext.Instance.RegisterInlineEvent(\"{eventID}\", \"{eventType}\", function (event){{{value}}});";
          _page.ClientScript.RegisterStartupScriptBlock(_page, typeof(CspEnabledHtmlTextWriter), eventID, script);
        }
        else
        {
          throw new NotSupportedException($"{name} is not supported.");
        }
      }
      else
      {
        base.AddAttribute(name, value);
      }
    }

    private string GetRandomEventID () //TODO:Type is a singleton why use dispose?
    {
      Span<byte> data = stackalloc byte[8];
      _randomNumberGenerator.GetBytes(data);
      return Convert.ToHexString(data);
    }
  }
}
#endif
