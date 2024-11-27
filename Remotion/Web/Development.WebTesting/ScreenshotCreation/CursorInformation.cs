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
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.ScreenshotCreation
{
  /// <summary>
  /// Provides information about the cursor at a specific point in time.
  /// </summary>
  public class CursorInformation
  {
    [StructLayout(LayoutKind.Sequential)]
    private struct CursorInfoDto
    {
      // ReSharper disable FieldCanBeMadeReadOnly.Local
      public uint Size;
      public uint Flags;
      public IntPtr CursorHandle;
      public Point ScreenPosition;
      // ReSharper enable FieldCanBeMadeReadOnly.Local
    }

    private const uint c_cursorVisible = 0x1;

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool GetCursorInfo (ref CursorInfoDto info);

    /// <summary>
    /// Represents an invisible cursor with default cursor image at position (0,0).
    /// </summary>
    public static readonly CursorInformation Empty = new CursorInformation(Point.Empty, Cursors.Default, false);

    /// <summary>
    /// Captures the current <see cref="CursorInformation"/>.
    /// </summary>
    public static CursorInformation Capture ()
    {
      var cursorInformation = new CursorInfoDto
                              {
                                  Size = (uint)Marshal.SizeOf(typeof(CursorInfoDto))
                              };

      if (!GetCursorInfo(ref cursorInformation))
        throw new InvalidOperationException("Could not retrieve the cursor information.", new Win32Exception(Marshal.GetLastWin32Error()));

      if (cursorInformation.Flags == c_cursorVisible)
        return new CursorInformation(cursorInformation.ScreenPosition, new Cursor(cursorInformation.CursorHandle), true);

      return new CursorInformation(cursorInformation.ScreenPosition, Cursors.Default, false);
    }

    private readonly Cursor _cursor;
    private readonly bool _isVisible;
    private readonly Point _position;

    public CursorInformation (Point position, [NotNull] Cursor cursor, bool isIsVisible)
    {
      ArgumentUtility.CheckNotNull("cursor", cursor);

      _position = position;
      _cursor = cursor;
      _isVisible = isIsVisible;
    }

    /// <summary>
    /// The <see cref="Cursor"/> object.
    /// </summary>
    [NotNull]
    public Cursor Cursor
    {
      get { return _cursor; }
    }

    /// <summary>
    /// Returns <see langword="true" /> if the cursor is visible, otherwise <see langword="false" />.
    /// </summary>
    public bool IsVisible
    {
      get { return _isVisible; }
    }

    /// <summary>
    /// The position of the cursor in desktop coordinates.
    /// </summary>
    public Point Position
    {
      get { return _position; }
    }

    /// <summary>
    /// Draws the <see cref="Cursor"/> onto the specified <see cref="Graphics"/>.
    /// </summary>
    public void Draw (Graphics graphics)
    {
      ArgumentUtility.CheckNotNull("graphics", graphics);

      if (!_isVisible)
        return;

      var bounds = new Rectangle(_position - new Size(_cursor.HotSpot), _cursor.Size);
      _cursor.Draw(graphics, bounds);
    }
  }
}
