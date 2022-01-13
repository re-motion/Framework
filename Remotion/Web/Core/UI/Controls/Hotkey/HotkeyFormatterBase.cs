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
    private enum HotkeyIndex
    {
      NoHotkey = -1,
      AmbiguousHotkey = -2
    }

    private const char c_hotkeyMarker = '&';

    protected HotkeyFormatterBase ()
    {
    }

    protected abstract void AppendHotkeyBeginTag (HtmlTextWriter writer, char hotkey);
    protected abstract void AppendHotkeyEndTag (HtmlTextWriter writer);

    public char? GetAccessKey (WebString value)
    {
      if (value.Type == WebStringType.Encoded)
      {
        return null;
      }
      else
      {
        var valueAsString = value.GetValue();
        var hotkeyIndex = GetHotkeyIndex(valueAsString);

        return hotkeyIndex switch
        {
            HotkeyIndex.AmbiguousHotkey => null,
            HotkeyIndex.NoHotkey => null,
            _ => GetNormalizedHotkeyCharacter(valueAsString[(int)hotkeyIndex + 1])
        };
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
        var textParts = Parse(value.GetValue());

        WebString.CreateFromText(textParts.BeforeHotkey).WriteTo(writer);
        if (textParts.Hotkey.HasValue)
        {
          AppendHotkeyBeginTag(writer, GetNormalizedHotkeyCharacter(textParts.Hotkey.Value));
          WebString.CreateFromText(textParts.Hotkey.Value.ToString()).WriteTo(writer);
          AppendHotkeyEndTag(writer);
          WebString.CreateFromText(textParts.AfterHotkey).WriteTo(writer);
        }
      }
    }

    private static (string BeforeHotkey, char? Hotkey, string? AfterHotkey) Parse (string value)
    {
      var hotkeyIndex = GetHotkeyIndex(value);

      if (hotkeyIndex == HotkeyIndex.AmbiguousHotkey)
        return (BeforeHotkey: value, Hotkey: null, AfterHotkey: null);

      if (hotkeyIndex == HotkeyIndex.NoHotkey)
        return (BeforeHotkey: GetSubStringWithEscapedHotkeys(value, 0, value.Length), Hotkey: null, AfterHotkey: null);

      return (
          BeforeHotkey: GetSubStringWithEscapedHotkeys(value, 0, (int)hotkeyIndex),
          Hotkey: value[(int)hotkeyIndex + 1],
          AfterHotkey: GetSubStringWithEscapedHotkeys(value, (int)hotkeyIndex + 2, value.Length - ((int)hotkeyIndex + 2)));
    }

    private static string GetSubStringWithEscapedHotkeys (string value, int startIndex, int length)
    {
      if (value.IndexOf(c_hotkeyMarker, startIndex, length) < 0)
        return value.Substring(startIndex, length);

      var buffer = new char[length];
      int charCount = 0;
      var maxCount = startIndex + length;
      for (int i = startIndex; i < maxCount; i++)
      {
        var currentChar = value[i];
        if (currentChar == c_hotkeyMarker && i + 1 < maxCount && value[i + 1] == c_hotkeyMarker)
          i++;

        buffer[charCount] = currentChar;
        charCount++;
      }

      return new string(buffer, 0, charCount);
    }

    private static HotkeyIndex GetHotkeyIndex (string value)
    {
      var hotkeyIndex = HotkeyIndex.NoHotkey;
      for (int i = 0; i < value.Length; i++)
      {
        if (value[i] == c_hotkeyMarker && i + 1 < value.Length)
        {
          if (value[i + 1] == c_hotkeyMarker)
          {
            i++;
          }
          else if (IsValidHotkeyCharacter(value, i + 1))
          {
            if (hotkeyIndex == HotkeyIndex.NoHotkey)
              hotkeyIndex = (HotkeyIndex)i;
            else
              return HotkeyIndex.AmbiguousHotkey;
          }
        }
      }
      return hotkeyIndex;
    }

    private static bool IsValidHotkeyCharacter (string text, int index)
    {
      return Char.IsLetterOrDigit(text, index);
    }

    private static char GetNormalizedHotkeyCharacter (char hotkey)
    {
      return Char.ToUpperInvariant(hotkey);
    }
  }
}
