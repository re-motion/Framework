// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.ScreenshotCreation.Drawing;

public class StringFormat
{
  public StringFormat (HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment, TextFlow textFlow)
  {
    ArgumentUtility.CheckNotNull(nameof(horizontalAlignment), horizontalAlignment);
    ArgumentUtility.CheckNotNull(nameof(verticalAlignment), verticalAlignment);
    ArgumentUtility.CheckNotNull(nameof(textFlow), textFlow);

    HorizontalAlignment = horizontalAlignment;
    VerticalAlignment = verticalAlignment;
    TextFlow = textFlow;
  }

  public StringFormat Clone (
      HorizontalAlignment? horizontalAlignment = null,
      VerticalAlignment? verticalAlignment = null,
      TextFlow? textFlow = null
  )
  {
    return new StringFormat(
        horizontalAlignment ?? HorizontalAlignment,
        verticalAlignment ?? VerticalAlignment,
        textFlow ?? TextFlow
    );
  }

  public HorizontalAlignment HorizontalAlignment { get; }
  public VerticalAlignment VerticalAlignment { get; }
  public TextFlow TextFlow { get; }

  public static StringFormat GenericDefault { get; } = new(HorizontalAlignment.Left, VerticalAlignment.Top, TextFlow.ClipBounds);
}
