using System;
using NUnit.Framework;
using Remotion.SecurityManager.Domain;
using Remotion.ServiceLocation;

namespace Remotion.SecurityManager.UnitTests.Domain
{
  [TestFixture]
  public class IDomainRevisionProviderTest
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
      var factory = _serviceLocator.GetInstance<IDomainRevisionProvider>();

      Assert.That(factory, Is.Not.Null);
      Assert.That(factory, Is.TypeOf(typeof(RevisionProvider)));
    }

    [Test]
    public void GetInstance_Twice ()
    {
      var factory1 = _serviceLocator.GetInstance<IDomainRevisionProvider>();
      var factory2 = _serviceLocator.GetInstance<IDomainRevisionProvider>();

      Assert.That(factory1, Is.SameAs(factory2));
    }
  }
}
