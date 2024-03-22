using NUnit.Framework;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine
{
  [TestFixture]
  public class WxeLifetimeManagementSettingsTest
  {
    [Test]
    public void Default ()
    {
      var settings = new WxeLifetimeManagementSettings();

      Assert.That(settings.RefreshInterval, Is.EqualTo(10));
      Assert.That(settings.FunctionTimeout, Is.EqualTo(20));
      Assert.That(settings.EnableSessionManagement, Is.EqualTo(true));
    }

    [Test]
    public void Create ()
    {
      var settings = WxeLifetimeManagementSettings.Create(functionTimeout: 42, enableSessionManagement: false, refreshInterval: 13);

      Assert.That(settings.RefreshInterval, Is.EqualTo(13));
      Assert.That(settings.FunctionTimeout, Is.EqualTo(42));
      Assert.That(settings.EnableSessionManagement, Is.EqualTo(false));
    }
  }
}
