using System;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.ServiceLocation;
using Remotion.Validation.Implementation;

namespace Remotion.Validation.UnitTests.Implementation
{
  [TestFixture]
  public class ValidatorBuilderSerializationDecoratorTest
  {
    private class NonSerializableValidatorBuilder : IValidatorBuilder
    {
      public IValidator BuildValidator (Type validatedType)
      {
        throw new NotImplementedException();
      }
    }

    [Test]
    public void SerializeAndDeserializeReturnsSameInstance ()
    {
      var serviceLocator = DefaultServiceLocator.CreateWithBootstrappedServices();
      serviceLocator.Register(
          new ServiceConfigurationEntry(
              typeof(IValidatorBuilder),
              new ServiceImplementationInfo(
                  typeof(NonSerializableValidatorBuilder),
                  LifetimeKind.Singleton,
                  RegistrationType.Single),
              new ServiceImplementationInfo(
                  typeof(ValidatorBuilderSerializationDecorator),
                  LifetimeKind.InstancePerDependency,
                  RegistrationType.Decorator)));

      using (new ServiceLocatorScope(serviceLocator))
      {
        var validatorBuilder = SafeServiceLocator.Current.GetInstance<IValidatorBuilder>();
        var serializedValidatorBuild = Serializer.SerializeAndDeserialize(validatorBuilder);
        Assert.That(serializedValidatorBuild, Is.SameAs(validatorBuilder));
      }
    }
  }
}
