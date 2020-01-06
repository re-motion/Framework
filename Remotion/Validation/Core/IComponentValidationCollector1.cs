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
using System.Linq.Expressions;
using Remotion.Validation.RuleBuilders;

namespace Remotion.Validation
{
  /// <summary>
  /// Defines an API to register component-specific rules for providing the validation semantic of <typeparamref name="TValidatedType"/>.
  /// </summary>
  public interface IComponentValidationCollector<TValidatedType> : IComponentValidationCollector
  {
    /// <summary>
    /// Registers a new validation rule for a property. 
    /// </summary>
    /// <typeparam name="TProperty">The <see cref="Type"/> of the validated property (used only for syntactical sugar).</typeparam>
    /// <param name="propertySelector">Specifies the property for which the validation rule is added.</param>
    /// <returns>A builder object used for specifying the validation rules of the property.</returns>
    /// <remarks>TODO RM-5906: usage sample</remarks>
    IConditionalAddingComponentRuleBuilder<TValidatedType, TProperty> AddRule<TProperty> (Expression<Func<TValidatedType, TProperty>> propertySelector);

    /// <summary>
    /// Registers which validation rules should be removed from the property. This is used to remove validation rules introduced by other validation 
    /// collectors of <typeparamref name="TValidatedType"/>.
    /// </summary>
    /// <typeparam name="TProperty">The <see cref="Type"/> of the validated property (used only for syntactical sugar).</typeparam>
    /// <param name="propertySelector">Specifies the property for which a specific validation rule should be removed.</param>
    /// <returns>A builder object used for specifying the validation rules to be removed from the property.</returns>
    /// <remarks>TODO RM-5906: usage sample</remarks>
    IRemovingComponentRuleBuilder<TValidatedType, TProperty> RemoveRule<TProperty> (Expression<Func<TValidatedType, TProperty>> propertySelector);
  }
}