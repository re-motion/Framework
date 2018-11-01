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
using System.Web;
using Remotion.Utilities;

namespace Remotion.Web.UI.Controls.Hotkey
{
  /// <summary>
  /// Base class for implementations of the <see cref="IHotkeyFormatter"/> interface.
  /// </summary>
  /// <remarks>
  /// This class provides a complete implementation of the interface, only requiring the definition of the template methods 
  /// for the hotkey highlighting (<see cref="AppendHotkeyBeginTag"/>, <see cref="AppendHotkeyEndTag"/>).
  /// </remarks>
  public abstract class HotkeyFormatterBase : IHotkeyFormatter
  {
    private struct TextParts
    {
      public readonly string BeforeHotkey;
      public readonly string Hotkey;
      public readonly string AfterHotkey;

      public TextParts (string beforeHotkey, string hotkey, string afterHotkey)
      {
        BeforeHotkey = beforeHotkey;
        Hotkey = hotkey;
        AfterHotkey = afterHotkey;
      }
    }

    protected HotkeyFormatterBase ()
    {
    }

    protected abstract void AppendHotkeyBeginTag (StringBuilder stringBuilder, string hotkey);
    protected abstract void AppendHotkeyEndTag (StringBuilder stringBuilder);

    public string FormatHotkey (TextWithHotkey textWithHotkey)
    {
      ArgumentUtility.CheckNotNull ("textWithHotkey", textWithHotkey);

      if (!textWithHotkey.Hotkey.HasValue)
        return null;

      return char.ToString (char.ToUpper (textWithHotkey.Hotkey.Value));
    }

    public string FormatText (TextWithHotkey textWithHotkey, bool encode)
    {
      ArgumentUtility.CheckNotNull ("textWithHotkey", textWithHotkey);

      var textParts = GetTextParts (textWithHotkey.Text, textWithHotkey.HotkeyIndex);
      if (encode)
        textParts = GetHtmlEncodedTextParts (textParts);

      return GetFormattedString (textParts);
    }

    private TextParts GetTextParts (string text, int? hotkeyIndex)
    {
      if (!hotkeyIndex.HasValue)
        return new TextParts (text, null, null);

      return new TextParts (
          text.Substring (0, hotkeyIndex.Value),
          text.Substring (hotkeyIndex.Value, 1),
          text.Substring (hotkeyIndex.Value + 1));
    }

    private TextParts GetHtmlEncodedTextParts (TextParts textParts)
    {
      return new TextParts (
          HttpUtility.HtmlEncode (textParts.BeforeHotkey),
          HttpUtility.HtmlEncode (textParts.Hotkey),
          HttpUtility.HtmlEncode (textParts.AfterHotkey));
    }

    private string GetFormattedString (TextParts textParts)
    {
      var stringBuilder = new StringBuilder (100);

      if (!string.IsNullOrEmpty (textParts.BeforeHotkey))
        stringBuilder.Append (textParts.BeforeHotkey);

      if (!string.IsNullOrEmpty (textParts.Hotkey))
      {
        AppendHotkeyBeginTag (stringBuilder, textParts.Hotkey);
        stringBuilder.Append (textParts.Hotkey);
        AppendHotkeyEndTag (stringBuilder);
      }

      if (!string.IsNullOrEmpty (textParts.AfterHotkey))
        stringBuilder.Append (textParts.AfterHotkey);

      return stringBuilder.ToString();
    }
  }
}