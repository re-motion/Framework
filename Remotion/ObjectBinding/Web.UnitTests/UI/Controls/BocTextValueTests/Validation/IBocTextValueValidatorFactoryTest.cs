using System;
using System.Linq;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation.Validation;
using Remotion.ObjectBinding.Web.UI.Controls.Validation.Factories;
using Remotion.ServiceLocation;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocTextValueTests.Validation
{
  [TestFixture]
  public class IBocTextValueValidatorFactoryTest
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
      var instance = _serviceLocator.GetInstance<IBocTextValueValidatorFactory>();

      Assert.That(instance, Is.InstanceOf<CompoundValidatorFactory<IBocTextValue>>());

      var factories = ((CompoundValidatorFactory<IBocTextValue>)instance).VlidatorFactories;
      Assert.That(
          factories.Select(f => f.GetType()),
          Is.EqualTo(new[] { typeof(BocTextValueValidatorFactory), typeof(ValidationBusinessObjectBoundEditableWebControlValidatorFactory) }));
    }

    [Test]
    public void GetInstance_Twice_ReturnsSameInstance ()
    {
      var instance1 = _serviceLocator.GetInstance<IBocTextValueValidatorFactory>();
      var instance2 = _serviceLocator.GetInstance<IBocTextValueValidatorFactory>();

      Assert.That(instance1, Is.InstanceOf<CompoundValidatorFactory<IBocTextValue>>());
      Assert.That(instance1, Is.SameAs(instance2));
    }
  }
}
