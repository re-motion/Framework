using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Moq;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ServiceLocation;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BusinessObjectReferenceDataSourceControlTests
{
  [TestFixture]
  public class BusinessObjectReferenceDataSourceControlTest
  {
    [Test]
    public void CreateValidators_UsesValidatorFactory ()
    {
      var control = new BusinessObjectReferenceDataSourceControl();
      var serviceLocatorMock = new Mock<IServiceLocator>();
      var factoryMock = new Mock<IBusinessObjectReferenceDataSourceControlValidatorFactory>();
      serviceLocatorMock.Setup(m => m.GetInstance<IBusinessObjectReferenceDataSourceControlValidatorFactory>()).Returns(factoryMock.Object).Verifiable();
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
