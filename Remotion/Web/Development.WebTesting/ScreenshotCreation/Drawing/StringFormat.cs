// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;

namespace Remotion.Web.Development.WebTesting.ScreenshotCreation.Drawing;

/// <summary>
/// Defines how a string is layout inside a container.
/// </summary>
public class StringFormat
{
  public static StringFormat GenericDefault { get; } = new(HorizontalAlignment.Left, VerticalAlignment.Top, TextFlow.ClipBounds);

  public HorizontalAlignment HorizontalAlignment { get; }

  public VerticalAlignment VerticalAlignment { get; }

  public TextFlow TextFlow { get; }

  public StringFormat (HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment, TextFlow textFlow)
  {
    HorizontalAlignment = horizontalAlignment;
    VerticalAlignment = verticalAlignment;
    TextFlow = textFlow;
  }
}
