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
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Resources;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.ExtensibleEnums;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement
{
  [TestFixture]
  public class PropertyValueTest : StandardMappingTest
  {
    private PropertyDefinition _orderNumberPropertyDefinition;

    public override void SetUp ()
    {
      base.SetUp();

      _orderNumberPropertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo("OrderNumber", typeof(int), false);
    }

    [Test]
    public void AreValuesDifferent_False ()
    {
      Assert.That(PropertyValue.AreValuesDifferent(10, 10), Is.False);
      Assert.That(PropertyValue.AreValuesDifferent(null, null), Is.False);
      Assert.That(PropertyValue.AreValuesDifferent("test", "test"), Is.False);

      Assert.That(PropertyValue.AreValuesDifferent(Tuple.Create(100), Tuple.Create(100)), Is.False);
      Assert.That(PropertyValue.AreValuesDifferent(Tuple.Create("Value"), Tuple.Create("Value")), Is.False);

      var array = new byte[] { 1, 2, 3 };
      Assert.That(PropertyValue.AreValuesDifferent(array, array), Is.False);

      var guidValue = Guid.NewGuid();
      Assert.That(
          PropertyValue.AreValuesDifferent(new ObjectID(typeof(Order), guidValue), new ObjectID(typeof(Order), guidValue)),
          Is.False);
    }

    [Test]
    public void AreValuesDifferent_True ()
    {
      Assert.That(PropertyValue.AreValuesDifferent(10, 11), Is.True);
      Assert.That(PropertyValue.AreValuesDifferent("test", "other"), Is.True);
      Assert.That(PropertyValue.AreValuesDifferent("test", null), Is.True);
      Assert.That(PropertyValue.AreValuesDifferent(null, "test"), Is.True);

      Assert.That(PropertyValue.AreValuesDifferent(Tuple.Create(100), Tuple.Create(50)), Is.True);
      Assert.That(PropertyValue.AreValuesDifferent(Tuple.Create("Value"), Tuple.Create("Other")), Is.True);
      Assert.That(PropertyValue.AreValuesDifferent(Tuple.Create(100), null), Is.True);
      Assert.That(PropertyValue.AreValuesDifferent(Tuple.Create("Value"), null), Is.True);
      Assert.That(PropertyValue.AreValuesDifferent(null, Tuple.Create(100)), Is.True);
      Assert.That(PropertyValue.AreValuesDifferent(null, Tuple.Create("Value")), Is.True);

      Assert.That(PropertyValue.AreValuesDifferent(new byte[] { 1, 2, 3 }, new byte[] { 1, 2, 3 }), Is.True);
      Assert.That(PropertyValue.AreValuesDifferent(new byte[] { 1, 2, 3 }, new byte[] { 1, 3, 2 }), Is.True);

      Assert.That(
          PropertyValue.AreValuesDifferent(new ObjectID(typeof(Order), Guid.NewGuid()), new ObjectID(typeof(Order), Guid.NewGuid())),
          Is.True);
    }

    [Test]
    public void PropertyValue_WithReferenceType_NotAllowed ()
    {
      var typeDefinition = TypeDefinitionObjectMother.CreateClassDefinition("ClassName");
      PropertyDefinition propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo(typeDefinition, "test", typeof(List<object>));
      Assert.That(
          () => new PropertyValue(propertyDefinition, null),
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo(
                  @"The property 'test' (declared on type 'Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order') is invalid because its values cannot be copied. "
                  + @"Only value types, strings, the Type type, byte arrays, types implementing IStructuralEquatable, and ObjectIDs are currently supported, "
                  + @"but the property's type is 'System.Collections.Generic.List`1[[" + typeof(object).AssemblyQualifiedName + "]]'."));
    }

    [Test]
    public void PropertyValue_WithValueType_Allowed ()
    {
      PropertyDefinition propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo("test", typeof(DateTime), false);
      Assert.That(() => new PropertyValue(propertyDefinition, DateTime.Now), Throws.Nothing);
    }

    [Test]
    public void PropertyValue_WithString_Allowed ()
    {
      PropertyDefinition propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo("test", typeof(string), true);
      Assert.That(() => new PropertyValue(propertyDefinition, null), Throws.Nothing);
    }

    [Test]
    public void PropertyValue_WithType_Allowed ()
    {
      PropertyDefinition propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo("test", typeof(Type), true);
      Assert.That(() => new PropertyValue(propertyDefinition, null), Throws.Nothing);
    }

    [Test]
    public void PropertyValue_WithExtensibleEnum_Allowed ()
    {
      PropertyDefinition propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo("test", typeof(Color), true);
      Assert.That(() => new PropertyValue(propertyDefinition, null), Throws.Nothing);
    }

    [Test]
    public void PropertyValue_WithStructuralEquatableType_Allowed ()
    {
      PropertyDefinition propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo("test", typeof(Tuple<int>), true);
      Assert.That(() => new PropertyValue(propertyDefinition, null), Throws.Nothing);
    }

    [Test]
    public void IsRelationProperty_False ()
    {
      PropertyDefinition intDefinition = CreateIntPropertyDefinition("test");
      var propertyValue1 = new PropertyValue(intDefinition, 5);
      Assert.That(propertyValue1.IsRelationProperty, Is.False);
    }

    [Test]
    public void IsRelationProperty_True ()
    {
      PropertyDefinition propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo_ObjectID();
      var propertyValue1 = new PropertyValue(propertyDefinition, null);
      Assert.That(propertyValue1.IsRelationProperty, Is.True);
    }

    [Test]
    public void SettingOfValueForValueType ()
    {
      PropertyValue propertyValue = CreateIntPropertyValue("test", 5);

      Assert.AreEqual(5, propertyValue.Value, "Value after initialization");
      Assert.AreEqual(5, propertyValue.OriginalValue, "OriginalValue after initialization");
      Assert.IsFalse(propertyValue.HasChanged, "HasChanged after initialization");
      Assert.IsFalse(propertyValue.HasBeenTouched, "HasBeenTouched after initialization");

      propertyValue.Value = 5;

      Assert.AreEqual(5, propertyValue.Value, "Value after change #1");
      Assert.AreEqual(5, propertyValue.OriginalValue, "OriginalValue after change #1");
      Assert.IsFalse(propertyValue.HasChanged, "HasChanged after change #1");
      Assert.IsTrue(propertyValue.HasBeenTouched, "HasBeenTouched after change #1");

      propertyValue.Value = 10;

      Assert.AreEqual(10, propertyValue.Value, "Value after change #2");
      Assert.AreEqual(5, propertyValue.OriginalValue, "OriginalValue after change #2");
      Assert.IsTrue(propertyValue.HasChanged, "HasChanged after change #2");
      Assert.IsTrue(propertyValue.HasBeenTouched, "HasBeenTouched after change #2");

      propertyValue.Value = 20;

      Assert.AreEqual(20, propertyValue.Value, "Value after change #3");
      Assert.AreEqual(5, propertyValue.OriginalValue, "OriginalValue after change #3");
      Assert.IsTrue(propertyValue.HasChanged, "HasChanged after change #3");
      Assert.IsTrue(propertyValue.HasBeenTouched, "HasBeenTouched after change #3");

      propertyValue.Value = 5;

      Assert.AreEqual(5, propertyValue.Value, "Value after change #4");
      Assert.AreEqual(5, propertyValue.OriginalValue, "OriginalValue after change #4");
      Assert.IsFalse(propertyValue.HasChanged, "HasChanged after change #4");
      Assert.IsTrue(propertyValue.HasBeenTouched, "HasBeenTouched after change #4");
    }

    [Test]
    public void SettingOfNullValueForNullableValueType ()
    {
      PropertyValue propertyValue = CreateNullableIntPropertyValue("test", null);

      Assert.IsNull(propertyValue.Value, "Value after initialization");
      Assert.IsNull(propertyValue.OriginalValue, "OriginalValue after initialization");
      Assert.IsFalse(propertyValue.HasChanged, "HasChanged after initialization");
      Assert.IsFalse(propertyValue.HasBeenTouched, "HasBeenTouched after initialization");

      propertyValue.Value = null;

      Assert.IsNull(propertyValue.Value, "Value after change #1");
      Assert.IsNull(propertyValue.OriginalValue, "OriginalValue after change #1");
      Assert.IsFalse(propertyValue.HasChanged, "HasChanged after change #1");
      Assert.IsTrue(propertyValue.HasBeenTouched, "HasBeenTouched after change #1");

      propertyValue.Value = 10;

      Assert.AreEqual(10, propertyValue.Value, "Value after change #2");
      Assert.IsNull(propertyValue.OriginalValue, "OriginalValue after change #2");
      Assert.IsTrue(propertyValue.HasChanged, "HasChanged after change #2");
      Assert.IsTrue(propertyValue.HasBeenTouched, "HasBeenTouched after change #2");

      propertyValue.Value = null;

      Assert.IsNull(propertyValue.Value, "Value after change #3");
      Assert.IsNull(propertyValue.OriginalValue, "OriginalValue after change #3");
      Assert.IsFalse(propertyValue.HasChanged, "HasChanged after change #3");
      Assert.IsTrue(propertyValue.HasBeenTouched, "HasBeenTouched after change #3");
    }

    [Test]
    public void SettingOfNullValueForString ()
    {
      PropertyValue propertyValue = CreateStringPropertyValue("test", null);

      Assert.IsNull(propertyValue.Value, "Value after initialization");
      Assert.IsNull(propertyValue.OriginalValue, "OriginalValue after initialization");
      Assert.IsFalse(propertyValue.HasChanged, "HasChanged after initialization");
      Assert.IsFalse(propertyValue.HasBeenTouched, "HasBeenTouched after initialization");

      propertyValue.Value = null;

      Assert.IsNull(propertyValue.Value, "Value after change #1");
      Assert.IsNull(propertyValue.OriginalValue, "OriginalValue after change #1");
      Assert.IsFalse(propertyValue.HasChanged, "HasChanged after change #1");
      Assert.IsTrue(propertyValue.HasBeenTouched, "HasBeenTouched after change #1");

      propertyValue.Value = "Test Value";

      Assert.AreEqual("Test Value", propertyValue.Value, "Value after change #2");
      Assert.IsNull(propertyValue.OriginalValue, "OriginalValue after change #2");
      Assert.IsTrue(propertyValue.HasChanged, "HasChanged after change #2");
      Assert.IsTrue(propertyValue.HasBeenTouched, "HasBeenTouched after change #2");

      propertyValue.Value = null;

      Assert.IsNull(propertyValue.Value, "Value after change #3");
      Assert.IsNull(propertyValue.OriginalValue, "OriginalValue after change #3");
      Assert.IsFalse(propertyValue.HasChanged, "HasChanged after change #3");
      Assert.IsTrue(propertyValue.HasBeenTouched, "HasBeenTouched after change #3");
    }

    [Test]
    public void SettingOfNullValueForStructuralEquatableType ()
    {
      PropertyDefinition definition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo("test", typeof(Tuple<string>), true);

      var propertyValue = new PropertyValue(definition, null);

      Assert.IsNull(propertyValue.Value, "Value after initialization");
      Assert.IsNull(propertyValue.OriginalValue, "OriginalValue after initialization");
      Assert.IsFalse(propertyValue.HasChanged, "HasChanged after initialization");
      Assert.IsFalse(propertyValue.HasBeenTouched, "HasBeenTouched after initialization");

      propertyValue.Value = null;

      Assert.IsNull(propertyValue.Value, "Value after change #1");
      Assert.IsNull(propertyValue.OriginalValue, "OriginalValue after change #1");
      Assert.IsFalse(propertyValue.HasChanged, "HasChanged after change #1");
      Assert.IsTrue(propertyValue.HasBeenTouched, "HasBeenTouched after change #1");

      propertyValue.Value = Tuple.Create("Test Value");

      Assert.AreEqual(Tuple.Create("Test Value"), propertyValue.Value, "Value after change #2");
      Assert.IsNull(propertyValue.OriginalValue, "OriginalValue after change #2");
      Assert.IsTrue(propertyValue.HasChanged, "HasChanged after change #2");
      Assert.IsTrue(propertyValue.HasBeenTouched, "HasBeenTouched after change #2");

      propertyValue.Value = null;

      Assert.IsNull(propertyValue.Value, "Value after change #3");
      Assert.IsNull(propertyValue.OriginalValue, "OriginalValue after change #3");
      Assert.IsFalse(propertyValue.HasChanged, "HasChanged after change #3");
      Assert.IsTrue(propertyValue.HasBeenTouched, "HasBeenTouched after change #3");
    }

    [Test]
    public void DoesNotPerformMaxLengthCheck ()
    {
      PropertyDefinition definition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo_MaxLength("test", typeof(string), 10);

      var propertyValue = new PropertyValue(definition, "12345");
      propertyValue.Value = "12345678901";
      Assert.That(propertyValue.Value, Is.EqualTo("12345678901"));
    }

    [Test]
    public void DoesNotPerformMaxLengthCheckInConstructor ()
    {
      PropertyDefinition definition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo_MaxLength("test", typeof(string), 10);

      var propertyValue = new PropertyValue(definition, "12345678901");
      Assert.That(propertyValue.Value, Is.EqualTo("12345678901"));
    }

    [Test]
    public void TypeCheckInConstructor ()
    {
      PropertyDefinition definition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo("test", typeof(string));

      Assert.That(
          () => new PropertyValue(definition, 123),
          Throws.TypeOf<InvalidTypeException>()
              .With.Message.EqualTo("Actual type 'System.Int32' of property 'test' does not match expected type 'System.String'."));
    }

    [Test]
    public void TypeCheck ()
    {
      PropertyDefinition definition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo("test", typeof(string));
      var propertyValue = new PropertyValue(definition, "123");

      Assert.That(
          () => propertyValue.Value = 123,
          Throws.TypeOf<InvalidTypeException>()
              .With.Message.EqualTo("Actual type 'System.Int32' of property 'test' does not match expected type 'System.String'."));
    }

    [Test]
    public void SetNotNullableStringToNull ()
    {
      PropertyDefinition definition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo("test", typeof(string), false);
      var propertyValue = new PropertyValue(definition, string.Empty);

      propertyValue.Value = null;
      Assert.That(propertyValue.Value, Is.Null);
    }

    [Test]
    public void SetNullableBinary ()
    {
      PropertyDefinition definition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo("test", typeof(byte[]), true);

      var propertyValue = new PropertyValue(definition, null);
      Assert.That(propertyValue.Value, Is.Null);
    }

    [Test]
    public void SetNotNullableBinary ()
    {
      PropertyDefinition definition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo("test", typeof(byte[]), false);

      var propertyValue = new PropertyValue(definition, new byte[0]);
      ResourceManager.IsEmptyImage((byte[])propertyValue.Value);

      propertyValue.Value = ResourceManager.GetImage1();
      ResourceManager.IsEqualToImage1((byte[])propertyValue.Value);
    }

    [Test]
    public void SetBinaryWithInvalidType ()
    {
      PropertyDefinition definition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo("test", typeof(byte[]));

      Assert.That(
          () => new PropertyValue(definition, new int[0]),
          Throws.TypeOf<InvalidTypeException>()
              .With.Message.EqualTo("Actual type 'System.Int32[]' of property 'test' does not match expected type 'System.Byte[]'."));
    }

    [Test]
    public void SetNotNullableBinaryToNullViaConstructor ()
    {
      PropertyDefinition definition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo("test", typeof(byte[]), false);

      var propertyValue = new PropertyValue(definition, null);
      Assert.That(propertyValue.Value, Is.Null);
    }

    [Test]
    public void SetNotNullableBinaryToNullViaProperty ()
    {
      PropertyDefinition definition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo("test", typeof(byte[]), false);
      var propertyValue = new PropertyValue(definition, ResourceManager.GetImage1());

      propertyValue.Value = null;
      Assert.That(propertyValue.Value, Is.Null);
    }

    [Test]
    public void SetNullableExtensibleEnum ()
    {
      PropertyDefinition definition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo("test", typeof(Color), true);

      var propertyValue = new PropertyValue(definition, null);
      Assert.That(propertyValue.Value, Is.Null);
    }

    [Test]
    public void SetNotNullableExtensibleEnum ()
    {
      PropertyDefinition definition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo("test", typeof(Color), false);

      var propertyValue = new PropertyValue(definition, ExtensibleEnum<Color>.Values.Red());
      Assert.That(propertyValue.Value, Is.EqualTo(ExtensibleEnum<Color>.Values.Red()));
    }

    [Test]
    public void SetExtensibleEnumWithInvalidType ()
    {
      PropertyDefinition definition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo("test", typeof(Color), false);

      Assert.That(
          () => new PropertyValue(definition, 12),
          Throws.TypeOf<InvalidTypeException>()
              .With.Message.EqualTo("Actual type 'System.Int32' of property 'test' does not match expected type 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Color'."));
    }

    [Test]
    public void SetNotNullableExtensibleEnumToNullViaConstructor ()
    {
      PropertyDefinition definition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo("test", typeof(Color), false);

      var propertyValue = new PropertyValue(definition, null);
      Assert.That(propertyValue.Value, Is.Null);
    }

    [Test]
    public void SetNotNullableExtensibleEnumToNullViaProperty ()
    {
      PropertyDefinition definition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo("test", typeof(Color), false);
      var propertyValue = new PropertyValue(definition, ExtensibleEnum<Color>.Values.Red());

      propertyValue.Value = null;
      Assert.That(propertyValue.Value, Is.Null);
    }

    [Test]
    public void SetNullableStructuralEquatableType ()
    {
      PropertyDefinition definition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo("test", typeof(Tuple<int>), true);

      var propertyValue = new PropertyValue(definition, null);
      Assert.That(propertyValue.Value, Is.Null);
    }

    [Test]
    public void SetNotNullableStructuralEquatableType ()
    {
      PropertyDefinition definition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo("test", typeof(Tuple<int>), false);

      var propertyValue = new PropertyValue(definition, Tuple.Create(50));
      Assert.That(propertyValue.Value, Is.EqualTo(Tuple.Create(50)));
    }

    [Test]
    public void SetStructuralEquatableTypeWithInvalidType ()
    {
      PropertyDefinition definition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo("test", typeof(Tuple<int>), false);

      Assert.That(
          () => new PropertyValue(definition, 12),
          Throws.TypeOf<InvalidTypeException>()
              .With.Message.EqualTo("Actual type 'System.Int32' of property 'test' does not match expected type 'System.Tuple`1[System.Int32]'."));
    }

    [Test]
    public void SetNotNullableStructuralEquatableTypeToNullViaConstructor ()
    {
      PropertyDefinition definition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo("test", typeof(Tuple<int>), false);

      var propertyValue = new PropertyValue(definition, null);
      Assert.That(propertyValue.Value, Is.Null);
    }

    [Test]
    public void SetNotNullableStructuralEquatableTypeToNullViaProperty ()
    {
      PropertyDefinition definition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo("test", typeof(Tuple<int>), false);
      var propertyValue = new PropertyValue(definition, Tuple.Create(50));

      propertyValue.Value = null;
      Assert.That(propertyValue.Value, Is.Null);
    }

    [Test]
    public void SetBinaryLargerThanMaxLength ()
    {
      PropertyDefinition definition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo_MaxLength("test", typeof(byte[]), 1000000);
      var propertyValue = new PropertyValue(definition, new byte[0]);

      byte[] value = ResourceManager.GetImageLarger1MB();
      propertyValue.Value = value;

      Assert.That(propertyValue.Value, Is.SameAs(value));
    }

    [Test]
    public void EnumCheck_ValidNonFlagsEnum ()
    {
      PropertyDefinition definition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo("test", typeof(DayOfWeek));

      var propertyValue = new PropertyValue(definition, DayOfWeek.Monday);
      propertyValue.Value = DayOfWeek.Monday;
      Assert.That(propertyValue.Value, Is.EqualTo(DayOfWeek.Monday));
    }

    [Test]
    public void EnumCheck_InvalidNonFlagsEnum ()
    {
      PropertyDefinition definition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo("test", typeof(DayOfWeek));
      var propertyValue = new PropertyValue(definition, DayOfWeek.Monday);

      Assert.That(
          () => propertyValue.Value = (DayOfWeek)17420,
          Throws.TypeOf<InvalidEnumValueException>()
              .With.Message.EqualTo("Value '17420' for property 'test' is not defined by enum type 'System.DayOfWeek'."));
    }

    [Test]
    public void EnumCheck_ValidFlagsEnum ()
    {
      PropertyDefinition definition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo("test", typeof(AttributeTargets));
      var propertyValue = new PropertyValue(definition, AttributeTargets.Method);

      propertyValue.Value = AttributeTargets.Field | AttributeTargets.Method;
      Assert.That(propertyValue.Value, Is.EqualTo(AttributeTargets.Field | AttributeTargets.Method));
    }

    [Test]
    public void EnumCheck_InvalidFlagsEnum ()
    {
      var definition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo("test", typeof(AttributeTargets));
      var propertyValue = new PropertyValue(definition, AttributeTargets.Method);

      Assert.That(
          () => propertyValue.Value = (AttributeTargets)(-1),
          Throws.TypeOf<InvalidEnumValueException>()
              .With.Message.EqualTo("Value '-1' for property 'test' is not defined by enum type 'System.AttributeTargets'."));
    }

    [Test]
    public void EnumCheck_ValidNullEnum ()
    {
      PropertyDefinition definition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo("test", typeof(DayOfWeek?), true);

      var propertyValue = new PropertyValue(definition, DayOfWeek.Monday);
      propertyValue.Value = DayOfWeek.Monday;
      Assert.That(propertyValue.Value, Is.EqualTo(DayOfWeek.Monday));
      propertyValue.Value = null;
      Assert.That(propertyValue.Value, Is.Null);
    }

    [Test]
    public void EnumCheck_InvalidNullEnum ()
    {
      PropertyDefinition definition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo("test", typeof(DayOfWeek?), true);
      var propertyValue = new PropertyValue(definition, DayOfWeek.Monday);

      Assert.That(
          () => propertyValue.Value = (DayOfWeek)17420,
          Throws.TypeOf<InvalidEnumValueException>()
              .With.Message.EqualTo("Value '17420' for property 'test' is not defined by enum type 'System.DayOfWeek'."));
    }

    [Test]
    public void EnumCheck_InvalidNonNullEnum_Null ()
    {
      PropertyDefinition definition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo("test", typeof(DayOfWeek), false);
      var propertyValue = new PropertyValue(definition, DayOfWeek.Monday);

      Assert.That(
          () => propertyValue.Value = null,
          Throws.InvalidOperationException
              .With.Message.EqualTo("Property 'test' does not allow null values."));
    }

    [Test]
    public void EnumCheckInConstructor ()
    {
      PropertyDefinition definition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo("test", typeof(DayOfWeek));

      Assert.That(
          () => new PropertyValue(definition, (DayOfWeek)17420),
          Throws.TypeOf<InvalidEnumValueException>()
              .With.Message.EqualTo("Value '17420' for property 'test' is not defined by enum type 'System.DayOfWeek'."));
    }

    [Test]
    public void SetRelationPropertyDirectly ()
    {
      PropertyDefinition definition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo_ObjectID();
      var propertyValue = new PropertyValue(definition, null);

      propertyValue.Value = DomainObjectIDs.Customer1;

      Assert.That(propertyValue.Value, Is.EqualTo(DomainObjectIDs.Customer1));
    }

    [Test]
    public void Commit ()
    {
      PropertyValue propertyValue = CreateIntPropertyValue("testProperty", 0);
      propertyValue.Value = 5;
      Assert.That(propertyValue.OriginalValue, Is.EqualTo(0));
      Assert.That(propertyValue.Value, Is.EqualTo(5));
      Assert.That(propertyValue.HasChanged, Is.True);
      Assert.That(propertyValue.HasBeenTouched, Is.True);

      propertyValue.CommitState();

      Assert.That(propertyValue.OriginalValue, Is.EqualTo(5));
      Assert.That(propertyValue.Value, Is.EqualTo(5));
      Assert.That(propertyValue.HasChanged, Is.False);
      Assert.That(propertyValue.HasBeenTouched, Is.False);
    }

    [Test]
    public void Rollback ()
    {
      PropertyValue propertyValue = CreateIntPropertyValue("testProperty", 0);
      propertyValue.Value = 5;
      Assert.That(propertyValue.OriginalValue, Is.EqualTo(0));
      Assert.That(propertyValue.Value, Is.EqualTo(5));
      Assert.That(propertyValue.HasChanged, Is.True);
      Assert.That(propertyValue.HasBeenTouched, Is.True);

      propertyValue.RollbackState();

      Assert.That(propertyValue.OriginalValue, Is.EqualTo(0));
      Assert.That(propertyValue.Value, Is.EqualTo(0));
      Assert.That(propertyValue.HasChanged, Is.False);
      Assert.That(propertyValue.HasBeenTouched, Is.False);
    }

    [Test]
    public void SetDataFromSubTransaction_SetsValue ()
    {
      var source = new PropertyValue(_orderNumberPropertyDefinition, 1);
      var target = new PropertyValue(_orderNumberPropertyDefinition, 2);

      target.SetDataFromSubTransaction(source);

      Assert.That(target.Value, Is.EqualTo(1));
    }

    [Test]
    public void SetDataFromSubTransaction_HasBeenTouched_TrueIfPropertyWasTouched ()
    {
      var source = new PropertyValue(_orderNumberPropertyDefinition, 1);
      var target = new PropertyValue(_orderNumberPropertyDefinition, 1);

      target.Touch();
      Assert.That(source.HasBeenTouched, Is.False);
      Assert.That(target.HasBeenTouched, Is.True);

      target.SetDataFromSubTransaction(source);

      Assert.That(target.HasChanged, Is.False);
      Assert.That(target.HasBeenTouched, Is.True);
    }

    [Test]
    public void SetDataFromSubTransaction_HasBeenTouched_TrueIfSourcePropertyWasTouched ()
    {
      var source = new PropertyValue(_orderNumberPropertyDefinition, 1);
      var target = new PropertyValue(_orderNumberPropertyDefinition, 1);

      source.Touch();
      Assert.That(source.HasBeenTouched, Is.True);
      Assert.That(target.HasBeenTouched, Is.False);

      target.SetDataFromSubTransaction(source);

      Assert.That(target.HasChanged, Is.False);
      Assert.That(target.HasBeenTouched, Is.True);
    }

    [Test]
    public void SetDataFromSubTransaction_HasBeenTouched_TrueIfDataWasChanged ()
    {
      var source = new PropertyValue(_orderNumberPropertyDefinition, 1);
      var target = new PropertyValue(_orderNumberPropertyDefinition, 2);

      Assert.That(source.HasBeenTouched, Is.False);
      Assert.That(target.HasBeenTouched, Is.False);

      target.SetDataFromSubTransaction(source);

      Assert.That(target.HasChanged, Is.True);
      Assert.That(target.HasBeenTouched, Is.True);
    }

    [Test]
    public void SetDataFromSubTransaction_HasBeenTouched_FalseIfNothingHappened ()
    {
      var source = new PropertyValue(_orderNumberPropertyDefinition, 1);
      var target = new PropertyValue(_orderNumberPropertyDefinition, 1);

      Assert.That(source.HasBeenTouched, Is.False);
      Assert.That(target.HasBeenTouched, Is.False);

      target.SetDataFromSubTransaction(source);

      Assert.That(target.HasChanged, Is.False);
      Assert.That(target.HasBeenTouched, Is.False);
    }

    [Test]
    public void SetDataFromSubTransaction_InvalidDefinition ()
    {
      var source = new PropertyValue(_orderNumberPropertyDefinition, 1);
      var target = new PropertyValue(CreateIntPropertyDefinition("test"), 1);

      Assert.That(
          () => target.SetDataFromSubTransaction(source),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo(
              "Cannot set this property's value from 'Remotion.Data.DomainObjects.Mapping.PropertyDefinition: OrderNumber'; the properties "
              + "do not have the same property definition.", "source"));
    }

    [Test]
    [Ignore("TODO 954: Fix this bug! https://www.re-motion.org/jira/browse/RM-954")]
    public void BinaryDataBug ()
    {
      PropertyDefinition definition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo("testProperty2", typeof(byte[]), true);
      var propertyValue = new PropertyValue(definition, new byte[] { 1, 2, 3 });

      ((byte[])propertyValue.Value)[0] = 7;
      Assert.That(propertyValue.HasChanged, Is.True);
      Assert.That(((byte[])propertyValue.Value)[0], Is.EqualTo(7));
      Assert.That(((byte[])propertyValue.OriginalValue)[0], Is.EqualTo(1));

      propertyValue.RollbackState();
      Assert.That(propertyValue.HasChanged, Is.False);
      Assert.That(((byte[])propertyValue.Value)[0], Is.EqualTo(1));

      ((byte[])propertyValue.Value)[0] = 7;
      Assert.That(propertyValue.HasChanged, Is.True);
      Assert.That(((byte[])propertyValue.Value)[0], Is.EqualTo(7));

      propertyValue.CommitState();
      Assert.That(propertyValue.HasChanged, Is.False);
      Assert.That(((byte[])propertyValue.Value)[0], Is.EqualTo(7));
    }

    [Test]
    public void GetValueWithoutEvents_NoEvents ()
    {
      var clientTransactionMock = new TestableClientTransaction();
      using (clientTransactionMock.EnterDiscardingScope())
      {
        var dataContainer = Order.NewObject().InternalDataContainer;
        var propertyDefinition = GetPropertyDefinition(typeof(Order), "OrderNumber");

        ClientTransactionTestHelperWithMocks.EnsureTransactionThrowsOnEvents(clientTransactionMock);

        dataContainer.GetValueWithoutEvents(propertyDefinition, ValueAccess.Current);
      }
    }

    private PropertyValue CreateIntPropertyValue (string name, int intValue)
    {
      return CreatePropertyValue(name, typeof(int), false, intValue);
    }

    private PropertyValue CreateNullableIntPropertyValue (string name, int? intValue)
    {
      return CreatePropertyValue(name, typeof(int?), true, intValue);
    }

    private PropertyValue CreateStringPropertyValue (string name, string stringValue)
    {
      bool isNullable = (stringValue == null) ? true : false;
      return CreatePropertyValue(name, typeof(string), isNullable, stringValue);
    }

    private PropertyDefinition CreateIntPropertyDefinition (string name)
    {
      return PropertyDefinitionObjectMother.CreateForFakePropertyInfo(name, typeof(int), false);
    }

    private PropertyValue CreatePropertyValue (string name, Type propertyType, bool isNullable, object value)
    {
      return new PropertyValue(PropertyDefinitionObjectMother.CreateForFakePropertyInfo(name, propertyType, isNullable), value);
    }
  }
}
