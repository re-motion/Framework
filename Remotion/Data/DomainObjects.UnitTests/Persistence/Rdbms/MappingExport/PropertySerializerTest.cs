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
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.MappingExport;
using Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SchemaGenerationTestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.MappingExport
{
  [TestFixture]
  public class PropertySerializerTest : SchemaGenerationTestBase
  {
    private PropertySerializer _propertySerializer;
    private Mock<IRdbmsPersistenceModelProvider> _rdbmsPersistenceModelProviderStub;
    private Mock<IColumnSerializer> _columnSerializerStub;

    public override void SetUp ()
    {
      base.SetUp();
      _columnSerializerStub = new Mock<IColumnSerializer>();
      _propertySerializer = new PropertySerializer(_columnSerializerStub.Object);
      _rdbmsPersistenceModelProviderStub = new Mock<IRdbmsPersistenceModelProvider>();
    }

    [Test]
    public void Serialize_SerializesName ()
    {
      var sampleProperty = GetPropertyDefinition((ClassWithAllDataTypes _) => _.StringProperty);
      var actual = _propertySerializer.Serialize(sampleProperty, _rdbmsPersistenceModelProviderStub.Object);

      Assert.That(actual.Attributes().Select(a => a.Name.LocalName), Contains.Item("name"));
      Assert.That(
          actual.Attribute("name").Value,
          Is.EqualTo("Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SchemaGenerationTestDomain.ClassWithAllDataTypes.StringProperty"));
    }

    [Test]
    public void Serialize_SerializesDisplayName ()
    {
      var sampleProperty = GetPropertyDefinition((ClassWithAllDataTypes _) => _.StringProperty);
      var actual = _propertySerializer.Serialize(sampleProperty, _rdbmsPersistenceModelProviderStub.Object);

      Assert.That(actual.Attributes().Select(a => a.Name.LocalName), Contains.Item("displayName"));
      Assert.That(actual.Attribute("displayName").Value, Is.EqualTo("StringProperty"));
    }

    [Test]
    public void Serialize_SerializesType_SimpleType ()
    {
      var sampleProperty = GetPropertyDefinition((ClassWithAllDataTypes _) => _.StringProperty);
      var actual = _propertySerializer.Serialize(sampleProperty, _rdbmsPersistenceModelProviderStub.Object);

      Assert.That(actual.Attributes().Select(a => a.Name.LocalName), Contains.Item("type"));
      Assert.That(actual.Attribute("type").Value, Is.EqualTo("System.String"));
    }

    [Test]
    public void Serialize_SerializesType_DomainObjectProperty ()
    {
      var sampleProperty = GetPropertyDefinition((Company _) => _.Address);

      var actual = _propertySerializer.Serialize(sampleProperty, _rdbmsPersistenceModelProviderStub.Object);

      Assert.That(actual.Attributes().Select(a => a.Name.LocalName), Contains.Item("type"));
      Assert.That(
          actual.Attribute("type").Value,
          Is.EqualTo("Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SchemaGenerationTestDomain.Address, Remotion.Data.DomainObjects.UnitTests"));
    }

    [Test]
    public void Serialize_SerializesType_EnumProperty ()
    {
      var sampleProperty = GetPropertyDefinition((ClassWithAllDataTypes _) => _.EnumProperty);

      var actual = _propertySerializer.Serialize(sampleProperty, _rdbmsPersistenceModelProviderStub.Object);

      Assert.That(actual.Attributes().Select(a => a.Name.LocalName), Contains.Item("type"));
      Assert.That(
          actual.Attribute("type").Value,
          Is.EqualTo("Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SchemaGenerationTestDomain.ClassWithAllDataTypes+EnumType, Remotion.Data.DomainObjects.UnitTests"));
    }

    [Test]
    public void Serialize_SerializesType_ExtensibleEnumProperty ()
    {
      var sampleProperty = GetPropertyDefinition((ClassWithAllDataTypes _) => _.ExtensibleEnumProperty);

      var actual = _propertySerializer.Serialize(sampleProperty, _rdbmsPersistenceModelProviderStub.Object);

      Assert.That(actual.Attributes().Select(a => a.Name.LocalName), Contains.Item("type"));
      Assert.That(
          actual.Attribute("type").Value,
          Is.EqualTo("Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SchemaGenerationTestDomain.Color, Remotion.Data.DomainObjects.UnitTests"));
    }

    [Test]
    public void Serialize_AddsIsNullableAttribute ()
    {
      var sampleProperty = GetPropertyDefinition((ClassWithAllDataTypes _) => _.NaByteProperty);
      var actual = _propertySerializer.Serialize(sampleProperty, _rdbmsPersistenceModelProviderStub.Object);

      Assert.That(actual.Attributes().Select(a => a.Name.LocalName), Contains.Item("isNullable"));
      Assert.That(actual.Attribute("isNullable").Value, Is.EqualTo("true"));
    }

    [Test]
    public void Serialize_AddsIsNullableAttributeToNotNullableType ()
    {
      var sampleProperty = GetPropertyDefinition((ClassWithAllDataTypes _) => _.ByteProperty);
      var actual = _propertySerializer.Serialize(sampleProperty, _rdbmsPersistenceModelProviderStub.Object);

      Assert.That(actual.Attributes().Select(a => a.Name.LocalName), Contains.Item("isNullable"));
      Assert.That(actual.Attribute("isNullable").Value, Is.EqualTo("false"));
    }

    [Test]
    public void Serialize_SerializesType_NullableProperty ()
    {
      var sampleProperty = GetPropertyDefinition((ClassWithAllDataTypes _) => _.NaDateProperty);
      var actual = _propertySerializer.Serialize(sampleProperty, _rdbmsPersistenceModelProviderStub.Object);

      Assert.That(actual.Attributes().Select(a => a.Name.LocalName), Contains.Item("type"));
      Assert.That(actual.Attribute("type").Value, Is.EqualTo("System.DateOnly"));
    }

    [Test]
    public void Serialize_AddsMaxLengthAttribute ()
    {
      var sampleProperty = GetPropertyDefinition((ClassWithAllDataTypes _) => _.StringProperty);
      var actual = _propertySerializer.Serialize(sampleProperty, _rdbmsPersistenceModelProviderStub.Object);

      Assert.That(actual.Attributes().Select(a => a.Name.LocalName), Contains.Item("maxLength"));
      Assert.That(actual.Attribute("maxLength").Value, Is.EqualTo("100"));
    }

    [Test]
    public void Serialize_StringPropertyWithoutMaxLengthConstraint ()
    {
      var sampleProperty = GetPropertyDefinition((ClassWithAllDataTypes _) => _.StringPropertyWithoutMaxLength);
      var actual = _propertySerializer.Serialize(sampleProperty, _rdbmsPersistenceModelProviderStub.Object);
      Assert.That(actual.Attributes().Select(a => a.Name.LocalName).Contains("maxLength"), Is.False);
    }

    [Test]
    public void Serialize_AddsColumnElements ()
    {
      var sampleProperty = GetPropertyDefinition((ClassWithAllDataTypes _) => _.StringProperty);
      _columnSerializerStub
          .Setup(_ => _.Serialize(sampleProperty, _rdbmsPersistenceModelProviderStub.Object))
          .Returns(new[] { new XElement("column1"), new XElement("column2") });

      var actual = _propertySerializer.Serialize(sampleProperty, _rdbmsPersistenceModelProviderStub.Object);

      Assert.That(actual.Elements().Count(), Is.EqualTo(2));
      Assert.That(actual.Elements().ElementAt(0).Name.LocalName, Is.EqualTo("column1"));
      Assert.That(actual.Elements().ElementAt(1).Name.LocalName, Is.EqualTo("column2"));
    }
  }
}
