// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Primitives;
using Remotion.Utilities;

namespace Remotion.Web.ContentSecurityPolicy;

/// <summary>
/// Represents a string serialized Content Security Policy (CSP) header as an immutable object.
/// For example, "script-src 'self'; img-src 'self' https:".
/// </summary>
/// <remarks>
/// https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Security-Policy
/// </remarks>
public class CspHeader
{
  public static readonly CspHeader Empty = new(ImmutableDictionary<string, StringValues>.Empty);

  private readonly ImmutableDictionary<string, StringValues> _directives;

  private CspHeader (ImmutableDictionary<string, StringValues> directives)
  {
    ArgumentNullException.ThrowIfNull(directives);

    _directives = directives;
  }

  public bool IsEmpty => _directives.IsEmpty;

  /// <inheritdoc cref="AddDirectiveValue(string,string)"/>
  [Pure]
  public CspHeader AddDirectiveValue (CspDirectives directive, string value)
  {
    return AddDirectiveValue(GetCspDirectiveName(directive), value);
  }

  /// <summary>
  /// Adds the specified <paramref name="value"/> to a specified <paramref name="directive"/>.
  /// If the directive does not exist, this method behaves identically to <see cref="SetDirective(string, string)"/>.
  /// </summary>
  /// <remarks>
  /// https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Security-Policy
  /// </remarks>
  [Pure]
  public CspHeader AddDirectiveValue (string directive, string value)
  {
    ArgumentException.ThrowIfNullOrEmpty(directive);
    ArgumentException.ThrowIfNullOrEmpty(value);
    if (value.Contains(' '))
      throw new ArgumentException("Value must not contain spaces.", nameof(value));

    var existingValues = _directives.GetValueOrDefault(directive);
    return existingValues.Contains(value)
        ? this
        : new CspHeader(_directives.SetItem(directive, StringValues.Concat(existingValues, value)));
  }

  /// <inheritdoc cref="SetDirective(string,string)" />
  [Pure]
  public CspHeader SetDirective (CspDirectives directive, string value)
  {
    return SetDirective(GetCspDirectiveName(directive), value);
  }

  /// <summary>
  /// Sets the specified <paramref name="directive"/> to the specified <paramref name="value"/>.
  /// </summary>
  /// <remarks>
  /// https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Security-Policy
  /// </remarks>
  [Pure]
  public CspHeader SetDirective (string directive, string value)
  {
    ArgumentException.ThrowIfNullOrEmpty(directive);
    ArgumentNullException.ThrowIfNull(value);

    var values = ParseDirectiveValues(value);
    return new CspHeader(_directives.SetItem(directive, values));
  }

  /// <inheritdoc cref="RemoveDirective(string)" />
  [Pure]
  public CspHeader RemoveDirective (CspDirectives directive)
  {
    return RemoveDirective(GetCspDirectiveName(directive));
  }

  /// <summary>
  /// Removes the specified <paramref name="directive"/>.
  /// </summary>
  /// <remarks>
  /// https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Security-Policy
  /// </remarks>
  [Pure]
  public CspHeader RemoveDirective (string directive)
  {
    ArgumentException.ThrowIfNullOrEmpty(directive);

    return _directives.ContainsKey(directive)
        ? new CspHeader(_directives.Remove(directive))
        : this;
  }

  /// <inheritdoc cref="TryGetDirectiveValues(string,out Microsoft.Extensions.Primitives.StringValues)" />
  [Pure]
  public bool TryGetDirectiveValues (CspDirectives directive, out StringValues values)
  {
    return TryGetDirectiveValues(GetCspDirectiveName(directive), out values);
  }

  /// <summary>
  /// Gets the <see cref="StringValues"/> value associated with the specified <paramref name="directive"/>.
  /// </summary>
  /// <remarks>
  /// https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Security-Policy
  /// </remarks>
  [Pure]
  public bool TryGetDirectiveValues (string directive, out StringValues values)
  {
    ArgumentException.ThrowIfNullOrEmpty(directive);

    return _directives.TryGetValue(directive, out values);
  }

  /// <inheritdoc />
  public override string ToString ()
  {
    var stringBuilder = new StringBuilder();
    ToString(stringBuilder);

    return stringBuilder.ToString();
  }

  public void ToString (StringBuilder stringBuilder)
  {
    ArgumentNullException.ThrowIfNull(stringBuilder);

    var firstDirective = true;
    foreach (var (directiveName, values) in _directives.OrderBy(e => e.Key))
    {
      if (!firstDirective)
        stringBuilder.Append("; ");
      firstDirective = false;

      stringBuilder.Append(directiveName);

      foreach (var value in values)
      {
        stringBuilder.Append(' ');
        stringBuilder.Append(value);
      }
    }
  }

  private StringValues ParseDirectiveValues (string value)
  {
    // The spec would allow other whitespaces as well, but we ignore them here as space is the most relevant.
    // This mainly has an effect on the StringValues object but the output will still contain the whitespace characters.
    // The only side effect here is that space values are collapsed into a single one, which is not a semantic change.
    return new StringValues(value.Split(' ', StringSplitOptions.RemoveEmptyEntries));
  }

  private string GetCspDirectiveName (CspDirectives directive)
  {
    return directive switch
    {
        CspDirectives.BaseUri => "base-uri",
        CspDirectives.ChildSrc => "child-src",
        CspDirectives.ConnectSrc => "connect-src",
        CspDirectives.DefaultSrc => "default-src",
        CspDirectives.FontSrc => "font-src",
        CspDirectives.FormAction => "form-action",
        CspDirectives.FrameAncestors => "frame-ancestors",
        CspDirectives.FrameSrc => "frame-src",
        CspDirectives.ImgSrc => "img-src",
        CspDirectives.ManifestSrc => "manifest-src",
        CspDirectives.MediaSrc => "media-src",
        CspDirectives.ObjectSrc => "object-src",
        CspDirectives.ReportTo => "report-to",
        CspDirectives.Sandbox => "sandbox",
        CspDirectives.ScriptSrc => "script-src",
        CspDirectives.ScriptSrcAttr => "script-src-attr",
        CspDirectives.ScriptSrcElem => "script-src-elem",
        CspDirectives.StyleSrc => "style-src",
        CspDirectives.StyleSrcElem => "style-src-elem",
        CspDirectives.WorkerSrc => "worker-src",
        _ => throw new ArgumentOutOfRangeException(nameof(directive), directive, null)
    };
  }
}
