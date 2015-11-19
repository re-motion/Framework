using System;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ServiceLocation;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls
{
  [TestFixture]
  public class IUserControlBindingValidatorFactoryTest
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
    }

    [Test]
    public void GetInstance_Twice_ReturnsSameInstance ()
    {
      var instance1 = _serviceLocator.GetInstance<IUserControlBindingValidatorFactory>();
      var instance2 = _serviceLocator.GetInstance<IUserControlBindingValidatorFactory>();

      Assert.That (instance1, Is.InstanceOf<CompoundValidatorFactory<UserControlBinding>>());
      Assert.That (instance1, Is.SameAs (instance2));
    }
  }
}