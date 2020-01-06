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

namespace Remotion.Validation.RuleBuilders
{
  // TODO RM-5960: Rename to remove "Component". Replace with "Validation".
  /// <summary>
  /// Provides an API for removing a validation rule added by another component.
  /// </summary>
  /// <seealso cref="RemovingComponentRuleBuilder{T,TProperty}"/>
  public interface IRemovingComponentRuleBuilder<TValidatedType, out TProperty>
  {
    /// <summary>
    /// Removes validators of type <typeparamref name="TValidator"/>.
    /// </summary>
    /// <returns>An object to continue the fluent specification.</returns>
    [NotNull]
    IRemovingComponentRuleBuilder<TValidatedType, TProperty> Validator<TValidator> ();

    /// <summary>
    /// Removes validators of type <typeparamref name="TValidator"/> registered by the specified collector <typeparamref name="TCollectorTypeToRemoveFrom"/>.
    /// </summary>
    /// <returns>An object to continue the fluent specification.</returns>
    [NotNull]
    IRemovingComponentRuleBuilder<TValidatedType, TProperty> Validator<TValidator, TCollectorTypeToRemoveFrom> ();

    /// <summary>
    /// Removes validators of type <paramref name="validatorType"/>.
    /// </summary>
    /// <returns>An object to continue the fluent specification.</returns>
    [NotNull]
    IRemovingComponentRuleBuilder<TValidatedType, TProperty> Validator ([NotNull] Type validatorType);

    /// <summary>
    /// Removes validators of type <paramref name="validatorType"/> registered by the specified collector <paramref name="collectorTypeToRemoveFrom"/>.
    /// </summary>
    /// <returns>An object to continue the fluent specification.</returns>
    [NotNull]
    IRemovingComponentRuleBuilder<TValidatedType, TProperty> Validator (
        [NotNull] Type validatorType,
        [CanBeNull] Type collectorTypeToRemoveFrom);
  }
}