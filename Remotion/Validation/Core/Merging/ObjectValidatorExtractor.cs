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
using System.Collections.Generic;
using System.Linq;
using Remotion.Reflection;
using Remotion.Utilities;
using Remotion.Validation.Implementation;
using Remotion.Validation.RuleCollectors;
using Remotion.Validation.Validators;

namespace Remotion.Validation.Merging
{
  /// <summary>
  /// Default implementation of the <see cref="IPropertyValidatorExtractor"/> interface.
  /// </summary>
  public class ObjectValidatorExtractor : IObjectValidatorExtractor
  {
    private readonly ILookup<Type, RemovingObjectValidatorRegistration> _validatorTypesToRemove;
    private readonly ILogContext _logContext;

    public ObjectValidatorExtractor (IEnumerable<RemovingObjectValidatorRegistration> removingObjectValidatorRegistrations, ILogContext logContext)
    {
      ArgumentUtility.CheckNotNull("removingObjectValidatorRegistrations", removingObjectValidatorRegistrations);
      ArgumentUtility.CheckNotNull("logContext", logContext);

      _validatorTypesToRemove = removingObjectValidatorRegistrations.ToLookup(r => r.ValidatorType);
      _logContext = logContext;
    }

    public IEnumerable<IObjectValidator> ExtractObjectValidatorsToRemove (IAddingObjectValidationRuleCollector addingObjectValidationRuleCollector)
    {
      ArgumentUtility.CheckNotNull("addingObjectValidationRuleCollector", addingObjectValidationRuleCollector);

      foreach (var existingValidator in addingObjectValidationRuleCollector.Validators)
      {
        var removingObjectValidatorRegistrations = GetRemovingObjectValidatorRegistrations(existingValidator, addingObjectValidationRuleCollector).ToArray();
        if (removingObjectValidatorRegistrations.Any())
        {
          _logContext.ValidatorRemoved(existingValidator, removingObjectValidatorRegistrations, addingObjectValidationRuleCollector);
          yield return existingValidator;
        }
      }
    }

    private IEnumerable<RemovingObjectValidatorRegistration> GetRemovingObjectValidatorRegistrations (
        IObjectValidator validator,
        IAddingObjectValidationRuleCollector addingObjectValidationRuleCollector)
    {
      return _validatorTypesToRemove[validator.GetType()]
          .Where(rwc => IsTypeMatch(addingObjectValidationRuleCollector.ValidatedType, rwc.RemovingObjectValidationRuleCollector.ValidatedType))
          .Where(rwc => IsCollectorTypeMatch(addingObjectValidationRuleCollector.CollectorType, rwc.CollectorTypeToRemoveFrom))
          .Where(rwc => IsPredicateMatch(validator, rwc.ValidatorPredicate));

      static bool IsTypeMatch (ITypeInformation currentType, ITypeInformation typeToMatch)
      {
        // TODO-5906: should the object validator removal be based on the inheritance hierarchy or constrained to the exact type?
        return currentType.IsAssignableFrom(typeToMatch);
      }

      static bool IsCollectorTypeMatch (Type currentCollectorType, Type? collectorTypeToMatch)
      {
        return collectorTypeToMatch == null
               || collectorTypeToMatch == currentCollectorType;
      }

      static bool IsPredicateMatch (IObjectValidator currentValidator, Func<IObjectValidator, bool>? predicateToMatch)
      {
        return predicateToMatch == null
               || predicateToMatch(currentValidator);
      }
    }
  }
}
