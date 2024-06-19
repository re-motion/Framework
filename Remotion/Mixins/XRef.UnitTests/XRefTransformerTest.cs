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
using System.IO;
using NUnit.Framework;

namespace Remotion.Mixins.XRef.UnitTests
{
  [TestFixture]
  public class XRefTransformerTest
  {
    const string documentationDirectory = "MixinDoc";

    [SetUp]
    public void SetUp ()
    {
      Directory.CreateDirectory(documentationDirectory);
    }

    [TearDown]
    public void TearDown ()
    {
      Directory.Delete(documentationDirectory, true);
    }

    [Test]
    public void GeneateHtmlFromXml_NonExistingXmlInputFile ()
    {
      // save and redirect standard error
      var standardError = Console.Error;
      var textWriter = new StringWriter();
      Console.SetError(textWriter);

      var transfomer = new XRefTransformer("invalidFile.xml", "C:/");

      // error code 2 means - source file does not exist
      Assert.That(transfomer.GenerateHtmlFromXml(), Is.EqualTo(2));
      Assert.That(textWriter.ToString(), Is.EqualTo("Source file invalidFile.xml does not exist\r\n"));

      // restore standard error
      Console.SetError(standardError);
    }

    [Test]
    public void GeneateHtmlFromXml_ValidXmlInputFile ()
    {
      var fileName = Path.Combine(documentationDirectory, "index.html");
      var transfomer = new XRefTransformer(@"..\..\TestDomain\fullReportGeneratorExpectedOutput.xml", documentationDirectory);

      Assert.That(File.Exists(fileName), Is.False);
      Assert.That(transfomer.GenerateHtmlFromXml(), Is.EqualTo(0));
      Assert.That(File.Exists(fileName), Is.True);
    }
  }
}
