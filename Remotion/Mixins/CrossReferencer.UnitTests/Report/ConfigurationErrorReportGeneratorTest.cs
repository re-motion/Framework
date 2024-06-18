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
using MixinXRef.Report;
using MixinXRef.Utility;
using NUnit.Framework;

namespace MixinXRef.UnitTests.Report
{
  [TestFixture]
  public class ConfigurationErrorReportGeneratorTest
  {
    [Test]
    public void GenerateXml_NoErrors ()
    {
      var errorAggregator = new ErrorAggregator<Exception>();
      var reportGenerator = new ConfigurationErrorReportGenerator (errorAggregator);

      var output = reportGenerator.GenerateXml();
      var expectedOutput = new XElement ("ConfigurationErrors");

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_WithErrors ()
    {
      var errorAggregator = new ErrorAggregator<Exception>();

      var innerException1 = SetUpExceptionWithDummyStackTrace ("inner exception", null);
      var Exception1 = SetUpExceptionWithDummyStackTrace ("test configuration exception 1", innerException1);
      var Exception2 = SetUpExceptionWithDummyStackTrace ("test configuration excpetion 2", null);

      errorAggregator.AddException (Exception1);
      errorAggregator.AddException (Exception2);
      var reportGenerator = new ConfigurationErrorReportGenerator (errorAggregator);

      var output = reportGenerator.GenerateXml();
      var expectedOutput = new XElement (
          "ConfigurationErrors",
          new RecursiveExceptionReportGenerator (Exception1).GenerateXml(),
          new RecursiveExceptionReportGenerator (Exception2).GenerateXml()
          );

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    private Exception SetUpExceptionWithDummyStackTrace (string exceptionMessage, Exception innerException)
    {
      try
      {
        throw new Exception (exceptionMessage, innerException);
      }
      catch (Exception caughtException)
      {
        return caughtException;
      }
    }
  }
}