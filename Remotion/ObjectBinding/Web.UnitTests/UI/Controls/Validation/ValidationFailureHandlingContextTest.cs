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
using System.Text;
using Moq;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Validation;
using Remotion.ObjectBinding.Web.UI.Controls.Validation;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.Validation
{
  [TestFixture]
  public class ValidationFailureHandlingContextTest
  {
    private BocListMock _bocListStub;

    [SetUp]
    public void Setup ()
    {
      _bocListStub = new BocListMock();
    }

    [Test]
    public void AppendErrorMessage_AppendsReportedMessagesToStringBuilder ()
    {
      var context = new ValidationFailureHandlingContext(_bocListStub);

      context.ReportErrorMessage("First error message.");
      context.ReportErrorMessage("Second error message.");

      var stringBuilder = new StringBuilder();

      context.AppendErrorMessages(stringBuilder);

      Assert.That(stringBuilder.ToString(), Is.EqualTo("First error message.\r\nSecond error message.\r\n"));
    }

    [Test]
    public void AppendErrorMessages_WithExistingContentInStringBuilder_AppendsNewLineAndReportedMessagesToStringBuilder ()
    {
      var context = new ValidationFailureHandlingContext(_bocListStub);

      context.ReportErrorMessage("First error message.");
      context.ReportErrorMessage("Second error message.");

      var stringBuilder = new StringBuilder();
      stringBuilder.Append("Existing message without newline.");

      context.AppendErrorMessages(stringBuilder);

      Assert.That(stringBuilder.ToString(), Is.EqualTo("Existing message without newline.\r\nFirst error message.\r\nSecond error message.\r\n"));
    }

    [Test]
    public void AppendErrorMessages_WithTrailingNewLineInStringBuilder_AppendsReportedMessagesToStringBuilder ()
    {
      var context = new ValidationFailureHandlingContext(_bocListStub);

      context.ReportErrorMessage("First error message.");
      context.ReportErrorMessage("Second error message.");

      var stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("Existing message with newline.");

      context.AppendErrorMessages(stringBuilder);

      Assert.That(stringBuilder.ToString(), Is.EqualTo("Existing message with newline.\r\nFirst error message.\r\nSecond error message.\r\n"));
    }

    [Test]
    public void AppendErrorMessage_WithMessagesAfterReporting_AppendsReportedMessagesToStringBuilder ()
    {
      var context = new ValidationFailureHandlingContext(_bocListStub);

      context.ReportErrorMessage("First error message.");
      context.ReportErrorMessage("Second error message.");

      var stringBuilder = new StringBuilder();

      context.AppendErrorMessages(stringBuilder);
      Assert.That(stringBuilder.ToString(), Is.EqualTo("First error message.\r\nSecond error message.\r\n"));

      context.ReportErrorMessage("Third error message");

      stringBuilder = new StringBuilder();

      context.AppendErrorMessages(stringBuilder);
      Assert.That(stringBuilder.ToString(), Is.EqualTo("First error message.\r\nSecond error message.\r\nThird error message\r\n"));
    }

    [Test]
    public void BocList_ReturnsBocListSuppliedInCtor ()
    {
      var context = new ValidationFailureHandlingContext(_bocListStub);

      Assert.That(context.BocList, Is.SameAs(_bocListStub));
    }

    [Test]
    public void ValidationFailureRepository_ReturnsValidationFailureRepositoryOfBocList ()
    {
      var context = new ValidationFailureHandlingContext(_bocListStub);

      Assert.That(context.ValidationFailureRepository, Is.SameAs(_bocListStub.ValidationFailureRepository));
    }
  }
}
