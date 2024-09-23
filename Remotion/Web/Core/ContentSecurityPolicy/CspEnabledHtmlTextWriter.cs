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
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI;
using Remotion.Utilities;
using Remotion.Web.UI;

namespace Remotion.Web.ContentSecurityPolicy
{
  /// <summary>
  /// Represents <see cref="HtmlTextWriter"/> which implements Content-Security-Policy
  /// </summary>
  public class CspEnabledHtmlTextWriter : HtmlTextWriter
  {
    private static readonly IReadOnlyDictionary<string, string> s_supportedEvents =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
          { "onclick", "click" },
          { "onchange", "change" },
          { "onmouseover", "mouseover" },
          { "onmouseout", "mouseout" },
          { "onkeyup", "keyup" },
          { "onkeydown", "keydown" },
          { "onkeypress", "keypress" }
        };

    private readonly List<(string Type, string Value)> _registeredEvents = new();
    private readonly INonceGenerator _nonceGenerator;
    private readonly ISmartPage _page;
    private readonly string _requestNonce;

    public CspEnabledHtmlTextWriter (ISmartPage page, TextWriter writer, INonceGenerator nonceGenerator, string requestNonce)
        : base(writer)
    {
      ArgumentUtility.CheckNotNull("page", page);
      ArgumentUtility.CheckNotNull("writer", writer);
      ArgumentUtility.CheckNotNull("nonceGenerator", nonceGenerator);
      ArgumentUtility.CheckNotNullOrEmpty("requestNonce", requestNonce);

      _page = page;
      _nonceGenerator = nonceGenerator;
      _requestNonce = requestNonce;
    }

    public override void RenderBeginTag (HtmlTextWriterTag tagKey)
    {
      if (tagKey == HtmlTextWriterTag.Script)
      {
        AddAttribute("nonce", _requestNonce);
      }

      if (_registeredEvents.Count > 0)
      {
        var eventTargetID = _nonceGenerator.GenerateAlphaNumericNonce();
        AddAttribute("data-inline-event-target", eventTargetID);

        foreach (var registeredEvent in _registeredEvents)
        {
          var script =
              $"document.querySelector('[data-inline-event-target=\"{eventTargetID}\"]').addEventListener('{registeredEvent.Type}', function (event){{{registeredEvent.Value}}});";
          _page.ClientScript.RegisterStartupScriptBlock(_page, typeof(CspEnabledHtmlTextWriter), eventTargetID, script);
        }

        _registeredEvents.Clear();
      }

      base.RenderBeginTag(tagKey);
    }

    protected override void AddAttribute (string name, string? value, HtmlTextWriterAttribute key, bool encode, bool isUrl)
    {
      ArgumentUtility.CheckNotNull("name", name);

      if (name.StartsWith("on", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(value))
      {
        if (s_supportedEvents.TryGetValue(name, out var eventType))
        {
          if (_registeredEvents.Exists(e => eventType.Equals(e.Type)))
          {
            throw new ArgumentException($"Event handler '{name}' cannot be registered more than once.");
          }

          var trimmedValue = value.TrimStart();
          const string javascriptPrefix = "javascript:";
          if (trimmedValue.StartsWith(javascriptPrefix, StringComparison.OrdinalIgnoreCase))
            value = trimmedValue.Substring(javascriptPrefix.Length).TrimStart();

          _registeredEvents.Add((Type: eventType, Value: value));
        }
        else
        {
          throw new ArgumentException(
              $"The name of attribute '{name}' indicates a script event but the event type is not supported.",
              nameof(name));
        }
      }
      else
      {
        base.AddAttribute(name, value!, key, encode, isUrl);
      }
    }
  }
}
