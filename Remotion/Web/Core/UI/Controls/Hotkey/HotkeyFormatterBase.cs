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
using System.Text;
using System.Web.UI;
using Remotion.Utilities;

namespace Remotion.Web.UI.Controls.Hotkey
{
  /// <summary>
  /// Base class for implementations of the <see cref="IHotkeyFormatter"/> interface.
  /// </summary>
  /// <remarks>
  /// This class provides a complete implementation of the interface, only requiring the definition of the template methods 
  /// for the hotkey highlighting (<see cref="AppendHotkeyBeginTag"/>, <see cref="AppendHotkeyEndTag"/>).
  /// <para>
  ///  The following rules are applied when parsing the string:
  /// </para>
  /// <list type="bullet">
  ///   <item>
  ///     <description>
  ///       If the <see cref="WebString"/> is of <see cref="WebStringType"/>.<see cref="WebStringType.Encoded"/>, the original <see cref="WebString"/> value
  ///       is used as the formatted output.
  ///     </description>
  ///   </item>
  ///   <item>
  ///     <description>
  ///       If the <see cref="WebString"/> contains a single '<c>&amp;</c>'-character followed by a letter or a digit, then the letter or digit is used as hotkey.
  ///       The '<c>&amp;</c>' will be removed from the formatted output.
  ///     </description>
  ///   </item>
  ///   <item>
  ///     <description>
  ///       '<c>&amp;</c>'-characters can be escaped by using two '<c>&amp;</c>'.
  ///       The parsing logic merges them into a single '<c>&amp;</c>'-character for the formatted output.
  ///     </description>
  ///   </item>
  ///   <item>
  ///     <description>
  ///       If the <see cref="WebString"/> contains multiple possible hotkeys, then no further parsing is attempted and the original <see cref="WebString"/> value
  ///       is used as the output.
  ///     </description>
  ///   </item>
  /// </list>
  /// </remarks>
  public abstract class HotkeyFormatterBase : IHotkeyFormatter
  {
    private const char c_hotkeyMarker = '&';

    protected HotkeyFormatterBase ()
    {
    }

    protected abstract void AppendHotkeyBeginTag (HtmlTextWriter writer, string hotkey);
    protected abstract void AppendHotkeyEndTag (HtmlTextWriter writer);

    public char? GetAccessKey (WebString value)
    {
      if (value.Type == WebStringType.Encoded)
      {
        return null;
      }
      else
      {
        var textWithHotkey = Parse(value.GetValue());
        if (textWithHotkey.HotkeyIndex.HasValue)
          return Char.ToUpperInvariant(textWithHotkey.Text[textWithHotkey.HotkeyIndex.Value]);
        else
          return null;
      }
    }

    public void WriteTo (HtmlTextWriter writer, WebString value)
    {
      ArgumentUtility.CheckNotNull("writer", writer);

      if (value.Type == WebStringType.Encoded)
      {
        value.WriteTo(writer);
      }
      else
      {
        var textWithHotkey = Parse(value.GetValue());
        var textParts = GetTextParts(textWithHotkey.Text, textWithHotkey.HotkeyIndex);

        WebString.CreateFromText(textParts.BeforeHotkey).WriteTo(writer);
        if (!string.IsNullOrEmpty(textParts.Hotkey))
        {
          AppendHotkeyBeginTag(writer, textParts.Hotkey.ToUpperInvariant());
          WebString.CreateFromText(textParts.Hotkey).WriteTo(writer);
          AppendHotkeyEndTag(writer);
          WebString.CreateFromText(textParts.AfterHotkey).WriteTo(writer);
        }
      }
    }

    private static (string BeforeHotkey, string? Hotkey, string? AfterHotkey) GetTextParts (string text, int? hotkeyIndex)
    {
      if (!hotkeyIndex.HasValue)
        return (BeforeHotkey: text, Hotkey: null, AfterHotkey: null);

      return (
          BeforeHotkey: text.Substring(0, hotkeyIndex.Value),
          Hotkey: text.Substring(hotkeyIndex.Value, 1),
          AfterHotkey: text.Substring(hotkeyIndex.Value + 1));
    }

    private static (string Text, int? HotkeyIndex) Parse (string value)
    {
      var resultBuilder = new StringBuilder(value.Length);
      int? hotkeyIndex = null;
      for (int i = 0; i < value.Length; i++)
      {
        var currentChar = value[i];
        if (currentChar == c_hotkeyMarker && i + 1 < value.Length)
        {
          if (IsValidHotkeyCharacter(value, i + 1))
          {
            if (hotkeyIndex.HasValue)
              return (Text: value, HotkeyIndex: null);

            hotkeyIndex = resultBuilder.Length;
            continue;
          }
          else if (value[i + 1] == c_hotkeyMarker)
            i++;
        }

        resultBuilder.Append(currentChar);
      }

      return (Text: resultBuilder.ToString(), HotkeyIndex: hotkeyIndex);
    }

    private static bool IsValidHotkeyCharacter (string text, int index)
    {
      return Char.IsLetterOrDigit(text, index);
    }
  }
}
