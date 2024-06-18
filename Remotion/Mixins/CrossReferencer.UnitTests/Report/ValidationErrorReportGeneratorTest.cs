// This file is part of the MixinXRef project
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
using System;
using System.Xml.Linq;
using MixinXRef.Reflection;
using MixinXRef.Reflection.RemotionReflector;
using MixinXRef.Report;
using MixinXRef.Utility;
using NUnit.Framework;
using Remotion.Mixins.Validation;
using Rhino.Mocks;

namespace MixinXRef.UnitTests.Report
{
  [TestFixture]
  public class ValidationErrorReportGeneratorTest
  {
    [Test]
    public void GenerateXml_NoErrors ()
    {
      var errorAggregator = new ErrorAggregator<Exception>();
      var reportGenerator = new ValidationErrorReportGenerator (errorAggregator, Helpers.RemotionReflectorFactory.GetRemotionReflection ());

      var output = reportGenerator.GenerateXml();
      var expectedOutput = new XElement ("ValidationErrors");

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_WithErrors ()
    {
      var errorAggregator = new ErrorAggregator<Exception>();
      var validationException1 = SetUpExceptionWithDummyStackTrace("test validation exception", new DefaultValidationLog());

      errorAggregator.AddException (validationException1);
      var reportGenerator = new ValidationErrorReportGenerator (errorAggregator, Helpers.RemotionReflectorFactory.GetRemotionReflection ());

      var output = reportGenerator.GenerateXml();

      var validationExceptionElement = new RecursiveExceptionReportGenerator (validationException1).GenerateXml();
      validationExceptionElement.Add (
          new XElement ("ValidationLog",
                        new XAttribute("number-of-rules-executed", validationException1.ValidationLog.GetNumberOfRulesExecuted()),
                        new XAttribute("number-of-failures", validationException1.ValidationLog.GetNumberOfFailures()),
                        new XAttribute("number-of-unexpected-exceptions", validationException1.ValidationLog.GetNumberOfUnexpectedExceptions()),
                        new XAttribute("number-of-warnings", validationException1.ValidationLog.GetNumberOfWarnings()),
                        new XAttribute("number-of-successes", validationException1.ValidationLog.GetNumberOfSuccesses())
              ));

      var expectedOutput = new XElement ("ValidationErrors", validationExceptionElement);

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_WithValidationLogNullObject ()
    {
      var errorAggregator = new ErrorAggregator<Exception>();
      var validationException1 = SetUpExceptionWithDummyStackTrace("test validation exception", new DefaultValidationLog());

      errorAggregator.AddException (validationException1);
      var remotionReflectorStub = MockRepository.GenerateStub<IRemotionReflector>();
      var reportGenerator = new ValidationErrorReportGenerator (errorAggregator, remotionReflectorStub);

      remotionReflectorStub.Stub (_ => _.GetValidationLogFromValidationException (null)).IgnoreArguments()
          .Return (new ReflectedObject (new ValidationLogNullObject()));

      var output = reportGenerator.GenerateXml();

      var validationExceptionElement = new RecursiveExceptionReportGenerator (validationException1).GenerateXml();
      validationExceptionElement.Add (
          new XElement (
              "ValidationLog",
              new XAttribute ("number-of-rules-executed", 0),
              new XAttribute ("number-of-failures", 0),
              new XAttribute ("number-of-unexpected-exceptions", 0),
              new XAttribute ("number-of-warnings", 0),
              new XAttribute ("number-of-successes", 0)));

      var expectedOutput = new XElement ("ValidationErrors", validationExceptionElement);

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    private ValidationException SetUpExceptionWithDummyStackTrace(string exceptionMessage, IValidationLog validationLog)
    {
      try
      {
        throw new ValidationException(exceptionMessage, validationLog);
      }
      catch (ValidationException caughtException)
      {
        return caughtException;
      }
    }
  }
}