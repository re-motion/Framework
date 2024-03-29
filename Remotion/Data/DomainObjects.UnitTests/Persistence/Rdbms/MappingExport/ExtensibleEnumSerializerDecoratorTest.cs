﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using System.Linq;
using System.Xml.Linq;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms.MappingExport;
using Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SchemaGenerationTestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.MappingExport
{
  [TestFixture]
  public class ExtensibleEnumSerializerDecoratorTest : SchemaGenerationTestBase
  {
    [Test]
    public void Serialize_CreatesEnumTypeElement ()
    {
      var enumSerializerStub = new Mock<IEnumSerializer>();
      var enumSerializer = new ExtensibleEnumSerializerDecorator(enumSerializerStub.Object);
      enumSerializer.CollectPropertyType(GetPropertyDefinition((ClassWithAllDataTypes _) => _.ExtensibleEnumProperty));
      enumSerializerStub.Setup(_ => _.Serialize()).Returns(new XElement[0]);
      var actual = enumSerializer.Serialize().Single();

      Assert.That(actual.Name.LocalName, Is.EqualTo("enumType"));
    }

    [Test]
    public void Serialize_AddsElementsFromInnerEnumSerializer ()
    {
      var enumSerializerStub = new Mock<IEnumSerializer>();
      var enumSerializer = new ExtensibleEnumSerializerDecorator(enumSerializerStub.Object);
      enumSerializer.CollectPropertyType(GetPropertyDefinition((ClassWithAllDataTypes _) => _.ExtensibleEnumProperty));

      enumSerializerStub.Setup(_ => _.Serialize()).Returns(new[] { new XElement("innerResult1"), new XElement("innerResult2") });

      var actual = enumSerializer.Serialize();

      Assert.That(actual.Select(e => e.Name.LocalName), Is.EqualTo(new[] { "enumType", "innerResult1", "innerResult2" }));
    }

    [Test]
    public void Serialize_AddsTypeAttribute ()
    {
      var enumSerializerStub = new Mock<IEnumSerializer>();
      var enumSerializer = new ExtensibleEnumSerializerDecorator(enumSerializerStub.Object);
      enumSerializer.CollectPropertyType(GetPropertyDefinition((ClassWithAllDataTypes _) => _.ExtensibleEnumProperty));

      enumSerializerStub.Setup(_ => _.Serialize()).Returns(new XElement[0]);
      var actual = enumSerializer.Serialize().Single();

      Assert.That(actual.Attributes().Select(a => a.Name.LocalName), Contains.Item("type"));
      Assert.That(
          actual.Attribute("type").Value,
          Is.EqualTo("Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SchemaGenerationTestDomain.Color, Remotion.Data.DomainObjects.UnitTests"));
    }

    [Test]
    public void Serialize_AddsValueElementsForExtensibleEnumType ()
    {
      var enumSerializerStub = new Mock<IEnumSerializer>();
      var enumSerializer = new ExtensibleEnumSerializerDecorator(enumSerializerStub.Object);
      enumSerializer.CollectPropertyType(GetPropertyDefinition((ClassWithAllDataTypes _) => _.ExtensibleEnumProperty));
      enumSerializerStub.Setup(_ => _.Serialize()).Returns(new XElement[0]);
      var actual = enumSerializer.Serialize().Single();

      Assert.That(actual.Elements().Count(), Is.EqualTo(3));

      var firstValueElement = actual.Elements().ElementAt(0);
      Assert.That(firstValueElement.Name.LocalName, Is.EqualTo("value"));
      Assert.That(firstValueElement.Attributes().Select(a => a.Name.LocalName), Contains.Item("name"));
      Assert.That(firstValueElement.Attribute("name").Value, Is.EqualTo("Blue"));
      Assert.That(firstValueElement.Attributes().Select(a => a.Name.LocalName), Contains.Item("columnValue"));
      Assert.That(
          firstValueElement.Attribute("columnValue").Value,
          Is.EqualTo("Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SchemaGenerationTestDomain.ColorExtensions.Blue"));

      var secondValueElement = actual.Elements().ElementAt(1);
      Assert.That(secondValueElement.Name.LocalName, Is.EqualTo("value"));
      Assert.That(secondValueElement.Attributes().Select(a => a.Name.LocalName), Contains.Item("name"));
      Assert.That(secondValueElement.Attribute("name").Value, Is.EqualTo("Green"));
      Assert.That(secondValueElement.Attributes().Select(a => a.Name.LocalName), Contains.Item("columnValue"));
      Assert.That(
          secondValueElement.Attribute("columnValue").Value,
          Is.EqualTo("Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SchemaGenerationTestDomain.ColorExtensions.Green"));

      var thirdValueElement = actual.Elements().ElementAt(2);
      Assert.That(thirdValueElement.Name.LocalName, Is.EqualTo("value"));
      Assert.That(thirdValueElement.Attributes().Select(a => a.Name.LocalName), Contains.Item("name"));
      Assert.That(thirdValueElement.Attribute("name").Value, Is.EqualTo("Red"));
      Assert.That(thirdValueElement.Attributes().Select(a => a.Name.LocalName), Contains.Item("columnValue"));
      Assert.That(
          thirdValueElement.Attribute("columnValue").Value,
          Is.EqualTo("Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SchemaGenerationTestDomain.ColorExtensions.Red"));
    }

    [Test]
    public void CollectPropertyType_CollectsExtensibleEnumType ()
    {
      var enumSerializer = new ExtensibleEnumSerializerDecorator(new Mock<IEnumSerializer>().Object);
      enumSerializer.CollectPropertyType(GetPropertyDefinition((ClassWithAllDataTypes _) => _.ExtensibleEnumProperty));

      Assert.That(enumSerializer.EnumTypes, Contains.Item(typeof(Color)));
    }

    [Test]
    public void CollectPropertyType_DoesNotCollectDuplicates ()
    {
      var enumSerializer = new ExtensibleEnumSerializerDecorator(new Mock<IEnumSerializer>().Object);
      enumSerializer.CollectPropertyType(GetPropertyDefinition((ClassWithAllDataTypes _) => _.ExtensibleEnumProperty));
      enumSerializer.CollectPropertyType(GetPropertyDefinition((ClassWithAllDataTypes _) => _.ExtensibleEnumProperty));

      Assert.That(enumSerializer.EnumTypes.Count, Is.EqualTo(1));
    }

    [Test]
    public void CollectPropertyType_DoesNotCollectNonEnumTypes ()
    {
      var enumSerializer = new ExtensibleEnumSerializerDecorator(new Mock<IEnumSerializer>().Object);
      enumSerializer.CollectPropertyType(GetPropertyDefinition((ClassWithAllDataTypes _) => _.Int32Property));

      Assert.That(enumSerializer.EnumTypes, Is.Empty);
    }

    [Test]
    public void CollectPropertyType_DelegatesOtherTypesToInnerEnumSerializer ()
    {
      var enumSerializerStub = new Mock<IEnumSerializer>();
      var enumSerializer = new ExtensibleEnumSerializerDecorator(enumSerializerStub.Object);
      var expectedProperty = GetPropertyDefinition((ClassWithAllDataTypes _) => _.ByteProperty);
      enumSerializer.CollectPropertyType(expectedProperty);

      enumSerializerStub.Verify(_ => _.CollectPropertyType(expectedProperty), Times.AtLeastOnce());
    }
  }
}
