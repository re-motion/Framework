using System;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Validation;
using Remotion.ServiceLocation;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocListImplementation.Validation
{
  [TestFixture]
  public class IBocListValidatorFactoryTest
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
      var instance = _serviceLocator.GetInstance<IBocListValidatorFactory>();

      Assert.That (instance, Is.InstanceOf<CompoundValidatorFactory<IBocList>>());
    }

    [Test]
    public void GetInstance_Twice_ReturnsSameInstance ()
    {
      var instance1 = _serviceLocator.GetInstance<IBocListValidatorFactory>();
      var instance2 = _serviceLocator.GetInstance<IBocListValidatorFactory>();

      Assert.That (instance1, Is.InstanceOf<CompoundValidatorFactory<IBocList>>());
      Assert.That (instance1, Is.SameAs (instance2));
    }
  }
}