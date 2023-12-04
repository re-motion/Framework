using System;
using System.Linq;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValueImplementation.Validation;
using Remotion.ObjectBinding.Web.UI.Controls.Validation.Factories;
using Remotion.ServiceLocation;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocBooleanValueImplementation.Validation
{
  [TestFixture]
  public class IBocCheckBoxValidatorFactoryTest
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
      var instance = _serviceLocator.GetInstance<IBocCheckBoxValidatorFactory>();

      Assert.That(instance, Is.InstanceOf<CompoundValidatorFactory<IBocCheckBox>>());

      var factories = ((CompoundValidatorFactory<IBocCheckBox>)instance).VlidatorFactories;
      Assert.That(
          factories.Select(f => f.GetType()),
          Is.EqualTo(new[] { typeof(ValidationBusinessObjectBoundEditableWebControlValidatorFactory) }));
    }

    [Test]
    public void GetInstance_Twice_ReturnsSameInstance ()
    {
      var instance1 = _serviceLocator.GetInstance<IBocCheckBoxValidatorFactory>();
      var instance2 = _serviceLocator.GetInstance<IBocCheckBoxValidatorFactory>();

      Assert.That(instance1, Is.InstanceOf<CompoundValidatorFactory<IBocCheckBox>>());
      Assert.That(instance1, Is.SameAs(instance2));
    }
  }
}
