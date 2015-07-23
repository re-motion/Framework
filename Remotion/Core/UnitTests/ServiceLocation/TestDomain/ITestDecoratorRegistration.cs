using System;
using Remotion.ServiceLocation;

namespace Remotion.UnitTests.ServiceLocation.TestDomain
{
  public interface ITestDecoratorRegistration
  {
  }

  [ImplementationFor (typeof (ITestDecoratorRegistration), RegistrationType = RegistrationType.Single, Position = 1)]
  public class TestDecoratorRegistrationDecoratedObject1 : ITestDecoratorRegistration
  {
  }

  [ImplementationFor (typeof (ITestDecoratorRegistration), RegistrationType = RegistrationType.Single, Position = 2)]
  public class TestDecoratorRegistrationDecoratedObject2 : ITestDecoratorRegistration
  {
  }

  [ImplementationFor (typeof (ITestDecoratorRegistration), RegistrationType = RegistrationType.Decorator)]
  public class TestDecoratorRegistrationDecorator : ITestDecoratorRegistration
  {
    private readonly ITestDecoratorRegistration _decoratedObject;

    public TestDecoratorRegistrationDecorator (ITestDecoratorRegistration decoratedObject)
    {
      _decoratedObject = decoratedObject;
    }

    public ITestDecoratorRegistration DecoratedObject
    {
      get { return _decoratedObject; }
    }
  }
}