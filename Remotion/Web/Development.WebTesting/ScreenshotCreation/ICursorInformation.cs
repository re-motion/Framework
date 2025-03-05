// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System.Drawing;

namespace Remotion.Web.Development.WebTesting.ScreenshotCreation;

/// <summary>
/// Represents information about a mouse cursor and provides methods for drawing the cursor.
/// </summary>
public interface ICursorInformation
{
  /// <summary>
  /// Returns <see langword="true" /> if the cursor is visible, otherwise <see langword="false" />.
  /// </summary>
  public bool IsVisible { get; }

  /// <summary>
  /// Draws the Cursor onto the specified <see cref="Graphics"/>.
  /// </summary>
  public void Draw (Graphics canvas);

  /// <summary>
  /// The position of the cursor in desktop coordinates.
  /// </summary>
  public Point Position { get; }
}
