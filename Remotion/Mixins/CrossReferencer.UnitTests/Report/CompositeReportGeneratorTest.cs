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
using MixinXRef.UnitTests.Stub;
using NUnit.Framework;

namespace MixinXRef.UnitTests.Report
{
  [TestFixture]
  public class CompositeReportGeneratorTest
  {
    private CompositeReportGenerator _reportGenerator;

    [SetUp]
    public void SetUp ()
    {
      _reportGenerator = new CompositeReportGenerator (new ReportGeneratorStub ("Assemblies"), new ReportGeneratorStub ("InvolvedTypes"));
    }

    [Test]
    public void GenerateXml ()
    {
      XElement output = _reportGenerator.GenerateXml ();

      var expectedOutput = new XElement ("MixinXRefReport", new XElement ("Assemblies"), new XElement ("InvolvedTypes"));
      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }
  }
}