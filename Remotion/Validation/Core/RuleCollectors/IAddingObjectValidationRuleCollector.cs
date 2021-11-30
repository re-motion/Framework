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
using System.ComponentModel;
using JetBrains.Annotations;
using Remotion.Reflection;
using Remotion.Validation.Implementation;
using Remotion.Validation.Merging;
using Remotion.Validation.Rules;
using Remotion.Validation.Validators;

namespace Remotion.Validation.RuleCollectors
{
  /// <summary>
  /// Defines a rule associated with a <see cref="Type"/> which can have multiple validators. The validators of this rule belong to a component via 
  /// the <see cref="CollectorType"/> and are added to the validation specification if the component is used within the application.
  /// </summary>
  /// <seealso cref="AddingObjectValidationRuleCollector{TValidatedType}"/>
  /// <seealso cref="AddingObjectValidationRuleCollector"/>
  public interface IAddingObjectValidationRuleCollector
  {
    /// <summary>The validators that are grouped under this rule.</summary>
    IEnumerable<IObjectValidator> Validators { get; }

    /// <summary>
    /// Gets the <see cref="Type"/> for which the validators will be added.
    /// </summary>
    ITypeInformation ValidatedType { get; }

    /// <summary>
    /// Gets the <see cref="Type"/> of the <see cref="IValidationRuleCollector"/> with which the rule is associated.
    /// </summary>
    Type CollectorType { get; }

    /// <summary>
    /// Gets a flag whether the rule can be removed via an <see cref="IRemovingPropertyValidationRuleCollector"/>. 
    /// </summary>
    bool IsRemovable { get; }

    /// <summary>
    /// Sets the condition for evaluating the registered validators.
    /// </summary>
    void SetCondition<TValidatedType> ([NotNull] Func<TValidatedType, bool> predicate);

    /// <summary>
    /// Registers a validator with this <see cref="IAddingObjectValidationRuleCollector"/>.
    /// </summary>
    void RegisterValidator ([NotNull] Func<ObjectValidationRuleInitializationParameters, IObjectValidator> validatorFactory);

    /// <summary>
    /// Sets the <see cref="IsRemovable"/> flag, making the rule removable.
    /// </summary>
    void SetRemovable ();

    /// <summary>
    /// Applies the <paramref name="objectValidatorExtractor"/> to the registered <see cref="Validators"/>.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    void ApplyRemoveValidatorRegistrations ([NotNull] IObjectValidatorExtractor objectValidatorExtractor);

    /// <summary>
    /// Creates the <see cref="IValidationRule"/> for this <see cref="ValidatedType"/>.
    /// </summary>
    IValidationRule CreateValidationRule ([NotNull] IValidationMessageFactory validationMessageFactory);
  }
}
