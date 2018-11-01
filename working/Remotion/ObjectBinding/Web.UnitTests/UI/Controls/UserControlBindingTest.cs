using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.ObjectBinding.Web.UI.Controls;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls
{
  [TestFixture]
  public class UserControlBindingTest
  {
    [Test]
    public void CreateValidators_UsesValidatorFactory ()
    {
      var control = new UserControlBinding();
      var serviceLocatorMock = MockRepository.GenerateMock<IServiceLocator>();
      var factoryMock = MockRepository.GenerateMock<IUserControlBindingValidatorFactory>();
      serviceLocatorMock.Expect (m => m.GetInstance<IUserControlBindingValidatorFactory>()).Return (factoryMock);
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