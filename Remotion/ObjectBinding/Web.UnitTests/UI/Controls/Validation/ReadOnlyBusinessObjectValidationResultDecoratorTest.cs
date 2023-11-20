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
//
using System;
using Moq;
using NUnit.Framework;
using Remotion.ObjectBinding.Validation;
using Remotion.ObjectBinding.Web.UI.Controls.Validation;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.Validation
{
  [TestFixture]
  public class ReadOnlyBusinessObjectValidationResultDecoratorTest
  {
    [Test]
    public void GetValidationFailures_AlwaysPassesMarkAsHandledFalse ()
    {
      var businessObjectStub = Mock.Of<IBusinessObject>();
      var businessObjectPropertyStub = Mock.Of<IBusinessObjectProperty>();

      var expectedResultLength = 0; // Explicit variable to prevent inlining into Array.Empty
      var expectedResult = new BusinessObjectValidationFailure[expectedResultLength];

      var validationResultMock = new Mock<IBusinessObjectValidationResult>(MockBehavior.Strict);
      validationResultMock.Setup(e => e.GetValidationFailures(businessObjectStub, businessObjectPropertyStub, false))
          .Returns(expectedResult)
          .Verifiable();

      var readOnlyDecorator = new ReadOnlyBusinessObjectValidationResultDecorator(validationResultMock.Object);
      var result = readOnlyDecorator.GetValidationFailures(businessObjectStub, businessObjectPropertyStub, true);

      Assert.That(result, Is.SameAs(expectedResult));

      validationResultMock.Verify();
    }

    [Test]
    public void GetUnhandledValidationFailures_AlwaysPassesMarkAsHandledFalse ()
    {
      var businessObjectStub = Mock.Of<IBusinessObject>();

      var expectedResultLength = 0; // Explicit variable to prevent inlining into Array.Empty
      var expectedResult = new BusinessObjectValidationFailure[expectedResultLength];

      var validationResultMock = new Mock<IBusinessObjectValidationResult>(MockBehavior.Strict);
      validationResultMock.Setup(e => e.GetUnhandledValidationFailures(businessObjectStub, true, false))
          .Returns(expectedResult)
          .Verifiable();

      var readOnlyDecorator = new ReadOnlyBusinessObjectValidationResultDecorator(validationResultMock.Object);
      var result = readOnlyDecorator.GetUnhandledValidationFailures(businessObjectStub, true, true);

      Assert.That(result, Is.SameAs(expectedResult));

      validationResultMock.Verify();
    }
  }
}
