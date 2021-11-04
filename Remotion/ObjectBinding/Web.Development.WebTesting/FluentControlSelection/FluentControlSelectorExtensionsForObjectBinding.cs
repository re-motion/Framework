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
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlSelection;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.FluentControlSelection;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.FluentControlSelection
{
  /// <summary>
  /// Default extension methods for all re-motion provided <see cref="IControlSelectionCommandBuilder{TControlSelector,TControlObject}"/>
  /// implementations.
  /// </summary>
  public static class FluentControlSelectorExtensionsForObjectBinding
  {
    /// <summary>
    /// Extension method for selecting a control by <paramref name="displayName"/>.
    /// </summary>
    /// <returns>The <see cref="ControlObject"/> for the selected control.</returns>
    /// <exception cref="WebTestException">If the control cannot be found.</exception>
    /// <remarks>
    /// Uses the <see cref="DisplayNameControlSelectionCommandBuilder{TControlSelector,TControlObject}"/>.
    /// </remarks>
    [NotNull]
    public static TControlObject GetByDisplayName<TControlSelector, TControlObject> (
        [NotNull] this IFluentControlSelector<TControlSelector, TControlObject> fluentControlSelector,
        [NotNull] string displayName)
        where TControlSelector : IDisplayNameControlSelector<TControlObject>
        where TControlObject : ControlObject
    {
      ArgumentUtility.CheckNotNull ("fluentControlSelector", fluentControlSelector);
      ArgumentUtility.CheckNotNullOrEmpty ("displayName", displayName);

      return fluentControlSelector.GetControl (new DisplayNameControlSelectionCommandBuilder<TControlSelector, TControlObject> (displayName));
    }

    /// <summary>
    /// Extension method for selecting a control by <paramref name="displayName"/> if it exists.
    /// </summary>
    /// <returns>The <see cref="ControlObject"/> for the selected control, or <see langword="null"/> if no control could be found.</returns>
    /// <remarks>
    /// Uses the <see cref="DisplayNameControlSelectionCommandBuilder{TControlSelector,TControlObject}"/>.
    /// </remarks>
    [CanBeNull]
    public static TControlObject? GetByDisplayNameOrNull<TControlSelector, TControlObject> (
        [NotNull] this IFluentControlSelector<TControlSelector, TControlObject> fluentControlSelector,
        [NotNull] string displayName)
        where TControlSelector : IDisplayNameControlSelector<TControlObject>
        where TControlObject : ControlObject
    {
      ArgumentUtility.CheckNotNull ("fluentControlSelector", fluentControlSelector);
      ArgumentUtility.CheckNotNullOrEmpty ("displayName", displayName);

      return fluentControlSelector.GetControlOrNull (new DisplayNameControlSelectionCommandBuilder<TControlSelector, TControlObject> (displayName));
    }

    /// <summary>
    /// Extension method for checking if a control with the given <paramref name="displayName"/> exists.
    /// </summary>
    /// <returns><see langword="true" /> if a control has been found; otherwise, <see langword="false" />.</returns>
    /// <remarks>
    /// Uses the <see cref="DisplayNameControlSelectionCommandBuilder{TControlSelector,TControlObject}"/>.
    /// </remarks>
    public static bool ExistsByDisplayName<TControlSelector, TControlObject> (
        [NotNull] this IFluentControlSelector<TControlSelector, TControlObject> fluentControlSelector,
        [NotNull] string displayName)
        where TControlSelector : IDisplayNameControlSelector<TControlObject>
        where TControlObject : ControlObject
    {
      ArgumentUtility.CheckNotNull ("fluentControlSelector", fluentControlSelector);
      ArgumentUtility.CheckNotNullOrEmpty ("displayName", displayName);

      return fluentControlSelector.HasControl (new DisplayNameControlSelectionCommandBuilder<TControlSelector, TControlObject> (displayName));
    }

    /// <summary>
    /// Extension method for selecting a control by the <paramref name="domainProperty"/> it represents.
    /// </summary>
    /// <returns>The <see cref="ControlObject"/> for the selected control.</returns>
    /// <exception cref="WebTestException">If the control cannot be found.</exception>
    /// <remarks>
    /// Uses the <see cref="DomainPropertyControlSelectionCommandBuilder{TControlSelector,TControlObject}"/>.
    /// </remarks>
    [NotNull]
    public static TControlObject GetByDomainProperty<TControlSelector, TControlObject> (
        [NotNull] this IFluentControlSelector<TControlSelector, TControlObject> fluentControlSelector,
        [NotNull] string domainProperty,
        [CanBeNull] string? domainClass = null)
        where TControlSelector : IDomainPropertyControlSelector<TControlObject>
        where TControlObject : ControlObject
    {
      ArgumentUtility.CheckNotNull ("fluentControlSelector", fluentControlSelector);
      ArgumentUtility.CheckNotNullOrEmpty ("domainProperty", domainProperty);
      ArgumentUtility.CheckNotEmpty ("domainClass", domainClass);

      return fluentControlSelector.GetControl (
          new DomainPropertyControlSelectionCommandBuilder<TControlSelector, TControlObject> (domainProperty, domainClass));
    }

    /// <summary>
    /// Extension method for selecting a control by the <paramref name="domainProperty"/> it represents.
    /// </summary>
    /// <returns>The <see cref="ControlObject"/> for the selected control, or <see langword="null"/> if no control could be found.</returns>
    /// <remarks>
    /// Uses the <see cref="DomainPropertyControlSelectionCommandBuilder{TControlSelector,TControlObject}"/>.
    /// </remarks>
    [CanBeNull]
    public static TControlObject? GetByDomainPropertyOrNull<TControlSelector, TControlObject> (
        [NotNull] this IFluentControlSelector<TControlSelector, TControlObject> fluentControlSelector,
        [NotNull] string domainProperty,
        [CanBeNull] string? domainClass = null)
        where TControlSelector : IDomainPropertyControlSelector<TControlObject>
        where TControlObject : ControlObject
    {
      ArgumentUtility.CheckNotNull ("fluentControlSelector", fluentControlSelector);
      ArgumentUtility.CheckNotNullOrEmpty ("domainProperty", domainProperty);
      ArgumentUtility.CheckNotEmpty ("domainClass", domainClass);

      return fluentControlSelector.GetControlOrNull (
          new DomainPropertyControlSelectionCommandBuilder<TControlSelector, TControlObject> (domainProperty, domainClass));
    }

    /// <summary>
    /// Extension method for checking if a control representing the given <paramref name="domainProperty"/> exists.
    /// </summary>
    /// <returns><see langword="true" /> if a control has been found; otherwise, <see langword="false" />.</returns>
    /// <remarks>
    /// Uses the <see cref="DomainPropertyControlSelectionCommandBuilder{TControlSelector,TControlObject}"/>.
    /// </remarks>
    public static bool ExistsByDomainProperty<TControlSelector, TControlObject> (
        [NotNull] this IFluentControlSelector<TControlSelector, TControlObject> fluentControlSelector,
        [NotNull] string domainProperty,
        [CanBeNull] string? domainClass = null)
        where TControlSelector : IDomainPropertyControlSelector<TControlObject>
        where TControlObject : ControlObject
    {
      ArgumentUtility.CheckNotNull ("fluentControlSelector", fluentControlSelector);
      ArgumentUtility.CheckNotNullOrEmpty ("domainProperty", domainProperty);
      ArgumentUtility.CheckNotEmpty ("domainClass", domainClass);

      return fluentControlSelector.HasControl (
          new DomainPropertyControlSelectionCommandBuilder<TControlSelector, TControlObject> (domainProperty, domainClass));
    }
  }
}