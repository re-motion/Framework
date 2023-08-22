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
using System.Text;
using Moq;
using NUnit.Framework;
using Remotion.Globalization;
using Remotion.ObjectBinding.Validation;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Validation;
using Remotion.ObjectBinding.Web.UI.Controls.Validation;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.Validation
{
  [TestFixture]
  public class CheckRowsBocListValidationFailureHandlerTest
  {
    private Mock<IBocList> _bocListStub;
    private Mock<IResourceManager> _resourceManagerStub;
    private BocListValidationFailureRepository _repository;

    [SetUp]
    public void Setup ()
    {
      _bocListStub = new Mock<IBocList>();
      _resourceManagerStub = new Mock<IResourceManager>();
      _repository = new BocListValidationFailureRepository();

      _bocListStub.Setup(_ => _.ValidationFailureRepository).Returns(_repository);
      _bocListStub.Setup(_ => _.GetResourceManager()).Returns(_resourceManagerStub.Object);
    }

    [Test]
    public void HandleValidationFailures_WithoutHandledRowOrCellFailuresValidationFailures_ContextContainsEmptyMessage ()
    {
      _repository.AddValidationFailuresForBocList(new [] { BusinessObjectValidationFailure.Create("A handled list failure.") });
      _repository.AddValidationFailuresForDataRow(Mock.Of<IBusinessObject>(), new [] { BusinessObjectValidationFailure.Create("A row failure"),  });
      _repository.GetUnhandledValidationFailuresForBocList(true);
      _repository.AddValidationFailuresForBocList(new [] { BusinessObjectValidationFailure.Create("An unhandled list failure.") });

      var context = new ValidationFailureHandlingContext(_bocListStub.Object);

      var handler = new CheckRowsBocListValidationFailureHandler();

      handler.HandleValidationFailures(context);

      var stringBuilder = new StringBuilder();
      context.AppendErrorMessages(stringBuilder);
      Assert.That(stringBuilder.ToString(), Is.Empty);
    }

    [Test]
    public void HandleValidationFailures_WithValidationFailures_ContextContainsConcatenatedMessages ()
    {
      _repository.AddValidationFailuresForDataRow(Mock.Of<IBusinessObject>(), new [] { BusinessObjectValidationFailure.Create("A handled row failure"),  });
      _repository.GetUnhandledValidationFailuresForBocListAndContainingDataRowsAndDataCells(true);

      var errorMessage = "Error message from resource manager.";
      _resourceManagerStub
          .Setup(_ => _.TryGetString("Remotion.ObjectBinding.Web.UI.Controls.BocList.ValidationFailuresFoundInListErrorMessage", out errorMessage))
          .Returns(true);

      var context = new ValidationFailureHandlingContext(_bocListStub.Object);

      var handler = new CheckRowsBocListValidationFailureHandler();

      handler.HandleValidationFailures(context);

      var stringBuilder = new StringBuilder();
      context.AppendErrorMessages(stringBuilder);
      Assert.That(stringBuilder.ToString(), Is.EqualTo("Error message from resource manager.\r\n"));
    }
  }
}
