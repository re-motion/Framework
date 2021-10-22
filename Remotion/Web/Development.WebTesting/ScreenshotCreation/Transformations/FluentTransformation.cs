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
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Fluent;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Resolvers;

namespace Remotion.Web.Development.WebTesting.ScreenshotCreation.Transformations
{
  /// <summary>
  /// A transformation that applies the <see cref="ScreenshotTransformationCollection{T}"/> of the target <see cref="IFluentScreenshotElement{T}"/>.
  /// </summary>
  public class FluentTransformation<T> : IScreenshotTransformation<IFluentScreenshotElement<T>>
      where T : notnull
  {
    private readonly ScreenshotTransformationCollection<T> _transformations;

    public FluentTransformation (IFluentScreenshotElement<T> fluentTarget)
    {
      _transformations = fluentTarget.Transformations;
    }

    /// <inheritdoc />
    public int ZIndex
    {
      get { return 0; }
    }

    /// <inheritdoc />
    public ScreenshotTransformationContext<IFluentScreenshotElement<T>> BeginApply (
        ScreenshotTransformationContext<IFluentScreenshotElement<T>> context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      return ConvertContextBack (_transformations.BeginApply (ConvertContext (context)), context.Target);
    }

    /// <inheritdoc />
    public void EndApply (ScreenshotTransformationContext<IFluentScreenshotElement<T>> context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      _transformations.EndApply (ConvertContext (context));
    }

    private ScreenshotTransformationContext<T> ConvertContext (ScreenshotTransformationContext<IFluentScreenshotElement<T>> context)
    {
      return new ScreenshotTransformationContext<T> (
          context.Manipulation,
          context.Graphics,
          context.Target.Resolver,
          context.Target.Target,
          context.ResolvedElement);
    }

    private ScreenshotTransformationContext<IFluentScreenshotElement<T>> ConvertContextBack (
        ScreenshotTransformationContext<T> context,
        IFluentScreenshotElement<T> source)
    {
      return new ScreenshotTransformationContext<IFluentScreenshotElement<T>> (
          context.Manipulation,
          context.Graphics,
          FluentResolver<T>.Instance,
          FluentUtility.CloneWith (source, context.Target),
          context.ResolvedElement);
    }
  }
}