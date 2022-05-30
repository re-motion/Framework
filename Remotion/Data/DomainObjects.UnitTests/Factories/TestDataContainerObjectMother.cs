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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Resources;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Factories
{
  public class TestDataContainerObjectMother
  {
    private readonly DomainObjectIDs _domainObjectIDs;

    public TestDataContainerObjectMother ()
    {
      _domainObjectIDs = StandardConfiguration.Instance.GetDomainObjectIDs();
    }

    public DataContainer CreateCustomer1DataContainer ()
    {
      ObjectID id = _domainObjectIDs.Customer1;
      ClassDefinition classDefinition = id.ClassDefinition;
      Dictionary<PropertyDefinition, object> persistentPropertyValues = new Dictionary<PropertyDefinition, object>();

      // use GetPropertyDefinition because we are setting properties from the base class here
      persistentPropertyValues.Add(classDefinition.GetPropertyDefinition("Remotion.Data.DomainObjects.UnitTests.TestDomain.Company.Name"), "Kunde 1");
      persistentPropertyValues.Add(classDefinition.GetPropertyDefinition("Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.CustomerSince"), new DateTime(2000, 1, 1));
      persistentPropertyValues.Add(classDefinition.GetPropertyDefinition("Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Type"), Customer.CustomerType.Standard);
      persistentPropertyValues.Add(
          classDefinition.GetPropertyDefinition("Remotion.Data.DomainObjects.UnitTests.TestDomain.Company.IndustrialSector"),
          _domainObjectIDs.IndustrialSector1);

      DataContainer dataContainer = CreateExistingDataContainer(id, persistentPropertyValues);

      return dataContainer;
    }

    public DataContainer CreateClassWithAllDataTypes1DataContainer ()
    {
      ObjectID id = new ObjectID("ClassWithAllDataTypes", new Guid("{3F647D79-0CAF-4a53-BAA7-A56831F8CE2D}"));

      ClassDefinition classDefinition = id.ClassDefinition;
      Dictionary<PropertyDefinition, object> persistentPropertyValues = new Dictionary<PropertyDefinition, object>();

      persistentPropertyValues.Add(classDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.BooleanProperty"], false);
      persistentPropertyValues.Add(classDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.ByteProperty"], (byte)85);
      persistentPropertyValues.Add(classDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.DateProperty"], new DateTime(2005, 1, 1));
      persistentPropertyValues.Add(classDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.DateTimeProperty"], new DateTime(2005, 1, 1, 17, 0, 0));
      persistentPropertyValues.Add(classDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.DecimalProperty"], (decimal)123456.789);
      persistentPropertyValues.Add(classDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.DoubleProperty"], 987654.321);
      persistentPropertyValues.Add(classDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.EnumProperty"], ClassWithAllDataTypes.EnumType.Value1);
      persistentPropertyValues.Add(classDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.ExtensibleEnumProperty"], Color.Values.Red());
      persistentPropertyValues.Add(classDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.FlagsProperty"], ClassWithAllDataTypes.FlagsType.Flag2);
      persistentPropertyValues.Add(
          classDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.GuidProperty"],
          new Guid("{236C2DCE-43BD-45ad-BDE6-15F8C05C4B29}"));
      persistentPropertyValues.Add(classDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.Int16Property"], (short)32767);
      persistentPropertyValues.Add(classDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.Int32Property"], 2147483647);
      persistentPropertyValues.Add(classDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.Int64Property"], (long)9223372036854775807);
      persistentPropertyValues.Add(classDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.SingleProperty"], (float)6789.321);
      persistentPropertyValues.Add(classDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.StringProperty"], "abcdeföäü");
      persistentPropertyValues.Add(
          classDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.StringPropertyWithoutMaxLength"],
          "12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890");
      persistentPropertyValues.Add(classDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.BinaryProperty"], ResourceManager.GetImage1());

      persistentPropertyValues.Add(classDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaBooleanProperty"], true);
      persistentPropertyValues.Add(classDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaByteProperty"], (byte)78);
      persistentPropertyValues.Add(classDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDateProperty"], new DateTime(2005, 2, 1));
      persistentPropertyValues.Add(classDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDateTimeProperty"], new DateTime(2005, 2, 1, 5, 0, 0));
      persistentPropertyValues.Add(classDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDecimalProperty"], 765.098m);
      persistentPropertyValues.Add(classDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDoubleProperty"], 654321.789d);
      persistentPropertyValues.Add(classDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaEnumProperty"], ClassWithAllDataTypes.EnumType.Value2);
      persistentPropertyValues.Add(
          classDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaFlagsProperty"],
          ClassWithAllDataTypes.FlagsType.Flag1 | ClassWithAllDataTypes.FlagsType.Flag2);
      persistentPropertyValues.Add(
          classDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaGuidProperty"],
          new Guid("{19B2DFBE-B7BB-448e-8002-F4DBF6032AE8}"));
      persistentPropertyValues.Add(classDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt16Property"], (short)12000);
      persistentPropertyValues.Add(classDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt32Property"], -2147483647);
      persistentPropertyValues.Add(classDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt64Property"], 3147483647L);
      persistentPropertyValues.Add(classDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaSingleProperty"], 12.456F);

      persistentPropertyValues.Add(classDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.ExtensibleEnumWithNullValueProperty"], null);
      persistentPropertyValues.Add(classDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaBooleanWithNullValueProperty"], null);
      persistentPropertyValues.Add(classDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaByteWithNullValueProperty"], null);
      persistentPropertyValues.Add(classDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDateWithNullValueProperty"], null);
      persistentPropertyValues.Add(classDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDateTimeWithNullValueProperty"], null);
      persistentPropertyValues.Add(classDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDecimalWithNullValueProperty"], null);
      persistentPropertyValues.Add(classDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDoubleWithNullValueProperty"], null);
      persistentPropertyValues.Add(classDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaEnumWithNullValueProperty"], null);
      persistentPropertyValues.Add(classDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaFlagsWithNullValueProperty"], null);
      persistentPropertyValues.Add(classDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaGuidWithNullValueProperty"], null);
      persistentPropertyValues.Add(classDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt16WithNullValueProperty"], null);
      persistentPropertyValues.Add(classDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt32WithNullValueProperty"], null);
      persistentPropertyValues.Add(classDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt64WithNullValueProperty"], null);
      persistentPropertyValues.Add(classDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaSingleWithNullValueProperty"], null);
      persistentPropertyValues.Add(classDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.StringWithNullValueProperty"], null);
      persistentPropertyValues.Add(classDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NullableBinaryProperty"], null);

      DataContainer dataContainer = CreateExistingDataContainer(id, persistentPropertyValues);

      return dataContainer;
    }

    public DataContainer CreatePartner1DataContainer ()
    {
      ObjectID id = _domainObjectIDs.Partner1;

      ClassDefinition classDefinition = id.ClassDefinition;
      Dictionary<PropertyDefinition, object> persistentPropertyValues = new Dictionary<PropertyDefinition, object>();

      // use GetPropertyDefinition because we are setting properties from the base class here
      persistentPropertyValues.Add(classDefinition.GetPropertyDefinition("Remotion.Data.DomainObjects.UnitTests.TestDomain.Company.Name"), "Partner 1");
      persistentPropertyValues.Add(classDefinition.GetPropertyDefinition("Remotion.Data.DomainObjects.UnitTests.TestDomain.Partner.ContactPerson"), _domainObjectIDs.Person1);
      persistentPropertyValues.Add(
          classDefinition.GetPropertyDefinition("Remotion.Data.DomainObjects.UnitTests.TestDomain.Company.IndustrialSector"),
          _domainObjectIDs.IndustrialSector1);

      DataContainer dataContainer = CreateExistingDataContainer(id, persistentPropertyValues);

      return dataContainer;
    }

    public DataContainer CreateDistributor2DataContainer ()
    {
      ObjectID id = _domainObjectIDs.Distributor2;

      ClassDefinition classDefinition = id.ClassDefinition;
      Dictionary<PropertyDefinition, object> persistentPropertyValues = new Dictionary<PropertyDefinition, object>();

      // use GetPropertyDefinition because we are setting properties from the base class here
      persistentPropertyValues.Add(classDefinition.GetPropertyDefinition("Remotion.Data.DomainObjects.UnitTests.TestDomain.Company.Name"), "Händler 2");
      persistentPropertyValues.Add(classDefinition.GetPropertyDefinition("Remotion.Data.DomainObjects.UnitTests.TestDomain.Partner.ContactPerson"), _domainObjectIDs.Person6);
      persistentPropertyValues.Add(
          classDefinition.GetPropertyDefinition("Remotion.Data.DomainObjects.UnitTests.TestDomain.Company.IndustrialSector"),
          _domainObjectIDs.IndustrialSector1);
      persistentPropertyValues.Add(classDefinition.GetPropertyDefinition("Remotion.Data.DomainObjects.UnitTests.TestDomain.Distributor.NumberOfShops"), 10);

      DataContainer dataContainer = CreateExistingDataContainer(id, persistentPropertyValues);

      return dataContainer;
    }

    public DataContainer CreateOrder1DataContainer ()
    {
      ObjectID id = _domainObjectIDs.Order1;

      ClassDefinition classDefinition = id.ClassDefinition;
      Dictionary<PropertyDefinition, object> persistentPropertyValues = new Dictionary<PropertyDefinition, object>();


      persistentPropertyValues.Add(classDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"], 1);
      persistentPropertyValues.Add(classDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.DeliveryDate"], new DateTime(2005, 1, 1));
      persistentPropertyValues.Add(classDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Official"], _domainObjectIDs.Official1);
      persistentPropertyValues.Add(classDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer"], _domainObjectIDs.Customer1);

      DataContainer dataContainer = CreateExistingDataContainer(id, persistentPropertyValues);

      return dataContainer;
    }

    public DataContainer CreateOrder2DataContainer ()
    {
      ObjectID id = _domainObjectIDs.Order3;

      ClassDefinition classDefinition = id.ClassDefinition;
      Dictionary<PropertyDefinition, object> persistentPropertyValues = new Dictionary<PropertyDefinition, object>();

      persistentPropertyValues.Add(classDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"], 3);
      persistentPropertyValues.Add(classDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.DeliveryDate"], new DateTime(2005, 3, 1));
      persistentPropertyValues.Add(classDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Official"], _domainObjectIDs.Official1);
      persistentPropertyValues.Add(classDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer"], _domainObjectIDs.Customer3);

      DataContainer dataContainer = CreateExistingDataContainer(id, persistentPropertyValues);

      return dataContainer;
    }

    public DataContainer CreateOrderTicket1DataContainer ()
    {
      ObjectID id = _domainObjectIDs.OrderTicket1;

      ClassDefinition classDefinition = id.ClassDefinition;
      Dictionary<PropertyDefinition, object> persistentPropertyValues = new Dictionary<PropertyDefinition, object>();

      persistentPropertyValues.Add(classDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.FileName"], @"C:\order1.png");
      persistentPropertyValues.Add(classDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order"], _domainObjectIDs.Order1);

      DataContainer dataContainer = CreateExistingDataContainer(id, persistentPropertyValues);

      return dataContainer;
    }

    public DataContainer CreateOrderTicket2DataContainer ()
    {
      ObjectID id = _domainObjectIDs.OrderTicket2;

      ClassDefinition classDefinition = id.ClassDefinition;
      Dictionary<PropertyDefinition, object> persistentPropertyValues = new Dictionary<PropertyDefinition, object>();

      persistentPropertyValues.Add(classDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.FileName"], @"C:\order3.png");
      persistentPropertyValues.Add(classDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order"], _domainObjectIDs.Order2);

      DataContainer dataContainer = CreateExistingDataContainer(id, persistentPropertyValues);

      return dataContainer;
    }

    public DataContainer CreateClassWithGuidKeyDataContainer ()
    {
      ObjectID id = new ObjectID("ClassWithGuidKey", new Guid("{7D1F5F2E-D111-433b-A675-300B55DC4756}"));

      Dictionary<PropertyDefinition, object> persistentPropertyValues = new Dictionary<PropertyDefinition, object>();

      DataContainer dataContainer = CreateExistingDataContainer(id, persistentPropertyValues);

      return dataContainer;
    }

    private DataContainer CreateExistingDataContainer (ObjectID id, Dictionary<PropertyDefinition, object> persistentPropertyValues)
    {
      return DataContainer.CreateForExisting(id, null, delegate (PropertyDefinition propertyDefinition)
      {
        return persistentPropertyValues.ContainsKey(propertyDefinition) ? persistentPropertyValues[propertyDefinition] : propertyDefinition.DefaultValue;
      });
    }
  }
}
