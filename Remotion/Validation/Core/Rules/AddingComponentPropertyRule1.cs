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
using Remotion.Validation.Validators;

namespace Remotion.Validation.Rules
{
  /// <summary>
  /// Default implementation of the <see cref="IAddingComponentPropertyRule"/> interface.
  /// </summary>
  public sealed class AddingComponentPropertyRule<TValidatedType, TProperty> : IAddingComponentPropertyRule
  {
    private sealed class DeferredInitializationValidationMessage : ValidationMessage
    {
      private ValidationMessage _validationMessage;

      public DeferredInitializationValidationMessage ()
      {
      }

      public void Initialize (ValidationMessage validationMessage)
      {
        if (_validationMessage != null)
          throw new InvalidOperationException ("Validation message has already been initialized.");

        _validationMessage = validationMessage;
      }

      public override string Format (CultureInfo culture, IFormatProvider formatProvider, params object[] parameters)
      {
        if (_validationMessage == null)
          throw new InvalidOperationException ("Validation message has not been initialized.");

        return _validationMessage.Format (culture, formatProvider, parameters);
      }

      public override string ToString ()
      {
        if (_validationMessage == null)
          return nameof (DeferredInitializationValidationMessage);

        return _validationMessage.ToString();
      }
    }

    private static readonly Func<TValidatedType, bool> s_trueCondition = _ => true;

    [NotNull]
    public Type CollectorType { get; }

    [NotNull]
    public Type ValidatedType { get; }

    [NotNull]
    public IPropertyInformation Property { get; }

    [NotNull]
    public Func<object, object> PropertyFunc { get; }

    public bool IsHardConstraint { get; private set; }

    [CanBeNull]
    public Func<TValidatedType, bool> Condition { get; private set; }

    private readonly List<IPropertyValidator> _validators = new List<IPropertyValidator>();

    private readonly List<(DeferredInitializationValidationMessage ValidationMessage, IPropertyValidator Validator)> _uninitializedValidationMessages =
        new List<(DeferredInitializationValidationMessage, IPropertyValidator)>();

    public AddingComponentPropertyRule (
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
      IsHardConstraint = false; 
    }

    IValidationRule IAddingComponentPropertyRule.CreateValidationRule (IValidationMessageFactory validationMessageFactory)
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
            $"The type '{typeof (TValidatedTypeForCondition).FullName}' of the predicate does not match the type '{typeof (TValidatedType).FullName}' of the property rule.");

      Condition = (Func<TValidatedType, bool>) (object) predicate;
    }

    public void SetHardConstraint ()
    {
      IsHardConstraint = true;
    }

    public void RegisterValidator (Func<PropertyRuleInitializationParameters, IPropertyValidator> validatorFactory)
    {
      ArgumentUtility.CheckNotNull ("validatorFactory", validatorFactory);

      var deferredInitializationValidationMessage = new DeferredInitializationValidationMessage();
      var initializationParameters = new PropertyRuleInitializationParameters (deferredInitializationValidationMessage);
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
      CheckForHardConstraintViolation (validatorsToRemove);
      foreach (var validator in validatorsToRemove)
      {
        // TODO RM-5906: test
        _validators.RemoveAll (v => v == validator);
        _uninitializedValidationMessages.RemoveAll (t => t.Validator == validator);
      }
    }

    private void CheckForHardConstraintViolation (IPropertyValidator[] validatorsToRemove)
    {
      if (IsHardConstraint && validatorsToRemove.Any())
      {
        throw new ValidationConfigurationException (
            string.Format ("Hard constraint validator(s) '{0}' on property '{1}.{2}' cannot be removed.",
                string.Join (", ", validatorsToRemove.Select (v => v.GetType().Name).ToArray()),
                Property.DeclaringType.FullName,
                Property.Name));
      }
    }

    public override string ToString ()
    {
      var sb = new StringBuilder (nameof (AddingComponentPropertyRule));

      var hasCondition = Condition != null;
      if (hasCondition || IsHardConstraint)
        sb.Append (" (");

      if (hasCondition)
        sb.Append ("CONDITIONAL");

      if (hasCondition && IsHardConstraint)
        sb.Append (", ");

      if (IsHardConstraint)
        sb.Append ("HARD CONSTRAINT");

      if (hasCondition || IsHardConstraint)
        sb.Append (")");

      sb.Append (": ");
      sb.Append (Property.DeclaringType != null ? Property.DeclaringType.FullName + "#" : string.Empty);
      sb.Append (Property.Name);

      return sb.ToString();
    }
  }
}