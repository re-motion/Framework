using System;
using System.Linq;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValueImplementation.Validation;
using Remotion.ObjectBinding.Web.UI.Controls.Validation.Factories;
using Remotion.ServiceLocation;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocDateTimeValueImplementation.Validation
{
  [TestFixture]
  public class IBocDateTimeValueValidatorFactoryTest
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
      var instance = _serviceLocator.GetInstance<IBocDateTimeValueValidatorFactory>();

      Assert.That(instance, Is.InstanceOf<CompoundValidatorFactory<IBocDateTimeValue>>());

      var factories = ((CompoundValidatorFactory<IBocDateTimeValue>)instance).VlidatorFactories;
      Assert.That(
          factories.Select(f => f.GetType()),
          Is.EqualTo(new[] { typeof(BocDateTimeValueValidatorFactory), typeof(ValidationBusinessObjectBoundEditableWebControlValidatorFactory) }));
    }

    [Test]
    public void GetInstance_Twice_ReturnsSameInstance ()
    {
      var instance1 = _serviceLocator.GetInstance<IBocDateTimeValueValidatorFactory>();
      var instance2 = _serviceLocator.GetInstance<IBocDateTimeValueValidatorFactory>();

      Assert.That(instance1, Is.InstanceOf<CompoundValidatorFactory<IBocDateTimeValue>>());
      Assert.That(instance1, Is.SameAs(instance2));
    }
  }
}
