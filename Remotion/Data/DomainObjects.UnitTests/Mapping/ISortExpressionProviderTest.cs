using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.ServiceLocation;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping
{
  [TestFixture]
  public class ISortExpressionProviderTest
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
      var instance = _serviceLocator.GetInstance<ISortExpressionDefinitionProvider>();

      Assert.That(instance, Is.TypeOf<SortExpressionDefinitionProvider>());
    }

    [Test]
    public void GetInstance_Twice_ReturnsSameInstance ()
    {
      var instance1 = _serviceLocator.GetInstance<ISortExpressionDefinitionProvider>();
      var instance2 = _serviceLocator.GetInstance<ISortExpressionDefinitionProvider>();

      Assert.That(instance1, Is.SameAs(instance2));
    }
  }
}
