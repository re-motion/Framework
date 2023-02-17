using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Moq;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ServiceLocation;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls
{
  [TestFixture]
  public class UserControlBindingTest
  {
    [Test]
    public void CreateValidators_UsesValidatorFactory ()
    {
      var control = new UserControlBinding();
      var serviceLocatorMock = new Mock<IServiceLocator>();
      var factoryMock = new Mock<IUserControlBindingValidatorFactory>();
      serviceLocatorMock.Setup(m => m.GetInstance<IUserControlBindingValidatorFactory>()).Returns(factoryMock.Object).Verifiable();
      factoryMock.Setup(f => f.CreateValidators(control, false)).Returns(new List<BaseValidator>()).Verifiable();

      using (new ServiceLocatorScope(serviceLocatorMock.Object))
      {
        control.CreateValidators();
      }

      factoryMock.Verify();
      serviceLocatorMock.Verify();
    }
  }
}
