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
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.Web.Development.WebTesting.FluentControlSelection
{
  /// <summary>
  /// Interface for <see cref="FluentControlSelector{TControlSelector,TControlObject}"/>, whose only job is to allow <see cref="GetControl"/> to
  /// be implemented explicitly and therefore prevent IntelliSense polluting.
  /// </summary>
  public interface IFluentControlSelector<out TControlSelector, TControlObject>
      where TControlSelector : IControlSelector
      where TControlObject : ControlObject
  {
    /// <summary>
    /// Performs the selection and returns the actual <see cref="ControlObject"/>.
    /// </summary>
    /// <param name="selectionCommandBuilder">The selection command builder which is combined with the <see cref="IControlSelector"/>.</param>
    /// <returns>The <see cref="ControlObject"/> for the selected control.</returns>
    /// <exception cref="WebTestException">If multiple matching controls are found but the specific <typeparamref name="TControlSelector"/> requires an exact match.</exception>
    /// <exception cref="WebTestException">If the control cannot be found.</exception>
    [NotNull]
    TControlObject GetControl ([NotNull] IControlSelectionCommandBuilder<TControlSelector, TControlObject> selectionCommandBuilder);

    /// <summary>
    /// Performs the selection and returns the actual <see cref="ControlObject"/> if the control exists.
    /// </summary>
    /// <param name="selectionCommandBuilder">The selection command builder which is combined with the <see cref="IControlSelector"/>.</param>
    /// <returns>The <see cref="ControlObject"/> for the selected control, or <see langword="null"/> if no control could be found.</returns>
    /// <exception cref="WebTestException">If multiple matching controls are found but the specific <typeparamref name="TControlSelector"/> requires an exact match.</exception>
    [CanBeNull]
    TControlObject? GetControlOrNull ([NotNull] IControlOptionalSelectionCommandBuilder<TControlSelector, TControlObject> selectionCommandBuilder);

    /// <summary>
    /// Tries to find the control and returns <see langword="true" /> if it exists.
    /// </summary>
    /// <param name="selectionCommandBuilder">The selection command builder which is combined with the <see cref="IControlSelector"/>. Must not be <see langword="null" />.</param>
    /// <returns><see langword="true" /> if a control has been found; otherwise, <see langword="false" />.</returns>
    bool HasControl ([NotNull] IControlExistsCommandBuilder<TControlSelector> selectionCommandBuilder);
  }
}