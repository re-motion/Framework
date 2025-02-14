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
using System.Drawing;
using JetBrains.Annotations;
using Microsoft.Maui.Graphics.Skia;
using Remotion.Utilities;
using SkiaSharp;

namespace Remotion.Web.Development.WebTesting.ScreenshotCreation.Annotations
{
  /// <summary>
  /// Provides the ability to freedraw using the <see cref="SkiaCanvas"/> object of the screenshot.
  /// </summary>
  public class ScreenshotCustomAnnotation : IScreenshotAnnotation
  {
    private readonly Action<SkiaCanvas, ResolvedScreenshotElement> _elementDrawAction;

    public ScreenshotCustomAnnotation ([NotNull] Action<SkiaCanvas, ResolvedScreenshotElement> elementDrawAction)
    {
      ArgumentUtility.CheckNotNull("elementDrawAction", elementDrawAction);

      _elementDrawAction = elementDrawAction;
    }

    /// <inheritdoc />
    public void Draw (SkiaCanvas canvas, ResolvedScreenshotElement resolvedScreenshotElement)
    {
      ArgumentUtility.CheckNotNull("canvas", canvas);
      ArgumentUtility.CheckNotNull("resolvedScreenshotElement", resolvedScreenshotElement);

      _elementDrawAction(canvas, resolvedScreenshotElement);
    }
  }
}
