using System;
using NUnit.Framework;
using Remotion.Reflection;
using Remotion.ServiceLocation;

namespace Remotion.UnitTests.ServiceLocation
{
  public class IMemberInformationNameResolverTest
  {
    private DefaultServiceLocator _serviceLocator;

    [SetUp]
    public void SetUp ()
    {
      _serviceLocator = DefaultServiceLocator.Create();
    }

    [Test]
    public void GetInstance_Once ()
    {
      var factory = _serviceLocator.GetInstance<IMemberInformationNameResolver>();

      Assert.That (factory, Is.Not.Null);
      Assert.That (factory, Is.TypeOf (typeof (ReflectionBasedMemberInformationNameResolver)));
    }

    [Test]
    public void GetInstance_Twice_ReturnsSameInstance ()
    {
      var factory1 = _serviceLocator.GetInstance<IMemberInformationNameResolver>();
      var factory2 = _serviceLocator.GetInstance<IMemberInformationNameResolver>();

      Assert.That (factory1, Is.SameAs (factory2));
    }
  }
}