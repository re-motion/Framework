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
//
using System;
using System.IO;
using System.Web;
using System.Web.UI;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web
{
  /// <summary>
  /// The <see cref="WebString"/> structure represents encoded or unencoded HTML strings that are rendered onto a web page.
  /// Its <see cref="Type"/> determines if and how a <see cref="WebString"/> is encoded before being rendered.
  /// </summary>
  public readonly struct WebString : IEquatable<WebString>
  {
    private static readonly string[] s_newlineSplitSeparators = { "\r\n", "\r", "\n" };

    /// <summary>
    /// Creates a <see cref="WebString"/> from the specified <paramref name="html"/>.
    /// The value will be rendered without being encoded and thus must be a safe string.
    /// </summary>
    /// <param name="html">The HTML from which a <see cref="WebString"/> is created.</param>
    /// <returns>The newly created <see cref="WebString"/> containing the <paramref name="html"/>.</returns>
    public static WebString CreateFromHtml ([CanBeNull] string? html)
    {
      return new WebString (html, WebStringType.Encoded);
    }

    /// <summary>
    /// Creates a <see cref="WebString"/> from the specified <paramref name="text"/>.
    /// The value will be encoded before being rendered.
    /// </summary>
    /// <param name="text">The text from which a <see cref="WebString"/> is created.</param>
    /// <returns>The newly created <see cref="WebString"/> containing the <paramref name="text"/>.</returns>
    public static WebString CreateFromText ([CanBeNull] string? text)
    {
      return new WebString (text, WebStringType.PlainText);
    }

    /// <summary>
    /// Implements the equality operator for <see cref="WebString"/>s. The operator is implemented the same way as the <see cref="Equals(WebString)"/> method.
    /// </summary>
    /// <param name="left">The first value to be compared for equality.</param>
    /// <param name="right">The second value to be compared for equality.</param>
    /// <returns>
    ///   <see langword="true" /> if both <paramref name="left"/> and <paramref name="right"/> have the same string value
    ///   and the same type. Otherwise, <see langword="false" />.
    /// </returns>
    public static bool operator == (WebString left, WebString right)
    {
      return left.Equals (right);
    }

    /// <summary>
    /// Implements the inequality operator for <see cref="WebString"/>s. The operator is implemented the same way as the <see cref="Equals(WebString)"/> method.
    /// </summary>
    /// <param name="left">The first value to be compared for inequality.</param>
    /// <param name="right">The second value to be compared for inequality.</param>
    /// <returns>
    ///   <see langword="true" /> if both <paramref name="left"/> and <paramref name="right"/> are <see langword="null" /> do not have the same string value
    ///   or type. Otherwise, <see langword="false" />.
    /// </returns>
    public static bool operator != (WebString left, WebString right)
    {
      return !left.Equals (right);
    }

    [CanBeNull]
    private readonly string? _value;
    private readonly WebStringType _type;

    private WebString ([CanBeNull] string? value, WebStringType type)
    {
      _value = value;
      _type = type;
    }

    /// <summary>
    /// Gets the type of string represented by this <see cref="WebString"/>.
    /// </summary>
    /// <value>The type of string represented by this <see cref="WebString"/>.</value>
    public WebStringType Type => _type;

    /// <summary>
    /// Tests whether the <see cref="WebString"/>s value is <see langword="null" /> or an empty string.
    /// </summary>
    public bool IsEmpty => string.IsNullOrEmpty (_value);

    /// <summary>
    /// Gets the raw string value of the <see cref="WebString"/> or an empty string if there is no value.
    /// </summary>
    [NotNull]
    public string GetValue () => _value ?? string.Empty;

    /// <summary>
    /// Writes the <see cref="WebString"/>'s value to the specified <paramref name="writer"/>.
    /// The value is escaped if necessary, depending on the <see cref="Type"/> of the <see cref="WebString"/>.
    /// </summary>
    /// <param name="writer">The <see cref="HtmlTextWriter"/> where the value will be appended to. Must not be <see langword="null" />.</param>
    public void Write ([NotNull] HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull (nameof (writer), writer);

      switch (_type)
      {
        case WebStringType.PlainText:
          var value = GetValue();
          var parts = value.Split (s_newlineSplitSeparators, StringSplitOptions.None);

          for (var i = 0; i < parts.Length; i++)
          {
            HttpUtility.HtmlEncode (parts[i], writer);
            if (i < parts.Length - 1)
              writer.WriteBreak();
          }

          break;
        case WebStringType.Encoded:
          writer.Write (GetValue());
          break;
        default:
          throw new NotSupportedException ($"The WebStringType '{_type}' is not supported.");
      }
    }

    /// <summary>
    /// Adds the specified markup <paramref name="attribute"/> to the <paramref name="writer"/> using the <see cref="WebString"/> value.
    /// The value is escaped if necessary, depending on the <see cref="Type"/> of the <see cref="WebString"/>.
    /// </summary>
    /// <remarks>
    /// Renders an attribute with an empty value if the <see cref="WebString"/> has no value.
    /// </remarks>
    /// <param name="writer">The <see cref="HtmlTextWriter"/> where the attribute will be added. Must not be <see langword="null" />.</param>
    /// <param name="attribute">The attribute that is to be added.</param>
    public void AddAttribute ([NotNull] HtmlTextWriter writer, HtmlTextWriterAttribute attribute)
    {
      ArgumentUtility.CheckNotNull (nameof (writer), writer);

      switch (_type)
      {
        case WebStringType.PlainText:
          writer.AddAttribute (attribute, GetValue(), fEncode: true);
          break;
        case WebStringType.Encoded:
          writer.AddAttribute (attribute, GetValue(), fEncode: false);
          break;
        default:
          throw new NotSupportedException ($"The WebStringType '{_type}' is not supported.");
      }
    }

    /// <summary>
    /// Adds the specified markup <paramref name="attribute"/> to the <paramref name="writer"/> using the <see cref="WebString"/> value.
    /// The value is escaped if necessary, depending on the <see cref="Type"/> of the <see cref="WebString"/>.
    /// </summary>
    /// <remarks>
    /// Renders an attribute with an empty value if the <see cref="WebString"/> has no value.
    /// </remarks>
    /// <param name="writer">The <see cref="HtmlTextWriter"/> where the attribute will be added. Must not be <see langword="null" />.</param>
    /// <param name="attribute">The name of the attribute that is to be added. Must not be <see langword="null" /> or empty.</param>
    public void AddAttribute ([NotNull] HtmlTextWriter writer, [NotNull] string attribute)
    {
      ArgumentUtility.CheckNotNull (nameof (writer), writer);
      ArgumentUtility.CheckNotNullOrEmpty (nameof (attribute), attribute);

      switch (_type)
      {
        case WebStringType.PlainText:
          writer.AddAttribute (attribute, GetValue(), fEndode: true);
          break;
        case WebStringType.Encoded:
          writer.AddAttribute (attribute, GetValue(), fEndode: false);
          break;
        default:
          throw new NotSupportedException ($"The WebStringType '{_type}' is not supported.");
      }
    }

    /// <inheritdoc />
    public bool Equals (WebString other)
    {
      return GetValue() == other.GetValue() && _type == other._type;
    }

    /// <inheritdoc />
    public override bool Equals (object? obj)
    {
      return obj is WebString other && Equals (other);
    }

    /// <inheritdoc />
    public override int GetHashCode ()
    {
      unchecked
      {
        return (GetValue().GetHashCode() * 397) ^ (int) _type;
      }
    }

    /// <summary>
    /// Returns a HTML encoded string that represents the current <see cref="WebString"/>.
    /// </summary>
    /// <returns>A HTML encoded string that represents the current <see cref="WebString"/>.</returns>
    public override string ToString ()
    {
      return ToString (WebStringEncoding.Html);
    }

    /// <summary>
    /// Returns a string that represents the current <see cref="WebString"/> in the specified <paramref name="encoding"/>.
    /// </summary>
    /// <returns>A string that represents the current <see cref="WebString"/> in the specified <paramref name="encoding"/>.</returns>
    public string ToString (WebStringEncoding encoding)
    {
      var value = GetValue();
      if (Type == WebStringType.Encoded)
        return value;

      switch (encoding)
      {
        case WebStringEncoding.Html:
          return HttpUtility.HtmlEncode (value);
        case WebStringEncoding.HtmlWithTransformedLineBreaks:
          using (var stringWriter = new StringWriter())
          {
            using (var htmlTextWriter = new HtmlTextWriter (stringWriter))
            {
              Write (htmlTextWriter);
              return stringWriter.ToString();
            }
          }
        case WebStringEncoding.Attribute:
          return HttpUtility.HtmlAttributeEncode (value);
        default:
          throw new ArgumentOutOfRangeException (nameof (encoding), encoding, null);
      }
    }
  }
}