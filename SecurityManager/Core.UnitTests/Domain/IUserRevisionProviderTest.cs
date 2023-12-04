using System;
using NUnit.Framework;
using Remotion.SecurityManager.Domain;
using Remotion.ServiceLocation;

namespace Remotion.SecurityManager.UnitTests.Domain
{
  [TestFixture]
  public class IUserRevisionProviderTest
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
      var factory = _serviceLocator.GetInstance<IUserRevisionProvider>();

      Assert.That(factory, Is.Not.Null);
      Assert.That(factory, Is.TypeOf(typeof(UserRevisionProvider)));
    }

    [Test]
    public void GetInstance_Twice ()
    {
      var factory1 = _serviceLocator.GetInstance<IUserRevisionProvider>();
      var factory2 = _serviceLocator.GetInstance<IUserRevisionProvider>();

      Assert.That(factory1, Is.SameAs(factory2));
    }
  }
}
