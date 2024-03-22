using NUnit.Framework;
using Remotion.SecurityManager.Domain.AccessControl;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl
{
  [TestFixture]
  public class AccessControlSettingsTest
  {
    [Test]
    public void Default ()
    {
      var settings = new AccessControlSettings();

      Assert.That(settings.DisableSpecificUser, Is.False);
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void Custom (bool value)
    {
      var settings = AccessControlSettings.Create(disableSpecificUser: value);

      Assert.That(settings.DisableSpecificUser, Is.EqualTo(value));
    }
  }
}
