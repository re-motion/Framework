using System;
using Remotion.ServiceLocation;

namespace Remotion.UnitTests.ServiceLocation.TestDomain
{
  public interface ITestStackedDecorators
  {
  }

  [ImplementationFor (typeof (ITestStackedDecorators), RegistrationType = RegistrationType.Single, Position = 1)]
  public class TestStackedDecoratorsObject1 : ITestStackedDecorators
  {
  }

  [ImplementationFor (typeof (ITestStackedDecorators), RegistrationType = RegistrationType.Decorator, Position = 1)]
  public class TestStackedDecoratorsDecorator1 : ITestStackedDecorators
  {
    private readonly ITestStackedDecorators _decoratedObject;

    public TestStackedDecoratorsDecorator1 (ITestStackedDecorators decoratedObject)
    {
      _decoratedObject = decoratedObject;
    }

    public ITestStackedDecorators DecoratedObject
    {
      get { return _decoratedObject; }
    }
  }

  [ImplementationFor (typeof (ITestStackedDecorators), RegistrationType = RegistrationType.Decorator, Position = 2)]
  public class TestStackedDecoratorsDecorator2 : ITestStackedDecorators
  {
    private readonly ITestStackedDecorators _decoratedObject;

    public TestStackedDecoratorsDecorator2 (ITestStackedDecorators decoratedObject)
    {
      _decoratedObject = decoratedObject;
    }

    public ITestStackedDecorators DecoratedObject
    {
      get { return _decoratedObject; }
    }
  }
}