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
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.UI.Controls.Hotkey
{
  /// <summary>
  /// Represents all information required about a hotkey-enabled string.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Use the <see cref="HotkeyParser"/>'s <see cref="HotkeyParser.Parse"/> method to analyze a <see cref="string"/>.
  /// </para><para>
  /// Use the <see cref="IHotkeyFormatter"/> to prepare a <see cref="TextWithHotkey"/> for rendering (i.e. add high-lighting for the hotkey).
  /// </para>
  /// </remarks>
  public sealed class TextWithHotkey
  {
    private readonly string _text;
    private readonly int? _hotkeyIndex;
    private readonly char? _hotkey;

    public TextWithHotkey ([NotNull] string text, int? hotkeyIndex)
    {
      ArgumentUtility.CheckNotNull ("text", text);

      if (hotkeyIndex.HasValue && hotkeyIndex.Value < 0)
        throw new ArgumentOutOfRangeException ("hotkeyIndex", "The hotkeyIndex must not be a negative number.");

      if (hotkeyIndex.HasValue && hotkeyIndex.Value >= text.Length)
        throw new ArgumentOutOfRangeException ("hotkeyIndex", "The hotkeyIndex must be less then the length of the 'text' argument.");

      if (hotkeyIndex.HasValue && !HotkeyParser.IsValidHotkeyCharacter(text, hotkeyIndex.Value))
        throw new ArgumentException ("The hotkeyIndex must indicate a letter or digit character.", "hotkeyIndex");

      _text = text;
      _hotkeyIndex = hotkeyIndex;
      if (hotkeyIndex.HasValue)
        _hotkey = text[hotkeyIndex.Value];
    }

    public TextWithHotkey ([NotNull] string text, char hotkey)
    {
      ArgumentUtility.CheckNotNull ("text", text);
      if (!HotkeyParser.IsValidHotkeyCharacter(char.ToString (hotkey), 0))
        throw new ArgumentException ("The hotkey must be a letter or digit character.", "hotkey");

      _text = text;
      _hotkeyIndex = null;
      _hotkey = hotkey;
    }

    public string Text
    {
      get { return _text; }
    }

    public int? HotkeyIndex
    {
      get { return _hotkeyIndex; }
    }

    public char? Hotkey
    {
      get { return _hotkey; }
    }
  }
}