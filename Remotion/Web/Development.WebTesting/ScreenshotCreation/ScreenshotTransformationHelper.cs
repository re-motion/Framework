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
  internal class ScreenshotTransformationHelper<T> : IDisposable
      where T : notnull
  {
    private readonly ScreenshotTransformationContext<T> _context;
    private readonly IScreenshotTransformation<T> _transformation;

    public ScreenshotTransformationHelper (
        ScreenshotManipulation manipulation,
        Graphics graphics,
        IScreenshotElementResolver<T> resolver,
        T target,
        CoordinateSystem coordinateSystem,
        IScreenshotTransformation<T> transformation,
        IBrowserContentLocator locator)
    {
      ArgumentUtility.CheckNotNull("graphics", graphics);
      ArgumentUtility.CheckNotNull("resolver", resolver);
      ArgumentUtility.CheckNotNull("target", target);
      ArgumentUtility.CheckNotNull("transformation", transformation);
      ArgumentUtility.CheckNotNull("locator", locator);

      var resolvedElement = Resolve(resolver, target, locator, coordinateSystem);
      var context = new ScreenshotTransformationContext<T>(manipulation, graphics, resolver, target, resolvedElement);

      context = transformation.BeginApply(context);

      var parentBounds = context.ResolvedElement.ParentBounds;
      if (parentBounds.HasValue)
      {
        _context = context.CloneWith(
            resolvedElement: resolvedElement.CloneWith(elementBounds: Rectangle.Intersect(parentBounds.Value, context.ResolvedElement.ElementBounds)));
      }
      else
      {
        _context = context;
      }

      _transformation = transformation;
    }

    public ScreenshotTransformationContext<T> Context
    {
      get { return _context; }
    }

    public void Dispose ()
    {
      _transformation.EndApply(_context);
    }

    private ResolvedScreenshotElement Resolve (
        IScreenshotElementResolver<T> resolver,
        T target,
        IBrowserContentLocator locator,
        CoordinateSystem coordinateSystem)
    {
      switch (coordinateSystem)
      {
        case CoordinateSystem.Browser:
          return resolver.ResolveBrowserCoordinates(target);
        case CoordinateSystem.Desktop:
          return resolver.ResolveDesktopCoordinates(target, locator);
        default:
          throw new ArgumentOutOfRangeException("coordinateSystem", coordinateSystem, null);
      }
    }
  }
}
