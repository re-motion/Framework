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
using Remotion.Data.DomainObjects.Persistence.Rdbms.MappingExport;
using Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SchemaGenerationTestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.MappingExport
{
  [TestFixture]
  public class TypeSerializerTest : SchemaGenerationTestBase
  {

    public override void SetUp ()
    {
      base.SetUp();
    }

    [Test]
    public void Serialize_AddsIdAttribute ()
    {
      var typeSerializer = new TypeSerializer(new Mock<ITableSerializer>().Object);
      var classDefinition = MappingConfiguration.Current.GetTypeDefinition(typeof(ClassWithAllDataTypes));
      var actual = typeSerializer.Serialize(classDefinition);

      Assert.That(actual.Name.LocalName, Is.EqualTo("class"));
      Assert.That(actual.Attributes().Select(a => a.Name.LocalName), Contains.Item("id"));
      Assert.That(actual.Attribute("id").Value, Is.EqualTo("ClassWithAllDataTypes"));
    }

    [Test]
    public void Serialize_AddsBaseClassAttribute ()
    {
      var typeSerializer = new TypeSerializer(new Mock<ITableSerializer>().Object);
      var classDefinition = MappingConfiguration.Current.GetClassDefinition(typeof(DerivedAbstractClass));
      var actual = typeSerializer.Serialize(classDefinition);

      Assert.That(actual.Attributes().Select(a => a.Name.LocalName), Contains.Item("baseClass"));
      Assert.That(actual.Attribute("baseClass").Value, Is.EqualTo("AbstractClass"));
    }

    [Test]
    public void Serialize_ClassHasNoBaseClass_DoesNotAddAttribute ()
    {
      var typeSerializer = new TypeSerializer(new Mock<ITableSerializer>().Object);
      var classDefinition = MappingConfiguration.Current.GetClassDefinition(typeof(Ceo));
      var actual = typeSerializer.Serialize(classDefinition);

      Assert.That(actual.Attributes().Select(a => a.Name.LocalName).Contains("baseClass"), Is.False);
    }

    [Test]
    public void Serialize_AddsAbstractAttribute ()
    {
      var typeSerializer = new TypeSerializer(new Mock<ITableSerializer>().Object);
      var classDefinition = MappingConfiguration.Current.GetClassDefinition(typeof(Ceo));
      var actual = typeSerializer.Serialize(classDefinition);

      Assert.That(actual.Attributes().Select(a => a.Name.LocalName), Contains.Item("isAbstract"));
      Assert.That(actual.Attribute("isAbstract").Value, Is.EqualTo("false"));
    }

    [Test]
    public void Serialize_AddsAbstractAttribute_AbstractClass ()
    {
      var typeSerializer = new TypeSerializer(new Mock<ITableSerializer>().Object);
      var classDefinition = MappingConfiguration.Current.GetClassDefinition(typeof(DerivedAbstractClass));
      var actual = typeSerializer.Serialize(classDefinition);

      Assert.That(actual.Attributes().Select(a => a.Name.LocalName), Contains.Item("isAbstract"));
      Assert.That(actual.Attribute("isAbstract").Value, Is.EqualTo("true"));
    }

    [Test]
    public void Serialize_DoesNotExportTablesForAbstractNonInstantiableClasses ()
    {
      var tableSerializerStub = new Mock<ITableSerializer>();
      var typeSerializer = new TypeSerializer(tableSerializerStub.Object);
      var classDefinition = MappingConfiguration.Current.GetClassDefinition(typeof(AbstractWithoutConcreteClass));
      typeSerializer.Serialize(classDefinition);

      tableSerializerStub.Verify(_ => _.Serialize(classDefinition), Times.Never());
    }

    [Test]
    public void Serialize_AddsTableElements ()
    {
      var classDefinition = MappingConfiguration.Current.GetClassDefinition(typeof(Ceo));
      var tableSerializerMock = new Mock<ITableSerializer>();
      var expected1 = new XElement("property1");

      tableSerializerMock.Setup(_ => _.Serialize(classDefinition)).Returns(new [] { expected1 }).Verifiable();
      var typeSerializer = new TypeSerializer(tableSerializerMock.Object);

      var actual = typeSerializer.Serialize(classDefinition);
      tableSerializerMock.Verify();

      Assert.That(actual.Elements().Count(), Is.EqualTo(1));
      Assert.That(actual.Elements(), Contains.Item(expected1));
    }
  }
}
