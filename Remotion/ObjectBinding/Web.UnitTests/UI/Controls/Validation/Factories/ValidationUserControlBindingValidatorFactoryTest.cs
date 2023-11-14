using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.Validation;
using Remotion.ObjectBinding.Web.UI.Controls.Validation.Factories;
using Remotion.ServiceLocation;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.Validation.Factories
{
  [TestFixture]
  public class ValidationUserControlBindingValidatorFactoryTest
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
      var instance = _serviceLocator.GetInstance<IUserControlBindingValidatorFactory>();

      Assert.That(instance, Is.InstanceOf<CompoundValidatorFactory<UserControlBinding>>());

      var factories = ((CompoundValidatorFactory<UserControlBinding>)instance).VlidatorFactories.Select(f => f.GetType()).ToList();
      Assert.That(factories, Has.Member(typeof(ValidationUserControlBindingValidatorFactory)));
      Assert.That(factories, Has.Member(typeof(Web.UI.Controls.UserControlBindingValidatorFactory)));
      Assert.That(
          factories.IndexOf(typeof(Web.UI.Controls.UserControlBindingValidatorFactory)),
          Is.LessThan(factories.IndexOf(typeof(ValidationUserControlBindingValidatorFactory))));
      Assert.That(factories.Count, Is.EqualTo(2));
    }

    [Test]
    public void GetInstance_Twice_ReturnsSameInstance ()
    {
      var instance1 = _serviceLocator.GetInstance<IUserControlBindingValidatorFactory>();
      var instance2 = _serviceLocator.GetInstance<IUserControlBindingValidatorFactory>();

      Assert.That(instance1, Is.InstanceOf<CompoundValidatorFactory<UserControlBinding>>());
      Assert.That(instance1, Is.SameAs(instance2));
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void CreateValidators (bool isReadOnly)
    {
      var mock = new Mock<UserControlBinding>();
      mock.Setup(m => m.ID).Returns("ID").Verifiable();

      var factory = new ValidationUserControlBindingValidatorFactory();
      var validators = factory.CreateValidators(mock.Object, isReadOnly).ToArray();

      Assert.That(
          validators.Select(v => v.GetType()),
          Is.EquivalentTo(new[] { typeof(UserControlBindingValidationResultDispatchingValidator) }));
      Assert.That(validators, Has.All.Property("EnableViewState").False);
    }
  }
}
