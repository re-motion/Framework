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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Web;
using System.Web.UI;
using Remotion.Utilities;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.Web.ContentSecurityPolicy
{
  /// <summary>
  /// Represents <see cref="HtmlTextWriter"/> which implements Content-Security-Policy
  /// </summary>
  public class CspEnabledHtmlTextWriter : HtmlTextWriter
  {
    private enum RegisteredEventType
    {
      InlineEventAttribute,
      EventListener
    }

    private record RegisteredEvent (string Key, RegisteredEventType Type, string EventType, string EventValue, string OriginalValue)
    {
      public string GetScript (string eventTargetID)
      {
        return Type switch
        {
            RegisteredEventType.InlineEventAttribute => $"document.querySelector('[data-inline-event-target=\"{eventTargetID}\"]').{EventType} = function (event){{{EventValue}}};",
            RegisteredEventType.EventListener => $"document.querySelector('[data-inline-event-target=\"{eventTargetID}\"]').addEventListener('{EventType}', function (event){{{EventValue}}});",
            _ => throw new InvalidOperationException($"Unsupported registered event type '{Type}'.")
        };
      }
    }

    private delegate string HrefFormatterAction (ReadOnlySpan<char> value);

    private static readonly HrefFormatterAction s_hrefActionFormatter = static value =>
        $$"""
        let __defaultPrevented = event.defaultPrevented;

        event.preventDefault();
        event.preventDefault = () => {
          __defaultPrevented = true;
        };

        setTimeout(() => {
          if (!__defaultPrevented) {
            {{value}}
          }
        }, 0);
        """;

    /// <summary>
    /// Registers the specified <paramref name="eventName"/> as a supported event.
    /// The default list of events should be complete, but this method can be used in case it is not.
    /// </summary>
    /// <param name="eventName">The name of the attribute event (e.g. onclick).</param>
    public static void RegisterSupportedEvent (string eventName)
    {
      ArgumentException.ThrowIfNullOrEmpty(eventName);
      if (!eventName.StartsWith("on", StringComparison.OrdinalIgnoreCase))
        throw new ArgumentException("The specified event name must start with 'on'.", nameof(eventName));

      s_supportedEvents.TryAdd(eventName, eventName);
    }

    private static readonly ConcurrentDictionary<string, string> s_supportedEvents = new(
        new Dictionary<string, string>
        {
            { "onabort", "onabort" },
            { "onafterprint", "onafterprint" },
            { "onbeforeprint", "onbeforeprint" },
            { "onbeforeunload", "onbeforeunload" },
            { "onblur", "onblur" },
            { "oncanplay", "oncanplay" },
            { "oncanplaythrough", "oncanplaythrough" },
            { "onchange", "onchange" },
            { "onclick", "onclick" },
            { "oncontextmenu", "oncontextmenu" },
            { "oncopy", "oncopy" },
            { "oncuechange", "oncuechange" },
            { "oncut", "oncut" },
            { "ondblclick", "ondblclick" },
            { "ondrag", "ondrag" },
            { "ondragend", "ondragend" },
            { "ondragenter", "ondragenter" },
            { "ondragleave", "ondragleave" },
            { "ondragover", "ondragover" },
            { "ondragstart", "ondragstart" },
            { "ondrop", "ondrop" },
            { "ondurationchange", "ondurationchange" },
            { "onemptied", "onemptied" },
            { "onended", "onended" },
            { "onerror", "onerror" },
            { "onfocus", "onfocus" },
            { "onhashchange", "onhashchange" },
            { "oninput", "oninput" },
            { "oninvalid", "oninvalid" },
            { "onkeydown", "onkeydown" },
            { "onkeypress", "onkeypress" },
            { "onkeyup", "onkeyup" },
            { "onload", "onload" },
            { "onloadeddata", "onloadeddata" },
            { "onloadedmetadata", "onloadedmetadata" },
            { "onloadstart", "onloadstart" },
            { "onmessage", "onmessage" },
            { "onmousedown", "onmousedown" },
            { "onmousemove", "onmousemove" },
            { "onmouseout", "onmouseout" },
            { "onmouseover", "onmouseover" },
            { "onmouseup", "onmouseup" },
            { "onmousewheel", "onmousewheel" },
            { "onoffline", "onoffline" },
            { "ononline", "ononline" },
            { "onpagehide", "onpagehide" },
            { "onpageshow", "onpageshow" },
            { "onpaste", "onpaste" },
            { "onpause", "onpause" },
            { "onplay", "onplay" },
            { "onplaying", "onplaying" },
            { "onpopstate", "onpopstate" },
            { "onprogress", "onprogress" },
            { "onratechange", "onratechange" },
            { "onreset", "onreset" },
            { "onresize", "onresize" },
            { "onscroll", "onscroll" },
            { "onsearch", "onsearch" },
            { "onseeked", "onseeked" },
            { "onseeking", "onseeking" },
            { "onselect", "onselect" },
            { "onstalled", "onstalled" },
            { "onstorage", "onstorage" },
            { "onsubmit", "onsubmit" },
            { "onsuspend", "onsuspend" },
            { "ontimeupdate", "ontimeupdate" },
            { "ontoggle", "ontoggle" },
            { "onunload", "onunload" },
            { "onvolumechange", "onvolumechange" },
            { "onwaiting", "onwaiting" },
            { "onwheel", "onwheel" },
        }, StringComparer.OrdinalIgnoreCase);

    private readonly List<RegisteredEvent> _registeredEvents = new();
    private readonly INonceGenerator _nonceGenerator;
    private readonly ISmartPage _page;
    private readonly string _requestNonce;
    private readonly IRenderingFeatures _renderingFeatures;
    private readonly IFallbackNavigationUrlProvider _fallbackNavigationUrlProvider;

    private string? _lastInlineEventTargetId = null;

    public CspEnabledHtmlTextWriter (
        ISmartPage page,
        TextWriter writer,
        INonceGenerator nonceGenerator,
        string requestNonce,
        IRenderingFeatures renderingFeatures,
        IFallbackNavigationUrlProvider fallbackNavigationUrlProvider)
        : base(writer)
    {
      ArgumentNullException.ThrowIfNull(page);
      ArgumentNullException.ThrowIfNull(writer);
      ArgumentNullException.ThrowIfNull(nonceGenerator);
      ArgumentException.ThrowIfNullOrEmpty(requestNonce);
      ArgumentNullException.ThrowIfNull(renderingFeatures);

      _page = page;
      _nonceGenerator = nonceGenerator;
      _requestNonce = requestNonce;
      _renderingFeatures = renderingFeatures;
      _fallbackNavigationUrlProvider = fallbackNavigationUrlProvider;
    }

    protected override HtmlTextWriter CreateUpdatePanelHtmlTextWriter (TextWriter textWriter)
    {
      return new CspEnabledHtmlTextWriter(
          _page,
          textWriter,
          _nonceGenerator,
          _requestNonce,
          _renderingFeatures,
          _fallbackNavigationUrlProvider);
    }

    public override void RenderBeginTag (HtmlTextWriterTag tagKey)
    {
      if (tagKey == HtmlTextWriterTag.Script)
      {
        base.AddAttribute("nonce", _requestNonce);
      }

      if (_registeredEvents.Count > 0)
      {
        var eventTargetID = _nonceGenerator.GenerateAlphaNumericNonce();
        base.AddAttribute("data-inline-event-target", eventTargetID);

        foreach (var registeredEvent in _registeredEvents)
        {
          var script = registeredEvent.GetScript(eventTargetID);
          _page.ClientScript.RegisterStartupScriptBlock(_page, typeof(CspEnabledHtmlTextWriter), $"{eventTargetID}-{registeredEvent.Key}", script);
          if (_renderingFeatures.EnableDiagnosticMetadata)
            base.AddAttribute("data-event-content-" + registeredEvent.Key, registeredEvent.OriginalValue);
        }

        _registeredEvents.Clear();
      }

      base.RenderBeginTag(tagKey);
    }

    public sealed override void AddAttribute (string name, string? value)
    {
      ArgumentNullException.ThrowIfNull(name);
      if (!TryAddAttributeWithoutEncoding(name, value, false))
        base.AddAttribute(name, value);
    }

    public sealed override void AddAttribute (string name, string? value, bool encode)
    {
      ArgumentNullException.ThrowIfNull(name);
      if (!TryAddAttributeWithoutEncoding(name, value, !encode))
        base.AddAttribute(name, value, encode);
    }

    protected sealed override void AddAttribute (string name, string? value, HtmlTextWriterAttribute key)
    {
      base.AddAttribute(name, value, key);
    }

    protected sealed override void AddAttribute (string name, string? value, HtmlTextWriterAttribute key, bool encode, bool isUrl)
    {
      ArgumentNullException.ThrowIfNull(name);
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

    public override void WriteBeginTag (string tagName)
    {
      _lastInlineEventTargetId = null;

      base.WriteBeginTag(tagName);
      if (tagName.Equals("script", StringComparison.OrdinalIgnoreCase))
        base.WriteAttribute("nonce", _requestNonce);
    }

    public override void WriteFullBeginTag (string tagName)
    {
      _lastInlineEventTargetId = null;

      WriteBeginTag(tagName);
      base.Write('>');
    }

    public override void WriteAttribute (string name, string value)
    {
      WriteAttribute(name, value, false);
    }

    public override void WriteAttribute (string name, string value, bool fEncode)
    {
      if (TryCreateRegisteredEvent(name, value, fEncode, out var registeredEvent))
      {
        if (registeredEvent.Key == "href")
          base.WriteAttribute("href", _fallbackNavigationUrlProvider.GetURL());

        var eventTargetID = _lastInlineEventTargetId;
        if (eventTargetID == null)
        {
          eventTargetID = _nonceGenerator.GenerateAlphaNumericNonce();
          base.WriteAttribute("data-inline-event-target", eventTargetID);
          _lastInlineEventTargetId = eventTargetID;
        }

        var script = registeredEvent.GetScript(eventTargetID);
        _page.ClientScript.RegisterStartupScriptBlock(_page, typeof(CspEnabledHtmlTextWriter), $"{eventTargetID}-{registeredEvent.Key}", script);
        if (_renderingFeatures.EnableDiagnosticMetadata)
          base.WriteAttribute("data-event-content-" + registeredEvent.Key, registeredEvent.OriginalValue);
      }
      else
      {
        base.WriteAttribute(name, value, fEncode);
      }
    }

    private bool TryAddAttributeWithoutEncoding (string name, string? value, bool isAlreadyEncoded)
    {
      if (TryCreateRegisteredEvent(name, value, isAlreadyEncoded, out var registeredEvent))
      {
        if (registeredEvent.Key == "href")
          base.AddAttribute("href", _fallbackNavigationUrlProvider.GetURL());

        if (_registeredEvents.Exists(e => registeredEvent.Key.Equals(e.Key)))
          throw new ArgumentException($"Event handler '{registeredEvent.Key}' cannot be registered more than once.");

        _registeredEvents.Add(registeredEvent);
        return true;
      }

      return false;
    }

    private static bool TryCreateRegisteredEvent (
        string name,
        string? value,
        bool isAlreadyEncoded,
        [NotNullWhen(true)] out RegisteredEvent? registeredEvent)
    {
      registeredEvent = null;

      if (string.IsNullOrEmpty(value))
        return false;

      const string javaScriptUrlPrefix = "javascript:";

      var originalValue = value;
      var trimmedValue = value.AsSpan().TrimStart();
      if (name.StartsWith("on", StringComparison.OrdinalIgnoreCase) && s_supportedEvents.TryGetValue(name, out var eventType))
      {
        var actualValue = trimmedValue;
        if (actualValue.StartsWith(javaScriptUrlPrefix, StringComparison.OrdinalIgnoreCase))
          actualValue = actualValue[javaScriptUrlPrefix.Length..].TrimStart();

        if (isAlreadyEncoded)
          actualValue = HttpUtility.HtmlDecode(actualValue.ToString());

        registeredEvent = new RegisteredEvent(
            eventType,
            RegisteredEventType.InlineEventAttribute,
            eventType,
            actualValue.ToString(),
            originalValue);

        return true;
      }
      else if (name.Equals("href", StringComparison.OrdinalIgnoreCase) && trimmedValue.StartsWith(javaScriptUrlPrefix, StringComparison.OrdinalIgnoreCase))
      {
        var actualValue = trimmedValue[javaScriptUrlPrefix.Length..].TrimStart();

        if (isAlreadyEncoded)
          actualValue = HttpUtility.HtmlDecode(actualValue.ToString());

        registeredEvent = new RegisteredEvent(
            "href",
            RegisteredEventType.EventListener,
            "click",
            s_hrefActionFormatter(actualValue),
            originalValue);

        return true;
      }
      else
      {
        return false;
      }
    }
  }
}
