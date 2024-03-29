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
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlSelection
{
  /// <summary>
  /// Interface for <see cref="IControlSelector"/> implementations which provide the possibility to select their supported
  /// type of <typeparamref name="TControlObject"/> via the domain property they represent.
  /// </summary>
  /// <typeparam name="TControlObject">The specific <see cref="ControlObject"/> type to select.</typeparam>
  public interface IDomainPropertyControlSelector<out TControlObject> : IControlSelector
      where TControlObject : ControlObject
  {
    /// <summary>
    /// Selects the control within the given <paramref name="context"/> using the given <paramref name="domainProperty"/> and
    /// <paramref name="domainClass"/>. If the <paramref name="domainClass"/> is <see langword="null" />, only the <paramref name="domainProperty"/>
    /// is used for selection.
    /// </summary>
    /// <param name="context">The <see cref="ControlObjectContext"/> to select the <see cref="ControlObject"/> in. Must not be <see langword="null"/>.</param>
    /// <param name="domainProperty">The domain property represented by the <see cref="ControlObject"/>. Must not be <see langword="null"/> or empty.</param>
    /// <param name="domainClass">The class of the domain property represented by the <see cref="ControlObject"/>. Must not be empty.</param>
    /// <returns>The <see cref="ControlObject"/> for the selected control.</returns>
    /// <exception cref="WebTestException">If the control cannot be found.</exception>
    [NotNull]
    TControlObject SelectPerDomainProperty (
        [NotNull] ControlSelectionContext context,
        [NotNull] string domainProperty,
        [CanBeNull] string? domainClass);

    /// <summary>
    /// Selects the control, if it exists, within the given <paramref name="context"/> using the given <paramref name="domainProperty"/> and
    /// <paramref name="domainClass"/>. If the <paramref name="domainClass"/> is <see langword="null" />, only the <paramref name="domainProperty"/>
    /// is used for selection.
    /// </summary>
    /// <param name="context">The <see cref="ControlObjectContext"/> to select the <see cref="ControlObject"/> in. Must not be <see langword="null"/>.</param>
    /// <param name="domainProperty">The domain property represented by the <see cref="ControlObject"/>. Must not be <see langword="null"/> or empty.</param>
    /// <param name="domainClass">The class of the domain property represented by the <see cref="ControlObject"/>. Must not be empty.</param>
    /// <returns>The <see cref="ControlObject"/> for the selected control, or <see langword="null"/> if no control could be found.</returns>
    [CanBeNull]
    TControlObject? SelectOptionalPerDomainProperty (
        [NotNull] ControlSelectionContext context,
        [NotNull] string domainProperty,
        [CanBeNull] string? domainClass);

    /// <summary>
    /// Checks if a control within the given <paramref name="context"/> using the given <paramref name="domainProperty"/> and
    /// <paramref name="domainClass"/> exists. If the <paramref name="domainClass"/> is <see langword="null" />, only the 
    /// <paramref name="domainProperty"/> is used for the check.
    /// </summary>
    /// <param name="context">The <see cref="ControlObjectContext"/> to search the <see cref="ControlObject"/> in. Must not be <see langword="null"/>.</param>
    /// <param name="domainProperty">The domain property represented by the <see cref="ControlObject"/>. Must not be <see langword="null"/> or empty.</param>
    /// <param name="domainClass">The class of the domain property represented by the <see cref="ControlObject"/>. Must not be empty.</param>
    /// <returns><see langword="true" /> if a control has been found; otherwise, <see langword="false" />.</returns>
    bool ExistsPerDomainProperty (
        [NotNull] ControlSelectionContext context,
        [NotNull] string domainProperty,
        [CanBeNull] string? domainClass);
  }
}
