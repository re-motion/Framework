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

namespace Remotion.Web.Development.WebTesting.ScreenshotCreation.Fluent
{
  /// <summary>
  /// Common <see cref="IFluentScreenshotElement{T}"/> implementation. It hides all <see cref="IFluentScreenshotElement{T}"/> members
  /// in order to prevent the fluent APi user from seeing the guts of the API.
  /// </summary>
  /// <remarks>
  /// Generally speaking fluent API methods should:
  /// <list type="bullet">
  /// <item><description>return <see cref="FluentScreenshotElement{T}"/></description></item>
  /// <item><description>and extend <see cref="IFluentScreenshotElement{T}"/></description></item>
  /// </list>
  /// </remarks>
  public class FluentScreenshotElement<T> : IFluentScreenshotElement<T>
  {
    protected readonly IScreenshotElementResolver<T> Resolver;
    protected readonly T Target;

    protected ScreenshotTransformationCollection<T> Transformations;
    protected ElementVisibility? MinimumElementVisibility;

    public FluentScreenshotElement (
        [NotNull] T target,
        [NotNull] IScreenshotElementResolver<T> resolver,
        [CanBeNull] ElementVisibility? minimumElementVisibility = null)
    {
      ArgumentUtility.CheckNotNull ("target", target);
      ArgumentUtility.CheckNotNull ("resolver", resolver);

      Target = target;
      Resolver = resolver;

      Transformations = new ScreenshotTransformationCollection<T>();
      MinimumElementVisibility = minimumElementVisibility;
    }

    /// <inheritdoc />
    ElementVisibility? IFluentScreenshotElement.MinimumElementVisibility
    {
      get { return MinimumElementVisibility; }
      set { MinimumElementVisibility = value; }
    }

    /// <inheritdoc />
    IScreenshotElementResolver<T> IFluentScreenshotElement<T>.Resolver
    {
      get { return Resolver; }
    }

    /// <inheritdoc />
    T IFluentScreenshotElement<T>.Target
    {
      get { return Target; }
    }

    /// <inheritdoc />
    ScreenshotTransformationCollection<T> IFluentScreenshotElement<T>.Transformations
    {
      get { return Transformations; }
      set { Transformations = value; }
    }

    /// <inheritdoc />
    ResolvedScreenshotElement IFluentScreenshotElement.ResolveBrowserCoordinates ()
    {
      return Resolver.ResolveBrowserCoordinates (Target);
    }

    /// <inheritdoc />
    ResolvedScreenshotElement IFluentScreenshotElement.ResolveDesktopCoordinates (IBrowserContentLocator locator)
    {
      ArgumentUtility.CheckNotNull ("locator", locator);

      return Resolver.ResolveDesktopCoordinates (Target, locator);
    }
  }
}