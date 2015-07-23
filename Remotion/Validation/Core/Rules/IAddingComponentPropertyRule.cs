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
using System.ComponentModel;
using System.Reflection;
using FluentValidation;
using FluentValidation.Validators;
using JetBrains.Annotations;
using Remotion.Reflection;
using Remotion.Validation.Merging;

namespace Remotion.Validation.Rules
{
  /// <summary>
  /// Defines a rule associated with a <see cref="Property"/> which can have multiple validators. The validators of this rule belong to a component via 
  /// the <see cref="CollectorType"/> and are added to the validation specification if the component is used within the application.
  /// </summary>
  /// <seealso cref="AddingComponentPropertyRule"/>
  public interface IAddingComponentPropertyRule : IValidationRule
  {
    /// <summary>
    /// Gets the <see cref="Type"/> of the <see cref="IComponentValidationCollector"/> with which the rule is associated.
    /// </summary>
    Type CollectorType { get; }

    /// <summary>
    /// Gets the property for which the validators will be added.
    /// </summary>
    IPropertyInformation Property { get; }

    /// <summary>
    /// Gets a flag whether the rule can be removed via an <see cref="IRemovingComponentPropertyRule"/>. 
    /// </summary>
    bool IsHardConstraint { get; }

    /// <summary>
    /// Registers a validator with this <see cref="IAddingComponentPropertyRule"/>.
    /// </summary>
    void RegisterValidator ([NotNull] IPropertyValidator validator);

    /// <summary>
    /// Sets the <see cref="IsHardConstraint"/> flag, making the rule non-removeable.
    /// </summary>
    void SetHardConstraint ();

    /// <summary>
    /// Applies the <paramref name="propertyValidatorExtractor"/> to the registered <see cref="IValidationRule.Validators"/>.
    /// </summary>
    [EditorBrowsable (EditorBrowsableState.Never)]
    void ApplyRemoveValidatorRegistrations ([NotNull] IPropertyValidatorExtractor propertyValidatorExtractor);
  }
}