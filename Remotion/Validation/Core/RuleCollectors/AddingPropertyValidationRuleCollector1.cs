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
  /// Default implementation of the <see cref="IAddingPropertyValidationRuleCollector"/> interface.
  /// </summary>
  public sealed class AddingPropertyValidationRuleCollector<TValidatedType, TProperty> : IAddingPropertyValidationRuleCollector
  {
    [NotNull]
    public Type CollectorType { get; }

    [NotNull]
    public Type ValidatedType { get; }

    [NotNull]
    public IPropertyInformation Property { get; }

    [NotNull]
    public Func<object, object> PropertyFunc { get; }

    public bool IsRemovable { get; private set; }

    [CanBeNull]
    public Func<TValidatedType, bool>? Condition { get; private set; }

    private readonly List<IPropertyValidator> _validators = new List<IPropertyValidator>();

    private readonly List<(DeferredInitializationValidationMessage ValidationMessage, IPropertyValidator Validator)> _uninitializedValidationMessages =
        new List<(DeferredInitializationValidationMessage, IPropertyValidator)>();

    public AddingPropertyValidationRuleCollector (
       [NotNull] IPropertyInformation property,
       [NotNull]Func<object, object> propertyFunc,
       [NotNull]Type collectorType)
    {
      ArgumentUtility.CheckNotNull("property", property);
      ArgumentUtility.CheckNotNull("propertyFunc", propertyFunc);
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom("collectorType", collectorType, typeof(IValidationRuleCollector));

      CollectorType = collectorType;
      ValidatedType = typeof(TValidatedType);
      Property = property;
      PropertyFunc = propertyFunc;
      IsRemovable = false;
    }

    IValidationRule IAddingPropertyValidationRuleCollector.CreateValidationRule (IValidationMessageFactory validationMessageFactory)
    {
      ArgumentUtility.CheckNotNull("validationMessageFactory", validationMessageFactory);

      foreach (var tuple in _uninitializedValidationMessages)
      {
        var validator = tuple.Validator;
        var validationMessage = validationMessageFactory.CreateValidationMessageForPropertyValidator(validator, Property);
        Assertion.IsNotNull(
            validationMessage,
            string.Format(
                "The {0} did not return a result for '{1}' applied to property '{2}' on type '{3}'.",
                nameof(IValidationMessageFactory),
                validator.GetType().Name,
                Property.Name,
                Property.GetOriginalDeclaringType()!.GetFullNameSafe()));

        tuple.ValidationMessage.Initialize(validationMessage);
      }

      _uninitializedValidationMessages.Clear();

      return new PropertyValidationRule<TValidatedType, TProperty>(Property, PropertyFunc, Condition, _validators.ToArray());
    }

    public IEnumerable<IPropertyValidator> Validators
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

    public void RegisterValidator (Func<PropertyValidationRuleInitializationParameters, IPropertyValidator> validatorFactory)
    {
      ArgumentUtility.CheckNotNull("validatorFactory", validatorFactory);

      var deferredInitializationValidationMessage = new DeferredInitializationValidationMessage();
      var initializationParameters = new PropertyValidationRuleInitializationParameters(deferredInitializationValidationMessage);
      var validator = validatorFactory(initializationParameters);
      Assertion.IsNotNull(validator, "validatorFactory evaluated and returned null.");

      // TODO RM-5906: unique validators
      _validators.Add(validator);
      _uninitializedValidationMessages.Add((ValidationMessage: deferredInitializationValidationMessage, Validator: validator));
    }

    public void ApplyRemoveValidatorRegistrations (IPropertyValidatorExtractor propertyValidatorExtractor)
    {
      ArgumentUtility.CheckNotNull("propertyValidatorExtractor", propertyValidatorExtractor);

      var validatorsToRemove = propertyValidatorExtractor.ExtractPropertyValidatorsToRemove(this).ToArray();
      CheckForNonRemovablePropertyValidatorViolation(validatorsToRemove);
      foreach (var validator in validatorsToRemove)
      {
        // TODO RM-5906: test
        _validators.RemoveAll(v => v == validator);
        _uninitializedValidationMessages.RemoveAll(t => t.Validator == validator);
      }
    }

    private void CheckForNonRemovablePropertyValidatorViolation (IPropertyValidator[] validatorsToRemove)
    {
      if (!IsRemovable && validatorsToRemove.Any())
      {
        throw new ValidationConfigurationException(
            string.Format("Attempted to remove non-removable validator(s) '{0}' on property '{1}.{2}'.",
                string.Join(", ", validatorsToRemove.Select(v => v.GetType().Name).ToArray()),
                Property.DeclaringType!.GetFullNameSafe(),
                Property.Name));
      }
    }

    public override string ToString ()
    {
      var sb = new StringBuilder(nameof(AddingPropertyValidationRuleCollector));

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
      sb.Append(Property.DeclaringType != null ? Property.DeclaringType.GetFullNameSafe() + "#" : string.Empty);
      sb.Append(Property.Name);

      return sb.ToString();
    }
  }
}
