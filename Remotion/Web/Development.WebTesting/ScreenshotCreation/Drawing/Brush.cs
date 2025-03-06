// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using Remotion.Utilities;
using SkiaSharp;

namespace Remotion.Web.Development.WebTesting.ScreenshotCreation.Drawing;

/// <summary>
/// Defines how to draw graphical objects.
/// </summary>
public abstract class Brush : IDisposable
{
  public SKPaint SkiaPaint { get; }

  protected Brush (SKPaint skiaPaint)
  {
    ArgumentUtility.CheckNotNull(nameof(skiaPaint), skiaPaint);

    SkiaPaint = skiaPaint;
  }

  public void Dispose () => SkiaPaint.Dispose();
}
