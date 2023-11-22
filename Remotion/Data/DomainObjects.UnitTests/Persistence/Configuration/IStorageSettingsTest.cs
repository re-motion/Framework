using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.ServiceLocation;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Configuration
{
  [TestFixture]
  public class IStorageSettingsTest
  {
    private DefaultServiceLocator _serviceLocator;

    [SetUp]
    public void SetUp ()
    {
      _serviceLocator = DefaultServiceLocator.Create();
      _serviceLocator.RegisterSingle(() => StorageSettingsFactory.CreateForSqlServer("connectionString"));
    }

    [Test]
    public void GetInstance_Once ()
    {
      var factory = _serviceLocator.GetInstance<IStorageSettings>();

      Assert.That(factory, Is.Not.Null);
      Assert.That(factory, Is.TypeOf(typeof(DeferredStorageSettings)));
    }

    [Test]
    public void GetInstance_Twice_ReturnsSameInstance ()
    {
      var factory1 = _serviceLocator.GetInstance<IStorageSettings>();
      var factory2 = _serviceLocator.GetInstance<IStorageSettings>();

      Assert.That(factory1, Is.SameAs(factory2));
    }
  }
}
