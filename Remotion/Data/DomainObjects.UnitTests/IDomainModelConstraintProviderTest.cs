using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.ServiceLocation;

namespace Remotion.Data.DomainObjects.UnitTests
{
  [TestFixture]
  public class IDomainModelConstraintProviderTest
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
      var domainModelConstraintProvider = _serviceLocator.GetInstance<IDomainModelConstraintProvider>();

      Assert.That(domainModelConstraintProvider, Is.TypeOf<DomainModelConstraintProvider>());
    }

    [Test]
    public void GetInstance_Twice_ReturnsSameInstance ()
    {
      var domainModelConstraintProvider1 = _serviceLocator.GetInstance<IDomainModelConstraintProvider>();
      var domainModelConstraintProvider2 = _serviceLocator.GetInstance<IDomainModelConstraintProvider>();

      Assert.That(domainModelConstraintProvider1, Is.SameAs(domainModelConstraintProvider2));
    }
  }
}
