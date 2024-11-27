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

namespace Remotion.Web.Development.WebTesting.ScreenshotCreation
{
  /// <summary>
  /// A context for <see cref="IScreenshotTransformation{T}"/> to apply their transformations on.
  /// </summary>
  public class ScreenshotTransformationContext<T>
      where T : notnull
  {
    private readonly ScreenshotManipulation _manipulation;
    private readonly Graphics _graphics;
    private readonly IScreenshotElementResolver<T> _resolver;
    private readonly T _target;
    private readonly ResolvedScreenshotElement _resolvedElement;

    public ScreenshotTransformationContext (
        ScreenshotManipulation manipulation,
        [NotNull] Graphics graphics,
        [NotNull] IScreenshotElementResolver<T> resolver,
        [NotNull] T target,
        [NotNull] ResolvedScreenshotElement resolvedElement)
    {
      ArgumentUtility.CheckNotNull("graphics", graphics);
      ArgumentUtility.CheckNotNull("resolver", resolver);
      ArgumentUtility.CheckNotNull("target", target);
      ArgumentUtility.CheckNotNull("resolvedElement", resolvedElement);

      _manipulation = manipulation;
      _graphics = graphics;
      _resolver = resolver;
      _target = target;
      _resolvedElement = resolvedElement;
    }

    /// <summary>
    /// The action that is currently in process.
    /// </summary>
    public ScreenshotManipulation Manipulation
    {
      get { return _manipulation; }
    }

    /// <summary>
    /// The <see cref="System.Drawing.Graphics"/> used to draw the <see cref="IScreenshotAnnotation"/>.
    /// </summary>
    public Graphics Graphics
    {
      get { return _graphics; }
    }

    /// <summary>
    /// The resolver used to resolve <see cref="Target"/> to <see cref="ResolvedElement"/>.
    /// </summary>
    public IScreenshotElementResolver<T> Resolver
    {
      get { return _resolver; }
    }

    /// <summary>
    /// The <see cref="ResolvedScreenshotElement"/> that will be passed to the <see cref="IScreenshotAnnotation"/>.
    /// </summary>
    public ResolvedScreenshotElement ResolvedElement
    {
      get { return _resolvedElement; }
    }

    /// <summary>
    /// The target that was resolved to <see cref="ResolvedElement"/>.
    /// </summary>
    public T Target
    {
      get { return _target; }
    }

    /// <summary>
    /// Clones the current process overriding all specified properties.
    /// </summary>
    public ScreenshotTransformationContext<T> CloneWith (
        IScreenshotElementResolver<T>? resolver = null,
        OptionalParameter<T> target = default(OptionalParameter<T>),
        ResolvedScreenshotElement? resolvedElement = null)
    {
      if (target.HasValue && target.Value == null)
        throw new ArgumentNullException("target", "Value of optional parameter cannot be null.");

      return new ScreenshotTransformationContext<T>(
          _manipulation,
          _graphics,
          resolver ?? _resolver,
          Assertion.IsNotNull(target.GetValueOrDefault(_target)),
          resolvedElement ?? _resolvedElement);
    }
  }
}
