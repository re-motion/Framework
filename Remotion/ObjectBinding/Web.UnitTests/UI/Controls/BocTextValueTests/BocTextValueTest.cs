using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Moq;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation.Validation;
using Remotion.ServiceLocation;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocTextValueTests
{
  [TestFixture]
  public class BocTextValueTest
  {
    [Test]
    public void CreateValidators_UsesValidatorFactory ()
    {
      var control = new BocTextValue();
      var serviceLocatorMock = new Mock<IServiceLocator>();
      var factoryMock = new Mock<IBocTextValueValidatorFactory>();
      serviceLocatorMock.Setup(m => m.GetInstance<IBocTextValueValidatorFactory>()).Returns(factoryMock.Object).Verifiable();
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
