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
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.ScreenshotCreation.Resolvers
{
  /// <summary>
  /// Resolves <see cref="AutomationElement"/>s for screenshot annotations.
  /// </summary>
  public class AutomationElementResolver : IScreenshotElementResolver<AutomationElement>
  {
    /// <summary>
    /// Singleton instance of <see cref="AutomationElementResolver"/>.
    /// </summary>
    public static readonly AutomationElementResolver Instance = new AutomationElementResolver();

    private AutomationElementResolver ()
    {
    }

    /// <inheritdoc />
    public ResolvedScreenshotElement Resolve (AutomationElement target, CoordinateSystem coordinateSystem)
    {
      ArgumentUtility.CheckNotNull("target", target);

      if (coordinateSystem != CoordinateSystem.Desktop)
        throw new NotSupportedException(string.Format("The specified coordinate system '{0}' is not supported.", coordinateSystem));

      try
      {
        Point clickPoint;
        ElementVisibility visibility;
        if (target.Current.IsOffscreen)
          visibility = ElementVisibility.NotVisible;
        else if (target.TryGetClickablePoint(out clickPoint))
          visibility = ElementVisibility.FullyVisible;
        else
          visibility = ElementVisibility.NotVisible;

        var elementRect = target.Current.BoundingRectangle;
        var elementBounds = new Rectangle((int)elementRect.X, (int)elementRect.Y, (int)elementRect.Width, (int)elementRect.Height);
        var unresolvedBounds = elementBounds;

        var result = FindTopMostWindow(target);
        if (result == null)
          throw new InvalidOperationException("Could not find a parent window of the specified AutomationElement.");

        var windowRect = result.Current.BoundingRectangle;
        var windowBounds = new Rectangle((int)windowRect.X, (int)windowRect.Y, (int)windowRect.Width, (int)windowRect.Height);

        return new ResolvedScreenshotElement(CoordinateSystem.Desktop, elementBounds, visibility, windowBounds, unresolvedBounds);
      }
      catch (ElementNotAvailableException ex)
      {
        throw new InvalidOperationException("The specified AutomationElement is no longer available.", ex);
      }
    }

    [CanBeNull]
    private AutomationElement? FindTopMostWindow (AutomationElement element)
    {
      var walker = TreeWalker.ControlViewWalker;
      AutomationElement? result = null, current = element;
      do
      {
        current = walker.GetParent(current);
        if (current == null)
          break;

        if (Equals(current.Current.ControlType, ControlType.Window))
          result = current;
      } while (true);

      if (result == null && element.Current.ControlType.Equals(ControlType.Window))
        return element;

      return result;
    }

    public ResolvedScreenshotElement ResolveBrowserCoordinates (AutomationElement target)
    {
      throw new NotSupportedException();
    }

    public ResolvedScreenshotElement ResolveDesktopCoordinates (AutomationElement target, IBrowserContentLocator locator)
    {
      ArgumentUtility.CheckNotNull("target", target);
      ArgumentUtility.CheckNotNull("locator", locator);

      try
      {
        Point clickPoint;
        ElementVisibility visibility;
        if (target.Current.IsOffscreen)
          visibility = ElementVisibility.NotVisible;
        else if (target.TryGetClickablePoint(out clickPoint))
          visibility = ElementVisibility.FullyVisible;
        else
          visibility = ElementVisibility.NotVisible;

        var elementRect = target.Current.BoundingRectangle;
        var elementBounds = new Rectangle((int)elementRect.X, (int)elementRect.Y, (int)elementRect.Width, (int)elementRect.Height);
        var unresolvedBounds = elementBounds;

        var result = FindTopMostWindow(target);
        if (result == null)
          throw new InvalidOperationException("Could not find a parent window of the specified AutomationElement.");

        var windowRect = result.Current.BoundingRectangle;
        var windowBounds = new Rectangle((int)windowRect.X, (int)windowRect.Y, (int)windowRect.Width, (int)windowRect.Height);

        return new ResolvedScreenshotElement(CoordinateSystem.Desktop, elementBounds, visibility, windowBounds, unresolvedBounds);
      }
      catch (ElementNotAvailableException ex)
      {
        throw new InvalidOperationException("The specified AutomationElement is no longer available.", ex);
      }
    }
  }
}
