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
using System.Reflection;
using System.Xml.Linq;
using MixinXRef.Formatting;
using MixinXRef.Report;
using MixinXRef.UnitTests.TestDomain;
using MixinXRef.Utility;
using NUnit.Framework;

namespace MixinXRef.UnitTests.Report
{
  [TestFixture]
  public class AttributeReportGeneratorTest
  {
    [Test]
    public void GenerateXml_NoAttributes ()
    {
      // UselessObject has no attributes
      var involvedType = new InvolvedType (typeof (UselessObject));
      var reportGenerator = CreateReportGenerator (new IdentifierGenerator<Type>(), involvedType);

      var output = reportGenerator.GenerateXml();

      var expectedOutput = new XElement ("Attributes");
      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_WithAttributes ()
    {
      // Mixin2 has Serializable attribute
      var involvedType = new InvolvedType (typeof (Mixin2));

      var attributeIdentifier = new IdentifierGenerator<Type>();
      attributeIdentifier.GetIdentifier (typeof (SerializableAttribute));
      var reportGenerator = CreateReportGenerator (attributeIdentifier, involvedType);

      var output = reportGenerator.GenerateXml();

      var expectedOutput = new XElement (
          "Attributes",
          new XElement (
              "Attribute",
              new XAttribute ("id", "0"),
              new XAttribute ("assembly-ref", "0"),
              new XAttribute ("namespace", "System"),
              new XAttribute ("name", "SerializableAttribute"),
              new XElement (
                  "AppliedTo",
                  new XElement (
                      "InvolvedType-Reference",
                      new XAttribute ("ref", "0")
                      )
                  )
              )
          );
      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_WithNestedAttribute ()
    {
      // ClassWithNestedAttribute has 'ClassWithNestedAttribute.NestedAttribute' applied
      var involvedType = new InvolvedType (typeof (ClassWithNestedAttribute));

      var attributeIdentifier = new IdentifierGenerator<Type>();
      attributeIdentifier.GetIdentifier (typeof (ClassWithNestedAttribute.NestedAttribute));
      var reportGenerator = CreateReportGenerator (attributeIdentifier, involvedType);

      var output = reportGenerator.GenerateXml();

      var expectedOutput = new XElement (
          "Attributes",
          new XElement (
              "Attribute",
              new XAttribute ("id", "0"),
              new XAttribute ("assembly-ref", "0"),
              new XAttribute ("namespace", "MixinXRef.UnitTests.TestDomain"),
              new XAttribute ("name", "ClassWithNestedAttribute+NestedAttribute"),
              new XElement (
                  "AppliedTo",
                  new XElement (
                      "InvolvedType-Reference",
                      new XAttribute ("ref", "0")
                      )
                  )
              )
          );
      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }


    private AttributeReportGenerator CreateReportGenerator (IdentifierGenerator<Type> attributeIdentifier, params InvolvedType[] involvedTypes)
    {
      return new AttributeReportGenerator (
          involvedTypes,
          new IdentifierGenerator<Assembly>(),
          new IdentifierGenerator<Type>(),
          attributeIdentifier,
          Helpers.RemotionReflectorFactory.GetRemotionReflection (),
          new OutputFormatter());
    }
  }
}