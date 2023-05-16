using NUnit.Framework;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.ServiceLocation;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping
{
  [TestFixture]
  public class IPropertyDefaultValueProviderTest
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
      var domainModelConstraintProvider = _serviceLocator.GetInstance<IPropertyDefaultValueProvider>();

      Assert.That(domainModelConstraintProvider, Is.TypeOf<PropertyDefaultValueProvider>());
    }

    [Test]
    public void GetInstance_Twice_ReturnsSameInstance ()
    {
      var propertyDefaultValueProvider1 = _serviceLocator.GetInstance<IPropertyDefaultValueProvider>();
      var propertyDefaultValueProvider2 = _serviceLocator.GetInstance<IPropertyDefaultValueProvider>();

      Assert.That(propertyDefaultValueProvider1, Is.SameAs(propertyDefaultValueProvider2));
    }
  }
}
