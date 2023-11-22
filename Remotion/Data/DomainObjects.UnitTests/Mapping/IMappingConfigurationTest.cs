using System;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.ConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.ServiceLocation;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping
{
  [TestFixture]
  public class IMappingConfigurationTest
  {
    private DefaultServiceLocator _serviceLocator;

    [SetUp]
    public void SetUp ()
    {
      _serviceLocator = DefaultServiceLocator.Create();
      _serviceLocator.RegisterSingle(() => Mock.Of<IMappingLoader>());
      _serviceLocator.RegisterSingle(() => Mock.Of<IPersistenceModelLoader>());
    }

    [Test]
    public void GetInstance_Once ()
    {
      var domainModelConstraintProvider = _serviceLocator.GetInstance<IMappingConfiguration>();

      Assert.That(domainModelConstraintProvider, Is.TypeOf<MappingConfiguration>());
    }

    [Test]
    public void GetInstance_Twice_ReturnsSameInstance ()
    {
      var propertyDefaultValueProvider1 = _serviceLocator.GetInstance<IMappingConfiguration>();
      var propertyDefaultValueProvider2 = _serviceLocator.GetInstance<IMappingConfiguration>();

      Assert.That(propertyDefaultValueProvider1, Is.SameAs(propertyDefaultValueProvider2));
    }
  }
}
