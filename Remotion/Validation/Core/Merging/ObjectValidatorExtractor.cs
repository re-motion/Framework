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
using System.Linq;
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
    private readonly ILookup<Type, ObjectValidatorRegistrationWithContext> _validatorTypesToRemove;
    private readonly ILogContext _logContext;

    public ObjectValidatorExtractor (IEnumerable<ObjectValidatorRegistrationWithContext> removedPropertyRuleRegistrations, ILogContext logContext)
    {
      ArgumentUtility.CheckNotNull ("removedPropertyRuleRegistrations", removedPropertyRuleRegistrations);
      ArgumentUtility.CheckNotNull ("logContext", logContext);

      _validatorTypesToRemove = removedPropertyRuleRegistrations.ToLookup (r => r.ValidatorRegistration.ValidatorType);
      _logContext = logContext;
    }

    public IEnumerable<IObjectValidator> ExtractObjectValidatorsToRemove (IAddingObjectValidationRuleCollector addingObjectValidationRuleCollector)
    {
      ArgumentUtility.CheckNotNull ("addingObjectValidationRuleCollector", addingObjectValidationRuleCollector);

      foreach (var existingValidator in addingObjectValidationRuleCollector.Validators)
      {
        var removingValidatorRegistrationsWithContext = GetRemovingObjectValidatorRegistrations (existingValidator, addingObjectValidationRuleCollector).ToArray();
        if (removingValidatorRegistrationsWithContext.Any())
        {
          _logContext.ValidatorRemoved (existingValidator, removingValidatorRegistrationsWithContext, addingObjectValidationRuleCollector);
          yield return existingValidator;
        }
      }
    }

    private IEnumerable<ObjectValidatorRegistrationWithContext> GetRemovingObjectValidatorRegistrations (
        IObjectValidator validator,
        IAddingObjectValidationRuleCollector addingObjectValidationRuleCollector)
    {
      return _validatorTypesToRemove[validator.GetType()]
          .Where (
              rwc =>
                  // TODO-5906: should the object validator removal be based on the inheritance hierarchy or constrained to the exact type?
                  // ReSharper disable PossibleNullReferenceException
                  addingObjectValidationRuleCollector.ValidatedType.IsAssignableFrom (rwc.RemovingObjectValidationRuleCollector.ValidatedType)
                  // ReSharper restore PossibleNullReferenceException
                  && (rwc.ValidatorRegistration.CollectorTypeToRemoveFrom == null
                      || rwc.ValidatorRegistration.CollectorTypeToRemoveFrom == addingObjectValidationRuleCollector.CollectorType));
    }
  }
}