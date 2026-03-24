// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Remotion.Logging;

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

  //https://infra.spec.whatwg.org/#ascii-whitespace
  private static readonly char[] s_asciiWhitespaces = [' ', '\t', '\n', '\r', '\f'];
  private static readonly ILogger s_logger = LazyLoggerFactory.CreateLogger<CspHeader>();

  private readonly ImmutableDictionary<string, StringValues> _directives;

  /// <summary>
  /// Parses a CSP HTTP Header, provided as a <see cref="string"/>, into a <see cref="CspHeader"/>.
  /// </summary>
  /// <param name="input">The value of the CSP HTTP header as <see cref="string"/>.</param>
  /// <remarks>
  /// https://www.w3.org/TR/CSP3/#grammardef-serialized-policy
  /// </remarks>
  [Pure]
  public static CspHeader Parse (string input)
  {
    var directives = ImmutableDictionary.CreateBuilder<string, StringValues>();

    foreach (var rawToken in input.Split(';', StringSplitOptions.RemoveEmptyEntries))
    {
      var token = StripAsciiWhitespace(rawToken);

      if (string.IsNullOrWhiteSpace(token))
        continue;

      var (directiveName, directiveValuesStartIndex) = GetDirectiveName(token);
      if (directives.TryGetValue(directiveName, out var valueToBeUsed))
      {
        s_logger.LogWarning(
            "Multiple occurrences found for \"{DirectiveName}\". Any duplicate directives will be ignored. The value \"{DirectiveValue}\" will be used.",
            directiveName,
            valueToBeUsed);
        continue;
      }

      var directiveValues = ParseDirectiveValues(token[directiveValuesStartIndex..]);
      directives.Add(directiveName, directiveValues);
    }

    return new CspHeader(directives.ToImmutable());
  }

  private static (string directive, int endOfDirectiveNameIndex) GetDirectiveName (string token)
  {
    var endOfDirectiveNameIndex = token.IndexOfAny(s_asciiWhitespaces);
    return (token[..endOfDirectiveNameIndex].ToLower(), endOfDirectiveNameIndex);
  }

  private static string StripAsciiWhitespace (string input)
  {
    return input.Trim(s_asciiWhitespaces);
  }

  private CspHeader (ImmutableDictionary<string, StringValues> directives)
  {
    ArgumentNullException.ThrowIfNull(directives);

    _directives = directives;
  }

  public bool IsEmpty => _directives.IsEmpty;

  /// <inheritdoc cref="AddDirectiveValue(string,string)"/>
  [Pure]
  public CspHeader AddDirectiveValue (CspDirective directive, string value)
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
    if (value.IndexOfAny(s_asciiWhitespaces) >= 0)
      throw new ArgumentException("Value must not contain spaces.", nameof(value));

    var existingValues = _directives.GetValueOrDefault(directive);
    return existingValues.Contains(value)
        ? this
        : new CspHeader(_directives.SetItem(directive, StringValues.Concat(existingValues, value)));
  }

  /// <inheritdoc cref="SetDirective(string,string)" />
  [Pure]
  public CspHeader SetDirective (CspDirective directive, string value)
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
  public CspHeader RemoveDirective (CspDirective directive)
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
  public bool TryGetDirectiveValues (CspDirective directive, out StringValues values)
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

  private static StringValues ParseDirectiveValues (string value)
  {
    // The spec would allow other whitespaces as well, but we ignore them here as space is the most relevant.
    // This mainly has an effect on the StringValues object but the output will still contain the whitespace characters.
    // The only side effect here is that space values are collapsed into a single one, which is not a semantic change.
    return new StringValues(value.Split(s_asciiWhitespaces, StringSplitOptions.RemoveEmptyEntries));
  }

  private static string GetCspDirectiveName (CspDirective directive)
  {
    return directive switch
    {
        CspDirective.BaseUri => "base-uri",
        CspDirective.ChildSrc => "child-src",
        CspDirective.ConnectSrc => "connect-src",
        CspDirective.DefaultSrc => "default-src",
        CspDirective.FontSrc => "font-src",
        CspDirective.FormAction => "form-action",
        CspDirective.FrameAncestors => "frame-ancestors",
        CspDirective.FrameSrc => "frame-src",
        CspDirective.ImgSrc => "img-src",
        CspDirective.ManifestSrc => "manifest-src",
        CspDirective.MediaSrc => "media-src",
        CspDirective.ObjectSrc => "object-src",
        CspDirective.ReportTo => "report-to",
        CspDirective.Sandbox => "sandbox",
        CspDirective.ScriptSrc => "script-src",
        CspDirective.ScriptSrcAttr => "script-src-attr",
        CspDirective.ScriptSrcElem => "script-src-elem",
        CspDirective.StyleSrc => "style-src",
        CspDirective.StyleSrcElem => "style-src-elem",
        CspDirective.WorkerSrc => "worker-src",
        _ => throw new ArgumentOutOfRangeException(nameof(directive), directive, null)
    };
  }
}
