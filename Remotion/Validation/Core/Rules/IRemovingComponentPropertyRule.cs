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
using System.Reflection;
using JetBrains.Annotations;
using Remotion.Reflection;

namespace Remotion.Validation.Rules
{
  /// <summary>
  /// Defines a rule associated with a <see cref="Property"/> which specifies the validators to remove from the validation specification. The rule belongs to a component 
  /// via the <see cref="CollectorType"/> and is applied to the validation specification if the component is used within the application.
  /// </summary>
  /// <seealso cref="RemovingComponentPropertyRule"/>
  public interface IRemovingComponentPropertyRule
  {
    /// <summary>
    /// Gets the <see cref="Type"/> of the <see cref="IComponentValidationCollector"/> with which the rule is associated.
    /// </summary>
    Type CollectorType { get; }

    /// <summary>
    /// Gets the property for which the validators should be removed.
    /// </summary>
    IPropertyInformation Property { get; }

    /// <summary>
    /// Gets the validators registered for removal.
    /// </summary>
    IEnumerable<ValidatorRegistration> Validators { get; }

    /// <summary>
    /// Specifies that all validators of <paramref name="validatorType"/> should be removed.
    /// Note: It is not supported to remove validators which are registered with the <see cref="IAddingComponentPropertyRule.IsHardConstraint"/> flag set to <see langword="true" />.
    /// Attempting to do so will result in an exception when the validation rules aggregated.
    /// </summary>
    void RegisterValidator ([NotNull] Type validatorType);

    /// <summary>
    /// Specifies that all validators of <paramref name="validatorType"/> registered by <paramref name="collectorTypeToRemoveFrom"/> should be removed.
    /// Note: It is not supported to remove validators which are registered with the <see cref="IAddingComponentPropertyRule.IsHardConstraint"/> flag set to <see langword="true" />.
    /// Attempting to do so will result in an exception when the validation rules aggregated.
    /// </summary>
    void RegisterValidator ([NotNull] Type validatorType, [CanBeNull] Type collectorTypeToRemoveFrom);
  }
}