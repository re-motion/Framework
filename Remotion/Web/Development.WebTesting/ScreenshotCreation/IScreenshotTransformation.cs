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

namespace Remotion.Web.Development.WebTesting.ScreenshotCreation
{
  /// <summary>
  /// Represents a transformation that is applied to an annotation after resolving and before drawing.
  /// In comparison to building a custom resolver a <see cref="IScreenshotTransformation{T}"/> has the 
  /// ability to change the <see cref="Graphics"/> object used to draw or change the <see cref="ResolvedScreenshotElement"/>
  /// that will be used to draw the annotation. For a full list of what is available see <see cref="ScreenshotTransformationContext{T}"/>.
  /// </summary>
  public interface IScreenshotTransformation<T>
      where T : notnull
  {
    /// <summary>
    /// A <see cref="IScreenshotTransformation{T}"/> with a higher <see cref="ZIndex"/> will be applied 
    /// later than a <see cref="IScreenshotTransformation{T}"/> with a lower <see cref="ZIndex"/>.
    /// </summary>
    int ZIndex { get; }

    /// <summary>
    /// Applies the transformation, returning a new <see cref="ScreenshotTransformationContext{T}"/> 
    /// which will be used by following transformations and the annotation.
    /// </summary>
    ScreenshotTransformationContext<T> BeginApply ([NotNull] ScreenshotTransformationContext<T> context);

    /// <summary>
    /// Reverts any changes made during <see cref="BeginApply"/> that should not be persistent.
    /// </summary>
    /// <example>
    /// Changing the clipping area of the <see cref="Graphics"/> object in <see cref="BeginApply"/> should be undone in <see cref="EndApply"/>.
    /// </example>
    void EndApply ([NotNull] ScreenshotTransformationContext<T> context);
  }
}
