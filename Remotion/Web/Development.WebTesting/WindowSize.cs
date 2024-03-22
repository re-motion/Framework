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
namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Describes the size of the browser window.
  /// </summary>
  public record WindowSize(int Width, int Height, bool IsMaximized = false)
  {
    public static readonly WindowSize Maximized = new WindowSize(-1, -1, true );

    /// <summary>
    /// The width of the window.
    /// </summary>
    public int Width { get; } = Width;

    /// <summary>
    /// The height of the window.
    /// </summary>
    public int Height { get; } = Height;

    /// <summary>
    /// Determines whether the width and height values or the maximum values are used when setting the browser window size.
    /// </summary>
    public bool IsMaximized { get; } = IsMaximized;
  }
}
