// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System.Drawing;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Drawing;

namespace Remotion.Web.Development.WebTesting.ScreenshotCreation;

/// <summary>
/// Provides a null implementation for <see cref="ICursorInformation"/>.
/// The cursor is not visible, always positioned at (0,0), and drawing is a no-op.
/// </summary>
public class EmptyCursorInformation : ICursorInformation
{
  public static readonly EmptyCursorInformation Instance = new();

  private EmptyCursorInformation ()
  {
  }

  public bool IsVisible { get; } = false;

  public Point Position { get; } = Point.Empty;

  public void Draw (Canvas canvas)
  {
  }
}
