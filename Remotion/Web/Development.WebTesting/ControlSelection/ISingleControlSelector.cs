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
using Coypu;
using JetBrains.Annotations;

namespace Remotion.Web.Development.WebTesting.ControlSelection
{
  /// <summary>
  /// Interface for <see cref="IControlSelector"/> implementations which provide the possibility to select the only existing control matching their
  /// supported type of <typeparamref name="TControlObject"/>.
  /// </summary>
  /// <typeparam name="TControlObject">The specific <see cref="ControlObject"/> type to select.</typeparam>
  public interface ISingleControlSelector<out TControlObject> : IControlSelector
      where TControlObject : ControlObject
  {
    /// <summary>
    /// Selects the only control within the given <paramref name="context"/> matching the type <typeparamref name="TControlObject"/>.
    /// </summary>
    /// <returns>The control object.</returns>
    /// <exception cref="AmbiguousException">If multiple controls of type <typeparamref name="TControlObject"/> are found.</exception>
    /// <exception cref="MissingHtmlException">If no matching control can be found.</exception>
    TControlObject SelectSingle ([NotNull] ControlSelectionContext context);
  }
}