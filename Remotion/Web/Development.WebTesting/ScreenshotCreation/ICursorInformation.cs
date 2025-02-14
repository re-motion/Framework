// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System.Drawing;
using Microsoft.Maui.Graphics.Skia;

namespace Remotion.Web.Development.WebTesting.ScreenshotCreation;

public interface ICursorInformation
{
  /// <summary>
  /// Returns <see langword="true" /> if the cursor is visible, otherwise <see langword="false" />.
  /// </summary>
  public bool IsVisible { get; }

  /// <summary>
  /// Draws the Cursor onto the specified <see cref="Graphics"/>.
  /// </summary>
  public void Draw (SkiaCanvas canvas);

  /// <summary>
  /// The position of the cursor in desktop coordinates.
  /// </summary>
  public Point Position { get; }
}
