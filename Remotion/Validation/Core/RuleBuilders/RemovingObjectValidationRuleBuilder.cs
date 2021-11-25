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
using Remotion.Validation.RuleCollectors;
using Remotion.Validation.Validators;

namespace Remotion.Validation.RuleBuilders
{
  /// <summary>
  /// Default implementation of the <see cref="IRemovingObjectValidationRuleBuilder{TValidatedType}"/>.
  /// </summary>
  public class RemovingObjectValidationRuleBuilder<TValidatedType> : IRemovingObjectValidationRuleBuilder<TValidatedType>
  {
    private readonly IRemovingObjectValidationRuleCollector _removingObjectValidationRuleCollector;

    public RemovingObjectValidationRuleBuilder (IRemovingObjectValidationRuleCollector removingObjectValidationRuleCollector)
    {
      ArgumentUtility.CheckNotNull("removingObjectValidationRuleCollector", removingObjectValidationRuleCollector);

      _removingObjectValidationRuleCollector = removingObjectValidationRuleCollector;
    }

    public IRemovingObjectValidationRuleCollector RemovingObjectValidationRuleCollector
    {
      get { return _removingObjectValidationRuleCollector; }
    }

    public IRemovingObjectValidationRuleBuilder<TValidatedType> Validator<TValidator> (
        Func<TValidator, bool>? validatorPredicate)
        where TValidator : IObjectValidator
    {
      var typeCheckedValidatorPredicate = GetTypeCheckedValidatorPredicate(validatorPredicate);

      return Validator(typeof(TValidator), null, typeCheckedValidatorPredicate);
    }

    public IRemovingObjectValidationRuleBuilder<TValidatedType> Validator<TValidator, TCollectorTypeToRemoveFrom> (
        Func<TValidator, bool>? validatorPredicate)
        where TValidator : IObjectValidator
    {
      var typeCheckedValidatorPredicate = GetTypeCheckedValidatorPredicate(validatorPredicate);

      return Validator(typeof(TValidator), typeof(TCollectorTypeToRemoveFrom), typeCheckedValidatorPredicate);
    }

    public IRemovingObjectValidationRuleBuilder<TValidatedType> Validator (
        Type validatorType,
        Type? collectorTypeToRemoveFrom,
        Func<IObjectValidator, bool>? validatorPredicate)
    {
      ArgumentUtility.CheckNotNull("validatorType", validatorType);

      _removingObjectValidationRuleCollector.RegisterValidator(validatorType, collectorTypeToRemoveFrom, validatorPredicate);
      return this;
    }
    
    [CanBeNull]
    private static Func<IObjectValidator, bool>? GetTypeCheckedValidatorPredicate<TValidator> ([CanBeNull]Func<TValidator, bool>? validatorPredicate)
        where TValidator : IObjectValidator
    {
      if (validatorPredicate == null)
        return null;

      return validator => validatorPredicate(ArgumentUtility.CheckType<TValidator>(nameof(validator), validator));
    }
  }
}