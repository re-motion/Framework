using NUnit.Framework;
using Remotion.ServiceLocation;
using Remotion.Web.Services;

namespace Remotion.Web.UnitTests.Core.Services
{
  [TestFixture]
  public class IWebServiceFactoryTest
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
      var instance = _serviceLocator.GetInstance<IWebServiceFactory>();

      Assert.That(instance, Is.Not.Null);
      Assert.That(instance, Is.TypeOf(typeof(WebServiceFactory)));
    }

    [Test]
    public void GetInstance_Twice_ReturnsSameInstance ()
    {
      var instance1 = _serviceLocator.GetInstance<IWebServiceFactory>();
      var instance2 = _serviceLocator.GetInstance<IWebServiceFactory>();

      Assert.That(instance1, Is.SameAs(instance2));
    }
  }
}
