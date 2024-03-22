using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Validation;
using Remotion.ObjectBinding.Web.UI.Controls.Validation;
using Remotion.ObjectBinding.Web.UI.Controls.Validation.Factories;
using Remotion.ServiceLocation;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.Validation.Factories
{
  [TestFixture]
  public class ValidationBocListValidatorFactoryTest
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

      Assert.That(instance, Is.InstanceOf<CompoundValidatorFactory<IBocList>>());

      var factories = ((CompoundValidatorFactory<IBocList>)instance).VlidatorFactories.Select(f => f.GetType()).ToList();
      Assert.That(factories, Has.Member(typeof(BocListValidatorFactory)));
      Assert.That(factories, Has.Member(typeof(ValidationBocListValidatorFactory)));
      Assert.That(factories.IndexOf(typeof(BocListValidatorFactory)), Is.LessThan(factories.IndexOf(typeof(ValidationBocListValidatorFactory))));
      Assert.That(factories.Count, Is.EqualTo(2));
    }

    [Test]
    public void GetInstance_Twice_ReturnsSameInstance ()
    {
      var instance1 = _serviceLocator.GetInstance<IBocListValidatorFactory>();
      var instance2 = _serviceLocator.GetInstance<IBocListValidatorFactory>();

      Assert.That(instance1, Is.InstanceOf<CompoundValidatorFactory<IBocList>>());
      Assert.That(instance1, Is.SameAs(instance2));
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void CreateValidators (bool isReadOnly)
    {
      var mock = new Mock<IBocList>();
      mock.Setup(m => m.ID).Returns("ID").Verifiable();

      var factory = new ValidationBocListValidatorFactory();
      var validators = factory.CreateValidators(mock.Object, isReadOnly).ToArray();

      if (isReadOnly)
      {
        Assert.That(validators, Is.Empty);
      }
      else
      {
        Assert.That(validators.Select(v => v.GetType()), Is.EquivalentTo(new[] { typeof(BocListValidationResultDispatchingValidator) }));
        Assert.That(validators, Has.All.Property("EnableViewState").False);
      }
    }
  }
}
