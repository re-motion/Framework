using NUnit.Framework;
using Remotion.ServiceLocation;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine
{
  [TestFixture]
  public class WxeUrlSettingsTest
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
      var instance = _serviceLocator.GetInstance<WxeUrlSettings>();

      Assert.That(instance, Is.TypeOf<WxeUrlSettings>());

      Assert.That(instance.UrlMappingFile, Is.EqualTo("UrlMapping.xml"));
      Assert.That(instance.MaximumUrlLength, Is.EqualTo(1024));
      Assert.That(instance.DefaultWxeHandler, Is.Null);
    }

    [Test]
    public void GetInstance_Twice_ReturnsSameInstance ()
    {
      var instance1 = _serviceLocator.GetInstance<WxeUrlSettings>();
      var instance2 = _serviceLocator.GetInstance<WxeUrlSettings>();

      Assert.That(instance1, Is.SameAs(instance2));
    }

    [Test]
    public void Default ()
    {
      var settings = new WxeUrlSettings();

      Assert.That(settings.UrlMappingFile, Is.EqualTo("UrlMapping.xml"));
      Assert.That(settings.MaximumUrlLength, Is.EqualTo(1024));
      Assert.That(settings.DefaultWxeHandler, Is.Null);
    }

    [Test]
    public void Create ()
    {
      var settings = WxeUrlSettings.Create("someFile", 42, "someWxeHandler");

      Assert.That(settings.UrlMappingFile, Is.EqualTo("someFile"));
      Assert.That(settings.MaximumUrlLength, Is.EqualTo(42));
      Assert.That(settings.DefaultWxeHandler, Is.EqualTo("someWxeHandler"));
    }
  }
}
