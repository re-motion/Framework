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
using System.Linq;
using System.Xml.Linq;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.MappingExport;
using Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SchemaGenerationTestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.MappingExport
{
  [TestFixture]
  public class TableSerializerTest : SchemaGenerationTestBase
  {

    [Test]
    public void Serialize_CreatesTableElement ()
    {
      var tableSerializer = new TableSerializer(new Mock<IPropertySerializer>().Object);

      var actual =
          tableSerializer.Serialize(MappingConfiguration.Current.GetTypeDefinition(typeof(ClassWithAllDataTypes))).Single();

      Assert.That(actual.Name.LocalName, Is.EqualTo("table"));
      Assert.That(actual.Attributes().Select(a => a.Name.LocalName), Contains.Item("name"));
      Assert.That(actual.Attribute("name").Value, Is.EqualTo("TableWithAllDataTypes"));
    }

    [Test]
    public void Serialize_CreatesPersistenceModelProvider ()
    {
      var propertySerializerMock = new Mock<IPropertySerializer>();
      var tableSerializer = new TableSerializer(propertySerializerMock.Object);

      propertySerializerMock
          .Setup(_ => _.Serialize(It.IsNotNull<PropertyDefinition>(), It.IsNotNull<IRdbmsPersistenceModelProvider>()))
          .Returns((XElement)null)
          .Verifiable();

      tableSerializer.Serialize(MappingConfiguration.Current.GetClassDefinition(typeof(ClassWithAllDataTypes))).ToArray();
      propertySerializerMock.Verify(
          _ => _.Serialize(It.IsNotNull<PropertyDefinition>(), It.IsNotNull<IRdbmsPersistenceModelProvider>()),
          Times.AtLeastOnce());
    }

    [Test]
    public void Serialize_AddsPropertyElements ()
    {
      var typeDefinition = MappingConfiguration.Current.GetTypeDefinition(typeof(Ceo));
      var propertySerializerStub = new Mock<IPropertySerializer>();
      var expected1 = new XElement("property1");
      var expected2 = new XElement("property2");

      propertySerializerStub
          .Setup(
              _ => _.Serialize(
                  typeDefinition.GetPropertyDefinition("Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SchemaGenerationTestDomain.Ceo.Name"),
                  It.IsAny<IRdbmsPersistenceModelProvider>()))
          .Returns(expected1);
      propertySerializerStub
          .Setup(
              _ => _.Serialize(
                  typeDefinition.GetPropertyDefinition("Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SchemaGenerationTestDomain.Ceo.Company"),
                  It.IsAny<IRdbmsPersistenceModelProvider>()))
          .Returns(expected2);
      var tableSerializer = new TableSerializer(propertySerializerStub.Object);

      var actual = tableSerializer.Serialize(typeDefinition).Single();

      Assert.That(actual.Elements(), Is.EqualTo(new[] { expected1, expected2 }));
    }

    [Test]
    public void Serialize_OnlyAddsPersistentProperties ()
    {
      var typeDefinition = MappingConfiguration.Current.GetTypeDefinition(typeof(Ceo));
      var propertySerializerMock = new Mock<IPropertySerializer>(MockBehavior.Strict);

      propertySerializerMock
          .Setup(
              _ => _.Serialize(
                  It.Is<PropertyDefinition>(p => p.StorageClass == StorageClass.Persistent),
                  It.IsAny<IRdbmsPersistenceModelProvider>()))
          .Returns(new XElement("property"))
          .Verifiable();

      var tableSerializer = new TableSerializer(propertySerializerMock.Object);

      tableSerializer.Serialize(typeDefinition).ToArray();
      propertySerializerMock.Verify();
    }

    [Test]
    public void Serialize_AbstractDerivedClass_CreatesTableElementFromBaseClass ()
    {
      var tableSerializer = new TableSerializer(new Mock<IPropertySerializer>().Object);

      var actual =
          tableSerializer.Serialize(
              MappingConfiguration.Current.GetTypeDefinition(typeof(DerivedAbstractClass))).Single();

      Assert.That(actual.Name.LocalName, Is.EqualTo("table"));
      Assert.That(actual.Attributes().Select(a => a.Name.LocalName), Contains.Item("name"));
      Assert.That(actual.Attribute("name").Value, Is.EqualTo("AbstractClass"));
    }
  }
}
