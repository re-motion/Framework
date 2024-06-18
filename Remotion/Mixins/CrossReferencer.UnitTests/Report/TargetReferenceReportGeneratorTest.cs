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
using Remotion.Mixins.CrossReferencer.UnitTests.TestDomain;
using Remotion.Mixins.CrossReferencer.Utilities;

namespace Remotion.Mixins.CrossReferencer.UnitTests.Report
{
  [TestFixture]
  public class TargetReferenceReportGeneratorTest
  {
    [Test]
    public void GenerateXml_NonMixin ()
    {
      var type1 = new InvolvedType (typeof (object));

      var reportGenerator = new TargetReferenceReportGenerator (type1, new IdentifierGenerator<Type>());

      var output = reportGenerator.GenerateXml();

      Assert.That (output, Is.Null);
    }

    [Test]
    public void GenerateXml_ForMixin ()
    {
      var type1 = new InvolvedType (typeof (Mixin1));
      type1.TargetTypes.Add (new InvolvedType(typeof (TargetClass1)), null);

      var reportGenerator = new TargetReferenceReportGenerator (type1, new IdentifierGenerator<Type>());

      var output = reportGenerator.GenerateXml();

      var expectedOutput = new XElement (
          "Targets",
          new XElement (
              "Target",
              new XAttribute ("ref", "0")
              )
          );

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }
  }
}
