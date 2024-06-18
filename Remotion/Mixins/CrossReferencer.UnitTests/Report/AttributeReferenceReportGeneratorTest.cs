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
using Remotion.Mixins.CrossReferencer.UnitTests.TestDomain;
using Remotion.Mixins.CrossReferencer.Utilities;

namespace Remotion.Mixins.CrossReferencer.UnitTests.Report
{
  [TestFixture]
  public class AttributeReferenceReportGeneratorTest
  {
    private IIdentifierGenerator<Type> _identifierGenerator;
    private IRemotionReflector _remotionReflector;

    [SetUp]
    public void SetUp ()
    {
      _identifierGenerator = new IdentifierGenerator<Type>();
      _remotionReflector = MockRepository.GenerateStub<IRemotionReflector>();
      _remotionReflector.Stub(r => r.IsInfrastructureType(typeof(UsesAttribute))).Return(true);
      _remotionReflector.Stub(r => r.IsInfrastructureType(typeof(ExtendsAttribute))).Return(true);
    }

    [Test]
    public void GenerateXml_ZeroAttributes ()
    {
      var reportGenerator = new AttributeReferenceReportGenerator(typeof(UselessObject), _identifierGenerator, _remotionReflector);
      var output = reportGenerator.GenerateXml();

      var expectedOutput = new XElement("HasAttributes");

      Assert.That(output.ToString(), Is.EqualTo(expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_WithAttributes ()
    {
      _remotionReflector.Stub(r => r.IsInfrastructureType(typeof(SerializableAttribute))).Return(false);

      // Mixin2 has SerializableAttribute which has no parameters
      var reportGenerator = new AttributeReferenceReportGenerator(typeof(Mixin2), _identifierGenerator, _remotionReflector);
      var output = reportGenerator.GenerateXml();

      var expectedOutput = new XElement(
          "HasAttributes",
          new XElement("HasAttribute", new XAttribute("ref", "0"))
      );

      Assert.That(output.ToString(), Is.EqualTo(expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_WithAttributesWithParameters ()
    {
      _remotionReflector.Stub(r => r.IsInfrastructureType(typeof(BookAttribute))).Return(false);

      // ClassWithBookAttribute has the following attribute: [Book (1, Title = "C# in depth")]
      var reportGenerator = new AttributeReferenceReportGenerator(typeof(ClassWithBookAttribute), _identifierGenerator, _remotionReflector);

      var output = reportGenerator.GenerateXml();

      var expectedOutput = new XElement(
          "HasAttributes",
          new XElement(
              "HasAttribute",
              new XAttribute("ref", "0"),
              new XElement(
                  "Argument",
                  new XAttribute("kind", "constructor"),
                  new XAttribute("type", "Int32"),
                  new XAttribute("name", "id"),
                  new XAttribute("value", 1337)),
              new XElement(
                  "Argument",
                  new XAttribute("kind", "named"),
                  new XAttribute("type", "String"),
                  new XAttribute("name", "Title"),
                  new XAttribute("value", "C# in depth"))
          ));

      Assert.That(output.ToString(), Is.EqualTo(expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_WithAttributesWithFieldParameter ()
    {
      _remotionReflector.Stub(r => r.IsInfrastructureType(typeof(FieldParamAttribute))).Return(false);

      // ClassWithAttributeFieldParam has the following attribute: [FieldParam(new[] { "AttributeParam1", "AttributeParam2"})]
      var reportGenerator = new AttributeReferenceReportGenerator(typeof(ClassWithAttributeFieldParam), _identifierGenerator, _remotionReflector);

      var output = reportGenerator.GenerateXml();

      var expectedOutput = new XElement(
          "HasAttributes",
          new XElement(
              "HasAttribute",
              new XAttribute("ref", "0"),
              new XElement(
                  "Argument",
                  new XAttribute("kind", "constructor"),
                  new XAttribute("type", "String[]"),
                  new XAttribute("name", "stringArray"),
                  new XAttribute("value", "{AttributeParam1, AttributeParam2}")
              )
          )
      );

      Assert.That(output.ToString(), Is.EqualTo(expectedOutput.ToString()));
    }
  }
}
