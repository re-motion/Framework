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
using System.Xml.Linq;
using Moq;
using NUnit.Framework;
using Remotion.Mixins.CrossReferencer.Reflectors;
using Remotion.Mixins.CrossReferencer.Report;
using Remotion.Mixins.CrossReferencer.Utilities;
using Remotion.Mixins.Validation;

namespace Remotion.Mixins.CrossReferencer.UnitTests.Report
{
  [TestFixture]
  public class ValidationErrorReportGeneratorTest
  {
    [Test]
    public void GenerateXml_NoErrors ()
    {
      var errorAggregator = new ErrorAggregator<ValidationException>();
      var reportGenerator = new ValidationErrorReportGenerator(errorAggregator, new RemotionReflector());

      var output = reportGenerator.GenerateXml();
      var expectedOutput = new XElement("ValidationErrors");

      Assert.That(output.ToString(), Is.EqualTo(expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_WithErrors ()
    {
      var errorAggregator = new ErrorAggregator<ValidationException>();
      var validationException1 = SetUpExceptionWithDummyStackTrace("test validation exception", new DefaultValidationLog());

      errorAggregator.AddException(validationException1);
      var reportGenerator = new ValidationErrorReportGenerator(errorAggregator, new RemotionReflector());

      var output = reportGenerator.GenerateXml();

      var validationExceptionElement = new RecursiveExceptionReportGenerator(validationException1).GenerateXml();
      validationExceptionElement.Add(
          new XElement(
              "ValidationLog",
              new XAttribute("number-of-rules-executed", validationException1.ValidationLogData.NumberOfRulesExecuted),
              new XAttribute("number-of-failures", validationException1.ValidationLogData.NumberOfFailures),
              new XAttribute("number-of-unexpected-exceptions", validationException1.ValidationLogData.NumberOfUnexpectedExceptions),
              new XAttribute("number-of-warnings", validationException1.ValidationLogData.NumberOfWarnings),
              new XAttribute("number-of-successes", validationException1.ValidationLogData.NumberOfSuccesses)
          ));

      var expectedOutput = new XElement("ValidationErrors", validationExceptionElement);

      Assert.That(output.ToString(), Is.EqualTo(expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_WithValidationLogNullObject ()
    {
      var errorAggregator = new ErrorAggregator<ValidationException>();
      var validationException1 = SetUpExceptionWithDummyStackTrace("test validation exception", new DefaultValidationLog());

      errorAggregator.AddException(validationException1);
      var remotionReflectorStub = new Mock<IRemotionReflector>();
      var reportGenerator = new ValidationErrorReportGenerator(errorAggregator, remotionReflectorStub.Object);

      remotionReflectorStub
          .Setup(_ => _.GetValidationLogFromValidationException(It.IsAny<ValidationException>()))
          .Returns((ValidationException e) => e.ValidationLogData);

      var output = reportGenerator.GenerateXml();

      var validationExceptionElement = new RecursiveExceptionReportGenerator(validationException1).GenerateXml();
      validationExceptionElement.Add(
          new XElement(
              "ValidationLog",
              new XAttribute("number-of-rules-executed", 0),
              new XAttribute("number-of-failures", 0),
              new XAttribute("number-of-unexpected-exceptions", 0),
              new XAttribute("number-of-warnings", 0),
              new XAttribute("number-of-successes", 0)));

      var expectedOutput = new XElement("ValidationErrors", validationExceptionElement);

      Assert.That(output.ToString(), Is.EqualTo(expectedOutput.ToString()));
    }

    private ValidationException SetUpExceptionWithDummyStackTrace (string exceptionMessage, IValidationLog validationLog)
    {
      try
      {
        throw new ValidationException(new ValidationLogData());
      }
      catch (ValidationException caughtException)
      {
        return caughtException;
      }
    }
  }
}
