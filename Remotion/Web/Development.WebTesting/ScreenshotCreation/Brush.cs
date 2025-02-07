// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using SkiaSharp;

namespace Remotion.Web.Development.WebTesting.ScreenshotCreation;

// imitates System.Drawing.Brush
public abstract class Brush : MarshalByRefObject, ICloneable, IDisposable
{
  private SKPaint? _paint { get; set; }

  public SKPaint? Paint
  {
    get { return _paint; }
    set { _paint = value; }
  }

  public abstract object Clone ();

  public void Dispose ()
  {
    Dispose(true);
    GC.SuppressFinalize(this);
  }

  protected virtual void Dispose (bool disposing)
  {
    if (disposing)
    {
      _paint?.Dispose();
      _paint = null;
    }
  }

  ~Brush ()
  {
    Dispose(false);
  }
}
