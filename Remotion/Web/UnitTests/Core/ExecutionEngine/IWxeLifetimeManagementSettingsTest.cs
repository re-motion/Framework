using NUnit.Framework;
using Remotion.ServiceLocation;
using Remotion.Web.Configuration;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine
{
  [TestFixture]
  public class IWxeLifetimeManagementSettingsTest
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
      var instance = _serviceLocator.GetInstance<IWxeLifetimeManagementSettings>();

      Assert.That(instance, Is.TypeOf<ConfigurationBasedWxeLifetimeManagementSettings>());
    }

    [Test]
    public void GetInstance_Twice_ReturnsSameInstance ()
    {
      var instance1 = _serviceLocator.GetInstance<IWxeLifetimeManagementSettings>();
      var instance2 = _serviceLocator.GetInstance<IWxeLifetimeManagementSettings>();

      Assert.That(instance1, Is.SameAs(instance2));
    }
  }
}
