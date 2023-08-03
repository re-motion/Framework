using System;
using System.Collections.Generic;
using System.Linq;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BusinessObjectPropertyConstraints;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Validation;
using Remotion.Validation.Rules;

namespace Remotion.ObjectBinding.Validation
{
  [ImplementationFor(typeof(IBusinessObjectPropertyConstraintProvider), Position = Position, RegistrationType = RegistrationType.Multiple, Lifetime = LifetimeKind.Singleton)]
  public class ValidationBusinessObjectPropertyConstraintProvider : IBusinessObjectPropertyConstraintProvider
  {
    public const int Position = 0;

    public IValidatorProvider ValidatorProvider { get; }
    public IPropertyValidatorToBusinessObjectPropertyConstraintConverter PropertyValidatorConverter { get; }

    public ValidationBusinessObjectPropertyConstraintProvider (
        IValidatorProvider validatorProvider,
        IPropertyValidatorToBusinessObjectPropertyConstraintConverter propertyValidatorConverter)
    {
      ArgumentUtility.CheckNotNull("validatorProvider", validatorProvider);
      ArgumentUtility.CheckNotNull("propertyValidatorConverter", propertyValidatorConverter);

      ValidatorProvider = validatorProvider;
      PropertyValidatorConverter = propertyValidatorConverter;
    }

    public IEnumerable<IBusinessObjectPropertyConstraint> GetPropertyConstraints (
        IBusinessObjectClass businessObjectClass,
        IBusinessObjectProperty businessObjectProperty,
        IBusinessObject? obj)
    {
      ArgumentUtility.CheckNotNull("@class", businessObjectClass);
      ArgumentUtility.CheckNotNull("businessObjectProperty", businessObjectProperty);

      //TODO RM-5906: find a better way than hard-casting the IBusinessObjectClass to getting the type
      var businessObjectType = obj?.GetType() ?? (businessObjectClass as BindableObjectClass)?.ConcreteType;
      if (businessObjectType == null)
        return Enumerable.Empty<IBusinessObjectPropertyConstraint>();

      var actualBusinessObjectClass = obj?.BusinessObjectClass ?? businessObjectClass;

      var validator = ValidatorProvider.GetValidator(businessObjectType);
      var descriptor = validator.CreateDescriptor();
      var validationContext = new ValidationContext(obj);

      // TODO RM-5906: Unify property matching with implementation from BusinessObjectValidationResult

      return descriptor.ValidationRules
          .OfType<IPropertyValidationRule>()
          .Where(HasPropertyMatch)
          .Where(IsActive)
          .SelectMany(r => PropertyValidatorConverter.Convert(r.Validators));

      bool HasPropertyMatch (IPropertyValidationRule rule)
      {
        return businessObjectProperty.Identifier.Equals(actualBusinessObjectClass.GetPropertyDefinition(rule.Property.Name)?.Identifier);
      }

      bool IsActive (IPropertyValidationRule rule)
      {
        return rule.IsActive(validationContext);
      }
    }
  }
}
