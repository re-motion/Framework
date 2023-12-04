using System;
using NUnit.Framework;
using Remotion.ServiceLocation;
using Remotion.Web.UI.Controls.DatePickerButtonImplementation.Rendering;

namespace Remotion.Web.UnitTests.Core.UI.Controls.DatePickerButtonImplementation.Rendering
{
  [TestFixture]
  public class IDatePickerPageRendererTest
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
      var factory = _serviceLocator.GetInstance<IDatePickerPageRenderer>();

      Assert.That(factory, Is.Not.Null);
      Assert.That(factory, Is.TypeOf(typeof(DatePickerPageRenderer)));
    }

    [Test]
    public void GetInstance_Twice ()
    {
      var factory1 = _serviceLocator.GetInstance<IDatePickerPageRenderer>();
      var factory2 = _serviceLocator.GetInstance<IDatePickerPageRenderer>();

      Assert.That(factory1, Is.SameAs(factory2));
    }
  }
}
