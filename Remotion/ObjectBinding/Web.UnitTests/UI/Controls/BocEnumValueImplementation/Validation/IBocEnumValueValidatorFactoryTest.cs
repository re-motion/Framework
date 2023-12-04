using System;
using System.Linq;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocEnumValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocEnumValueImplementation.Validation;
using Remotion.ObjectBinding.Web.UI.Controls.Validation.Factories;
using Remotion.ServiceLocation;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocEnumValueImplementation.Validation
{
  [TestFixture]
  public class IBocEnumValueValidatorFactoryTest
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
      var instance = _serviceLocator.GetInstance<IBocEnumValueValidatorFactory>();

      Assert.That(instance, Is.InstanceOf<CompoundValidatorFactory<IBocEnumValue>>());

      var factories = ((CompoundValidatorFactory<IBocEnumValue>)instance).VlidatorFactories;
      Assert.That(
          factories.Select(f => f.GetType()),
          Is.EqualTo(new[] { typeof(BocEnumValueValidatorFactory), typeof(ValidationBusinessObjectBoundEditableWebControlValidatorFactory) }));
    }

    [Test]
    public void GetInstance_Twice_ReturnsSameInstance ()
    {
      var instance1 = _serviceLocator.GetInstance<IBocEnumValueValidatorFactory>();
      var instance2 = _serviceLocator.GetInstance<IBocEnumValueValidatorFactory>();

      Assert.That(instance1, Is.InstanceOf<CompoundValidatorFactory<IBocEnumValue>>());
      Assert.That(instance1, Is.SameAs(instance2));
    }
  }
}
