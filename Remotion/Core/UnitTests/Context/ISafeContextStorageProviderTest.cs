using System.Linq;
using NUnit.Framework;
using Remotion.Context;
using Remotion.ServiceLocation;

namespace Remotion.UnitTests.Context
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
      var instance = _serviceLocator.GetInstance<ISafeContextStorageProvider>();

      Assert.That (instance, Is.InstanceOf(typeof (CallContextStorageProvider)));
    }

    [Test]
    public void GetInstance_Twice_ReturnsSameInstance ()
    {
      var isntance1 = _serviceLocator.GetInstance<ISafeContextStorageProvider>();
      var instance2 = _serviceLocator.GetInstance<ISafeContextStorageProvider>();

      Assert.That (isntance1, Is.Not.SameAs (instance2));
    }
  }
}