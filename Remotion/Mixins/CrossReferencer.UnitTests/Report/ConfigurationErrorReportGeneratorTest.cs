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
using NUnit.Framework;
using Remotion.Mixins.CrossReferencer.Report;
using Remotion.Mixins.CrossReferencer.Utilities;

namespace Remotion.Mixins.CrossReferencer.UnitTests.Report
{
  [TestFixture]
  public class ConfigurationErrorReportGeneratorTest
  {
    [Test]
    public void GenerateXml_NoErrors ()
    {
      var errorAggregator = new ErrorAggregator<ConfigurationException>();
      var reportGenerator = new ConfigurationErrorReportGenerator(errorAggregator);

      var output = reportGenerator.GenerateXml();
      var expectedOutput = new XElement("ConfigurationErrors");

      Assert.That(output.ToString(), Is.EqualTo(expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_WithErrors ()
    {
      var errorAggregator = new ErrorAggregator<ConfigurationException>();

      var innerException1 = SetUpExceptionWithDummyStackTrace("inner exception", null);
      var Exception1 = SetUpExceptionWithDummyStackTrace("test configuration exception 1", innerException1);
      var Exception2 = SetUpExceptionWithDummyStackTrace("test configuration excpetion 2", null);

      errorAggregator.AddException(Exception1);
      errorAggregator.AddException(Exception2);
      var reportGenerator = new ConfigurationErrorReportGenerator(errorAggregator);

      var output = reportGenerator.GenerateXml();
      var expectedOutput = new XElement(
          "ConfigurationErrors",
          new RecursiveExceptionReportGenerator(Exception1).GenerateXml(),
          new RecursiveExceptionReportGenerator(Exception2).GenerateXml()
      );

      Assert.That(output.ToString(), Is.EqualTo(expectedOutput.ToString()));
    }

    private ConfigurationException SetUpExceptionWithDummyStackTrace (string exceptionMessage, Exception innerException)
    {
      try
      {
        throw new ConfigurationException(exceptionMessage, innerException);
      }
      catch (ConfigurationException caughtException)
      {
        return caughtException;
      }
    }
  }
}
