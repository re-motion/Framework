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

namespace Remotion.Web.Development.WebTesting.ScreenshotCreation.Fluent
{
  /// <summary>
  /// Represents an object of type <typeparamref name="T"/> in the fluent screenshot API.
  /// </summary>
  public interface IFluentScreenshotElement<T> : IFluentScreenshotElement
  {
    /// <summary>
    /// The <see cref="IScreenshotElementResolver{TElement}"/> that will be used 
    /// to resolve position and visibility of the <see cref="Target"/>.
    /// </summary>
    [NotNull]
    IScreenshotElementResolver<T> Resolver { get; }

    /// <summary>
    /// The underlying object that is wrapped by this <see cref="IFluentScreenshotElement{T}"/>.
    /// </summary>
    [NotNull]
    T Target { get; }

    /// <summary>
    /// A collection of transformations that will be applied to the resolved element 
    /// before applying the annotation. This makes dynamic transformations of the 
    /// resolved element easier.
    /// </summary>
    [NotNull]
    ScreenshotTransformationCollection<T> Transformations { get; set; }
  }
}