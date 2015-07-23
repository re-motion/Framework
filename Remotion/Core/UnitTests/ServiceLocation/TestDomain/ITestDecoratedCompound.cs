using System;
using System.Collections.Generic;
using Remotion.ServiceLocation;

namespace Remotion.UnitTests.ServiceLocation.TestDomain
{
  public interface IITestDecoratedCompound
  {
  }

  [ImplementationFor (typeof (IITestDecoratedCompound), RegistrationType = RegistrationType.Multiple, Position = 1)]
  public class ITestDecoratedCompoundObject1 : IITestDecoratedCompound
  {
  }

  [ImplementationFor (typeof (IITestDecoratedCompound), RegistrationType = RegistrationType.Multiple, Position = 2)]
  public class ITestDecoratedCompoundObject2 : IITestDecoratedCompound
  {
  }

  [ImplementationFor (typeof (IITestDecoratedCompound), RegistrationType = RegistrationType.Decorator)]
  public class ITestDecoratedCompoundDecorator : IITestDecoratedCompound
  {
    private readonly IITestDecoratedCompound _decoratedObject;

    public ITestDecoratedCompoundDecorator (IITestDecoratedCompound decoratedObject)
    {
      _decoratedObject = decoratedObject;
    }

    public IITestDecoratedCompound DecoratedObject
    {
      get { return _decoratedObject; }
    }
  }

  [ImplementationFor (typeof (IITestDecoratedCompound), RegistrationType = RegistrationType.Compound)]
  public class ITestDecoratedCompoundCompound : IITestDecoratedCompound
  {
    private readonly IEnumerable<IITestDecoratedCompound> _innerObjects;

    public ITestDecoratedCompoundCompound (IEnumerable<IITestDecoratedCompound> innerObjects)
    {
      _innerObjects = innerObjects;
    }

    public IEnumerable<IITestDecoratedCompound> InnerObjects
    {
      get { return _innerObjects; }
    }
  }
}