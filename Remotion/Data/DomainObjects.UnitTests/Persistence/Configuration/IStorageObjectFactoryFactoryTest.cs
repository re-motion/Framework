using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.ServiceLocation;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Configuration
{
  [TestFixture]
  public class IStorageObjectFactoryFactoryTest
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
      var factory = _serviceLocator.GetInstance<IStorageObjectFactoryFactory>();

      Assert.That(factory, Is.Not.Null);
      Assert.That(factory, Is.TypeOf(typeof(StorageObjectFactoryFactory)));
    }

    [Test]
    public void GetInstance_Twice_ReturnsSameInstance ()
    {
      var factory1 = _serviceLocator.GetInstance<IStorageObjectFactoryFactory>();
      var factory2 = _serviceLocator.GetInstance<IStorageObjectFactoryFactory>();

      Assert.That(factory1, Is.SameAs(factory2));
    }
  }
}
