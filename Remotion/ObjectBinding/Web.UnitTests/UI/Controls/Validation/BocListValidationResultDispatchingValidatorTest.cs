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
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Moq;
using NUnit.Framework;
using Remotion.Globalization;
using Remotion.ObjectBinding.Validation;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.EditableRowSupport;
using Remotion.ObjectBinding.Web.UI.Controls.Validation;
using Remotion.ObjectBinding.Web.UnitTests.Domain;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.Validation
{
  [TestFixture]
  public class BocListValidationResultDispatchingValidatorTest : BocTest
  {

    public (BocListMock boclist, BocSimpleColumnDefinition simpleColumnDefinition) InitiateBocListForEditModeWithStringClass (TypeWithString[] typeWithStrings)
    {
      var typeWithStringClass = BindableObjectProviderTestHelper.GetBindableObjectClass(typeof(TypeWithString));

      var typeWithStringFirstValuePath = BusinessObjectPropertyPath.CreateStatic(typeWithStringClass, "FirstValue");

      var typeWithStringFirstValueSimpleColumn = new BocSimpleColumnDefinition();
      typeWithStringFirstValueSimpleColumn.SetPropertyPath(typeWithStringFirstValuePath);


      var hostTypeForTypeWithString = HostTypeForTypeWithString.Create();
      hostTypeForTypeWithString.Strings = typeWithStrings;
      var businessObject = (IBusinessObject)hostTypeForTypeWithString;
      var businessObjectProperty = businessObject.BusinessObjectClass.GetPropertyDefinition("Strings");

      var bocList = new BocListMock();
      bocList.DataSource = new StubDataSource(typeWithStringClass);
      bocList.DataSource.BusinessObject = businessObject;
      bocList.DataSource.Mode = DataSourceMode.Edit;
      bocList.Property = (IBusinessObjectReferenceProperty)businessObjectProperty;
      bocList.FixedColumns.Add(typeWithStringFirstValueSimpleColumn);
      bocList.FixedColumns.Add(new BocValidationErrorIndicatorColumnDefinition());
      bocList.EnableAutoFocusOnSwitchToEditMode = false;

      Assert.That(bocList.IsReadOnly, Is.False);

      bocList.LoadValue(false);
      bocList.SwitchRowIntoEditMode(0);

      Assert.That(bocList.IsRowEditModeActive, Is.True);

      var editableRow = ((IBocList)bocList).EditModeController.GetEditableRow(0);
      ((EditableRow)editableRow).EnsureValidatorsRestored();

      Assert.That(editableRow.HasValidators(), Is.True);

      Assert.That(editableRow.HasValidators(0), Is.True);
      ControlCollection editableRowValidators = editableRow.GetValidators(0);
      Assert.That(editableRowValidators.Cast<BaseValidator>().Select(v => v.GetType()),
          Is.EqualTo(
              new[]
              {
                typeof(ControlCharactersCharactersValidator),
                typeof(BusinessObjectBoundEditableWebControlValidationResultDispatchingValidator)
              }));

      ((BaseValidator)editableRowValidators[0]).Validate();

      return (bocList, typeWithStringFirstValueSimpleColumn);
    }

    [Test]
    public void DispatchValidationFailures_WithInvisibleControl_UsesReadOnlyValidationResultForDispatching ()
    {
      IBusinessObject businessObject = TypeWithReference.Create();
      var businessObjectProperty = businessObject.BusinessObjectClass.GetPropertyDefinition("ReferenceList");

      var bocList = new BocListMock();
      bocList.ID = "someBocList";
      bocList.DataSource = new StubDataSource(businessObject.BusinessObjectClass);
      bocList.DataSource.BusinessObject = businessObject;
      bocList.Property = (IBusinessObjectReferenceProperty)businessObjectProperty;
      bocList.Visible = false;
      bocList.FixedColumns.Add(new BocValidationErrorIndicatorColumnDefinition());

      var validationResultMock = new Mock<IBusinessObjectValidationResult>(MockBehavior.Strict);
      validationResultMock.Setup(e => e.GetValidationFailures(businessObject, businessObjectProperty, false))
          .Returns(Array.Empty<BusinessObjectValidationFailure>())
          .Verifiable();

      var validator = new BocListValidationResultDispatchingValidator();
      validator.ControlToValidate = "someBocList";

      NamingContainer.Controls.Add(bocList);
      NamingContainer.Controls.Add(validator);

      validator.DispatchValidationFailures(validationResultMock.Object);

      validationResultMock.Verify();
    }

    [Test]
    public void DispatchValidationFailures_WithRowEditModeControlHavingInvalidValidator_HasFailureFromValidatorInRepositoryWithMetadata ()
    {
      var values = new[]
                   {
                     TypeWithString.Create("\u0001", "A")
                   };

      var (bocList, simpleColumnDefinition) = InitiateBocListForEditModeWithStringClass(values);

      var validator = new BocListValidationResultDispatchingValidator();
      validator.ControlToValidate = "someBocList";
      bocList.ID = "someBocList";

      NamingContainer.Controls.Add(bocList);
      NamingContainer.Controls.Add(validator);

      var validationResultMock = new Mock<IBusinessObjectValidationResult>();
      validationResultMock
          .Setup(e => e.GetValidationFailures(bocList.DataSource.BusinessObject, bocList.Property, true))
          .Returns(Array.Empty<BusinessObjectValidationFailure>())
          .Verifiable();

      validationResultMock
          .Setup(_ => _.GetValidationFailures((IBusinessObject)values[0], simpleColumnDefinition.GetPropertyPath().Properties.Single(), true))
          .Returns(Array.Empty<BusinessObjectValidationFailure>())
          .Verifiable();
      validationResultMock
          .Setup(_ => _.GetUnhandledValidationFailures(It.IsAny<IBusinessObject>(), It.IsAny<bool>(), It.IsAny<bool>()))
          .Returns(Array.Empty<BusinessObjectValidationFailure>());

      validator.DispatchValidationFailures(validationResultMock.Object);

      var bocListValidationFailureRepository = bocList.ValidationFailureRepository;
      var editedRow = ((IBocList)bocList).EditModeController.GetEditedRow();

      var failuresInRepository = bocListValidationFailureRepository.GetUnhandledValidationFailuresForBocListAndContainingDataRowsAndDataCells(true);
      Assert.That(failuresInRepository.Count, Is.EqualTo(1));
      Assert.That(failuresInRepository.Single().ColumnDefinition, Is.EqualTo(simpleColumnDefinition));
      Assert.That(failuresInRepository.Single().RowObject, Is.EqualTo(editedRow.BusinessObject));
      Assert.That(failuresInRepository.Single().Failure.ErrorMessage, Is.EqualTo("The value contains unsupported characters: \"\u0001\" (position: 1)."));

      validationResultMock.Verify();
    }

    [Test]
    public void BuildErrorMessage_OnSecondCall_Throws ()
    {
      var validationFailureRepository = new BocListValidationFailureRepository();
      validationFailureRepository.AddValidationFailuresForBocList(new [] { BusinessObjectValidationFailure.Create("A list failure") });

      var bocListMock = new Mock<Control> { CallBase = true };
      bocListMock.As<IBocList>().Setup(_ => _.ValidationFailureRepository).Returns(validationFailureRepository);
      bocListMock.Object.ID = "someBocList";

      var validator = new BocListValidationResultDispatchingValidator();
      validator.ControlToValidate = "someBocList";

      NamingContainer.Controls.Add(bocListMock.Object);
      NamingContainer.Controls.Add(validator);

      validator.BuildErrorMessage();

      Assert.That(
          () => validator.BuildErrorMessage(),
          Throws.TypeOf<InvalidOperationException>()
              .With.Message.EqualTo("The validation error message has already been built."));
    }

    [Test]
    public void BuildErrorMessage_WithoutValidationFailures_ReturnsMessageFromResourceManager ()
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
          .Setup(_ => _.GetUnhandledValidationFailuresForDataRowsAndContainingDataCells(true))
          .Returns(Array.Empty<BocListValidationFailureWithLocationInformation>());

      validationFailureRepoStub
          .Setup(_ => _.GetUnhandledValidationFailuresForBocListAndContainingDataRowsAndDataCells(true))
          .Returns(validationFailuresForBocList);

      var error = "New error just arrived";
      var resourceManagerStub = new Mock<IResourceManager>();
      resourceManagerStub.Setup(_ => _.TryGetString("Remotion.ObjectBinding.Web.UI.Controls.BocList.UnhandledValidationFailuresFoundErrorMessage", out error)).Returns(true);

      var bocListMock = new Mock<Control> { CallBase = true };
      bocListMock.As<IBocList>().Setup(_ => _.ValidationFailureRepository).Returns(validationFailureRepoStub.Object);
      bocListMock.As<IBocList>().Setup(_ => _.GetResourceManager()).Returns(resourceManagerStub.Object);
      bocListMock.Object.ID = "someBocList";

      var validator = new BocListValidationResultDispatchingValidator();
      validator.ErrorMessage = "This should not be output";
      validator.ControlToValidate = "someBocList";

      NamingContainer.Controls.Add(bocListMock.Object);
      NamingContainer.Controls.Add(validator);

      validator.BuildErrorMessage();

      Assert.That(validator.ErrorMessage, Is.EqualTo("New error just arrived"));
    }

    [Test]
    public void BuildErrorMessage_WithBocListValidationFailures_ReturnsFailuresOfBocList ()
    {
      var validationFailureRepository = new BocListValidationFailureRepository();
      validationFailureRepository.AddValidationFailuresForBocList(new [] { BusinessObjectValidationFailure.Create("Errors on BocList") });

      var resourceManagerStub = new Mock<IResourceManager>();

      var bocListMock = new Mock<Control> { CallBase = true };
      bocListMock.As<IBocList>().Setup(_ => _.ValidationFailureRepository).Returns(validationFailureRepository);
      bocListMock.As<IBocList>().Setup(_ => _.GetResourceManager()).Returns(resourceManagerStub.Object);
      bocListMock.Object.ID = "someBocList";

      var validator = new BocListValidationResultDispatchingValidator();
      validator.ErrorMessage = "This should not be output.";
      validator.ControlToValidate = "someBocList";

      NamingContainer.Controls.Add(bocListMock.Object);
      NamingContainer.Controls.Add(validator);

      validator.BuildErrorMessage();

      Assert.That(validator.ErrorMessage, Is.EqualTo("Errors on BocList\r\n"));
    }

    [Test]
    public void BuildErrorMessage_WithValidationFailuresForBoundProperty_ReturnsLocalizedInfo ()
    {
      var validationFailureRepository = new BocListValidationFailureRepository();
      validationFailureRepository.AddValidationFailuresForDataRow(Mock.Of<IBusinessObject>(), new [] { BusinessObjectValidationFailure.Create("Errors on BocList rows") });

      var error = "Errors on rows in BocList";

      var resourceManagerStub = new Mock<IResourceManager>();
      resourceManagerStub.Setup(_ => _.TryGetString("Remotion.ObjectBinding.Web.UI.Controls.BocList.ValidationFailuresFoundInOtherListPagesErrorMessage", out error)).Returns(true);

      var businessObjectStub = new Mock<IBusinessObject>();

      var bocListMock = new Mock<Control> { CallBase = true };
      bocListMock.As<IBocList>().Setup(_ => _.ValidationFailureRepository).Returns(validationFailureRepository);
      bocListMock.As<IBocList>().Setup(_ => _.GetResourceManager()).Returns(resourceManagerStub.Object);
      bocListMock.As<IBocList>().Setup(_ => _.Value).Returns(new[] { businessObjectStub.Object });
      bocListMock.Object.ID = "someBocList";

      var validator = new BocListValidationResultDispatchingValidator();
      validator.ErrorMessage = "This should not be output.";
      validator.ControlToValidate = "someBocList";

      NamingContainer.Controls.Add(bocListMock.Object);
      NamingContainer.Controls.Add(validator);

      validator.BuildErrorMessage();

      Assert.That(validator.ErrorMessage, Is.EqualTo("Errors on rows in BocList\r\n"));
    }

    [Test]
    public void BuildErrorMessage_WithoutErrors_ReturnsLocalizedInfo ()
    {
      var validationFailureRepository = new BocListValidationFailureRepository();

      var error = "Errors on rows in BocList";

      var resourceManagerStub = new Mock<IResourceManager>();
      resourceManagerStub.Setup(_ => _.TryGetString("Remotion.ObjectBinding.Web.UI.Controls.BocList.UnhandledValidationFailuresFoundErrorMessage", out error)).Returns(true);

      var businessObjectStub = new Mock<IBusinessObject>();

      var bocListMock = new Mock<Control> { CallBase = true };
      bocListMock.As<IBocList>().Setup(_ => _.ValidationFailureRepository).Returns(validationFailureRepository);
      bocListMock.As<IBocList>().Setup(_ => _.GetResourceManager()).Returns(resourceManagerStub.Object);
      bocListMock.As<IBocList>().Setup(_ => _.Value).Returns(new[] { businessObjectStub.Object });
      bocListMock.Object.ID = "someBocList";

      var validator = new BocListValidationResultDispatchingValidator();
      validator.ErrorMessage = "This should not be output.";
      validator.ControlToValidate = "someBocList";

      NamingContainer.Controls.Add(bocListMock.Object);
      NamingContainer.Controls.Add(validator);

      validator.BuildErrorMessage();

      Assert.That(validator.ErrorMessage, Is.EqualTo("Errors on rows in BocList"));
    }

    [Test]
    public void BuildErrorMessage_WithValidationFailuresForBoundPropertyAndRowFailures_ReturnsAppendedMessageText ()
    {
      var validationFailureRepository = new BocListValidationFailureRepository();
      validationFailureRepository.AddValidationFailuresForBocList(new [] { BusinessObjectValidationFailure.Create("A list error") });
      validationFailureRepository.AddValidationFailuresForDataRow(Mock.Of<IBusinessObject>(), new [] { BusinessObjectValidationFailure.Create("A row error") });

      var error = "Errors on rows in BocList";

      var resourceManagerStub = new Mock<IResourceManager>();
      resourceManagerStub.Setup(_ => _.TryGetString("Remotion.ObjectBinding.Web.UI.Controls.BocList.ValidationFailuresFoundInOtherListPagesErrorMessage", out error)).Returns(true);

      var businessObjectStub = new Mock<IBusinessObject>();

      var bocListMock = new Mock<Control> { CallBase = true };
      bocListMock.As<IBocList>().Setup(_ => _.ValidationFailureRepository).Returns(validationFailureRepository);
      bocListMock.As<IBocList>().Setup(_ => _.GetResourceManager()).Returns(resourceManagerStub.Object);
      bocListMock.As<IBocList>().Setup(_ => _.Value).Returns(new[] { businessObjectStub.Object });
      bocListMock.Object.ID = "someBocList";

      var validator = new BocListValidationResultDispatchingValidator();
      validator.ErrorMessage = "This should not be output.";
      validator.ControlToValidate = "someBocList";

      NamingContainer.Controls.Add(bocListMock.Object);
      NamingContainer.Controls.Add(validator);

      validator.BuildErrorMessage();

      Assert.That(validator.ErrorMessage, Is.EqualTo("Errors on rows in BocList\r\nA list error\r\n"));
    }

    [Test]
    public void BuildErrorMessage_WithAllKindsOfFailures_ReturnsMessagesFromAllHandlers ()
    {
      var validationFailureRepository = new BocListValidationFailureRepository();
      validationFailureRepository.AddValidationFailuresForBocList(new [] { BusinessObjectValidationFailure.Create("A list error") });

      var rowObject = Mock.Of<IBusinessObject>();
      validationFailureRepository.AddValidationFailuresForDataRow(rowObject, new [] { BusinessObjectValidationFailure.Create("A handled row error") });
      validationFailureRepository.GetUnhandledValidationFailuresForDataRow(rowObject, true);

      validationFailureRepository.AddValidationFailuresForDataRow(Mock.Of<IBusinessObject>(), new [] { BusinessObjectValidationFailure.Create("An unhandled row error") });


      var resourceManagerStub = new Mock<IResourceManager>();
      var error1 = "Error details in rows";
      resourceManagerStub.Setup(_ => _.TryGetString("Remotion.ObjectBinding.Web.UI.Controls.BocList.ValidationFailuresFoundInListErrorMessage", out error1)).Returns(true);
      var error2 = "Errors on other page";
      resourceManagerStub.Setup(_ => _.TryGetString("Remotion.ObjectBinding.Web.UI.Controls.BocList.ValidationFailuresFoundInOtherListPagesErrorMessage", out error2)).Returns(true);

      var businessObjectStub = new Mock<IBusinessObject>();

      var bocListMock = new Mock<Control> { CallBase = true };
      bocListMock.As<IBocList>().Setup(_ => _.ValidationFailureRepository).Returns(validationFailureRepository);
      bocListMock.As<IBocList>().Setup(_ => _.GetResourceManager()).Returns(resourceManagerStub.Object);
      bocListMock.As<IBocList>().Setup(_ => _.Value).Returns(new[] { businessObjectStub.Object });
      bocListMock.Object.ID = "someBocList";

      var validator = new BocListValidationResultDispatchingValidator();
      validator.ErrorMessage = "This should not be output.";
      validator.ControlToValidate = "someBocList";

      NamingContainer.Controls.Add(bocListMock.Object);
      NamingContainer.Controls.Add(validator);

      validator.BuildErrorMessage();

      Assert.That(validator.ErrorMessage, Is.EqualTo("Error details in rows\r\nErrors on other page\r\nA list error\r\n"));
    }

    [Test]
    public void EvaluateIsValid_WithoutFailures_EmptyErrorMessageAndValid ()
    {
      var validationFailureRepository = new BocListValidationFailureRepository();

      var error = "BocList has validation errors.";
      var resourceManagerStub = new Mock<IResourceManager>();
      resourceManagerStub.Setup(_ => _.TryGetString("Remotion.ObjectBinding.Web.UI.Controls.BocList.RemainingValidationFailureText", out error)).Returns(true);

      var bocListMock = new Mock<Control> { CallBase = true };
      bocListMock.As<IBocList>().Setup(_ => _.ValidationFailureRepository).Returns(validationFailureRepository);
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
      var validationFailureRepository = new BocListValidationFailureRepository();
      validationFailureRepository.AddValidationFailuresForBocList(new [] { BusinessObjectValidationFailure.Create("A list error") });

      var error = "BocList has validation errors.";
      var resourceManagerStub = new Mock<IResourceManager>();
      resourceManagerStub.Setup(_ => _.TryGetString("Remotion.ObjectBinding.Web.UI.Controls.BocList.UnhandledValidationFailuresFoundErrorMessage", out error)).Returns(true);

      var bocListMock = new Mock<Control> { CallBase = true };
      bocListMock.As<IBocList>().Setup(_ => _.ValidationFailureRepository).Returns(validationFailureRepository);
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
