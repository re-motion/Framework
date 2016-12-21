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
using JetBrains.Annotations;

namespace Remotion.Web.UI.Controls.Hotkey
{
  /// <summary>
  /// Provides a <see cref="Parse"/> method for anayzling a <see cref="string"/> and creating a <see cref="TextWithHotkey"/> from it.
  /// </summary>
  /// <remarks>
  ///  The following rules are applied when parsing the string:
  /// <list type="bullet">
  ///   <item>
  ///     <description>
  ///       If the string contains a single '<c>&amp;</c>'-character followed by a letter or a digit, then the letter or digit is used as hotkey.
  ///       The '<c>&amp;</c>' will be removed from the resulting <see cref="TextWithHotkey.Text"/>.
  ///     </description>
  ///   </item>
  ///   <item>
  ///     <description>
  ///       '<c>&amp;</c>'-characters can be escaped by using two '<c>&amp;</c>'. 
  ///       The parsing logic merges them into a single '<c>&amp;</c>'-character for the resulting <see cref="TextWithHotkey.Text"/>.
  ///     </description>
  ///   </item>
  ///   <item>
  ///     <description>
  ///       If the string contains multiple possible hotkeys, then no further parsing is attempted and the original string is used as the resulting <see cref="TextWithHotkey.Text"/>.
  ///     </description>
  ///   </item>
  /// </list>
  /// </remarks>
  public static class HotkeyParser
  {
    private const char c_hotkeyMarker = '&';

    /// <summary>
    /// Parses the <paramref name="value"/> and creates a new <see cref="TextWithHotkey"/>.
    /// </summary>
    /// <param name="value">The <see cref="string"/> to be analyzed. Can be <see langword="null" />.</param>
    /// <returns>
    /// An instance of <see cref="TextWithHotkey"/>. If <paramref name="value"/> is <see langword="null" />, 
    /// the resulting <see cref="TextWithHotkey"/> contains an empty <see cref="TextWithHotkey.Text"/>.
    /// </returns>
    public static TextWithHotkey Parse ([CanBeNull]string value)
    {
      if (String.IsNullOrEmpty (value))
        return new TextWithHotkey (String.Empty, null);

      var resultBuilder = new StringBuilder (value.Length);
      int? hotkeyIndex = null;
      for (int i = 0; i < value.Length; i++)
      {
        var currentChar = value[i];
        if (currentChar == c_hotkeyMarker && i + 1 < value.Length)
        {
          if (IsValidHotkeyCharacter (value, i + 1))
          {
            if (hotkeyIndex.HasValue)
              return new TextWithHotkey (value, null);

            hotkeyIndex = resultBuilder.Length;
            continue;
          }
          else if (value[i + 1] == c_hotkeyMarker)
            i++;
        }

        resultBuilder.Append (currentChar);
      }

      return new TextWithHotkey (resultBuilder.ToString(), hotkeyIndex);
    }

    /// <summary>
    /// Escapes all the <c>&amp;</c> inside <paramref name="value"/> with their escape sequecence.
    /// </summary>
    /// <param name="value">The <see cref="string"/> to be escaped. Can be <see langword="null" />.</param>
    /// <returns>The escaped <see cref="string"/> or <see langword="null"/> if the <paramref name="value"/> was <see langword="null" />.</returns>
    public static string Escape ([CanBeNull]string value)
    {
      if (value == null)
        return null;

      return value.Replace (new string (c_hotkeyMarker, 1), new string (c_hotkeyMarker, 2));
    }

    internal static bool IsValidHotkeyCharacter (string text, int index)
    {
      return Char.IsLetterOrDigit (text, index);
    }
  }
}