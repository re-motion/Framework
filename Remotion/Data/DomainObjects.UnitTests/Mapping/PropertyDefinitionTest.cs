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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration;
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping
{
  [TestFixture]
  public class PropertyDefinitionTest : StandardMappingTest
  {
    private ClassDefinition _classDefinition;
    private Mock<IPropertyInformation> _propertyInformationStub;

    public override void SetUp ()
    {
      base.SetUp();

      _classDefinition = ClassDefinitionObjectMother.CreateClassDefinition();
      _propertyInformationStub = new Mock<IPropertyInformation>();
      _propertyInformationStub.Setup(stub => stub.Name).Returns("Test");
      _propertyInformationStub.Setup(stub => stub.DeclaringType).Returns(TypeAdapter.Create(typeof(Order)));
      _propertyInformationStub.Setup(stub => stub.GetOriginalDeclaringType()).Returns(TypeAdapter.Create(typeof(Order)));
    }

    [Test]
    public void Initialize ()
    {
      _propertyInformationStub.Setup(stub => stub.PropertyType).Returns(typeof(string));
      var actual = new PropertyDefinition(
          _classDefinition, _propertyInformationStub.Object, "Test", false, false, null, StorageClass.Persistent);

      Assert.That(actual.ClassDefinition, Is.SameAs(_classDefinition));
      Assert.That(actual.IsNullable, Is.False);
      Assert.That(actual.MaxLength, Is.Null);
      Assert.That(actual.PropertyName, Is.EqualTo("Test"));
      Assert.That(actual.PropertyType, Is.EqualTo(typeof(string)));
      Assert.That(actual.StorageClass, Is.EqualTo(StorageClass.Persistent));
      Assert.That(actual.IsObjectID, Is.False);
      Assert.That(actual.PropertyInfo, Is.SameAs(_propertyInformationStub.Object));
    }

    [Test]
    public void Initialize_WithReferenceType_IsNullableFalse ()
    {
      _propertyInformationStub.Setup(stub => stub.PropertyType).Returns(typeof(string));
      var actual = new PropertyDefinition(
          _classDefinition, _propertyInformationStub.Object, "Test", false, false, null, StorageClass.Persistent);

      Assert.That(actual.IsNullable, Is.False);
    }

    [Test]
    public void Initialize_WithReferenceType_IsNullableTrue ()
    {
      _propertyInformationStub.Setup(stub => stub.PropertyType).Returns(typeof(string));
      var actual = new PropertyDefinition(
          _classDefinition, _propertyInformationStub.Object, "Test", false, true, null, StorageClass.Persistent);

      Assert.That(actual.IsNullable, Is.True);
    }

    [Test]
    public void Initialize_WithNonNullableValueType_IsNullableFalse ()
    {
      _propertyInformationStub.Setup(stub => stub.PropertyType).Returns(typeof(int));
      var actual = new PropertyDefinition(
          _classDefinition, _propertyInformationStub.Object, "Test", false, false, null, StorageClass.Persistent);

      Assert.That(actual.IsNullable, Is.False);
    }

    [Test]
    public void Initialize_WithNullableValueType_IsNullableFalse ()
    {
      _propertyInformationStub.Setup(stub => stub.PropertyType).Returns(typeof(int?));
      var actual = new PropertyDefinition(
          _classDefinition, _propertyInformationStub.Object, "Test", false, false, null, StorageClass.Persistent);

      Assert.That(actual.IsNullable, Is.False);
    }

    [Test]
    public void Initialize_WithNullableValueType_IsNullableTrue ()
    {
      _propertyInformationStub.Setup(stub => stub.PropertyType).Returns(typeof(int?));
      var actual = new PropertyDefinition(
          _classDefinition, _propertyInformationStub.Object, "Test", false, true, null, StorageClass.Persistent);

      Assert.That(actual.IsNullable, Is.True);
    }

    [Test]
    public void Initialize_WithString_MaxLengthSet ()
    {
      _propertyInformationStub.Setup(stub => stub.PropertyType).Returns(typeof(string));
      var actual = new PropertyDefinition(
          _classDefinition, _propertyInformationStub.Object, "Test", false, true, 50, StorageClass.Persistent);

      Assert.That(actual.MaxLength, Is.EqualTo(50));
    }

    [Test]
    public void Initialize_WithString_MaxLengthNotSet ()
    {
      _propertyInformationStub.Setup(stub => stub.PropertyType).Returns(typeof(string));
      var actual = new PropertyDefinition(
          _classDefinition, _propertyInformationStub.Object, "Test", false, true, null, StorageClass.Persistent);

      Assert.That(actual.MaxLength, Is.Null);
    }

    [Test]
    public void Initialize_WithByteArray_MaxLengthSet ()
    {
      _propertyInformationStub.Setup(stub => stub.PropertyType).Returns(typeof(byte[]));
      var actual = new PropertyDefinition(
          _classDefinition, _propertyInformationStub.Object, "Test", false, true, 50, StorageClass.Persistent);

      Assert.That(actual.MaxLength, Is.EqualTo(50));
    }

    [Test]
    public void Initialize_WithByteArrayString_MaxLengthNotSet ()
    {
      _propertyInformationStub.Setup(stub => stub.PropertyType).Returns(typeof(byte[]));
      var actual = new PropertyDefinition(
          _classDefinition, _propertyInformationStub.Object, "Test", false, true, null, StorageClass.Persistent);

      Assert.That(actual.MaxLength, Is.Null);
    }

    [Test]
    public void Initialize_WithOtherType_MaxLengthNotSet ()
    {
      _propertyInformationStub.Setup(stub => stub.PropertyType).Returns(typeof(int));
      var actual = new PropertyDefinition(
          _classDefinition, _propertyInformationStub.Object, "Test", false, false, null, StorageClass.Persistent);

      Assert.That(actual.MaxLength, Is.Null);
    }

    [Test]
    public void IsObjectID_True ()
    {
      _propertyInformationStub.Setup(stub => stub.PropertyType).Returns(typeof(DomainObject));
      var actual = new PropertyDefinition(
          _classDefinition, _propertyInformationStub.Object, "Test", true, false, null, StorageClass.Persistent);
      Assert.That(actual.IsObjectID, Is.True);
    }

    [Test]
    public void IsObjectID_False ()
    {
      _propertyInformationStub.Setup(stub => stub.PropertyType).Returns(typeof(int));
      var actual = new PropertyDefinition(
          _classDefinition, _propertyInformationStub.Object, "Test", false, false, null, StorageClass.Persistent);
      Assert.That(actual.IsObjectID, Is.False);
    }

    [Test]
    public new void ToString ()
    {
      PropertyDefinition propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo(_classDefinition, "ThePropertyName");

      Assert.That(propertyDefinition.ToString(), Is.EqualTo(typeof(PropertyDefinition).FullName + ": ThePropertyName"));
    }

    [Test]
    public void SetStorageProperty ()
    {
      PropertyDefinition propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo(_classDefinition);
      var storagePropertyDefinition = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty("Test");

      propertyDefinition.SetStorageProperty(storagePropertyDefinition);

      Assert.That(propertyDefinition.StoragePropertyDefinition, Is.SameAs(storagePropertyDefinition));
    }
  }
}
