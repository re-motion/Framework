﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Interface for all <see cref="WebTestObject{TWebTestObjectContext}"/>s which host child controls within their scope.
  /// </summary>
  public interface IControlHost
  {
    /// <summary>
    /// Finds a child <see cref="ControlObject"/> within the host's scope as specified by the given <paramref name="controlSelectionCommand"/>.
    /// </summary>
    /// <typeparam name="TControlObject">The type of the child control to be found.</typeparam>
    /// <param name="controlSelectionCommand">Specifies the child control, see <see cref="IControlSelectionCommand{TControlObject}"/> for more information.</param>
    /// <returns>The specified child <see cref="ControlObject"/>.</returns>
    /// <exception cref="WebTestException">If the selection command cannot unambiguously identify the child control.</exception>
    /// <exception cref="WebTestException">If the selection command cannot find the child control.</exception>
    [NotNull]
    TControlObject GetControl<TControlObject> ([NotNull] IControlSelectionCommand<TControlObject> controlSelectionCommand)
        where TControlObject : ControlObject;

    /// <summary>
    /// Tries to find a child <see cref="ControlObject"/> within the host's scope as specified by the given <paramref name="controlSelectionCommand"/>.
    /// </summary>
    /// <typeparam name="TControlObject">The type of the child control to be found.</typeparam>
    /// <param name="controlSelectionCommand">Specifies the child control, see <see cref="IControlSelectionCommand{TControlObject}"/> for more information.</param>
    /// <returns>The specified child <see cref="ControlObject"/> or <see langword="null"/> if no <see cref="ControlObject"/> could be found.</returns>
    /// <exception cref="WebTestException">If the selection command cannot unambiguously identify the child control.</exception>
    [CanBeNull]
    TControlObject? GetControlOrNull<TControlObject> ([NotNull] IControlOptionalSelectionCommand<TControlObject> controlSelectionCommand)
        where TControlObject : ControlObject;

    /// <summary>
    /// Checks whether a child <see cref="ControlObject"/> is within the host's scope as specified by the given <paramref name="controlSelectionCommand"/>.
    /// </summary>
    /// <param name="controlSelectionCommand">Specifies the child control, see <see cref="IControlSelectionCommand{TControlObject}"/> for more information.</param>
    /// <returns>Returns <see langword="true"/> if the specified child <see cref="ControlObject"/> could be found; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="WebTestException">If the selection command cannot unambiguously identify the child control.</exception>
    bool HasControl ([NotNull] IControlExistsCommand controlSelectionCommand);
  }
}
