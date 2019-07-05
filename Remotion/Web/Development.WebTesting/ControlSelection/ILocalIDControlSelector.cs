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
  /// Interface for <see cref="IControlSelector"/> implementations which provide the possibility to select their supported
  /// type of <typeparamref name="TControlObject"/> via their local ID within the naming container.
  /// </summary>
  /// <typeparam name="TControlObject">The specific <see cref="ControlObject"/> type to select.</typeparam>
  public interface ILocalIDControlSelector<out TControlObject> : IControlSelector
      where TControlObject : ControlObject
  {
    /// <summary>
    /// Selects the control within the given <paramref name="context"/> using the given <paramref name="localID"/>.
    /// </summary>
    /// <param name="context">The <see cref="ControlSelectionContext"/> to select the <see cref="ControlObject"/> in. Must not be <see langword="null"/>.</param>
    /// <param name="localID">The local ID of the <see cref="ControlObject"/>. Must not be <see langword="null"/> or empty.</param>
    /// <returns>The <see cref="ControlObject"/> for the selected control.</returns>
    /// <exception cref="MissingHtmlException">If the control cannot be found.</exception>
    [NotNull]
    TControlObject SelectPerLocalID ([NotNull] ControlSelectionContext context, [NotNull] string localID);
    
    /// <summary>
    /// Selects the control, if it exists, within the given <paramref name="context"/> using the given <paramref name="localID"/>.
    /// </summary>
    /// <param name="context">The <see cref="ControlSelectionContext"/> to select the <see cref="ControlObject"/> in. Must not be <see langword="null"/>.</param>
    /// <param name="localID">The local ID of the <see cref="ControlObject"/>. Must not be <see langword="null"/> or empty.</param>
    /// <returns>The <see cref="ControlObject"/> for the selected control, or <see langword="null"/> if no control could be found.</returns>
    [CanBeNull]
    TControlObject SelectOptionalPerLocalID ([NotNull] ControlSelectionContext context, [NotNull] string localID);

    /// <summary>
    /// Checks if a control within the given <paramref name="context"/> using the given <paramref name="localID"/> exists.
    /// </summary>
    /// <param name="context">The <see cref="ControlSelectionContext"/> to search the <see cref="ControlObject"/> in. Must not be <see langword="null"/>.</param>
    /// <param name="localID">The local ID of the <see cref="ControlObject"/>. Must not be <see langword="null"/> or empty.</param>
    /// <returns><see langword="true" /> if a control has been found; otherwise, <see langword="false" />.</returns>
    bool ExistsPerLocalID ([NotNull] ControlSelectionContext context, [NotNull] string localID);
  }
}