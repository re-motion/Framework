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
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting.ScreenshotCreation
{
  /// <summary>
  /// Returns the bounding box of the resolved element and optionally the parent clipping area.
  /// </summary>
  public class ResolvedScreenshotElement
  {
    private readonly CoordinateSystem _coordinateSystem;
    private readonly Rectangle _elementBounds;
    private readonly ElementVisibility _elementVisibility;
    private readonly Rectangle? _parentBounds;
    private readonly Rectangle _unresolvedBounds;

    public ResolvedScreenshotElement (
        CoordinateSystem coordinateSystem,
        Rectangle elementBounds,
        ElementVisibility elementVisibility,
        Rectangle? parentBounds,
        Rectangle unresolvedBounds)
    {
      _coordinateSystem = coordinateSystem;
      _elementBounds = elementBounds;
      _parentBounds = parentBounds;
      _unresolvedBounds = unresolvedBounds;
      _elementVisibility = elementVisibility;
    }

    /// <summary>
    /// The <see cref="ScreenshotCreation.CoordinateSystem"/> 
    /// </summary>
    public CoordinateSystem CoordinateSystem
    {
      get { return _coordinateSystem; }
    }

    /// <summary>
    /// Bounds of the resolved element in <see cref="CoordinateSystem"/> points.
    /// </summary>
    public Rectangle ElementBounds
    {
      get { return _elementBounds; }
    }

    /// <summary>
    /// Unresolved bounds of the resolved element.
    /// </summary>
    public Rectangle UnresolvedBounds
    {
      get { return _unresolvedBounds; }
    }

    /// <summary>
    /// Returns <see langword="true" /> if the resolved element is visible, <see langword="false" /> otherwise.
    /// </summary>
    public ElementVisibility ElementVisibility
    {
      get { return _elementVisibility; }
    }

    /// <summary>
    /// The bounds of the parent element.
    /// </summary>
    /// <remarks>
    /// The bounds of the parent element are restricted to the visible area and therefore can be empty.
    /// </remarks>
    public Rectangle? ParentBounds
    {
      get { return _parentBounds; }
    }

    /// <summary>
    /// Clones this <see cref="ResolvedScreenshotElement"/> replacing all specified properties.
    /// </summary>
    public ResolvedScreenshotElement CloneWith (
        CoordinateSystem? coordinateSystem = null,
        Rectangle? elementBounds = null,
        Rectangle? unresolvedBounds = null,
        OptionalParameter<Rectangle?> parentBounds = default(OptionalParameter<Rectangle?>),
        ElementVisibility? elementVisibility = null)
    {
      return new ResolvedScreenshotElement(
          coordinateSystem ?? _coordinateSystem,
          elementBounds ?? _elementBounds,
          elementVisibility ?? _elementVisibility,
          parentBounds.GetValueOrDefault(_parentBounds),
          unresolvedBounds ?? _unresolvedBounds);
    }
  }
}
