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
using Remotion.ObjectBinding.Validation;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Validation;
using Remotion.ObjectBinding.Web.UI.Controls.Validation;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.Validation
{
  [TestFixture]
  public class BocListListValidationFailureHandlerTest
  {
    [Test]
    public void HandleValidationFailures_WithValidationFailures_ContextContainsConcatenatedMessages ()
    {
      var bocListStub = new Mock<IBocList>();
      var repository = new BocListValidationFailureRepository();

      bocListStub.Setup(_ => _.ValidationFailureRepository).Returns(repository);

      var validationFailures = new[]
                               {
                                     BusinessObjectValidationFailure.Create("Error #1"),
                                     BusinessObjectValidationFailure.Create("Error #2")
                               };

      repository.AddValidationFailuresForBocList(validationFailures);

      var context = new ValidationFailureHandlingContext(bocListStub.Object);

      var handler = new BocListListValidationFailureHandler();

      handler.HandleValidationFailures(context);

      var stringBuilder = new StringBuilder();
      context.AppendErrorMessages(stringBuilder);
      Assert.That(stringBuilder.ToString(), Is.EqualTo("Error #1\r\nError #2\r\n"));
    }

    [Test]
    public void HandleValidationFailures_WithoutValidationFailures_ContextContainsEmptyMessage ()
    {
      var bocListStub = new Mock<IBocList>();

      var repository = new BocListValidationFailureRepository();

      bocListStub.Setup(_ => _.ValidationFailureRepository).Returns(repository);

      var context = new ValidationFailureHandlingContext(bocListStub.Object);

      var handler = new BocListListValidationFailureHandler();

      handler.HandleValidationFailures(context);

      var stringBuilder = new StringBuilder();
      context.AppendErrorMessages(stringBuilder);
      Assert.That(stringBuilder.ToString(), Is.Empty);
    }
  }
}
