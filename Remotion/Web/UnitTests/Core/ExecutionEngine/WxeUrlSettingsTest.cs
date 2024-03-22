using System.Threading;
using NUnit.Framework;
using Remotion.Development.NUnit.UnitTesting;
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

      Assert.That(settings.MaximumUrlLength, Is.EqualTo(1024));
      Assert.That(settings.DefaultWxeHandler, Is.Null);
    }

    [Test]
    public void Create ()
    {
      var settings = WxeUrlSettings.Create(42, " ~/someWxeHandler ");

      Assert.That(settings.MaximumUrlLength, Is.EqualTo(42));
      Assert.That(settings.DefaultWxeHandler, Is.EqualTo("~/someWxeHandler"));
    }

    [Test]
    public void Create_NullValues ()
    {
      var settings = WxeUrlSettings.Create(0, null);

      Assert.That(settings.DefaultWxeHandler, Is.Null);
    }

    [Test]
    public void Create_WithDefaultWxeHandlerAsAbsoluteFilePath_Throws ()
    {
      Assert.That(
          () => WxeUrlSettings.Create(0, "C:\\myhandler"),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo("No absolute paths are allowed. Resource: 'C:\\myhandler'", "defaultWxeHandler"));
    }

    [Test]
    public void Create_WithDefaultWxeHandlerAsAbsoluteUrlPath_Throws ()
    {
      Assert.That(
          () => WxeUrlSettings.Create(0, "/myhandler"),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo("No absolute paths are allowed. Resource: '/myhandler'", "defaultWxeHandler"));
    }

    [Test]
    public void Create_WithRelativePath_IsMadeApplicationRelative ()
    {
      var settings = WxeUrlSettings.Create(0, " someWxeHandler ");

      Assert.That(settings.DefaultWxeHandler, Is.EqualTo("~/someWxeHandler"));
    }
  }
}
