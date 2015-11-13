using System;
using System.Runtime.Serialization;
using FluentValidation;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Validation.Implementation
{
  /// <summary>
  /// Enables <see cref="IValidatorBuilder"/> to be serialized/deserialized.
  /// </summary>
  [Serializable]
  [ImplementationFor (typeof (IValidatorBuilder), RegistrationType = RegistrationType.Decorator, Position = Int32.MinValue)]
  public class ValidatorBuilderSerializationDecorator : IValidatorBuilder, IObjectReference
  {
    [NonSerialized]
    private readonly IValidatorBuilder _validatorBuilder;

    public ValidatorBuilderSerializationDecorator (IValidatorBuilder validatorBuilder)
    {
      ArgumentUtility.CheckNotNull ("validatorBuilder", validatorBuilder);

      _validatorBuilder = validatorBuilder;
    }

    public IValidatorBuilder InnerValidatorBuilder
    {
      get { return _validatorBuilder; }
    }

    public IValidator BuildValidator (Type validatedType)
    {
      ArgumentUtility.CheckNotNull ("validatedType", validatedType);

      return _validatorBuilder.BuildValidator (validatedType);
    }

    object IObjectReference.GetRealObject (StreamingContext context)
    {
      return SafeServiceLocator.Current.GetInstance<IValidatorBuilder>();
    }
  }
}