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
using System.Data.SqlClient;
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.UnitTests.Resources;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.IntegrationTests
{
  [TestFixture]
  public class SqlProviderSaveExistingTest : SqlProviderBaseTest
  {
    [Test]
    public void Save ()
    {
      var savedClassWithAllDataTypes = LoadDataContainer(DomainObjectIDs.ClassWithAllDataTypes1);

      Assert.That(GetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "EnumProperty"), Is.EqualTo(ClassWithAllDataTypes.EnumType.Value1));
      SetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "EnumProperty", ClassWithAllDataTypes.EnumType.Value2);

      Provider.Save(new[] { savedClassWithAllDataTypes });

      DataContainer reloadedClassWithAllDataTypes = ReloadDataContainer(DomainObjectIDs.ClassWithAllDataTypes1);

      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "EnumProperty"), Is.EqualTo(ClassWithAllDataTypes.EnumType.Value2));
    }

    [Test]
    public void SaveAllSimpleDataTypes ()
    {
      DataContainer savedClassWithAllDataTypes = LoadDataContainer(DomainObjectIDs.ClassWithAllDataTypes1);

      Assert.That(GetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "BooleanProperty"), Is.EqualTo(false));
      Assert.That(GetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "ByteProperty"), Is.EqualTo(85));
      Assert.That(GetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "DateProperty"), Is.EqualTo(new DateOnly(2005, 1, 1)));
      Assert.That(GetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "DateTimeProperty"), Is.EqualTo(new DateTime(2005, 1, 1, 17, 0, 0)));
      Assert.That(GetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "DecimalProperty"), Is.EqualTo(123456.789));
      Assert.That(GetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "DoubleProperty"), Is.EqualTo(987654.321));
      Assert.That(GetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "EnumProperty"), Is.EqualTo(ClassWithAllDataTypes.EnumType.Value1));
      Assert.That(GetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "ExtensibleEnumProperty"), Is.EqualTo(Color.Values.Red()));
      Assert.That(GetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "GuidProperty"), Is.EqualTo(new Guid("{236C2DCE-43BD-45ad-BDE6-15F8C05C4B29}")));
      Assert.That(GetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "Int16Property"), Is.EqualTo(32767));
      Assert.That(GetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "Int32Property"), Is.EqualTo(2147483647));
      Assert.That(GetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "Int64Property"), Is.EqualTo(9223372036854775807));
      Assert.That(GetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "SingleProperty"), Is.EqualTo(6789.321f));
      Assert.That(GetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "StringProperty"), Is.EqualTo("abcdeföäü"));
      Assert.That(
          GetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "StringPropertyWithoutMaxLength"),
          Is.EqualTo("12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890"));
      ResourceManager.IsEqualToImage1(
          (byte[])GetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "BinaryProperty"));

      SetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "BooleanProperty", true);
      SetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "ByteProperty", (byte)42);
      SetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "DateProperty", new DateOnly(1972, 10, 26));
      SetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "DateTimeProperty", new DateTime(1974, 10, 26, 15, 17, 19));
      SetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "DecimalProperty", (decimal)564.956);
      SetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "DoubleProperty", 5334.2456);
      SetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "EnumProperty", ClassWithAllDataTypes.EnumType.Value0);
      SetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "ExtensibleEnumProperty", Color.Values.Green());
      SetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "GuidProperty", new Guid("{98E0FE88-7DB4-4f6c-A1C1-86682D5C95C9}"));
      SetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "Int16Property", (short)67);
      SetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "Int32Property", 42424242);
      SetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "Int64Property", 424242424242424242);
      SetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "SingleProperty", (float)42.42);
      SetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "StringProperty", "zyxwvuZaphodBeeblebrox");
      SetPropertyValue(
          savedClassWithAllDataTypes,
          typeof(ClassWithAllDataTypes),
          "StringPropertyWithoutMaxLength",
          "123450987612345098761234509876123450987612345098761234509876123450987612345098761234509876123450987612345098761234509876123450987612345098761234509876");
      SetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "BinaryProperty", ResourceManager.GetImage2());

      Provider.Save(new[] { savedClassWithAllDataTypes });

      DataContainer reloadedClassWithAllDataTypes = ReloadDataContainer(DomainObjectIDs.ClassWithAllDataTypes1);

      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "BooleanProperty"), Is.EqualTo(true));
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "ByteProperty"), Is.EqualTo(42));
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "DateProperty"), Is.EqualTo(new DateOnly(1972, 10, 26)));
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "DateTimeProperty"), Is.EqualTo(new DateTime(1974, 10, 26, 15, 17, 19)));
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "DecimalProperty"), Is.EqualTo(564.956));
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "DoubleProperty"), Is.EqualTo(5334.2456));
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
      ResourceManager.IsEqualToImage2(
          (byte[])GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "BinaryProperty"));
    }

    [Test]
    public void SaveAllNullableTypes ()
    {
      DataContainer savedClassWithAllDataTypes = LoadDataContainer(DomainObjectIDs.ClassWithAllDataTypes1);

      Assert.That(GetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaBooleanProperty"), Is.EqualTo(true));
      Assert.That(GetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaByteProperty"), Is.EqualTo((byte)78));
      Assert.That(GetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaDateProperty"), Is.EqualTo(new DateOnly(2005, 2, 1)));
      Assert.That(GetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaDateTimeProperty"), Is.EqualTo(new DateTime(2005, 2, 1, 5, 0, 0)));
      Assert.That(GetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaDecimalProperty"), Is.EqualTo(765.098m));
      Assert.That(GetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaDoubleProperty"), Is.EqualTo(654321.789d));
      Assert.That(GetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaEnumProperty"), Is.EqualTo(ClassWithAllDataTypes.EnumType.Value2));
      Assert.That(GetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaGuidProperty"), Is.EqualTo(new Guid("{19B2DFBE-B7BB-448e-8002-F4DBF6032AE8}")));
      Assert.That(GetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaInt16Property"), Is.EqualTo((short)12000));
      Assert.That(GetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaInt32Property"), Is.EqualTo(-2147483647));
      Assert.That(GetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaInt64Property"), Is.EqualTo(3147483647L));
      Assert.That(GetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaSingleProperty"), Is.EqualTo(12.456F));
      Assert.That(GetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NullableBinaryProperty"), Is.Null);

      SetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaBooleanProperty", false);
      SetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaByteProperty", (byte)100);
      SetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaDateProperty", new DateOnly(2007, 1, 18));
      SetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaDateTimeProperty", new DateTime(
          2005, 1, 18, 10, 10, 10));
      SetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaDecimalProperty", 20.123m);
      SetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaDoubleProperty", 56.87d);
      SetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaEnumProperty", ClassWithAllDataTypes.EnumType.Value0);
      SetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaGuidProperty", new Guid("{10FD9EDE-F3BB-4bb9-9434-9B121C6733A0}"));
      SetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaInt16Property", (short)-43);
      SetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaInt32Property", -42);
      SetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaInt64Property", -41L);
      SetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaSingleProperty", -40F);
      SetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NullableBinaryProperty", ResourceManager.GetImage1());

      Provider.Save(new[] { savedClassWithAllDataTypes });

      DataContainer reloadedClassWithAllDataTypes = ReloadDataContainer(DomainObjectIDs.ClassWithAllDataTypes1);

      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaBooleanProperty"), Is.EqualTo(false));
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaByteProperty"), Is.EqualTo((byte)100));
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaDateProperty"), Is.EqualTo(new DateOnly(2007, 1, 18)));
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaDateTimeProperty"), Is.EqualTo(new DateTime(2005, 1, 18, 10, 10, 10)));
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaDecimalProperty"), Is.EqualTo(20.123m));
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaDoubleProperty"), Is.EqualTo(56.87d));
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaEnumProperty"), Is.EqualTo(ClassWithAllDataTypes.EnumType.Value0));
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaGuidProperty"), Is.EqualTo(new Guid("{10FD9EDE-F3BB-4bb9-9434-9B121C6733A0}")));
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaInt16Property"), Is.EqualTo((short)-43));
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaInt32Property"), Is.EqualTo(-42));
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaInt64Property"), Is.EqualTo(-41L));
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaSingleProperty"), Is.EqualTo(-40F));
    }

    [Test]
    public void SaveAllNullableTypesWithNull ()
    {
      // Note for NullableBinaryProperty: Because the value in the database is already null, the property has
      //  to be changed first to something different to ensure the null value is written back.
      DataContainer tempClassWithAllDataTypes = LoadDataContainer(DomainObjectIDs.ClassWithAllDataTypes1);
      SetPropertyValue(tempClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NullableBinaryProperty", ResourceManager.GetImage1());
      Provider.Save(new[] { tempClassWithAllDataTypes });

      var savedClassWithAllDataTypes = LoadDataContainer(DomainObjectIDs.ClassWithAllDataTypes1);

      Assert.That(GetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaBooleanProperty"), Is.EqualTo(true));
      Assert.That(GetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaByteProperty"), Is.EqualTo((byte)78));
      Assert.That(GetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaDateProperty"), Is.EqualTo(new DateOnly(2005, 2, 1)));
      Assert.That(GetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaDateTimeProperty"), Is.EqualTo(new DateTime(2005, 2, 1, 5, 0, 0)));
      Assert.That(GetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaDecimalProperty"), Is.EqualTo(765.098m));
      Assert.That(GetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaDoubleProperty"), Is.EqualTo(654321.789d));
      Assert.That(GetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaEnumProperty"), Is.EqualTo(ClassWithAllDataTypes.EnumType.Value2));
      Assert.That(GetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaGuidProperty"), Is.EqualTo(new Guid("{19B2DFBE-B7BB-448e-8002-F4DBF6032AE8}")));
      Assert.That(GetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaInt16Property"), Is.EqualTo((short)12000));
      Assert.That(GetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaInt32Property"), Is.EqualTo(-2147483647));
      Assert.That(GetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaInt64Property"), Is.EqualTo(3147483647L));
      Assert.That(GetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaSingleProperty"), Is.EqualTo(12.456F));
      ResourceManager.IsEqualToImage1(
          (byte[])GetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NullableBinaryProperty"));

      SetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaBooleanProperty", null);
      SetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaByteProperty", null);
      SetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaDateProperty", null);
      SetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaDateTimeProperty", null);
      SetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaDecimalProperty", null);
      SetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaDoubleProperty", null);
      SetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaEnumProperty", null);
      SetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaGuidProperty", null);
      SetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaInt16Property", null);
      SetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaInt32Property", null);
      SetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaInt64Property", null);
      SetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaSingleProperty", null);
      SetPropertyValue(savedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NullableBinaryProperty", null);

      Provider.Save(new[] { savedClassWithAllDataTypes });

      DataContainer reloadedClassWithAllDataTypes = ReloadDataContainer(DomainObjectIDs.ClassWithAllDataTypes1);

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
      Assert.That(GetPropertyValue(reloadedClassWithAllDataTypes, typeof(ClassWithAllDataTypes), "NullableBinaryProperty"), Is.Null);
    }

    [Test]
    public void SaveWithNoChanges ()
    {
      DataContainer classWithAllDataTypes = LoadDataContainer(DomainObjectIDs.ClassWithAllDataTypes1);

      Provider.Save(new[] { classWithAllDataTypes });

      // expectation: no exception
    }


    [Test]
    public void SaveMultipleDataContainers ()
    {
      DataContainer savedOrderContainer = LoadDataContainer(DomainObjectIDs.Order1);
      DataContainer savedOrderItemContainer = LoadDataContainer(DomainObjectIDs.OrderItem1);

      Assert.That(GetPropertyValue(savedOrderContainer, typeof(Order), "OrderNumber"), Is.EqualTo(1));
      Assert.That(GetPropertyValue(savedOrderItemContainer, typeof(OrderItem), "Product"), Is.EqualTo("Mainboard"));

      SetPropertyValue(savedOrderContainer, typeof(Order), "OrderNumber", 10);
      SetPropertyValue(savedOrderItemContainer, typeof(OrderItem), "Product", "Raumschiff");

      Provider.Save(new[] { savedOrderContainer, savedOrderItemContainer });

      DataContainer orderContainer = ReloadDataContainer(DomainObjectIDs.Order1);
      DataContainer orderItemContainer = ReloadDataContainer(DomainObjectIDs.OrderItem1);

      Assert.That(GetPropertyValue(orderContainer, typeof(Order), "OrderNumber"), Is.EqualTo(10));
      Assert.That(GetPropertyValue(orderItemContainer, typeof(OrderItem), "Product"), Is.EqualTo("Raumschiff"));
    }

    [Test]
    public void ConcurrentSave ()
    {
      DataContainer orderContainer1 = LoadDataContainer(DomainObjectIDs.Order1);
      DataContainer orderContainer2 = LoadDataContainer(DomainObjectIDs.Order1);

      SetPropertyValue(orderContainer1, typeof(Order), "OrderNumber", 10);
      SetPropertyValue(orderContainer2, typeof(Order), "OrderNumber", 11);

      Provider.Save(new[] { orderContainer1 });

      var exception = Assert.Throws<ConcurrencyViolationException>(() => Provider.Save(new[] { orderContainer2 }));
      Assert.That(exception.IDs, Is.EqualTo(new [] { orderContainer2.ID }));
    }

    [Test]
    public void SaveWithConnect ()
    {
      DataContainer savedOrderContainer = LoadDataContainerWithSeparateProvider(DomainObjectIDs.Order1);
      SetPropertyValue(savedOrderContainer, typeof(Order), "OrderNumber", 10);

      Assert.That(!Provider.IsConnected);

      Provider.Save(new[] { savedOrderContainer });

      DataContainer reloadedOrderContainer = ReloadDataContainer(DomainObjectIDs.Order1);
      Assert.That(GetPropertyValue(reloadedOrderContainer, typeof(Order), "OrderNumber"), Is.EqualTo(10));
    }

    [Test]
    public void WrapSqlException ()
    {
      DataContainer orderContainer = LoadDataContainer(DomainObjectIDs.Order1);

      SetPropertyValue(orderContainer, typeof(Order), "Customer", new ObjectID(typeof(Customer), Guid.NewGuid()));
      Assert.That(
          () => Provider.Save(new[] { orderContainer }),
          Throws.InstanceOf<RdbmsProviderException>());
    }

    [Test]
    public void UpdateTimestamps ()
    {
      DataContainer orderContainer = LoadDataContainer(DomainObjectIDs.Order1);

      object oldTimestamp = orderContainer.Timestamp;
      SetPropertyValue(orderContainer, typeof(Order), "OrderNumber", 10);

      Provider.Save(new[] { orderContainer });
      Provider.UpdateTimestamps(new[] { orderContainer });

      Assert.That(oldTimestamp.Equals(orderContainer.Timestamp), Is.False);
    }

    [Test]
    public void UpdateTimestampsWithConnect ()
    {
      DataContainer orderContainer = LoadDataContainerWithSeparateProvider(DomainObjectIDs.Order1);
      object oldTimestamp = orderContainer.Timestamp;
      SetPropertyValue(orderContainer, typeof(Order), "OrderNumber", 10);

      Assert.That(!Provider.IsConnected);

      Provider.UpdateTimestamps(new[] { orderContainer });

      Assert.That(oldTimestamp.Equals(orderContainer.Timestamp), Is.False);
    }

    [Test]
    public void UpdateTimestampsForMultipleDataContainers ()
    {
      DataContainer orderContainer = LoadDataContainer(DomainObjectIDs.Order1);
      DataContainer orderItemContainer = LoadDataContainer(DomainObjectIDs.OrderItem1);

      object oldOrderTimestamp = orderContainer.Timestamp;
      object oldOrderItemTimestamp = orderItemContainer.Timestamp;

      SetPropertyValue(orderContainer, typeof(Order), "OrderNumber", 10);
      SetPropertyValue(orderItemContainer, typeof(OrderItem), "Product", "Raumschiff");

      Provider.Save(new[] { orderContainer, orderItemContainer });
      Provider.UpdateTimestamps(new[] { orderContainer, orderItemContainer });

      Assert.That(oldOrderTimestamp.Equals(orderContainer.Timestamp), Is.False);
      Assert.That(oldOrderItemTimestamp.Equals(orderItemContainer.Timestamp), Is.False);
    }

    [Test]
    public void TransactionalSave ()
    {
      DataContainer savedOrderContainer = LoadDataContainer(DomainObjectIDs.Order1);

      SetPropertyValue(savedOrderContainer, typeof(Order), "OrderNumber", 10);

      Provider.BeginTransaction();
      Provider.Save(new[] { savedOrderContainer });
      Provider.Commit();

      DataContainer reloadedOrderContainer = ReloadDataContainer(DomainObjectIDs.Order1);
      Assert.That(GetPropertyValue(reloadedOrderContainer, typeof(Order), "OrderNumber"), Is.EqualTo(10));
    }

    [Test]
    public void TransactionalLoadDataContainerAndSave ()
    {
      Provider.BeginTransaction();
      DataContainer reloadedAndSavedOrderContainer = Provider.LoadDataContainer(DomainObjectIDs.Order1).LocatedObject;

      SetPropertyValue(reloadedAndSavedOrderContainer, typeof(Order), "OrderNumber", 10);

      Provider.Save(new[] { reloadedAndSavedOrderContainer });
      Provider.Commit();

      DataContainer reloadedOrderContainer = ReloadDataContainer(DomainObjectIDs.Order1);
      Assert.That(GetPropertyValue(reloadedOrderContainer, typeof(Order), "OrderNumber"), Is.EqualTo(10));
    }

    [Test]
    public void TransactionalLoadDataContainersByRelatedIDAndSave ()
    {
      Provider.BeginTransaction();

      var relationEndPointDefinition = GetEndPointDefinition(typeof(OrderTicket), "Order");
      var orderTicketContainers = Provider.LoadDataContainersByRelatedID(
          (RelationEndPointDefinition)relationEndPointDefinition,
          null,
          DomainObjectIDs.Order1).ToList();

      SetPropertyValue(orderTicketContainers[0], typeof(OrderTicket), "FileName", "C:\newFile.jpg");

      Provider.Save(orderTicketContainers);
      Provider.Commit();

      DataContainer reloadedOrderTicketContainer = ReloadDataContainer(DomainObjectIDs.OrderTicket1);
      Assert.That(GetPropertyValue(reloadedOrderTicketContainer, typeof(OrderTicket), "FileName"), Is.EqualTo("C:\newFile.jpg"));
    }

    [Test]
    public void TransactionalSaveAndSetTimestamp ()
    {
      Provider.BeginTransaction();
      DataContainer savedOrderContainer = Provider.LoadDataContainer(DomainObjectIDs.Order1).LocatedObject;

      object oldTimestamp = savedOrderContainer.Timestamp;
      SetPropertyValue(savedOrderContainer, typeof(Order), "OrderNumber", 10);

      Provider.Save(new[] { savedOrderContainer });
      Provider.UpdateTimestamps(new[] { savedOrderContainer });
      Provider.Commit();

      DataContainer reloadedOrderContainer = ReloadDataContainer(DomainObjectIDs.Order1);
      Assert.That(GetPropertyValue(reloadedOrderContainer, typeof(Order), "OrderNumber"), Is.EqualTo(10));
      Assert.That(oldTimestamp.Equals(reloadedOrderContainer.Timestamp), Is.False);
    }

    [Test]
    public void CommitWithoutBeginTransaction ()
    {
      DataContainer orderContainer = LoadDataContainer(DomainObjectIDs.Order1);
      SetPropertyValue(orderContainer, typeof(Order), "OrderNumber", 10);

      Provider.Save(new[] { orderContainer });
      Assert.That(
          () => Provider.Commit(),
          Throws.InvalidOperationException
              .With.Message.EqualTo("Commit cannot be called without calling BeginTransaction first."));
    }

    [Test]
    public void SaveWithRollback ()
    {
      Provider.BeginTransaction();
      DataContainer savedOrderContainer = Provider.LoadDataContainer(DomainObjectIDs.Order1).LocatedObject;

      SetPropertyValue(savedOrderContainer, typeof(Order), "OrderNumber", 10);

      Provider.Save(new[] { savedOrderContainer });
      Provider.UpdateTimestamps(new[] { savedOrderContainer });
      Provider.Rollback();

      DataContainer reloadedOrderContainer = LoadDataContainer(DomainObjectIDs.Order1);
      Assert.That(GetPropertyValue(reloadedOrderContainer, typeof(Order), "OrderNumber"), Is.EqualTo(1));
    }

    [Test]
    public void RollbackWithoutBeginTransaction ()
    {
      DataContainer orderContainer = LoadDataContainer(DomainObjectIDs.Order1);
      SetPropertyValue(orderContainer, typeof(Order), "OrderNumber", 10);

      Provider.Save(new[] { orderContainer });
      Assert.That(
          () => Provider.Rollback(),
          Throws.InvalidOperationException
              .With.Message.EqualTo("Rollback cannot be called without calling BeginTransaction first."));
    }

    [Test]
    public void SaveForeignKeyInSameStorageProvider ()
    {
      DataContainer orderTicketContainer = LoadDataContainer(DomainObjectIDs.OrderTicket1);
      SetPropertyValue(orderTicketContainer, typeof(OrderTicket), "Order", DomainObjectIDs.Order3);

      Provider.Save(new[] { orderTicketContainer });

      // expectation: no exception
    }

    [Test]
    public void SaveForeignKeyInOtherStorageProvider ()
    {
      DataContainer savedOrderContainer = LoadDataContainer(DomainObjectIDs.Order2);
      SetPropertyValue(savedOrderContainer, typeof(Order), "Official", DomainObjectIDs.Official2);

      Provider.Save(new[] { savedOrderContainer });

      DataContainer reloadedOrderContainer = ReloadDataContainer(DomainObjectIDs.Order2);
      Assert.That(GetPropertyValue(reloadedOrderContainer, typeof(Order), "Official"), Is.EqualTo(DomainObjectIDs.Official2));
    }

    [Test]
    public void SaveForeignKeyWithClassIDColumnAndDerivedClass ()
    {
      DataContainer savedCeoContainer = LoadDataContainer(DomainObjectIDs.Ceo1);
      SetPropertyValue(savedCeoContainer, typeof(Ceo), "Company", DomainObjectIDs.Partner1);

      Provider.Save(new[] { savedCeoContainer });

      DataContainer reloadedCeoContainer = ReloadDataContainer(DomainObjectIDs.Ceo1);
      Assert.That(GetPropertyValue(reloadedCeoContainer, typeof(Ceo), "Company"), Is.EqualTo(DomainObjectIDs.Partner1));
    }

    [Test]
    public void SaveForeignKeyWithClassIDColumnAndBaseClass ()
    {
      DataContainer savedCeoContainer = LoadDataContainer(DomainObjectIDs.Ceo1);
      SetPropertyValue(savedCeoContainer, typeof(Ceo), "Company", DomainObjectIDs.Supplier1);

      Provider.Save(new[] { savedCeoContainer });

      DataContainer reloadedCeoContainer = ReloadDataContainer(DomainObjectIDs.Ceo1);
      Assert.That(GetPropertyValue(reloadedCeoContainer, typeof(Ceo), "Company"), Is.EqualTo(DomainObjectIDs.Supplier1));
    }

    [Test]
    public void SaveNullForeignKey ()
    {
      DataContainer savedComputerContainer = LoadDataContainer(DomainObjectIDs.Computer1);
      var propertyDefinition = GetPropertyDefinition(typeof(Computer), "Employee");
      savedComputerContainer.SetValue(propertyDefinition, null);
      Provider.Save(new[] { savedComputerContainer });

      DataContainer reloadedComputerContainer = ReloadDataContainer(DomainObjectIDs.Computer1);
      Assert.That(reloadedComputerContainer.GetValue(propertyDefinition), Is.Null);
    }

    [Test]
    public void SaveNullForeignKeyWithInheritance ()
    {
      DataContainer savedCeoContainer = LoadDataContainer(DomainObjectIDs.Ceo1);
      SetPropertyValue(savedCeoContainer, typeof(Ceo), "Company", null);

      Provider.Save(new[] { savedCeoContainer });
      Provider.Disconnect();

      using (SqlConnection connection = new SqlConnection(TestDomainConnectionString))
      {
        connection.Open();
        using (SqlCommand command = new SqlCommand("select * from Ceo where ID = @id", connection))
        {
          command.Parameters.AddWithValue("@id", DomainObjectIDs.Ceo1.Value);
          using (SqlDataReader reader = command.ExecuteReader())
          {
            reader.Read();
            int columnOrdinal = reader.GetOrdinal("CompanyIDClassID");
            Assert.That(reader.IsDBNull(columnOrdinal), Is.True);
          }
        }
      }
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
  }
}
