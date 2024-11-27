// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using SkiaSharp;

namespace Remotion.Web.Development.WebTesting.SystemDrawingImitators;

// imitates System.Drawing.Pen
public class Pen
{
  public SKColor Color { get; set; }

  private float _width;

  public float Width
  {
    get { return _width; }
    set
    {
      if (value <= 0)
        throw new ArgumentOutOfRangeException(nameof(value), "New width must be greater than zero.");

      _width = value;
    }
  }


  public Pen (SKColor color, float width = 1.0f)
  {
    if (width <= 0)
      throw new ArgumentOutOfRangeException(nameof(width), "Width must be greater than zero.");

    Color = color;
    Width = width;
  }
}
