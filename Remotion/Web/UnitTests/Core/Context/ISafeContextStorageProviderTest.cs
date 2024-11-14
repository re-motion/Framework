using System;
using NUnit.Framework;
using Remotion.Context;
using Remotion.ServiceLocation;

namespace Remotion.Web.UnitTests.Core.Context
{
  [TestFixture]
  public class ISafeContextStorageProviderTest
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
      var instances = _serviceLocator.GetInstance<ISafeContextStorageProvider>();

      Assert.That(instances, Is.InstanceOf(typeof(AsyncLocalStorageProvider)));
    }

    [Test]
    public void GetInstance_Twice_ReturnsSameInstance ()
    {
      var instance1 = _serviceLocator.GetInstance<ISafeContextStorageProvider>();
      var instance2 = _serviceLocator.GetInstance<ISafeContextStorageProvider>();

      Assert.That(instance1, Is.SameAs(instance2));
    }
  }
}
