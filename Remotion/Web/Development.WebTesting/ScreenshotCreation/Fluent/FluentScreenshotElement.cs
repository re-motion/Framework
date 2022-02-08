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
using System.Threading;
using Coypu;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.Utilities;

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
  public class FluentScreenshotElement<T> : IFluentScreenshotElement<T>, IFluentScreenshotElementWithCovariance<T>
      where T : notnull
  {
    private readonly IScreenshotElementResolver<T> _resolver;
    private readonly T _target;

    private ScreenshotTransformationCollection<T> _transformations;
    private ElementVisibility? _minimumElementVisibility;

    public FluentScreenshotElement (
        [NotNull] T target,
        [NotNull] IScreenshotElementResolver<T> resolver,
        [CanBeNull] ElementVisibility? minimumElementVisibility = null)
    {
      ArgumentUtility.CheckNotNull("target", target);
      ArgumentUtility.CheckNotNull("resolver", resolver);

      _target = target;
      _resolver = resolver;

      _transformations = new ScreenshotTransformationCollection<T>();
      _minimumElementVisibility = minimumElementVisibility;
    }

    public FluentScreenshotElement<T> ScrollIntoView ()
    {
      if (_target is not ElementScope elementScope)
        throw new InvalidOperationException($"'{nameof(ScrollIntoView)}' can only be used with Coypu.ElementScope instances.");

      JavaScriptExecutor.GetJavaScriptExecutor(elementScope).ExecuteScript("arguments[0].scrollIntoView(true);", elementScope.Native);

      // The scrolling sometimes takes some time - 200ms should be enough.
      Thread.Sleep(200);

      return this;
    }

    /// <inheritdoc />
    T IFluentScreenshotElementWithCovariance<T>.Target
    {
      get { return _target; }
    }

    /// <inheritdoc />
    ElementVisibility? IFluentScreenshotElement.MinimumElementVisibility
    {
      get { return _minimumElementVisibility; }
      set { _minimumElementVisibility = value; }
    }

    /// <inheritdoc />
    IScreenshotElementResolver<T> IFluentScreenshotElement<T>.Resolver
    {
      get { return _resolver; }
    }

    /// <inheritdoc />
    T IFluentScreenshotElement<T>.Target
    {
      get { return _target; }
    }

    /// <inheritdoc />
    ScreenshotTransformationCollection<T> IFluentScreenshotElement<T>.Transformations
    {
      get { return _transformations; }
      set { _transformations = value; }
    }

    /// <inheritdoc />
    ResolvedScreenshotElement IFluentScreenshotElement.ResolveBrowserCoordinates ()
    {
      return _resolver.ResolveBrowserCoordinates(_target);
    }

    /// <inheritdoc />
    ResolvedScreenshotElement IFluentScreenshotElement.ResolveDesktopCoordinates (IBrowserContentLocator locator)
    {
      ArgumentUtility.CheckNotNull("locator", locator);

      return _resolver.ResolveDesktopCoordinates(_target, locator);
    }
  }
}
