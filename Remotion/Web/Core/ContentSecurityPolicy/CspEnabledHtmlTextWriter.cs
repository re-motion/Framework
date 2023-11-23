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
using Remotion.Web.UI;

namespace Remotion.Web.ContentSecurityPolicy
{
  public interface INonceGenerator
  {
    string GenerateAlphaNumericNonce ();
  }

  public class NonceGenerator : INonceGenerator
  {
    public string GenerateAlphaNumericNonce ()
    {
      Span<byte> data = stackalloc byte[8];
      using (var randomNumberGenerator = RandomNumberGenerator.Create())
      {
        randomNumberGenerator.GetBytes(data);
      }
      return Convert.ToHexString(data);
    }
  }

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

    private INonceGenerator _nonceGenerator;
    private readonly ISmartPage _page;
    private readonly string _nonceValue;

    public CspEnabledHtmlTextWriter (ISmartPage page, TextWriter writer, string nonceValue, INonceGenerator nonceGenerator)
        : base(writer)
    {
      _page = page;
      _nonceValue = nonceValue;
      _nonceGenerator = nonceGenerator;
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
          var eventID = _nonceGenerator.GenerateAlphaNumericNonce();
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
  }
}
#endif
