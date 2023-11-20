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
  public class BocListRowAndCellValidationFailureHandlerTest
  {
    private Mock<IBocList> _bocListStub;
    private BocListValidationFailureRepository _repository;

    [SetUp]
    public void Setup ()
    {
      _bocListStub = new Mock<IBocList>();
      _repository = new BocListValidationFailureRepository();
    }

    [Test]
    public void HandleValidationFailures_WithoutValidationFailures_ContextContainsEmptyMessage ()
    {
      _bocListStub.Setup(_ => _.ValidationFailureRepository).Returns(_repository);

      var context = new ValidationFailureHandlingContext(_bocListStub.Object);

      var handler = new BocListRowAndCellValidationFailureHandler();

      handler.HandleValidationFailures(context);

      var stringBuilder = new StringBuilder();
      context.AppendErrorMessages(stringBuilder);
      Assert.That(stringBuilder.ToString(), Is.Empty);
    }

    [Test]
    public void HandleValidationFailures_WithValidationFailures_ContextContainsConcatenatedMessages ()
    {
      var resourceManagerStub = new Mock<IResourceManager>();

      _bocListStub.Setup(_ => _.ValidationFailureRepository).Returns(_repository);
      _bocListStub.Setup(_ => _.GetResourceManager()).Returns(resourceManagerStub.Object);

      _repository.AddValidationFailuresForDataRow(Mock.Of<IBusinessObject>(), new[] { BusinessObjectValidationFailure.Create("A row failure") });

      var errorMessage = "Error message from resource manager.";
      resourceManagerStub
          .Setup(_ => _.TryGetString("Remotion.ObjectBinding.Web.UI.Controls.BocList.ValidationFailuresFoundInOtherListPagesErrorMessage", out errorMessage))
          .Returns(true);

      var context = new ValidationFailureHandlingContext(_bocListStub.Object);

      var handler = new BocListRowAndCellValidationFailureHandler();

      handler.HandleValidationFailures(context);

      var stringBuilder = new StringBuilder();
      context.AppendErrorMessages(stringBuilder);
      Assert.That(stringBuilder.ToString(), Is.EqualTo("Error message from resource manager.\r\n"));
    }
  }
}
