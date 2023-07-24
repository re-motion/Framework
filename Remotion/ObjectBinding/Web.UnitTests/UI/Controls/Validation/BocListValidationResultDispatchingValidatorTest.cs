// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// The re-motion Core Framework is free software; you can redistribute it
// and/or modify it under the terms of the GNU Lesser General Public License
// as published by the Free Software Foundation; either version 2.1 of the
// License, or (at your option) any later version.
//
// re-motion is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// using NUnit.Framework;
//
using System;
using System.Web.UI;
using Moq;
using NUnit.Framework;
using Remotion.Globalization;
using Remotion.ObjectBinding.Validation;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.Validation;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.Validation
{
  [TestFixture]
  public class BocListValidationResultDispatchingValidatorTest : BocTest
  {
    [Test]
    public void RefreshErrorMessage_WithoutValidationFailures_ReturnsMessageFromResourceManager ()
    {
      var validationFailureRepoStub = new Mock<IBocListValidationFailureRepository>();
      validationFailureRepoStub
          .Setup(_ => _.GetUnhandledValidationFailuresForBocList(true))
          .Returns(Array.Empty<BocListValidationFailureWithLocationInformation>());


      var validationFailuresForBocList = new[]
                                         {
                                           BocListValidationFailureWithLocationInformation.CreateFailure(BusinessObjectValidationFailure.Create("Errors on BocList"))
                                         };

      validationFailureRepoStub
          .SetupSequence(_ => _.GetUnhandledValidationFailuresForBocListAndContainingDataRowsAndDataCells(true))
          .Returns(Array.Empty<BocListValidationFailureWithLocationInformation>())
          .Returns(validationFailuresForBocList);

      var error = "New error just arrived";
      var resourceManagerStub = new Mock<IResourceManager>();
      resourceManagerStub.Setup(_ => _.TryGetString("Remotion.ObjectBinding.Web.UI.Controls.BocList.ValidationFailuresFoundInListErrorMessage", out error)).Returns(true);

      var bocListMock = new Mock<Control> { CallBase = true };
      bocListMock.As<IBocList>().Setup(_ => _.ValidationFailureRepository).Returns(validationFailureRepoStub.Object);
      bocListMock.As<IBocList>().Setup(_ => _.GetResourceManager()).Returns(resourceManagerStub.Object);
      bocListMock.Object.ID = "someBocList";

      var validator = new BocListValidationResultDispatchingValidator();
      validator.ErrorMessage = "This should not be output";
      validator.ControlToValidate = "someBocList";

      NamingContainer.Controls.Add(bocListMock.Object);
      NamingContainer.Controls.Add(validator);

      ((IValidatorWithDynamicErrorMessage)validator).RefreshErrorMessage();

      Assert.That(validator.ErrorMessage, Is.EqualTo("New error just arrived"));
    }

    [Test]
    public void RefreshErrorMessage_WithBocListValidationFailures_ReturnsFailuresOfBocList ()
    {
      var validationFailureRepoStub = new Mock<IBocListValidationFailureRepository>();
      var resourceManagerStub = new Mock<IResourceManager>();

      var bocListMock = new Mock<Control> { CallBase = true };
      bocListMock.As<IBocList>().Setup(_ => _.ValidationFailureRepository).Returns(validationFailureRepoStub.Object);
      bocListMock.As<IBocList>().Setup(_ => _.GetResourceManager()).Returns(resourceManagerStub.Object);
      bocListMock.Object.ID = "someBocList";

      var validationFailuresForBocList = new[]
                                         {
                                           BocListValidationFailureWithLocationInformation.CreateFailure(BusinessObjectValidationFailure.Create("Errors on BocList"))
                                         };

      validationFailureRepoStub
          .Setup(_ => _.GetUnhandledValidationFailuresForBocList(true))
          .Returns(validationFailuresForBocList);
      validationFailureRepoStub
          .Setup(_ => _.GetUnhandledValidationFailuresForBocListAndContainingDataRowsAndDataCells(true))
          .Returns(Array.Empty<BocListValidationFailureWithLocationInformation>());

      var validator = new BocListValidationResultDispatchingValidator();
      validator.ErrorMessage = "This should not be output.";
      validator.ControlToValidate = "someBocList";

      NamingContainer.Controls.Add(bocListMock.Object);
      NamingContainer.Controls.Add(validator);

      ((IValidatorWithDynamicErrorMessage)validator).RefreshErrorMessage();

      Assert.That(validator.ErrorMessage, Is.EqualTo("Errors on BocList\r\n"));
    }

    [Test]
    public void RefreshErrorMessage_WithValidationFailuresForBoundProperty_ReturnsLocalizedInfo ()
    {
      var validationFailuresForBocListRows = new[]
                                             {
                                               BocListValidationFailureWithLocationInformation.CreateFailure(BusinessObjectValidationFailure.Create("Errors on BocList rows"))
                                             };

      var validationFailureRepoStub = new Mock<IBocListValidationFailureRepository>();
      validationFailureRepoStub
          .Setup(_ => _.GetUnhandledValidationFailuresForBocList(true))
          .Returns(Array.Empty<BocListValidationFailureWithLocationInformation>());

      validationFailureRepoStub
          .SetupSequence(_ => _.GetUnhandledValidationFailuresForBocListAndContainingDataRowsAndDataCells(true))
          .Returns(validationFailuresForBocListRows)
          .Returns(Array.Empty<BocListValidationFailureWithLocationInformation>());

      var error = "Errors on rows in BocList";

      var resourceManagerStub = new Mock<IResourceManager>();
      resourceManagerStub.Setup(_ => _.TryGetString("Remotion.ObjectBinding.Web.UI.Controls.BocList.ValidationFailuresFoundInOtherListPagesErrorMessage", out error)).Returns(true);

      var businessObjectStub = new Mock<IBusinessObject>();

      var bocListMock = new Mock<Control> { CallBase = true };
      bocListMock.As<IBocList>().Setup(_ => _.ValidationFailureRepository).Returns(validationFailureRepoStub.Object);
      bocListMock.As<IBocList>().Setup(_ => _.GetResourceManager()).Returns(resourceManagerStub.Object);
      bocListMock.As<IBocList>().Setup(_ => _.Value).Returns(new[] { businessObjectStub.Object });
      bocListMock.Object.ID = "someBocList";

      var validator = new BocListValidationResultDispatchingValidator();
      validator.ErrorMessage = "This should not be output.";
      validator.ControlToValidate = "someBocList";

      NamingContainer.Controls.Add(bocListMock.Object);
      NamingContainer.Controls.Add(validator);

      ((IValidatorWithDynamicErrorMessage)validator).RefreshErrorMessage();

      Assert.That(validator.ErrorMessage, Is.EqualTo("Errors on rows in BocList\r\n"));
    }

    [Test]
    public void RefreshErrorMessage_WithoutErrors_ReturnsLocalizedInfo ()
    {
      var validationFailureRepoStub = new Mock<IBocListValidationFailureRepository>();
      validationFailureRepoStub
          .Setup(_ => _.GetUnhandledValidationFailuresForBocList(true))
          .Returns(Array.Empty<BocListValidationFailureWithLocationInformation>());

      validationFailureRepoStub
          .Setup(_ => _.GetUnhandledValidationFailuresForBocListAndContainingDataRowsAndDataCells(true))
          .Returns(Array.Empty<BocListValidationFailureWithLocationInformation>());


      var error = "Errors on rows in BocList";

      var resourceManagerStub = new Mock<IResourceManager>();
      resourceManagerStub.Setup(_ => _.TryGetString("Remotion.ObjectBinding.Web.UI.Controls.BocList.ValidationFailuresFoundInListErrorMessage", out error)).Returns(true);

      var businessObjectStub = new Mock<IBusinessObject>();

      var bocListMock = new Mock<Control> { CallBase = true };
      bocListMock.As<IBocList>().Setup(_ => _.ValidationFailureRepository).Returns(validationFailureRepoStub.Object);
      bocListMock.As<IBocList>().Setup(_ => _.GetResourceManager()).Returns(resourceManagerStub.Object);
      bocListMock.As<IBocList>().Setup(_ => _.Value).Returns(new[] { businessObjectStub.Object });
      bocListMock.Object.ID = "someBocList";

      var validator = new BocListValidationResultDispatchingValidator();
      validator.ErrorMessage = "This should not be output.";
      validator.ControlToValidate = "someBocList";

      NamingContainer.Controls.Add(bocListMock.Object);
      NamingContainer.Controls.Add(validator);

      ((IValidatorWithDynamicErrorMessage)validator).RefreshErrorMessage();

      Assert.That(validator.ErrorMessage, Is.EqualTo("Errors on rows in BocList"));
    }

    [Test]
    public void RefreshErrorMessage_WithValidationFailuresForBoundPropertyAndRowFailures_ReturnsAppendedMessageText ()
    {
      var validationFailuresForBocList = new[]
                                         {
                                           BocListValidationFailureWithLocationInformation.CreateFailure(BusinessObjectValidationFailure.Create("Errors on BocList"))
                                         };

      var validationFailuresForBocListRows = new[]
                                             {
                                               BocListValidationFailureWithLocationInformation.CreateFailure(BusinessObjectValidationFailure.Create("Errors on BocList rows"))
                                             };

      var validationFailureRepoStub = new Mock<IBocListValidationFailureRepository>();
      validationFailureRepoStub
          .Setup(_ => _.GetUnhandledValidationFailuresForBocList(true))
          .Returns(validationFailuresForBocList);

      validationFailureRepoStub
          .SetupSequence(_ => _.GetUnhandledValidationFailuresForBocListAndContainingDataRowsAndDataCells(true))
          .Returns(validationFailuresForBocListRows)
          .Returns(Array.Empty<BocListValidationFailureWithLocationInformation>());


      var error = "Errors on rows in BocList";

      var resourceManagerStub = new Mock<IResourceManager>();
      resourceManagerStub.Setup(_ => _.TryGetString("Remotion.ObjectBinding.Web.UI.Controls.BocList.ValidationFailuresFoundInOtherListPagesErrorMessage", out error)).Returns(true);

      var businessObjectStub = new Mock<IBusinessObject>();

      var bocListMock = new Mock<Control> { CallBase = true };
      bocListMock.As<IBocList>().Setup(_ => _.ValidationFailureRepository).Returns(validationFailureRepoStub.Object);
      bocListMock.As<IBocList>().Setup(_ => _.GetResourceManager()).Returns(resourceManagerStub.Object);
      bocListMock.As<IBocList>().Setup(_ => _.Value).Returns(new[] { businessObjectStub.Object });
      bocListMock.Object.ID = "someBocList";

      var validator = new BocListValidationResultDispatchingValidator();
      validator.ErrorMessage = "This should not be output.";
      validator.ControlToValidate = "someBocList";

      NamingContainer.Controls.Add(bocListMock.Object);
      NamingContainer.Controls.Add(validator);

      ((IValidatorWithDynamicErrorMessage)validator).RefreshErrorMessage();

      Assert.That(validator.ErrorMessage, Is.EqualTo("Errors on BocList\r\nErrors on rows in BocList\r\n"));
    }

    [Test]
    public void EvaluateIsValid_WithoutFailures_EmptyErrorMessageAndValid ()
    {
      var validationFailureRepoStub = new Mock<IBocListValidationFailureRepository>();
      validationFailureRepoStub
          .Setup(_ => _.GetUnhandledValidationFailuresForBocListAndContainingDataRowsAndDataCells(false))
          .Returns(Array.Empty<BocListValidationFailureWithLocationInformation>());

      var error = "BocList has validation errors.";
      var resourceManagerStub = new Mock<IResourceManager>();
      resourceManagerStub.Setup(_ => _.TryGetString("Remotion.ObjectBinding.Web.UI.Controls.BocList.RemainingValidationFailureText", out error)).Returns(true);

      var bocListMock = new Mock<Control> { CallBase = true };
      bocListMock.As<IBocList>().Setup(_ => _.ValidationFailureRepository).Returns(validationFailureRepoStub.Object);
      bocListMock.As<IBocList>().Setup(_ => _.GetResourceManager()).Returns(resourceManagerStub.Object);
      bocListMock.Object.ID = "someBocList";

      var validator = new BocListValidationResultDispatchingValidator();
      validator.ErrorMessage = "This should not be output.";
      validator.ControlToValidate = "someBocList";

      NamingContainer.Controls.Add(bocListMock.Object);
      NamingContainer.Controls.Add(validator);

      validator.Validate();

      Assert.That(validator.ErrorMessage, Is.EqualTo(""));
      Assert.That(validator.IsValid, Is.True);
    }

    [Test]
    public void EvaluateIsValid_WithFailures_ErrorMessageFromResourceManagerAndInvalid ()
    {
      var validationFailuresForBocListRows = new[]
                                             {
                                               BocListValidationFailureWithLocationInformation.CreateFailure(BusinessObjectValidationFailure.Create("Errors on BocList rows."))
                                             };

      var validationFailureRepoStub = new Mock<IBocListValidationFailureRepository>();
      validationFailureRepoStub
          .Setup(_ => _.GetUnhandledValidationFailuresForBocListAndContainingDataRowsAndDataCells(false))
          .Returns(validationFailuresForBocListRows);

      var error = "BocList has validation errors.";
      var resourceManagerStub = new Mock<IResourceManager>();
      resourceManagerStub.Setup(_ => _.TryGetString("Remotion.ObjectBinding.Web.UI.Controls.BocList.ValidationFailuresFoundInListErrorMessage", out error)).Returns(true);

      var bocListMock = new Mock<Control> { CallBase = true };
      bocListMock.As<IBocList>().Setup(_ => _.ValidationFailureRepository).Returns(validationFailureRepoStub.Object);
      bocListMock.As<IBocList>().Setup(_ => _.GetResourceManager()).Returns(resourceManagerStub.Object);
      bocListMock.Object.ID = "someBocList";

      var validator = new BocListValidationResultDispatchingValidator();
      validator.ErrorMessage = "This should not be output.";
      validator.ControlToValidate = "someBocList";

      NamingContainer.Controls.Add(bocListMock.Object);
      NamingContainer.Controls.Add(validator);

      validator.Validate();

      Assert.That(validator.ErrorMessage, Is.EqualTo("BocList has validation errors."));
      Assert.That(validator.IsValid, Is.False);
    }
  }
}
