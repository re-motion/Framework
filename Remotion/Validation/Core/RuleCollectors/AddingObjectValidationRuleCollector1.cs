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
using System.Text;
using JetBrains.Annotations;
using Remotion.Reflection;
using Remotion.Utilities;
using Remotion.Validation.Implementation;
using Remotion.Validation.Merging;
using Remotion.Validation.Rules;
using Remotion.Validation.Validators;

namespace Remotion.Validation.RuleCollectors
{
  /// <summary>
  /// Default implementation of the <see cref="IAddingObjectValidationRuleCollector"/> interface.
  /// </summary>
  public sealed class AddingObjectValidationRuleCollector<TValidatedType> : IAddingObjectValidationRuleCollector
  {
    [NotNull]
    public Type CollectorType { get; }

    [NotNull]
    public ITypeInformation ValidatedType { get; }

    public bool IsRemovable { get; private set; }

    [CanBeNull]
    public Func<TValidatedType, bool>? Condition { get; private set; }

    private readonly List<IObjectValidator> _validators = new List<IObjectValidator>();

    private readonly List<(DeferredInitializationValidationMessage ValidationMessage, IObjectValidator Validator)> _uninitializedValidationMessages =
        new List<(DeferredInitializationValidationMessage, IObjectValidator)>();

    public AddingObjectValidationRuleCollector (
       [NotNull]Type collectorType)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom("collectorType", collectorType, typeof(IValidationRuleCollector));

      CollectorType = collectorType;
      ValidatedType = TypeAdapter.Create(typeof(TValidatedType));
      IsRemovable = false;
    }

    IValidationRule IAddingObjectValidationRuleCollector.CreateValidationRule (IValidationMessageFactory validationMessageFactory)
    {
      ArgumentUtility.CheckNotNull("validationMessageFactory", validationMessageFactory);

      foreach (var tuple in _uninitializedValidationMessages)
      {
        var validator = tuple.Validator;
        var validationMessage = validationMessageFactory.CreateValidationMessageForObjectValidator(validator, ValidatedType);
        Assertion.IsNotNull(
            validationMessage,
            "The {0} did not return a result for '{1}' applied to type '{2}'.",
            nameof(IValidationMessageFactory),
            validator.GetType().Name,
            ValidatedType.GetFullNameSafe());

        tuple.ValidationMessage.Initialize(validationMessage);
      }

      _uninitializedValidationMessages.Clear();

      return new ObjectValidationRule<TValidatedType>(Condition, _validators.ToArray());
    }

    public IEnumerable<IObjectValidator> Validators
    {
      get { return _validators.ToArray(); }
    }

    public void SetCondition<TValidatedTypeForCondition> (Func<TValidatedTypeForCondition, bool> predicate)
    {
      ArgumentUtility.CheckNotNull("predicate", predicate);

      if (typeof(TValidatedTypeForCondition) != typeof(TValidatedType))
      {
        throw new ArgumentException(
            $"The type '{typeof(TValidatedTypeForCondition).GetFullNameSafe()}' of the predicate "
            + $"does not match the type '{typeof(TValidatedType).GetFullNameSafe()}' of the validation rule.", "predicate");
      }

      Condition = (Func<TValidatedType, bool>)(object)predicate;
    }

    public void SetRemovable ()
    {
      IsRemovable = true;
    }

    public void RegisterValidator (Func<ObjectValidationRuleInitializationParameters, IObjectValidator> validatorFactory)
    {
      ArgumentUtility.CheckNotNull("validatorFactory", validatorFactory);

      var deferredInitializationValidationMessage = new DeferredInitializationValidationMessage();
      var initializationParameters = new ObjectValidationRuleInitializationParameters(deferredInitializationValidationMessage);
      var validator = validatorFactory(initializationParameters);
      Assertion.IsNotNull(validator, "validatorFactory evaluated and returned null.");

      // TODO RM-5906: unique validators
      _validators.Add(validator);
      _uninitializedValidationMessages.Add((ValidationMessage: deferredInitializationValidationMessage, Validator: validator));
    }

    public void ApplyRemoveValidatorRegistrations (IObjectValidatorExtractor objectValidatorExtractor)
    {
      ArgumentUtility.CheckNotNull("objectValidatorExtractor", objectValidatorExtractor);

      var validatorsToRemove = objectValidatorExtractor.ExtractObjectValidatorsToRemove(this).ToArray();
      CheckForNonRemovableObjectValidatorViolation(validatorsToRemove);
      foreach (var validator in validatorsToRemove)
      {
        // TODO RM-5906: test
        _validators.RemoveAll(v => v == validator);
        _uninitializedValidationMessages.RemoveAll(t => t.Validator == validator);
      }
    }

    private void CheckForNonRemovableObjectValidatorViolation (IObjectValidator[] validatorsToRemove)
    {
      if (!IsRemovable && validatorsToRemove.Any())
      {
        throw new ValidationConfigurationException(
            string.Format("Attempted to remove non-removable validator(s) '{0}' on type '{1}'.",
                string.Join(", ", validatorsToRemove.Select(v => v.GetType().Name).ToArray()),
                ValidatedType.GetFullNameSafe()));
      }
    }

    public override string ToString ()
    {
      var sb = new StringBuilder(nameof(AddingObjectValidationRuleCollector));

      var hasCondition = Condition != null;
      if (hasCondition || IsRemovable)
        sb.Append(" (");

      if (hasCondition)
        sb.Append("CONDITIONAL");

      if (hasCondition && IsRemovable)
        sb.Append(", ");

      if (IsRemovable)
        sb.Append("REMOVABLE");

      if (hasCondition || IsRemovable)
        sb.Append(")");

      sb.Append(": ");
      sb.Append(ValidatedType.GetFullNameSafe());

      return sb.ToString();
    }
  }
}
