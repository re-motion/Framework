using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation.Validation;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocTextValueTests
{
  [TestFixture]
  public class BocTextValueTest
  {
    [Test]
    public void CreateValidators_UsesValidatorFactory ()
    {
      var control = new BocTextValue();
      var serviceLocatorMock = MockRepository.GenerateMock<IServiceLocator>();
      var factoryMock = MockRepository.GenerateMock<IBocTextValueValidatorFactory>();
      serviceLocatorMock.Expect (m => m.GetInstance<IBocTextValueValidatorFactory>()).Return (factoryMock);
      factoryMock.Expect (f => f.CreateValidators (control, false)).Return (new List<BaseValidator>());

      using (new ServiceLocatorScope (serviceLocatorMock))
      {
        control.CreateValidators();
      }

      factoryMock.VerifyAllExpectations();
      serviceLocatorMock.VerifyAllExpectations();
    }
  }
}