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
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.ScreenshotCreation
{
  /// <summary>
  /// Provides simple cropping for screenshots.
  /// </summary>
  public class ScreenshotCropping : IScreenshotCropping
  {
    private readonly WebPadding _padding;

    private bool _isRestrictedByImageBounds;
    private bool _isRestrictedByParent;

    public ScreenshotCropping (WebPadding padding)
    {
      _padding = padding;

      _isRestrictedByImageBounds = true;
      _isRestrictedByParent = true;
    }

    /// <summary>
    /// If <see langword="true" /> the cropped area will never leave the image bounds.
    /// </summary>
    public bool IsRestrictedByImageBounds
    {
      get { return _isRestrictedByImageBounds; }
      set { _isRestrictedByImageBounds = value; }
    }

    /// <summary>
    /// If <see langword="true" /> the cropped area will never leave the parent bounds.
    /// </summary>
    public bool IsRestrictedByParent
    {
      get { return _isRestrictedByParent; }
      set { _isRestrictedByParent = value; }
    }

    /// <summary>
    /// The <see cref="WebPadding"/> that is applied to the element before cropping.
    /// </summary>
    public WebPadding Padding
    {
      get { return _padding; }
    }

    /// <inheritdoc />
    public Rectangle ApplyOnElement (Rectangle screenshotBounds, ResolvedScreenshotElement screenshotElement)
    {
      ArgumentUtility.CheckNotNull("screenshotElement", screenshotElement);

      var area = _padding.Apply(screenshotElement.ElementBounds);

      if (_isRestrictedByImageBounds)
        area = Rectangle.Intersect(area, screenshotBounds);

      if (_isRestrictedByParent && screenshotElement.ParentBounds.HasValue)
        return Rectangle.Intersect(area, screenshotElement.ParentBounds.Value);

      return area;
    }
  }
}
