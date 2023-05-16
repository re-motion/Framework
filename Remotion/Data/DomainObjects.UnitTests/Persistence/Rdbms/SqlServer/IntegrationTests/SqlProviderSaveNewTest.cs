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
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Resources;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.IntegrationTests
{
  [TestFixture]
  public class SqlProviderSaveNewTest : SqlProviderBaseTest
  {
    [Test]
    public void NewDataContainer ()
    {
      DataContainer newDataContainer = CreateNewDataContainer(typeof(Computer));
      SetPropertyValue(newDataContainer, typeof(Computer), "SerialNumber", "123");

      ObjectID newID = newDataContainer.ID;

      Provider.Save(new[] { newDataContainer });

      DataContainer reloadedDataContainer = ReloadDataContainer(newID);

      Assert.That(reloadedDataContainer, Is.Not.Null);
      Assert.That(reloadedDataContainer.ID, Is.EqualTo(newDataContainer.ID));
    }

    [Test]
    public void AllDataTypes ()
    {
      DataContainer classWithAllDataTypes = CreateNewDataContainer(typeof(ClassWithAllDataTypes));
      ObjectID newID = classWithAllDataTypes.ID;

      SetPropertyValue(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "BooleanProperty", true);
      SetPropertyValue(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "ByteProperty", (byte)42);
      SetPropertyValue(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "DateProperty", new DateTime(1974, 10, 25));
      SetPropertyValue(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "DateTimeProperty", new DateTime(
          1974, 10, 26, 18, 9, 18));
      SetPropertyValue(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "DecimalProperty", (decimal)564.956);
      SetPropertyValue(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "DoubleProperty", 5334.2456);
      SetPropertyValue(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "EnumProperty",
          ClassWithAllDataTypes.EnumType.Value0);
      SetPropertyValue(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "ExtensibleEnumProperty", Color.Values.Green());
      SetPropertyValue(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "GuidProperty",
          new Guid("{98E0FE88-7DB4-4f6c-A1C1-86682D5C95C9}"));
      SetPropertyValue(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "Int16Property", (short)67);
      SetPropertyValue(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "Int32Property", 42424242);
      SetPropertyValue(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "Int64Property", 424242424242424242);
      SetPropertyValue(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "SingleProperty", (float)42.42);
      SetPropertyValue(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "StringProperty", "zyxwvuZaphodBeeblebrox");
      SetPropertyValue(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "StringPropertyWithoutMaxLength",
          "123450987612345098761234509876123450987612345098761234509876123450987612345098761234509876123450987612345098761234509876123450987612345098761234509876");
      SetPropertyValue(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "BinaryProperty", ResourceManager.GetImage1());

      SetPropertyValue(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaBooleanProperty", false);
      SetPropertyValue(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaByteProperty", (byte)21);
      SetPropertyValue(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaDateProperty", new DateTime(2007, 1, 18));
      SetPropertyValue(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaDateTimeProperty", new DateTime(
          2005, 1, 18, 11, 11, 11));
      SetPropertyValue(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaDecimalProperty", 50m);
      SetPropertyValue(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaDoubleProperty", 56.87d);
      SetPropertyValue(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaEnumProperty",
          ClassWithAllDataTypes.EnumType.Value1);
      SetPropertyValue(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaGuidProperty",
          new Guid("{19B2DFBE-B7BB-448e-8002-F4DBF6032AE8}"));
      SetPropertyValue(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaInt16Property", (short)51);
      SetPropertyValue(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaInt32Property", 52);
      SetPropertyValue(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaInt64Property", 53L);
      SetPropertyValue(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaSingleProperty", 54F);

      Provider.Save(new[] { classWithAllDataTypes });

      var reloadedClassWithAllDataTypes = ReloadDataContainer(newID);

      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "BooleanProperty"), Is.EqualTo(true));
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "ByteProperty"), Is.EqualTo(42));
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "DateProperty"), Is.EqualTo(new DateTime(1974, 10, 25)));
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "DateTimeProperty"), Is.EqualTo(new DateTime(1974, 10, 26, 18, 9, 18)));
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "DecimalProperty"), Is.EqualTo(564.956));
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "DoubleProperty"), Is.EqualTo(5334.2456d));
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "EnumProperty"), Is.EqualTo(ClassWithAllDataTypes.EnumType.Value0));
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "ExtensibleEnumProperty"), Is.EqualTo(Color.Values.Green()));
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "GuidProperty"), Is.EqualTo(new Guid("{98E0FE88-7DB4-4f6c-A1C1-86682D5C95C9}")));
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "Int16Property"), Is.EqualTo(67));
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "Int32Property"), Is.EqualTo(42424242));
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "Int64Property"), Is.EqualTo(424242424242424242));
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "SingleProperty"), Is.EqualTo(42.42f));
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "StringProperty"), Is.EqualTo("zyxwvuZaphodBeeblebrox"));
      Assert.That(
          GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "StringPropertyWithoutMaxLength"),
          Is.EqualTo("123450987612345098761234509876123450987612345098761234509876123450987612345098761234509876123450987612345098761234509876123450987612345098761234509876"));
      ResourceManager.IsEqualToImage1(
          (byte[])GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "BinaryProperty"));

      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaBooleanProperty"), Is.EqualTo(false));
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaByteProperty"), Is.EqualTo(21));
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaDateProperty"), Is.EqualTo(new DateTime(2007, 1, 18)));
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaDateTimeProperty"), Is.EqualTo(new DateTime(2005, 1, 18, 11, 11, 11)));
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaDecimalProperty"), Is.EqualTo(50m));
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaDoubleProperty"), Is.EqualTo(56.87d));
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaEnumProperty"), Is.EqualTo(ClassWithAllDataTypes.EnumType.Value1));
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaGuidProperty"), Is.EqualTo(new Guid("{19B2DFBE-B7BB-448e-8002-F4DBF6032AE8}")));
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaInt16Property"), Is.EqualTo(51));
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaInt32Property"), Is.EqualTo(52));
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaInt64Property"), Is.EqualTo(53));
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaSingleProperty"), Is.EqualTo(54F));

      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaBooleanWithNullValueProperty"), Is.Null);
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaByteWithNullValueProperty"), Is.Null);
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaDateWithNullValueProperty"), Is.Null);
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaDateTimeWithNullValueProperty"), Is.Null);
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaDecimalWithNullValueProperty"), Is.Null);
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaDoubleWithNullValueProperty"), Is.Null);
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaEnumWithNullValueProperty"), Is.Null);
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaGuidWithNullValueProperty"), Is.Null);
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaInt16WithNullValueProperty"), Is.Null);
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaInt32WithNullValueProperty"), Is.Null);
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaInt64WithNullValueProperty"), Is.Null);
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaSingleWithNullValueProperty"), Is.Null);
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "StringWithNullValueProperty"), Is.Null);
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "ExtensibleEnumWithNullValueProperty"), Is.Null);
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NullableBinaryProperty"), Is.Null);
    }

    [Test]
    public void AllDataTypes_DefaultValues ()
    {
      DataContainer classWithAllDataTypes = CreateNewDataContainer(typeof(ClassWithAllDataTypes));
      ObjectID newID = classWithAllDataTypes.ID;

      SetPropertyValue(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "BinaryProperty", Array.Empty<byte>());
      SetPropertyValue(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "DateProperty", new DateTime(1753, 1, 1));
      SetPropertyValue(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "DateTimeProperty", new DateTime(
          1753, 1, 1, 0, 0, 0));
      SetPropertyValue(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "DecimalProperty", 0m);
      SetPropertyValue(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "ExtensibleEnumProperty", Color.Values.Blue());
      SetPropertyValue(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "StringProperty", "Test");
      SetPropertyValue(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "StringPropertyWithoutMaxLength", "NoMaxLength");

      Provider.Save(new[] { classWithAllDataTypes });

      var reloadedClassWithAllDataTypes = ReloadDataContainer(newID);

      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "BooleanProperty"), Is.EqualTo(false));
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "ByteProperty"), Is.EqualTo((byte)0));
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "DateProperty"), Is.EqualTo(new DateTime(1753, 1, 1)));
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "DateTimeProperty"), Is.EqualTo(new DateTime(1753, 1, 1, 0, 0, 0)));
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "DoubleProperty"), Is.EqualTo(0d));
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "EnumProperty"), Is.EqualTo(ClassWithAllDataTypes.EnumType.Value0));
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "ExtensibleEnumProperty"), Is.EqualTo(Color.Values.Blue()));
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "GuidProperty"), Is.EqualTo(Guid.Empty));
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "Int16Property"), Is.EqualTo((short)0));
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "Int32Property"), Is.EqualTo(0));
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "Int64Property"), Is.EqualTo(0L));
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "SingleProperty"), Is.EqualTo(0F));
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "StringProperty"), Is.EqualTo("Test"));
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "StringPropertyWithoutMaxLength"), Is.EqualTo("NoMaxLength"));
      ResourceManager.IsEmptyImage(
          (byte[])GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "BinaryProperty"));

      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaBooleanProperty"), Is.Null);
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaByteProperty"), Is.Null);
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaDateProperty"), Is.Null);
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaDateTimeProperty"), Is.Null);
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaDecimalProperty"), Is.Null);
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaDoubleProperty"), Is.Null);
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaEnumProperty"), Is.Null);
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaGuidProperty"), Is.Null);
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaInt16Property"), Is.Null);
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaInt32Property"), Is.Null);
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaInt64Property"), Is.Null);
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaSingleProperty"), Is.Null);

      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaBooleanWithNullValueProperty"), Is.Null);
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaByteWithNullValueProperty"), Is.Null);
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaDateWithNullValueProperty"), Is.Null);
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaDateTimeWithNullValueProperty"), Is.Null);
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaDecimalWithNullValueProperty"), Is.Null);
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaDoubleWithNullValueProperty"), Is.Null);
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaEnumWithNullValueProperty"), Is.Null);
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaGuidWithNullValueProperty"), Is.Null);
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaInt16WithNullValueProperty"), Is.Null);
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaInt32WithNullValueProperty"), Is.Null);
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaInt64WithNullValueProperty"), Is.Null);
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaSingleWithNullValueProperty"), Is.Null);
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "StringWithNullValueProperty"), Is.Null);
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "ExtensibleEnumWithNullValueProperty"), Is.Null);
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NullableBinaryProperty"), Is.Null);
    }

    [Test]
    public void ExistingObjectRelatesToNew ()
    {
      var newDataContainer = CreateNewDataContainer(typeof(Employee));
      var existingDataContainer = LoadDataContainer(DomainObjectIDs.Employee1);

      newDataContainer.SetValue(GetPropertyDefinition(typeof(Employee), "Name"), "Supervisor");
      var supervisorProperty = GetPropertyDefinition(typeof(Employee), "Supervisor");
      existingDataContainer.SetValue(supervisorProperty, newDataContainer.ID);

      Provider.Save(new[] { newDataContainer, existingDataContainer });

      DataContainer newSupervisorContainer = ReloadDataContainer(newDataContainer.ID);
      DataContainer existingSubordinateContainer = ReloadDataContainer(existingDataContainer.ID);

      Assert.That(newSupervisorContainer, Is.Not.Null);
      Assert.That(existingSubordinateContainer.GetValue(supervisorProperty), Is.EqualTo(newSupervisorContainer.ID));
    }

    [Test]
    public void NewObjectRelatesToExisting ()
    {
      var newDataContainer = CreateNewDataContainer(typeof(Order));
      var deliveryDateProperty = GetPropertyDefinition(typeof(Order), "DeliveryDate");
      var customerProperty = GetPropertyDefinition(typeof(Order), "Customer");
      var officialProperty = GetPropertyDefinition(typeof(Order), "Official");
      newDataContainer.SetValue(deliveryDateProperty, new DateTime(2005, 12, 24));
      newDataContainer.SetValue(customerProperty, DomainObjectIDs.Customer1);
      newDataContainer.SetValue(officialProperty, DomainObjectIDs.Official1);

      Provider.Save(new[] { newDataContainer });

      DataContainer loadedDataContainer = ReloadDataContainer(newDataContainer.ID);

      Assert.That(loadedDataContainer, Is.Not.Null);
      Assert.That(loadedDataContainer.GetValue(customerProperty), Is.EqualTo(DomainObjectIDs.Customer1));
      Assert.That(loadedDataContainer.GetValue(officialProperty), Is.EqualTo(DomainObjectIDs.Official1));
    }

    [Test]
    public void NewRelatedObjects ()
    {
      var newCustomerDataContainer = CreateNewDataContainer(typeof(Customer));
      SetPropertyValue(newCustomerDataContainer, typeof(Company), "Name", "MyCustomer");
      var newOrderDataContainer = CreateNewDataContainer(typeof(Order));

      var deliveryDateProperty = GetPropertyDefinition(typeof(Order), "DeliveryDate");
      var customerProperty = GetPropertyDefinition(typeof(Order), "Customer");
      newOrderDataContainer.SetValue(deliveryDateProperty, new DateTime(2005, 12, 24));
      newOrderDataContainer.SetValue(customerProperty, newCustomerDataContainer.ID);

      Provider.Save(new[] { newOrderDataContainer, newCustomerDataContainer });

      DataContainer reloadedCustomerContainer = ReloadDataContainer(newCustomerDataContainer.ID);
      DataContainer reloadedOrderContainer = ReloadDataContainer(newOrderDataContainer.ID);

      Assert.That(reloadedCustomerContainer, Is.Not.Null);
      Assert.That(reloadedOrderContainer, Is.Not.Null);
      Assert.That(reloadedOrderContainer.GetValue(customerProperty), Is.EqualTo(reloadedCustomerContainer.ID));
    }

    [Test]
    public void SaveNullBinary ()
    {
      DataContainer dataContainer = CreateNewDataContainer(typeof(ClassWithAllDataTypes));
      ObjectID newID = dataContainer.ID;

      SetDefaultValues(dataContainer);
      var propertyDefinition = GetPropertyDefinition(typeof(ClassWithAllDataTypes), "NullableBinaryProperty");
      dataContainer.SetValue(propertyDefinition, null);

      Provider.Save(new[] { dataContainer });

      DataContainer reloadedDataContainer = ReloadDataContainer(newID);
      Assert.That(reloadedDataContainer.GetValue(propertyDefinition), Is.Null);
    }

    [Test]
    public void SaveEmptyBinary ()
    {
      DataContainer dataContainer = CreateNewDataContainer(typeof(ClassWithAllDataTypes));
      ObjectID newID = dataContainer.ID;

      SetDefaultValues(dataContainer);
      var propertyDefinition = GetPropertyDefinition(typeof(ClassWithAllDataTypes), "NullableBinaryProperty");
      dataContainer.SetValue(propertyDefinition, new byte[0]);

      Provider.Save(new[] { dataContainer });

      DataContainer reloadedDataContainer = ReloadDataContainer(newID);
      ResourceManager.IsEmptyImage((byte[])reloadedDataContainer.GetValue(propertyDefinition));
    }

    [Test]
    public void SaveLargeBinary ()
    {
      DataContainer dataContainer = CreateNewDataContainer(typeof(ClassWithAllDataTypes));
      ObjectID newID = dataContainer.ID;

      SetDefaultValues(dataContainer);
      var propertyDefinition = GetPropertyDefinition(typeof(ClassWithAllDataTypes), "BinaryProperty");
      dataContainer.SetValue(propertyDefinition, ResourceManager.GetImageLarger1MB());

      Provider.Save(new[] { dataContainer });

      DataContainer reloadedDataContainer = ReloadDataContainer(newID);
      ResourceManager.IsEqualToImageLarger1MB((byte[])reloadedDataContainer.GetValue(propertyDefinition));
    }

    [Test]
    public void SaveLongString ()
    {
      DataContainer dataContainer = CreateNewDataContainer(typeof(ClassWithAllDataTypes));
      ObjectID newID = dataContainer.ID;
      var stringValue = new string('b', 4001);

      SetDefaultValues(dataContainer);
      var propertyDefinition = GetPropertyDefinition(typeof(ClassWithAllDataTypes), "StringPropertyWithoutMaxLength");
      dataContainer.SetValue(propertyDefinition, stringValue);

      Provider.Save(new[] { dataContainer });

      DataContainer reloadedDataContainer = ReloadDataContainer(newID);
      Assert.That(reloadedDataContainer.GetValue(propertyDefinition), Is.EqualTo(stringValue));
    }

    [Test]
    public void SaveEmpty ()
    {
      Provider.Save(new DataContainer[0]);
    }

    private void SetDefaultValues (DataContainer classWithAllDataTypesContainer)
    {
      // Note: Date properties must be set, because SQL Server only accepts dates past 1/1/1753.
      SetPropertyValue(classWithAllDataTypesContainer, typeof(ClassWithAllDataTypes), "DateProperty", DateTime.Now);
      SetPropertyValue(classWithAllDataTypesContainer, typeof(ClassWithAllDataTypes), "DateTimeProperty", DateTime.Now);

      // Note: SqlDecimal has problems with Decimal.MinValue => Set this property too.
      SetPropertyValue(classWithAllDataTypesContainer, typeof(ClassWithAllDataTypes), "DecimalProperty", 10m);

      // Note: PropertyDefaultValueProvider returns null for these properties, which cannot be saved
      SetPropertyValue(classWithAllDataTypesContainer, typeof(ClassWithAllDataTypes), "BinaryProperty", Array.Empty<byte>());
      SetPropertyValue(classWithAllDataTypesContainer, typeof(ClassWithAllDataTypes), "ExtensibleEnumProperty", Color.Values.Blue());
      SetPropertyValue(classWithAllDataTypesContainer, typeof(ClassWithAllDataTypes), "StringProperty", "Test");
      SetPropertyValue(classWithAllDataTypesContainer, typeof(ClassWithAllDataTypes), "StringPropertyWithoutMaxLength", "NoMaxLength");
    }

    private DataContainer LoadDataContainer (ObjectID id)
    {
      return Provider.LoadDataContainer(id).LocatedObject;
    }

    private DataContainer LoadDataContainerWithSeparateProvider (ObjectID id)
    {
      using (var separateProvider = CreateRdbmsProvider())
      {
        return separateProvider.LoadDataContainer(id).LocatedObject;
      }
    }

    private DataContainer ReloadDataContainer (ObjectID id)
    {
      Provider.Disconnect();

      return LoadDataContainerWithSeparateProvider(id);
    }

    private DataContainer CreateNewDataContainer (Type type)
    {
      return DataContainer.CreateNew(Provider.CreateNewObjectID(MappingConfiguration.Current.GetTypeDefinition(type)));
    }

  }
}
