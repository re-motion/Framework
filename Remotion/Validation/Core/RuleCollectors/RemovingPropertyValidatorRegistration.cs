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
using Remotion.Utilities;
using Remotion.Validation.Validators;

namespace Remotion.Validation.RuleCollectors
{
  /// <summary>
  /// Represents the information required to remove validators of type <see cref="ValidatorType"/>
  /// registered by collector type <see cref="CollectorTypeToRemoveFrom"/> and provided the <see cref="ValidatorPredicate"/> matches.
  /// </summary>
  public class RemovingPropertyValidatorRegistration
  {
    [NotNull]
    public Type ValidatorType { get; }

    [CanBeNull]
    public Type? CollectorTypeToRemoveFrom { get; }

    [CanBeNull]
    public Func<IPropertyValidator, bool>? ValidatorPredicate { get; }

    [NotNull]
    public IRemovingPropertyValidationRuleCollector RemovingPropertyValidationRuleCollector { get; }

    public RemovingPropertyValidatorRegistration (
        [NotNull] Type validatorType,
        [CanBeNull] Type? collectorTypeToRemoveFrom,
        [CanBeNull] Func<IPropertyValidator, bool>? validatorPredicate,
        [NotNull] IRemovingPropertyValidationRuleCollector removingPropertyValidationRuleCollector)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom("validatorType", validatorType, typeof(IPropertyValidator));
      ArgumentUtility.CheckTypeIsAssignableFrom("collectorTypeToRemoveFrom", collectorTypeToRemoveFrom, typeof(IValidationRuleCollector));
      ArgumentUtility.CheckNotNull("removingPropertyValidationRuleCollector", removingPropertyValidationRuleCollector);

      ValidatorType = validatorType;
      CollectorTypeToRemoveFrom = collectorTypeToRemoveFrom;
      ValidatorPredicate = validatorPredicate;
      RemovingPropertyValidationRuleCollector = removingPropertyValidationRuleCollector;
    }
  }
}
