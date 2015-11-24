using System;
using System.Linq;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.Validation.UI.Controls;
using Remotion.ObjectBinding.Web.Validation.UI.Controls.Factories;
using Remotion.ServiceLocation;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.Web.Validation.UnitTests.Factories
{
  [TestFixture]
  public class UserControlBindingValidatorValidatorFactoryTest
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

      Assert.That (instance, Is.InstanceOf<CompoundValidatorFactory<UserControlBinding>>());

      var factories = ((CompoundValidatorFactory<UserControlBinding>) instance).VlidatorFactories.Select (f => f.GetType()).ToList();
      Assert.That (factories, Has.Member (typeof (UserControlBindingValidatorValidatorFactory)));
      Assert.That (factories.Count, Is.EqualTo (1));
    }

    [Test]
    public void GetInstance_Twice_ReturnsSameInstance ()
    {
      var instance1 = _serviceLocator.GetInstance<IUserControlBindingValidatorFactory>();
      var instance2 = _serviceLocator.GetInstance<IUserControlBindingValidatorFactory>();

      Assert.That (instance1, Is.InstanceOf<CompoundValidatorFactory<UserControlBinding>>());
      Assert.That (instance1, Is.SameAs (instance2));
    }

    [Test]
    [TestCase (true)]
    [TestCase (false)]
    public void CreateValidators (bool isReadOnly)
    {
      var mock = MockRepository.GenerateMock<UserControlBinding>();
      mock.Expect (m => m.ID).Return ("ID");

      var factory = new UserControlBindingValidatorValidatorFactory();
      var validators = factory.CreateValidators (mock, isReadOnly);

      if (isReadOnly)
        Assert.That (validators, Is.Empty);
      else
        Assert.That (validators.Select (v => v.GetType()), Is.EquivalentTo (new[] { typeof (UserControlBindingValidator) }));
    }
  }
}