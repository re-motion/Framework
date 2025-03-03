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
using System.Web;
using System.Web.UI;
using Remotion.Utilities;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls.Rendering;

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
          { "onclick", "onclick" },
          { "onchange", "onchange" },
          // can be removed once WebTreeView learns to use modern html script registration.
          { "oncontextmenu", "oncontextmenu" },
          { "onmouseover", "onmouseover" },
          { "onmouseout", "onmouseout" },
          { "onkeyup", "onkeyup" },
          { "onkeydown", "onkeydown" },
          { "onkeypress", "onkeypress" }
        };

    private readonly List<(string Type, string Value)> _registeredEvents = new();
    private readonly INonceGenerator _nonceGenerator;
    private readonly ISmartPage _page;
    private readonly string _requestNonce;
    private readonly IRenderingFeatures _renderingFeatures;

    public CspEnabledHtmlTextWriter (ISmartPage page, TextWriter writer, INonceGenerator nonceGenerator, string requestNonce, IRenderingFeatures renderingFeatures)
        : base(writer)
    {
      ArgumentUtility.CheckNotNull("page", page);
      ArgumentUtility.CheckNotNull("writer", writer);
      ArgumentUtility.CheckNotNull("nonceGenerator", nonceGenerator);
      ArgumentUtility.CheckNotNullOrEmpty("requestNonce", requestNonce);
      ArgumentUtility.CheckNotNull("renderingFeatures", renderingFeatures);

      _page = page;
      _nonceGenerator = nonceGenerator;
      _requestNonce = requestNonce;
      _renderingFeatures = renderingFeatures;
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
              $"document.querySelector('[data-inline-event-target=\"{eventTargetID}\"]').{registeredEvent.Type} = function (event){{{registeredEvent.Value}}};";
          _page.ClientScript.RegisterStartupScriptBlock(_page, typeof(CspEnabledHtmlTextWriter), $"{eventTargetID}-{registeredEvent.Type}", script);
          if (_renderingFeatures.EnableDiagnosticMetadata)
            AddAttribute("data-event-content-" + registeredEvent.Type, script);
        }

        _registeredEvents.Clear();
      }

      base.RenderBeginTag(tagKey);
    }

    public sealed override void AddAttribute (string name, string? value)
    {
      ArgumentUtility.CheckNotNull("name", name);
      if (!TryAddAttributeWithoutEncoding(name, value, false))
        base.AddAttribute(name, value);
    }

    public sealed override void AddAttribute (string name, string? value, bool encode)
    {
      ArgumentUtility.CheckNotNull("name", name);
      if (!TryAddAttributeWithoutEncoding(name, value, !encode))
        base.AddAttribute(name, value, encode);
    }

    protected sealed override void AddAttribute (string name, string? value, HtmlTextWriterAttribute key)
    {
      base.AddAttribute(name, value, key);
    }

    protected sealed override void AddAttribute (string name, string? value, HtmlTextWriterAttribute key, bool encode, bool isUrl)
    {
      ArgumentUtility.CheckNotNull("name", name);
      if (!TryAddAttributeWithoutEncoding(name, value, !encode))
        base.AddAttribute(name, value!, key, encode, isUrl);
    }

    public sealed override void AddAttribute (HtmlTextWriterAttribute key, string? value)
    {
      base.AddAttribute(key, value);
    }

    public sealed override void AddAttribute (HtmlTextWriterAttribute key, string? value, bool fEncode)
    {
      base.AddAttribute(key, value, fEncode);
    }

    private bool TryAddAttributeWithoutEncoding (string name, string? value, bool isAlreadyEncoded)
    {
      if (!name.StartsWith("on", StringComparison.OrdinalIgnoreCase))
        return false;

      if (string.IsNullOrEmpty(value))
        return false;

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

        if (isAlreadyEncoded)
          value = HttpUtility.HtmlDecode(value);

        _registeredEvents.Add((Type: eventType, Value: value));
      }
      else
      {
        throw new ArgumentException(
            $"The name of attribute '{name}' indicates a script event but the event type is not supported.",
            nameof(name));
      }

      return true;
    }
  }
}
