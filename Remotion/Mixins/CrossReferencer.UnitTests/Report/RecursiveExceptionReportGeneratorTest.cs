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
using NUnit.Framework;

namespace MixinXRef.UnitTests.Report
{
  [TestFixture]
  public class RecursiveExceptionReportGeneratorTest
  {
    [Test]
    public void GenerateXml_ForExceptionWithoutInnerException ()
    {
      var exception = SetUpExceptionWithDummyStackTrace ("plain exception", null);

      var reportGenerator = new RecursiveExceptionReportGenerator (exception);
      var output = reportGenerator.GenerateXml();

      var expectedOutput = new XElement (
          "Exception",
          new XAttribute ("type", exception.GetType()),
          new XElement ("Message", new XCData (exception.Message)),
          new XElement ("StackTrace", new XCData (exception.StackTrace))
          );

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_ForExceptionWithInnerException ()
    {
      var innerException = SetUpExceptionWithDummyStackTrace ("inner exception", null);
      var outerException = SetUpExceptionWithDummyStackTrace ("exception with inner exception", innerException);
      var reportGenerator = new RecursiveExceptionReportGenerator (outerException);

      var output = reportGenerator.GenerateXml();

      var expectedOutput = new XElement (
          "Exception",
          new XAttribute ("type", outerException.GetType()),
          new XElement ("Message", new XCData (outerException.Message)),
          new XElement ("StackTrace", new XCData (outerException.StackTrace)),
          new XElement (
              "Exception",
              new XAttribute ("type", innerException.GetType()),
              new XElement ("Message", new XCData (innerException.Message)),
              new XElement ("StackTrace", new XCData (innerException.StackTrace)))
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