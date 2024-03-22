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
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.ScreenshotCreation.Fluent
{
  /// <summary>
  /// Decorates a given <see cref="IFluentScreenshotElement{T}"/> but uses <see cref="ElementVisibility"/>.<see cref="ElementVisibility.PartiallyVisible"/>
  /// as <see cref="MinimumElementVisibility"/> if the actual visibility is more restrictive.
  /// </summary>
  public class AllowPartialVisibilityFluentScreenshotElementDecorator<T> : IFluentScreenshotElement<T>
      where T : notnull
  {
    private readonly IFluentScreenshotElement<T> _inner;

    public AllowPartialVisibilityFluentScreenshotElementDecorator (IFluentScreenshotElement<T> inner)
    {
      ArgumentUtility.CheckNotNull("inner", inner);

      _inner = inner;
    }

    /// <inheritdoc />
    public ElementVisibility? MinimumElementVisibility
    {
      get
      {
        var minimumElementVisibility = _inner.MinimumElementVisibility;
        return minimumElementVisibility is null or > ElementVisibility.PartiallyVisible
            ? ElementVisibility.PartiallyVisible
            : minimumElementVisibility.Value;
      }
      set => _inner.MinimumElementVisibility = value;
    }

    /// <inheritdoc />
    public ResolvedScreenshotElement ResolveBrowserCoordinates () => _inner.ResolveBrowserCoordinates();

    /// <inheritdoc />
    public ResolvedScreenshotElement ResolveDesktopCoordinates (IBrowserContentLocator locator) => _inner.ResolveDesktopCoordinates(locator);

    /// <inheritdoc />
    public IScreenshotElementResolver<T> Resolver => _inner.Resolver;

    /// <inheritdoc />
    public T Target => _inner.Target;

    /// <inheritdoc />
    public ScreenshotTransformationCollection<T> Transformations
    {
      get => _inner.Transformations;
      set => _inner.Transformations = value;
    }
  }
}
