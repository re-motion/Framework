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
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting.ScreenshotCreation.Fluent
{
  /// <summary>
  /// Provides helper methods for <see cref="ISelfResolvable"/>s.
  /// </summary>
  public static class SelfResolvableFluentScreenshot
  {
    private class GenericFluentScreenshotElement<T> : FluentScreenshotElement<T>
        where T : ISelfResolvable
    {
      public GenericFluentScreenshotElement (
          [NotNull] T target,
          [CanBeNull] ElementVisibility? minimumElementVisibility = null)
          : base (target, GenericFluentResolver<T>.Instance, minimumElementVisibility)
      {
      }
    }

    private class GenericFluentResolver<T> : IScreenshotElementResolver<T>
        where T : ISelfResolvable
    {
      public static readonly GenericFluentResolver<T> Instance = new GenericFluentResolver<T>();

      private GenericFluentResolver ()
      {
      }

      /// <inheritdoc />
      public ResolvedScreenshotElement ResolveBrowserCoordinates (T target)
      {
        ArgumentUtility.CheckNotNull ("target", target);

        return target.ResolveBrowserCoordinates();
      }

      /// <inheritdoc />
      public ResolvedScreenshotElement ResolveDesktopCoordinates (T target, IBrowserContentLocator locator)
      {
        ArgumentUtility.CheckNotNull ("target", target);

        return target.ResolveDesktopCoordinates (locator);
      }
    }

    /// <summary>
    /// Creates a new <see cref="FluentScreenshotElement{T}"/> that will resolve itself 
    /// from the specified <see cref="ISelfResolvable"/> <paramref name="target"/>.
    /// </summary>
    public static FluentScreenshotElement<T> Create<T> (
        [NotNull] T target,
        [CanBeNull] ElementVisibility? minimumElementVisibility = null)
        where T : ISelfResolvable
    {
      ArgumentUtility.CheckNotNull ("target", target);

      return new GenericFluentScreenshotElement<T> (target, minimumElementVisibility);
    }
  }
}