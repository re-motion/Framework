using NUnit.Framework;
using Remotion.Data.DomainObjects.ConfigurationLoader;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.ServiceLocation;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping
{
  [TestFixture]
  public class IMappingLoaderTest
  {
    private DefaultServiceLocator _serviceLocator;

    [SetUp]
    public void SetUp ()
    {
      _serviceLocator = DefaultServiceLocator.CreateWithBootstrappedServices();
    }

    [Test]
    public void GetInstance_Once ()
    {
      var factory = _serviceLocator.GetInstance<IMappingLoader>();

      Assert.That(factory, Is.Not.Null);
      Assert.That(factory, Is.TypeOf(typeof(MappingReflector)));
    }

    [Test]
    public void GetInstance_Twice_ReturnsSameInstance ()
    {
      var factory1 = _serviceLocator.GetInstance<IMappingLoader>();
      var factory2 = _serviceLocator.GetInstance<IMappingLoader>();

      Assert.That(factory1, Is.SameAs(factory2));
    }
  }
}
