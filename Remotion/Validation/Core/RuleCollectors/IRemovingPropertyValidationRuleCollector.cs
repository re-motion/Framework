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
using System.Collections.Generic;
using JetBrains.Annotations;
using Remotion.Reflection;
using Remotion.Validation.Implementation;
using Remotion.Validation.Validators;

namespace Remotion.Validation.RuleCollectors
{
  /// <summary>
  /// Defines a rule associated with an <see cref="IPropertyInformation"/> which specifies the validators to remove from the validation specification. The rule belongs to a component 
  /// via the <see cref="CollectorType"/> and is applied to the validation specification if the component is used within the application.
  /// </summary>
  /// <seealso cref="RemovingPropertyValidationRuleCollector"/>
  public interface IRemovingPropertyValidationRuleCollector
  {
    /// <summary>
    /// Gets the <see cref="Type"/> of the <see cref="IValidationRuleCollector"/> with which the rule is associated.
    /// </summary>
    Type CollectorType { get; }

    /// <summary>
    /// Gets the property for which the validators should be removed.
    /// </summary>
    IPropertyInformation Property { get; }

    /// <summary>
    /// Gets the validators registered for removal.
    /// </summary>
    IEnumerable<RemovingPropertyValidatorRegistration> Validators { get; }

    /// <summary>
    /// Specifies that all validators of <paramref name="validatorType"/> should be removed, provided that they where registered by <paramref name="collectorTypeToRemoveFrom"/>
    /// and are matched by <paramref name="validatorPredicate"/>.
    /// </summary>
    /// <remarks>
    /// It is only supported to remove validators which are registered with the <see cref="IAddingPropertyValidationRuleCollector.IsRemovable"/> flag set to <see langword="true" />.
    /// Attempting to do so will result in an exception when the validation rules aggregated.
    /// </remarks>
    void RegisterValidator (
        [NotNull] Type validatorType,
        [CanBeNull] Type collectorTypeToRemoveFrom,
        [CanBeNull] Func<IPropertyValidator, bool> validatorPredicate);
  }
}