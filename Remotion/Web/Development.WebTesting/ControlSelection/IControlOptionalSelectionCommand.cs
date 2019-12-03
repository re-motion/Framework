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

namespace Remotion.Web.Development.WebTesting.ControlSelection
{
  /// <summary>
  /// Base interface for all <typeparamref name="TControlObject"/> optional selection commands. A selection command encapsulates an implementation
  /// of <see cref="IControlSelector"/> and all necessary selection parameters.
  /// </summary>
  /// <typeparam name="TControlObject">The specific <see cref="ControlObject"/> type to select.</typeparam>
  public interface IControlOptionalSelectionCommand<out TControlObject>
      where TControlObject : ControlObject
  {
    /// <summary>
    /// Performs the selection within the given <paramref name="context"/> and returns the actual control or <see langword="null" /> if it does not exist.
    /// </summary>
    /// <returns>The <see cref="ControlObject"/> for the selected control, or <see langword="null"/> if no control could be found.</returns>
    /// <exception cref="WebTestException">If multiple matching controls are found but the specific implementation of <see cref="IControlOptionalSelectionCommand{TControlObject}"/> requires an exact match.</exception>
    [CanBeNull]
    TControlObject SelectOptional ([NotNull] ControlSelectionContext context);
  }
}
