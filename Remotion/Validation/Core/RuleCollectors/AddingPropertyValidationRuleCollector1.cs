using System;
using System.Collections.Generic;
using System.Globalization;
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
    private static readonly Func<TValidatedType, bool> s_trueCondition = _ => true;

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
    public Func<TValidatedType, bool> Condition { get; private set; }

    private readonly List<IPropertyValidator> _validators = new List<IPropertyValidator>();

    private readonly List<(DeferredInitializationValidationMessage ValidationMessage, IPropertyValidator Validator)> _uninitializedValidationMessages =
        new List<(DeferredInitializationValidationMessage, IPropertyValidator)>();

    public AddingPropertyValidationRuleCollector (
       [NotNull] IPropertyInformation property,
       [NotNull]Func<object, object> propertyFunc,
       [NotNull]Type collectorType)
    {
      ArgumentUtility.CheckNotNull ("property", property);
      ArgumentUtility.CheckNotNull ("propertyFunc", propertyFunc);
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("collectorType", collectorType, typeof (IValidationRuleCollector));

      CollectorType = collectorType;
      ValidatedType = typeof (TValidatedType);
      Property = property;
      PropertyFunc = propertyFunc;
      IsRemovable = false; 
    }

    IValidationRule IAddingPropertyValidationRuleCollector.CreateValidationRule (IValidationMessageFactory validationMessageFactory)
    {
      ArgumentUtility.CheckNotNull ("validationMessageFactory", validationMessageFactory);

      foreach (var tuple in _uninitializedValidationMessages)
      {
        var validatorType = tuple.Validator.GetType();
        var validationMessage = validationMessageFactory.CreateValidationMessageForPropertyValidator (validatorType, Property);
        if (validationMessage == null)
        {
          throw new InvalidOperationException (
              $"The {nameof (IValidationMessageFactory)} did not return a result for {validatorType.Name} applied to property '{Property.Name}' on type '{Property.GetOriginalDeclaringType().FullName}'.");
        }
        tuple.ValidationMessage.Initialize (validationMessage);
      }

      _uninitializedValidationMessages.Clear();

      return new PropertyValidationRule<TValidatedType, TProperty> (Property, PropertyFunc, Condition ?? s_trueCondition, _validators.ToArray());
    }

    public IEnumerable<IPropertyValidator> Validators
    {
      get { return _validators.ToArray(); }
    }

    public void SetCondition<TValidatedTypeForCondition> (Func<TValidatedTypeForCondition, bool> predicate)
    {
      ArgumentUtility.CheckNotNull ("predicate", predicate);

      if (typeof (TValidatedTypeForCondition) != typeof (TValidatedType))
        throw new ArgumentException (
            $"The type '{typeof (TValidatedTypeForCondition).FullName}' of the predicate does not match the type '{typeof (TValidatedType).FullName}' of the validation rule.");

      Condition = (Func<TValidatedType, bool>) (object) predicate;
    }

    public void SetRemovable ()
    {
      IsRemovable = true;
    }

    public void RegisterValidator (Func<PropertyValidationRuleInitializationParameters, IPropertyValidator> validatorFactory)
    {
      ArgumentUtility.CheckNotNull ("validatorFactory", validatorFactory);

      var deferredInitializationValidationMessage = new DeferredInitializationValidationMessage();
      var initializationParameters = new PropertyValidationRuleInitializationParameters (deferredInitializationValidationMessage);
      var validator = validatorFactory (initializationParameters);
      Assertion.IsNotNull (validator, "validatorFactory evaluated and returned null.");

      // TODO RM-5906: unique validators
      _validators.Add (validator);
      _uninitializedValidationMessages.Add ((ValidationMessage: deferredInitializationValidationMessage, Validator: validator));
    }

    public void ApplyRemoveValidatorRegistrations (IPropertyValidatorExtractor propertyValidatorExtractor)
    {
      ArgumentUtility.CheckNotNull ("propertyValidatorExtractor", propertyValidatorExtractor);

      var validatorsToRemove = propertyValidatorExtractor.ExtractPropertyValidatorsToRemove (this).ToArray();
      CheckForNonRemovablePropertyValidatorViolation (validatorsToRemove);
      foreach (var validator in validatorsToRemove)
      {
        // TODO RM-5906: test
        _validators.RemoveAll (v => v == validator);
        _uninitializedValidationMessages.RemoveAll (t => t.Validator == validator);
      }
    }

    private void CheckForNonRemovablePropertyValidatorViolation (IPropertyValidator[] validatorsToRemove)
    {
      if (!IsRemovable && validatorsToRemove.Any())
      {
        throw new ValidationConfigurationException (
            string.Format ("Attempted to remove non-removable validator(s) '{0}' on property '{1}.{2}'.",
                string.Join (", ", validatorsToRemove.Select (v => v.GetType().Name).ToArray()),
                Property.DeclaringType.FullName,
                Property.Name));
      }
    }

    public override string ToString ()
    {
      var sb = new StringBuilder (nameof (AddingPropertyValidationRuleCollector));

      var hasCondition = Condition != null;
      if (hasCondition || IsRemovable)
        sb.Append (" (");

      if (hasCondition)
        sb.Append ("CONDITIONAL");

      if (hasCondition && IsRemovable)
        sb.Append (", ");

      if (IsRemovable)
        sb.Append ("REMOVABLE");

      if (hasCondition || IsRemovable)
        sb.Append (")");

      sb.Append (": ");
      sb.Append (Property.DeclaringType != null ? Property.DeclaringType.FullName + "#" : string.Empty);
      sb.Append (Property.Name);

      return sb.ToString();
    }
  }
}