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
using System.ComponentModel;
using System.Web.UI;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web
{
  /// <summary>
  /// The <see cref="PlainTextString"/> structure represents strings that are encoded before being rendered onto a web page.
  /// </summary>
  [TypeConverter(typeof(PlainTextStringConverter))]
  public readonly struct PlainTextString : IEquatable<PlainTextString>
  {
    private static readonly string[] s_newlineSplitSeparators = { "\r\n", "\r", "\n" };

    public static readonly PlainTextString Empty = new(string.Empty);

    /// <summary>
    /// Creates a <see cref="PlainTextString"/> from the specified <paramref name="text"/>.
    /// The value will be encoded before being rendered.
    /// </summary>
    /// <param name="text">The text from which a <see cref="PlainTextString"/> is created.</param>
    /// <returns>The newly created <see cref="PlainTextString"/> containing the <paramref name="text"/>.</returns>
    public static PlainTextString CreateFromText ([CanBeNull] string? text)
    {
      return new PlainTextString(text);
    }

    /// <summary>
    /// Implements the equality operator for <see cref="PlainTextString"/>s. The operator is implemented the same way as the <see cref="Equals(PlainTextString)"/> method.
    /// </summary>
    /// <param name="left">The first value to be compared for equality.</param>
    /// <param name="right">The second value to be compared for equality.</param>
    /// <returns>
    ///   <see langword="true" /> if both <paramref name="left"/> and <paramref name="right"/> have the same string value. Otherwise, <see langword="false" />.
    /// </returns>
    public static bool operator == (PlainTextString left, PlainTextString right)
    {
      return left.Equals(right);
    }

    /// <summary>
    /// Implements the inequality operator for <see cref="PlainTextString"/>s. The operator is implemented the same way as the <see cref="Equals(PlainTextString)"/> method.
    /// </summary>
    /// <param name="left">The first value to be compared for inequality.</param>
    /// <param name="right">The second value to be compared for inequality.</param>
    /// <returns>
    ///   <see langword="true" /> if both <paramref name="left"/> and <paramref name="right"/> do not have the same string value. Otherwise, <see langword="false" />.
    /// </returns>
    public static bool operator != (PlainTextString left, PlainTextString right)
    {
      return !left.Equals(right);
    }

    /// <summary>
    /// Implements the implicit conversion from <see cref="PlainTextString"/> to <see cref="WebString"/>.
    /// </summary>
    /// <param name="value">The <see cref="PlainTextString"/> to be converted.</param>
    /// <returns>A <see cref="WebString"/> of type <see cref="WebStringType"/>.<see cref="WebStringType.PlainText"/>.</returns>
    public static implicit operator WebString (PlainTextString value)
    {
      return WebString.CreateFromText(value._value);
    }

    /// <summary>
    /// Implements the explicit conversion from <see cref="string"/> to <see cref="PlainTextString"/>.
    /// </summary>
    /// <param name="value">The <see cref="string"/> to be converted.</param>
    /// <returns>A <see cref="PlainTextString"/> with the specified <paramref name="value"/>.</returns>
    public static explicit operator PlainTextString (string value)
    {
      return new PlainTextString(value);
    }

    [CanBeNull]
    private readonly string? _value;

    private PlainTextString ([CanBeNull] string? value)
    {
      _value = value;
    }

    /// <summary>
    /// Tests whether the <see cref="PlainTextString"/>s value is <see langword="null" /> or an empty string.
    /// </summary>
    public bool IsEmpty => string.IsNullOrEmpty(_value);

    /// <summary>
    /// Gets the raw string value of the <see cref="PlainTextString"/> or an empty string if there is no value.
    /// </summary>
    [NotNull]
    public string GetValue () => _value ?? string.Empty;

    /// <summary>
    /// Writes the <see cref="PlainTextString"/>'s value to the specified <paramref name="writer"/>.
    /// The value is escaped before being written.
    /// </summary>
    /// <param name="writer">The <see cref="HtmlTextWriter"/> where the value will be appended to. Must not be <see langword="null" />.</param>
    public void WriteTo ([NotNull] HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull(nameof(writer), writer);

      ((WebString)this).WriteTo(writer);
    }

    /// <summary>
    /// Adds the specified markup <paramref name="attribute"/> to the <paramref name="writer"/> using the <see cref="PlainTextString"/> value.
    /// The value is escaped before being written.
    /// </summary>
    /// <remarks>
    /// Renders an attribute with an empty value if the <see cref="PlainTextString"/> has no value.
    /// </remarks>
    /// <param name="writer">The <see cref="HtmlTextWriter"/> where the attribute will be added. Must not be <see langword="null" />.</param>
    /// <param name="attribute">The attribute that is to be added.</param>
    public void AddAttributeTo ([NotNull] HtmlTextWriter writer, HtmlTextWriterAttribute attribute)
    {
      ArgumentUtility.CheckNotNull(nameof(writer), writer);

      writer.AddAttribute(attribute, GetValue(), fEncode: true);
    }

    /// <summary>
    /// Adds the specified markup <paramref name="attribute"/> to the <paramref name="writer"/> using the <see cref="PlainTextString"/> value.
    /// The value is escaped before being written.
    /// </summary>
    /// <remarks>
    /// Renders an attribute with an empty value if the <see cref="PlainTextString"/> has no value.
    /// </remarks>
    /// <param name="writer">The <see cref="HtmlTextWriter"/> where the attribute will be added. Must not be <see langword="null" />.</param>
    /// <param name="attribute">The name of the attribute that is to be added. Must not be <see langword="null" /> or empty.</param>
    public void AddAttributeTo ([NotNull] HtmlTextWriter writer, [NotNull] string attribute)
    {
      ArgumentUtility.CheckNotNull(nameof(writer), writer);
      ArgumentUtility.CheckNotNullOrEmpty(nameof(attribute), attribute);

      writer.AddAttribute(attribute, GetValue(), fEndode: true);
    }

    /// <inheritdoc />
    public bool Equals (PlainTextString other)
    {
      return GetValue() == other.GetValue();
    }

    /// <inheritdoc />
    public override bool Equals (object? obj)
    {
      return obj is PlainTextString other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode ()
    {
      return GetValue().GetHashCode();
    }

    /// <summary>
    /// Returns a HTML encoded string that represents the current <see cref="PlainTextString"/>.
    /// </summary>
    /// <returns>A HTML encoded string that represents the current <see cref="PlainTextString"/>.</returns>
    public override string ToString ()
    {
      return ((WebString)this).ToString();
    }

    /// <summary>
    /// Returns a string that represents the current <see cref="PlainTextString"/> in the specified <paramref name="encoding"/>.
    /// </summary>
    /// <returns>A string that represents the current <see cref="PlainTextString"/> in the specified <paramref name="encoding"/>.</returns>
    public string ToString (WebStringEncoding encoding)
    {
      return ((WebString)this).ToString(encoding);
    }
  }
}
