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
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.NonPersistent;
using Remotion.Data.DomainObjects.Persistence.NonPersistent.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.Mapping.SortExpressions;
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration;
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.MixedMapping;
using Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping
{
  public class FakeMappingConfiguration
  {
    // types

    // static members and constants

    private static readonly DoubleCheckedLockingContainer<FakeMappingConfiguration> s_current
        = new DoubleCheckedLockingContainer<FakeMappingConfiguration>(() => new FakeMappingConfiguration());

    public static FakeMappingConfiguration Current
    {
      get { return s_current.Value; }
    }

    public static void Reset ()
    {
      s_current.Value = null;
    }

    // member fields

    private readonly StorageProviderDefinition _defaultStorageProviderDefinition;
    private readonly StorageProviderDefinition _nonPersistentProviderDefinition;
    private readonly ReadOnlyDictionary<Type, ClassDefinition> _typeDefinitions;
    private readonly ReadOnlyDictionary<string, RelationDefinition> _relationDefinitions;

    // construction and disposing

    private FakeMappingConfiguration ()
    {
      _defaultStorageProviderDefinition = new UnitTestStorageProviderStubDefinition("DefaultStorageProvider");
      _nonPersistentProviderDefinition =
          new NonPersistentProviderDefinition("NonPersistentStorageProvider", new NonPersistentStorageObjectFactory());
      _typeDefinitions = new ReadOnlyDictionary<Type, ClassDefinition>(CreateClassDefinitions());
      _relationDefinitions = new ReadOnlyDictionary<string, RelationDefinition>(CreateRelationDefinitions());

      foreach (ClassDefinition classDefinition in _typeDefinitions.Values)
        classDefinition.SetReadOnly();
    }

    // methods and properties

    public StorageProviderDefinition DefaultStorageProviderDefinition
    {
      get { return _defaultStorageProviderDefinition; }
    }

    public StorageProviderDefinition NonPersistentStorageProviderDefinition
    {
      get { return _nonPersistentProviderDefinition; }
    }

    public ReadOnlyDictionary<Type, ClassDefinition> TypeDefinitions
    {
      get { return _typeDefinitions; }
    }

    public ReadOnlyDictionary<string, RelationDefinition> RelationDefinitions
    {
      get { return _relationDefinitions; }
    }

    #region Methods for creating class definitions

    private Dictionary<Type, ClassDefinition> CreateClassDefinitions ()
    {
      var classDefinitions = new List<ClassDefinition>();

      ClassDefinition company = CreateCompanyDefinition(null);
      classDefinitions.Add(company);

      ClassDefinition customer = CreateCustomerDefinition(company);
      classDefinitions.Add(customer);

      ClassDefinition partner = CreatePartnerDefinition(company);
      classDefinitions.Add(partner);

      ClassDefinition supplier = CreateSupplierDefinition(partner);
      classDefinitions.Add(supplier);

      ClassDefinition distributor = CreateDistributorDefinition(partner);
      classDefinitions.Add(distributor);

      classDefinitions.Add(CreateOrderDefinition(null));
      classDefinitions.Add(CreateOrderViewModelDefinition(null));
      classDefinitions.Add(CreateOrderTicketDefinition(null));
      classDefinitions.Add(CreateOrderItemDefinition(null));

      classDefinitions.Add(CreateProductDefinition(null));
      classDefinitions.Add(CreateProductReviewDefinition(null));

      ClassDefinition officialDefinition = CreateOfficialDefinition(null);
      classDefinitions.Add(officialDefinition);
      classDefinitions.Add(CreateSpecialOfficialDefinition(officialDefinition));

      classDefinitions.Add(CreateCeoDefinition(null));
      classDefinitions.Add(CreatePersonDefinition(null));

      classDefinitions.Add(CreateClientDefinition(null));
      classDefinitions.Add(CreateLocationDefinition(null));

      ClassDefinition fileSystemItemDefinition = CreateFileSystemItemDefinition(null);
      classDefinitions.Add(fileSystemItemDefinition);
      classDefinitions.Add(CreateFolderDefinition(fileSystemItemDefinition));
      classDefinitions.Add(CreateFileDefinition(fileSystemItemDefinition));

      classDefinitions.Add(CreateClassWithOptionalOneToOneRelationAndOppositeDerivedClassDefinition(null));
      classDefinitions.Add(CreateClassWithAllDataTypesDefinition(null));
      classDefinitions.Add(CreateClassWithDefaultStorageClassTransaction(null));
      classDefinitions.Add(CreateClassWithGuidKeyDefinition(null));
      classDefinitions.Add(CreateClassWithInvalidKeyTypeDefinition(null));
      classDefinitions.Add(CreateClassWithoutIDColumnDefinition(null));
      classDefinitions.Add(CreateClassWithoutClassIDColumnDefinition(null));
      classDefinitions.Add(CreateClassWithoutTimestampColumnDefinition(null));
      classDefinitions.Add(CreateClassWithValidRelationsDefinition(null));
      classDefinitions.Add(CreateClassWithInvalidRelationDefinition(null));
      classDefinitions.Add(CreateIndustrialSectorDefinition(null));
      classDefinitions.Add(CreateEmployeeDefinition(null));
      classDefinitions.Add(CreateComputerDefinition(null));
      classDefinitions.Add(CreateClassWithRelatedClassIDColumnAndNoInheritanceDefinition(null));

      var targetClassForPersistentMixinDefinition = CreateTargetClassForPersistentMixinDefinition(null);
      classDefinitions.Add(targetClassForPersistentMixinDefinition);
      var derivedTargetClassForPersistentMixinDefinition = CreateDerivedTargetClassForPersistentMixinDefinition(targetClassForPersistentMixinDefinition);
      classDefinitions.Add(derivedTargetClassForPersistentMixinDefinition);
      var derivedDerivedTargetClassForPersistentMixinDefinition = CreateDerivedDerivedTargetClassForPersistentMixinDefinition(derivedTargetClassForPersistentMixinDefinition);
      classDefinitions.Add(derivedDerivedTargetClassForPersistentMixinDefinition);
      var relationTargetForPersistentMixinDefinition = CreateRelationTargetForPersistentMixinDefinition(null);
      classDefinitions.Add(relationTargetForPersistentMixinDefinition);

      CalculateAndSetDerivedClasses(classDefinitions);

      return classDefinitions.ToDictionary(cd => cd.ClassType);
    }

    private void CalculateAndSetDerivedClasses (IEnumerable<ClassDefinition> classDefinitions)
    {
      var classesByBaseClass = (from classDefinition in classDefinitions
                                where classDefinition.BaseClass != null
                                group classDefinition by classDefinition.BaseClass)
          .ToDictionary(grouping => grouping.Key, grouping => (IEnumerable<ClassDefinition>)grouping);

      foreach (var classDefinition in classDefinitions)
      {
        IEnumerable<ClassDefinition> derivedClasses;
        if (!classesByBaseClass.TryGetValue(classDefinition, out derivedClasses))
          derivedClasses = Enumerable.Empty<ClassDefinition>();

        classDefinition.SetDerivedClasses(derivedClasses);
      }
    }

    private ClassDefinition CreateCompanyDefinition (ClassDefinition baseClass)
    {
      ClassDefinition company = CreateClassDefinition("Company", "Company", typeof(Company), false, baseClass);

      var properties = new List<PropertyDefinition>();
      properties.Add(
          CreatePersistentPropertyDefinition(company, typeof(Company), "Name", "Name", false, 100));
      properties.Add(
          CreatePersistentPropertyDefinition(company, typeof(Company), "IndustrialSector", "IndustrialSectorID", true, null));
      company.SetPropertyDefinitions(new PropertyDefinitionCollection(properties, true));

      return company;
    }

    private ClassDefinition CreateCustomerDefinition (ClassDefinition baseClass)
    {
      ClassDefinition customer = CreateClassDefinition("Customer", "Company", typeof(Customer), false, baseClass);

      var properties = new List<PropertyDefinition>();
      properties.Add(
          CreatePersistentPropertyDefinition(customer, typeof(Customer), "CustomerSince", "CustomerSince", true, null));
      properties.Add(
          CreatePersistentPropertyDefinition(customer, typeof(Customer), "Type", "CustomerType", false, null));
      customer.SetPropertyDefinitions(new PropertyDefinitionCollection(properties, true));

      return customer;
    }

    private ClassDefinition CreatePartnerDefinition (ClassDefinition baseClass)
    {
      ClassDefinition partner = CreateClassDefinition("Partner", "Company", typeof(Partner), false, baseClass);

      var properties = new List<PropertyDefinition>();
      properties.Add(
          CreatePersistentPropertyDefinition(partner, typeof(Partner), "ContactPerson", "ContactPersonID", true, null));
      partner.SetPropertyDefinitions(new PropertyDefinitionCollection(properties, true));

      return partner;
    }

    private ClassDefinition CreateSupplierDefinition (ClassDefinition baseClass)
    {
      ClassDefinition supplier = CreateClassDefinition("Supplier", "Company", typeof(Supplier), false, baseClass);

      var properties = new List<PropertyDefinition>();
      properties.Add(
          CreatePersistentPropertyDefinition(supplier, typeof(Supplier), "SupplierQuality", "SupplierQuality", false, null));
      supplier.SetPropertyDefinitions(new PropertyDefinitionCollection(properties, true));

      return supplier;
    }

    private ClassDefinition CreateDistributorDefinition (ClassDefinition baseClass)
    {
      ClassDefinition distributor = CreateClassDefinition("Distributor", "Company", typeof(Distributor), false, baseClass);

      var properties = new List<PropertyDefinition>();
      properties.Add(
          CreatePersistentPropertyDefinition(distributor, typeof(Distributor), "NumberOfShops", "NumberOfShops", false, null));
      distributor.SetPropertyDefinitions(new PropertyDefinitionCollection(properties, true));

      return distributor;
    }

    private ClassDefinition CreateOrderDefinition (ClassDefinition baseClass)
    {
      ClassDefinition order = CreateClassDefinition("Order", "Order", typeof(Order), false, baseClass);

      var properties = new List<PropertyDefinition>();
      properties.Add(
          CreatePersistentPropertyDefinition(order, typeof(Order), "OrderNumber", "OrderNo", false, null));
      properties.Add(
          CreatePersistentPropertyDefinition(order, typeof(Order), "DeliveryDate", "DeliveryDate", false, null));
      properties.Add(
          CreatePersistentPropertyDefinition(order, typeof(Order), "Customer", "CustomerID", true, null));
      properties.Add(
          CreatePersistentPropertyDefinition(order, typeof(Order), "Official", "OfficialID", true, null));
      order.SetPropertyDefinitions(new PropertyDefinitionCollection(properties, true));

      return order;
    }

    private ClassDefinition CreateOrderViewModelDefinition (ClassDefinition baseClass)
    {
      ClassDefinition orderViewModel = CreateClassDefinitionForNonPersistentProvider(
          "OrderViewModel", typeof(OrderViewModel), false, baseClass);

      var properties = new List<PropertyDefinition>();
      properties.Add(
          CreateTransactionPropertyDefinition(orderViewModel, typeof(OrderViewModel), "OrderSum", false, null));
      properties.Add(
          CreateTransactionPropertyDefinition(orderViewModel, typeof(OrderViewModel), "Object", true, null));
      orderViewModel.SetPropertyDefinitions(new PropertyDefinitionCollection(properties, true));

      return orderViewModel;
    }

    private ClassDefinition CreateOfficialDefinition (ClassDefinition baseClass)
    {
      ClassDefinition official = CreateClassDefinition(
          "Official", "Official", typeof(Official), false, baseClass);

      var properties = new List<PropertyDefinition>();
      properties.Add(
          CreatePersistentPropertyDefinition(official, typeof(Official), "Name", "Name", false, 100));
      official.SetPropertyDefinitions(new PropertyDefinitionCollection(properties, true));

      return official;
    }

    private ClassDefinition CreateSpecialOfficialDefinition (ClassDefinition officialDefinition)
    {
      var specialOfficial = CreateClassDefinition("SpecialOfficial", "Official", typeof(SpecialOfficial), false, officialDefinition);
      specialOfficial.SetPropertyDefinitions(new PropertyDefinitionCollection());

      return specialOfficial;
    }

    private ClassDefinition CreateOrderTicketDefinition (ClassDefinition baseClass)
    {
      ClassDefinition orderTicket = CreateClassDefinition("OrderTicket", "OrderTicket", typeof(OrderTicket), false, baseClass);

      var properties = new List<PropertyDefinition>();
      properties.Add(
          CreatePersistentPropertyDefinition(orderTicket, typeof(OrderTicket), "FileName", "FileName", false, 255));
      properties.Add(
          CreatePersistentPropertyDefinition(orderTicket, typeof(OrderTicket), "Order", "OrderID", true, null));
      properties.Add(
          CreateTransactionPropertyDefinition(orderTicket, typeof(OrderTicket), "Int32TransactionProperty", false, null));
      orderTicket.SetPropertyDefinitions(new PropertyDefinitionCollection(properties, true));

      return orderTicket;
    }

    private ClassDefinition CreateOrderItemDefinition (ClassDefinition baseClass)
    {
      ClassDefinition orderItem = CreateClassDefinition("OrderItem", "OrderItem", typeof(OrderItem), false, baseClass);

      var properties = new List<PropertyDefinition>();
      properties.Add(
          CreatePersistentPropertyDefinition(orderItem, typeof(OrderItem), "Order", "OrderID", true, null));
      properties.Add(
          CreatePersistentPropertyDefinition(orderItem, typeof(OrderItem), "Position", "Position", false, null));
      properties.Add(
          CreatePersistentPropertyDefinition(orderItem, typeof(OrderItem), "Product", "Product", false, 100));
      orderItem.SetPropertyDefinitions(new PropertyDefinitionCollection(properties, true));

      return orderItem;
    }

    private ClassDefinition CreateProductDefinition (ClassDefinition baseClass)
    {
      var product = CreateClassDefinition("Product", "Product", typeof(Product), false, baseClass);

      var properties = new List<PropertyDefinition>();
      properties.Add(
          CreatePersistentPropertyDefinition(product, typeof(Product), "Name", "Name", false, 100));
      properties.Add(
          CreatePersistentPropertyDefinition(product, typeof(Product), "Price", "Price", false, null));
      product.SetPropertyDefinitions(new PropertyDefinitionCollection(properties, true));

      return product;
    }

    private ClassDefinition CreateProductReviewDefinition (ClassDefinition baseClass)
    {
      var productReview = CreateClassDefinition("ProductReview", "ProductReview", typeof(ProductReview), false, baseClass);

      var properties = new List<PropertyDefinition>();
      properties.Add(
          CreatePersistentPropertyDefinition(productReview, typeof(ProductReview), "Product", "ProductID", true, null));
      properties.Add(
          CreatePersistentPropertyDefinition(productReview, typeof(ProductReview), "Reviewer", "ReviewerID", true, null));
      properties.Add(
          CreatePersistentPropertyDefinition(productReview, typeof(ProductReview), "CreatedAt", "CreatedAt", false, null));
      properties.Add(
          CreatePersistentPropertyDefinition(productReview, typeof(ProductReview), "Comment", "Comment", false, 1000));
      productReview.SetPropertyDefinitions(new PropertyDefinitionCollection(properties, true));

      return productReview;
    }

    private ClassDefinition CreateCeoDefinition (ClassDefinition baseClass)
    {
      ClassDefinition ceo = CreateClassDefinition(
          "Ceo", "Ceo", typeof(Ceo), false, baseClass);

      var properties = new List<PropertyDefinition>();
      properties.Add(
          CreatePersistentPropertyDefinition(
              ceo, typeof(Ceo), "Name", "Name", false, 100));
      properties.Add(
          CreatePersistentPropertyDefinition(ceo, typeof(Ceo), "Company", "CompanyID", true, null));
      ceo.SetPropertyDefinitions(new PropertyDefinitionCollection(properties, true));

      return ceo;
    }

    private ClassDefinition CreatePersonDefinition (ClassDefinition baseClass)
    {
      ClassDefinition person = CreateClassDefinition(
          "Person", "Person", typeof(Person), false, baseClass);

      var properties = new List<PropertyDefinition>();
      properties.Add(
          CreatePersistentPropertyDefinition(
              person, typeof(Person), "Name", "Name", false, 100));
      person.SetPropertyDefinitions(new PropertyDefinitionCollection(properties, true));

      return person;
    }

    private ClassDefinition CreateClientDefinition (ClassDefinition baseClass)
    {
      ClassDefinition client = CreateClassDefinition(
          "Client", "Client", typeof(Client), false, baseClass);

      var properties = new List<PropertyDefinition>();
      properties.Add(
          CreatePersistentPropertyDefinition(client, typeof(Client), "ParentClient", "ParentClientID", true, null));
      client.SetPropertyDefinitions(new PropertyDefinitionCollection(properties, true));

      return client;
    }

    private ClassDefinition CreateLocationDefinition (ClassDefinition baseClass)
    {
      ClassDefinition location = CreateClassDefinition(
          "Location", "Location", typeof(Location), false, baseClass);

      var properties = new List<PropertyDefinition>();
      properties.Add(
          CreatePersistentPropertyDefinition(location, typeof(Location), "Client", "ClientID", true, null));
      location.SetPropertyDefinitions(new PropertyDefinitionCollection(properties, true));

      return location;
    }

    private ClassDefinition CreateFileSystemItemDefinition (ClassDefinition baseClass)
    {
      ClassDefinition fileSystemItem = CreateClassDefinition(
          "FileSystemItem", "FileSystemItem", typeof(FileSystemItem), false, baseClass);

      var properties = new List<PropertyDefinition>();
      properties.Add(
          CreatePersistentPropertyDefinition(fileSystemItem, typeof(FileSystemItem), "ParentFolder", "ParentFolderID", true, null));
      fileSystemItem.SetPropertyDefinitions(new PropertyDefinitionCollection(properties, true));

      return fileSystemItem;
    }

    private ClassDefinition CreateFolderDefinition (ClassDefinition baseClass)
    {
      ClassDefinition folder = CreateClassDefinition(
          "Folder", "FileSystemItem", typeof(Folder), false, baseClass);
      folder.SetPropertyDefinitions(new PropertyDefinitionCollection());

      return folder;
    }

    private ClassDefinition CreateFileDefinition (ClassDefinition baseClass)
    {
      ClassDefinition file = CreateClassDefinition(
          "File", "FileSystemItem", typeof(File), false, baseClass);
      file.SetPropertyDefinitions(new PropertyDefinitionCollection());

      return file;
    }

    //TODO: remove Date and NaDate properties
    private ClassDefinition CreateClassWithAllDataTypesDefinition (ClassDefinition baseClass)
    {
      ClassDefinition classWithAllDataTypes = CreateClassDefinition("ClassWithAllDataTypes", "TableWithAllDataTypes", typeof(ClassWithAllDataTypes), false, baseClass);

      var properties = new List<PropertyDefinition>();
      properties.Add(
          CreatePersistentPropertyDefinition(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "BooleanProperty", "Boolean", false, null));
      properties.Add(
          CreatePersistentPropertyDefinition(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "ByteProperty", "Byte", false, null));
      properties.Add(
          CreatePersistentPropertyDefinition(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "DateProperty", "Date", false, null));
      properties.Add(
          CreatePersistentPropertyDefinition(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "DateTimeProperty", "DateTime", false, null));
      properties.Add(
          CreatePersistentPropertyDefinition(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "DecimalProperty", "Decimal", false, null));
      properties.Add(
          CreatePersistentPropertyDefinition(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "DoubleProperty", "Double", false, null));
      properties.Add(
          CreatePersistentPropertyDefinition(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "EnumProperty", "Enum", false, null));
      properties.Add(
          CreatePersistentPropertyDefinition(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "ExtensibleEnumProperty", "ExtensibleEnum", false, null));
      properties.Add(
          CreatePersistentPropertyDefinition(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "FlagsProperty", "Flags", false, null));
      properties.Add(
          CreatePersistentPropertyDefinition(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "GuidProperty", "Guid", false, null));
      properties.Add(
          CreatePersistentPropertyDefinition(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "Int16Property", "Int16", false, null));
      properties.Add(
          CreatePersistentPropertyDefinition(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "Int32Property", "Int32", false, null));
      properties.Add(
          CreatePersistentPropertyDefinition(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "Int64Property", "Int64", false, null));
      properties.Add(
          CreatePersistentPropertyDefinition(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "SingleProperty", "Single", false, null));
      properties.Add(
          CreatePersistentPropertyDefinition(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "StringProperty", "String", false, 100));
      properties.Add(
          CreatePersistentPropertyDefinition(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "StringPropertyWithoutMaxLength", "StringWithoutMaxLength", false, null));
      properties.Add(
          CreatePersistentPropertyDefinition(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "BinaryProperty", "Binary", false, null));

      properties.Add(
          CreatePersistentPropertyDefinition(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaBooleanProperty", "NaBoolean", true, null));
      properties.Add(
          CreatePersistentPropertyDefinition(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaByteProperty", "NaByte", true, null));
      properties.Add(
          CreatePersistentPropertyDefinition(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaDateProperty", "NaDate", true, null));
      properties.Add(
          CreatePersistentPropertyDefinition(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaDateTimeProperty", "NaDateTime", true, null));
      properties.Add(
          CreatePersistentPropertyDefinition(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaDecimalProperty", "NaDecimal", true, null));
      properties.Add(
          CreatePersistentPropertyDefinition(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaDoubleProperty", "NaDouble", true, null));
      properties.Add(
          CreatePersistentPropertyDefinition(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaEnumProperty", "NaEnum", true, null));
      properties.Add(
          CreatePersistentPropertyDefinition(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaFlagsProperty", "NaFlags", true, null));
      properties.Add(
          CreatePersistentPropertyDefinition(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaGuidProperty", "NaGuid", true, null));
      properties.Add(
          CreatePersistentPropertyDefinition(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaInt16Property", "NaInt16", true, null));
      properties.Add(
          CreatePersistentPropertyDefinition(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaInt32Property", "NaInt32", true, null));
      properties.Add(
          CreatePersistentPropertyDefinition(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaInt64Property", "NaInt64", true, null));
      properties.Add(
          CreatePersistentPropertyDefinition(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaSingleProperty", "NaSingle", true, null));
      properties.Add(
          CreatePersistentPropertyDefinition(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "StringWithNullValueProperty", "StringWithNullValue", true, 100));
      properties.Add(
          CreatePersistentPropertyDefinition(
              classWithAllDataTypes,
              typeof(ClassWithAllDataTypes),
              "ExtensibleEnumWithNullValueProperty",
              "ExtensibleEnumWithNullValue",
              true,
              null));
      properties.Add(
          CreatePersistentPropertyDefinition(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaBooleanWithNullValueProperty", "NaBooleanWithNullValue", true, null));
      properties.Add(
          CreatePersistentPropertyDefinition(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaByteWithNullValueProperty", "NaByteWithNullValue", true, null));
      properties.Add(
          CreatePersistentPropertyDefinition(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaDateWithNullValueProperty", "NaDateWithNullValue", true, null));
      properties.Add(
          CreatePersistentPropertyDefinition(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaDateTimeWithNullValueProperty", "NaDateTimeWithNullValue", true, null));
      properties.Add(
          CreatePersistentPropertyDefinition(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaDecimalWithNullValueProperty", "NaDecimalWithNullValue", true, null));
      properties.Add(
          CreatePersistentPropertyDefinition(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaDoubleWithNullValueProperty", "NaDoubleWithNullValue", true, null));
      properties.Add(
          CreatePersistentPropertyDefinition(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaEnumWithNullValueProperty", "NaEnumWithNullValue", true, null));
      properties.Add(
          CreatePersistentPropertyDefinition(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaFlagsWithNullValueProperty", "NaFlagsWithNullValue", true, null));
      properties.Add(
          CreatePersistentPropertyDefinition(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaGuidWithNullValueProperty", "NaGuidWithNullValue", true, null));
      properties.Add(
          CreatePersistentPropertyDefinition(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaInt16WithNullValueProperty", "NaInt16WithNullValue", true, null));
      properties.Add(
          CreatePersistentPropertyDefinition(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaInt32WithNullValueProperty", "NaInt32WithNullValue", true, null));
      properties.Add(
          CreatePersistentPropertyDefinition(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaInt64WithNullValueProperty", "NaInt64WithNullValue", true, null));
      properties.Add(
          CreatePersistentPropertyDefinition(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "NaSingleWithNullValueProperty", "NaSingleWithNullValue", true, null));
      properties.Add(
          CreatePersistentPropertyDefinition(classWithAllDataTypes, typeof(ClassWithAllDataTypes), "NullableBinaryProperty", "NullableBinary", true, 1000000));
      classWithAllDataTypes.SetPropertyDefinitions(new PropertyDefinitionCollection(properties, true));

      return classWithAllDataTypes;
    }

    private ClassDefinition CreateClassWithDefaultStorageClassTransaction (ClassDefinition baseClass)
    {
      ClassDefinition classDefinition = CreateClassDefinition(
          "ClassWithDefaultStorageClassTransaction",
          "TableWithDefaultStorageClassTransaction",
          typeof(ClassWithDefaultStorageClassTransaction),
          false,
          baseClass);

      var properties = new List<PropertyDefinition>();
      properties.Add(
          CreateTransactionPropertyDefinition(classDefinition, typeof(ClassWithDefaultStorageClassTransaction), "NoAttribute", false, null));
      properties.Add(
          CreatePersistentPropertyDefinition(classDefinition, typeof(ClassWithDefaultStorageClassTransaction), "Persistent", "Persistent", false, null));
      properties.Add(
          CreateTransactionPropertyDefinition(classDefinition, typeof(ClassWithDefaultStorageClassTransaction), "Transaction", false, null));
      classDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection(properties, true));

      return classDefinition;
    }

    private ClassDefinition CreateClassWithGuidKeyDefinition (ClassDefinition baseClass)
    {
      ClassDefinition classDefinition = CreateClassDefinition(
          "ClassWithGuidKey",
          "TableWithGuidKey",
          typeof(ClassWithGuidKey),
          false,
          baseClass);
      classDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection());

      return classDefinition;
    }

    private ClassDefinition CreateClassWithInvalidKeyTypeDefinition (ClassDefinition baseClass)
    {
      ClassDefinition classDefinition = CreateClassDefinition(
          "ClassWithKeyOfInvalidType",
          "TableWithKeyOfInvalidType",
          typeof(ClassWithKeyOfInvalidType),
          false,
          baseClass);
      classDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection());

      return classDefinition;
    }

    private ClassDefinition CreateClassWithoutIDColumnDefinition (ClassDefinition baseClass)
    {
      ClassDefinition classDefinition = CreateClassDefinition(
          "ClassWithoutIDColumn",
          "TableWithoutIDColumn",
          typeof(ClassWithoutIDColumn),
          false,
          baseClass);
      classDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection());

      return classDefinition;
    }

    private ClassDefinition CreateClassWithoutClassIDColumnDefinition (ClassDefinition baseClass)
    {
      ClassDefinition classDefinition = CreateClassDefinition(
          "ClassWithoutClassIDColumn",
          "TableWithoutClassIDColumn",
          typeof(ClassWithoutClassIDColumn),
          false,
          baseClass);
      classDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection());

      return classDefinition;
    }

    private ClassDefinition CreateClassWithoutTimestampColumnDefinition (ClassDefinition baseClass)
    {
      ClassDefinition classDefinition =
          CreateClassDefinition(
              "ClassWithoutTimestampColumn",
              "TableWithoutTimestampColumn",
              typeof(ClassWithoutTimestampColumn),
              false,
              baseClass);
      classDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection());

      return classDefinition;
    }

    private ClassDefinition CreateClassWithValidRelationsDefinition (ClassDefinition baseClass)
    {
      ClassDefinition classDefinition = CreateClassDefinition(
          "ClassWithValidRelations",
          "TableWithValidRelations",
          typeof(ClassWithValidRelations),
          false,
          baseClass);

      var properties = new List<PropertyDefinition>();
      properties.Add(
          CreatePersistentPropertyDefinition(classDefinition, typeof(ClassWithValidRelations), "ClassWithGuidKeyOptional", "TableWithGuidKeyOptionalID", true, null));

      properties.Add(
          CreatePersistentPropertyDefinition(classDefinition, typeof(ClassWithValidRelations), "ClassWithGuidKeyNonOptional", "TableWithGuidKeyNonOptionalID", true, null));
      classDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection(properties, true));

      return classDefinition;
    }

    private ClassDefinition CreateClassWithInvalidRelationDefinition (ClassDefinition baseClass)
    {
      ClassDefinition classDefinition = CreateClassDefinition(
          "ClassWithInvalidRelation",
          "TableWithInvalidRelation",
          typeof(ClassWithInvalidRelation),
          false,
          baseClass);

      var properties = new List<PropertyDefinition>();
      properties.Add(
          CreatePersistentPropertyDefinition(classDefinition, typeof(ClassWithInvalidRelation), "ClassWithGuidKey", "TableWithGuidKeyID", true, null));
      classDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection(properties, true));

      return classDefinition;
    }

    private ClassDefinition CreateClassWithOptionalOneToOneRelationAndOppositeDerivedClassDefinition (ClassDefinition baseClass)
    {
      ClassDefinition classDefinition =
          CreateClassDefinition(
              "ClassWithOptionalOneToOneRelationAndOppositeDerivedClass",
              "TableWithOptionalOneToOneRelationAndOppositeDerivedClass",
              typeof(ClassWithOptionalOneToOneRelationAndOppositeDerivedClass),
              false,
              baseClass);

      var properties = new List<PropertyDefinition>();
      properties.Add(
          CreatePersistentPropertyDefinition(classDefinition, typeof(ClassWithOptionalOneToOneRelationAndOppositeDerivedClass), "Company", "CompanyID", true, null));
      classDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection(properties, true));

      return classDefinition;
    }

    private ClassDefinition CreateIndustrialSectorDefinition (ClassDefinition baseClass)
    {
      ClassDefinition industrialSector = CreateClassDefinition("IndustrialSector", "IndustrialSector", typeof(IndustrialSector), false, baseClass);

      var properties = new List<PropertyDefinition>();
      properties.Add(
          CreatePersistentPropertyDefinition(
              industrialSector, typeof(IndustrialSector), "Name", "Name", false, 100));
      industrialSector.SetPropertyDefinitions(new PropertyDefinitionCollection(properties, true));

      return industrialSector;
    }

    private ClassDefinition CreateEmployeeDefinition (ClassDefinition baseClass)
    {
      ClassDefinition employee = CreateClassDefinition("Employee", "Employee", typeof(Employee), false, baseClass);

      var properties = new List<PropertyDefinition>();
      properties.Add(
          CreatePersistentPropertyDefinition(
              employee, typeof(Employee), "Name", "Name", false, 100));
      properties.Add(
          CreatePersistentPropertyDefinition(employee, typeof(Employee), "Supervisor", "SupervisorID", true, null));
      employee.SetPropertyDefinitions(new PropertyDefinitionCollection(properties, true));

      return employee;
    }

    private ClassDefinition CreateTargetClassForPersistentMixinDefinition (ClassDefinition baseClass)
    {
      ClassDefinition targetClassForPersistentMixin = CreateClassDefinition(
          "TargetClassForPersistentMixin",
          "MixedDomains_Target",
          typeof(TargetClassForPersistentMixin),
          false,
          baseClass,
          typeof(MixinAddingPersistentProperties));

      var properties = new List<PropertyDefinition>();
      properties.Add(
          CreatePersistentPropertyDefinition(
              targetClassForPersistentMixin, typeof(MixinAddingPersistentProperties), "PersistentProperty", "PersistentProperty", false, null));
      properties.Add(
          CreatePersistentPropertyDefinition(
              targetClassForPersistentMixin, typeof(MixinAddingPersistentProperties), "ExtraPersistentProperty", "ExtraPersistentProperty", false, null));
      properties.Add(
          CreatePersistentPropertyDefinition(
              targetClassForPersistentMixin, typeof(MixinAddingPersistentProperties), "UnidirectionalRelationProperty", "UnidirectionalRelationPropertyID", true, null));
      properties.Add(
          CreatePersistentPropertyDefinition(
              targetClassForPersistentMixin, typeof(MixinAddingPersistentProperties), "RelationProperty", "RelationPropertyID", true, null));
      properties.Add(
          CreatePersistentPropertyDefinition(
              targetClassForPersistentMixin, typeof(MixinAddingPersistentProperties), "CollectionPropertyNSide", "CollectionPropertyNSideID", true, null));
      properties.Add(
          CreatePersistentPropertyDefinition(
              targetClassForPersistentMixin, typeof(BaseForMixinAddingPersistentProperties), "PrivateBaseRelationProperty", "PrivateBaseRelationPropertyID", true, null));
      targetClassForPersistentMixin.SetPropertyDefinitions(new PropertyDefinitionCollection(properties, true));

      return targetClassForPersistentMixin;
    }

    private ClassDefinition CreateDerivedTargetClassForPersistentMixinDefinition (ClassDefinition baseClass)
    {
      ClassDefinition derivedTargetClassForPersistentMixin = CreateClassDefinition(
          "DerivedTargetClassForPersistentMixin",
          "MixedDomains_Target",
          typeof(DerivedTargetClassForPersistentMixin),
          false,
          baseClass);
      derivedTargetClassForPersistentMixin.SetPropertyDefinitions(new PropertyDefinitionCollection());

      return derivedTargetClassForPersistentMixin;
    }

    private ClassDefinition CreateDerivedDerivedTargetClassForPersistentMixinDefinition (ClassDefinition baseClass)
    {
      ClassDefinition derivedDerivedTargetClassForPersistentMixin = CreateClassDefinition(
          "DerivedDerivedTargetClassForPersistentMixin",
          "MixedDomains_Target",
          typeof(DerivedDerivedTargetClassForPersistentMixin),
          false,
          baseClass);
      derivedDerivedTargetClassForPersistentMixin.SetPropertyDefinitions(new PropertyDefinitionCollection());

      return derivedDerivedTargetClassForPersistentMixin;
    }

    private ClassDefinition CreateComputerDefinition (ClassDefinition baseClass)
    {
      ClassDefinition computer = CreateClassDefinition("Computer", "Computer", typeof(Computer), false, baseClass);

      var properties = new List<PropertyDefinition>();
      properties.Add(
          CreatePersistentPropertyDefinition(computer, typeof(Computer), "SerialNumber", "SerialNumber", false, 20));
      properties.Add(
          CreatePersistentPropertyDefinition(computer, typeof(Computer), "Employee", "EmployeeID", true, null));
      properties.Add(
          CreateTransactionPropertyDefinition(computer, typeof(Computer), "Int32TransactionProperty", false, null));
      properties.Add(
          CreateTransactionPropertyDefinition(computer, typeof(Computer), "DateTimeTransactionProperty", false, null));
      properties.Add(
          CreateTransactionPropertyDefinition(computer, typeof(Computer), "EmployeeTransactionProperty", true, null));
      computer.SetPropertyDefinitions(new PropertyDefinitionCollection(properties, true));

      return computer;
    }

    private ClassDefinition CreateClassWithRelatedClassIDColumnAndNoInheritanceDefinition (ClassDefinition baseClass)
    {
      ClassDefinition classDefinition =
          CreateClassDefinition(
              "ClassWithRelatedClassIDColumnAndNoInheritance",
              "TableWithRelatedClassIDColumnAndNoInheritance",
              typeof(ClassWithRelatedClassIDColumnAndNoInheritance),
              false,
              baseClass);

      var properties = new List<PropertyDefinition>();
      properties.Add(
          CreatePersistentPropertyDefinition(classDefinition, typeof(ClassWithRelatedClassIDColumnAndNoInheritance), "ClassWithGuidKey", "TableWithGuidKeyID", true, null));
      classDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection(properties, true));

      return classDefinition;
    }

    private ClassDefinition CreateRelationTargetForPersistentMixinDefinition (ClassDefinition baseClass)
    {
      ClassDefinition relationTargetForPersistentMixinDefinition = CreateClassDefinition(
          "RelationTargetForPersistentMixin",
          "MixedDomains_RelationTarget",
          typeof(RelationTargetForPersistentMixin),
          false,
          baseClass);

      var properties = new List<PropertyDefinition>();
      properties.Add(
          CreatePersistentPropertyDefinition(
              relationTargetForPersistentMixinDefinition,
              typeof(RelationTargetForPersistentMixin),
              "RelationProperty2",
              "RelationProperty2ID",
              true,
              null));
      properties.Add(
          CreatePersistentPropertyDefinition(
              relationTargetForPersistentMixinDefinition,
              typeof(RelationTargetForPersistentMixin),
              "RelationProperty3",
              "RelationProperty3ID",
              true,
              null));
      relationTargetForPersistentMixinDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection(properties, true));

      return relationTargetForPersistentMixinDefinition;
    }

    private ClassDefinition CreateClassDefinition (
        string id,
        string entityName,
        Type classType,
        bool isAbstract,
        ClassDefinition baseClass,
        params Type[] persistentMixins)
    {
      // Don't use ClassDefinitionObjectMother: since this configuration is compared with the actual configuration, we must exactly define 
      // the mapping objects
      var classDefinition = new ClassDefinition(
          id,
          classType,
          isAbstract,
          baseClass,
          null,
          DefaultStorageClass.Persistent,
          new PersistentMixinFinderStub(classType, persistentMixins),
          MappingReflectorObjectMother.DomainObjectCreator);
      SetFakeStorageEntity(classDefinition, entityName);
      return classDefinition;
    }

    private ClassDefinition CreateClassDefinitionForNonPersistentProvider (
        string id,
        Type classType,
        bool isAbstract,
        ClassDefinition baseClass,
        params Type[] persistentMixins)
    {
      // Don't use ClassDefinitionObjectMother: since this configuration is compared with the actual configuration, we must exactly define 
      // the mapping objects
      var classDefinition = new ClassDefinition(
          id,
          classType,
          isAbstract,
          baseClass,
          null,
          DefaultStorageClass.Transaction,
          new PersistentMixinFinderStub(classType, persistentMixins),
          MappingReflectorObjectMother.DomainObjectCreator);
      classDefinition.SetStorageEntity(new NonPersistentStorageEntity(_nonPersistentProviderDefinition));
      return classDefinition;
    }

    private void SetFakeStorageEntity (ClassDefinition classDefinition, string entityName)
    {
      var tableDefinition = TableDefinitionObjectMother.Create(
          _defaultStorageProviderDefinition,
          new EntityNameDefinition(null, entityName),
          new EntityNameDefinition(null, classDefinition.ID + "View"));
      classDefinition.SetStorageEntity(tableDefinition);
    }

    private PropertyDefinition CreatePersistentPropertyDefinition (
        ClassDefinition classDefinition, Type declaringType, string propertyName, string columnName, bool isNullable, int? maxLength)
    {
      var propertyDefinition = CreatePropertyDefinition(classDefinition, declaringType, propertyName, isNullable, maxLength, StorageClass.Persistent);
      propertyDefinition.SetStorageProperty(new FakeStoragePropertyDefinition(columnName));
      return propertyDefinition;
    }

    private PropertyDefinition CreateTransactionPropertyDefinition (
        ClassDefinition classDefinition, Type declaringType, string propertyName, bool isNullable, int? maxLength)
    {
      return CreatePropertyDefinition(classDefinition, declaringType, propertyName, isNullable, maxLength, StorageClass.Transaction);
    }

    private PropertyDefinition CreatePropertyDefinition (
        ClassDefinition classDefinition, Type declaringType, string propertyName, bool isNullable, int? maxLength, StorageClass storageClass)
    {
      var propertyInfo = declaringType.GetProperty(
          propertyName,
          BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
      Assertion.IsNotNull(propertyInfo);
      var propertyInformation = PropertyInfoAdapter.Create(propertyInfo);
      var fullName = declaringType.FullName + "." + propertyName;

      // Don't use PropertyDefinitionObjectMother: since this configuration is compared with the actual configuration, we must exactly define 
      // the mapping objects
      return new PropertyDefinition(
          classDefinition,
          propertyInformation,
          fullName,
          ReflectionUtility.IsDomainObject(propertyInformation.PropertyType),
          isNullable,
          maxLength,
          storageClass,
          propertyInformation.PropertyType.IsValueType ? Activator.CreateInstance(propertyInformation.PropertyType) : null);
    }

    #endregion

    #region Methods for creating relation definitions

    private Dictionary<string, RelationDefinition> CreateRelationDefinitions ()
    {
      var relationDefinitions = new List<RelationDefinition>();

      relationDefinitions.Add(CreateCustomerToOrderRelationDefinition());

      relationDefinitions.Add(CreateOrderToOrderItemRelationDefinition());
      relationDefinitions.Add(CreateOrderToOrderTicketRelationDefinition());
      relationDefinitions.Add(CreateOrderToOfficialRelationDefinition());
      relationDefinitions.Add(CreateOrderViewModelToOrderRelationDefinition());

      relationDefinitions.Add(CreateProductToProductReviewRelationDefinition());
      relationDefinitions.Add(CreatePersonToProductReviewRelationDefinition());

      relationDefinitions.Add(CreateCompanyToCeoRelationDefinition());
      relationDefinitions.Add(CreatePartnerToPersonRelationDefinition());
      relationDefinitions.Add(CreateClientToLocationRelationDefinition());
      relationDefinitions.Add(CreateParentClientToChildClientRelationDefinition());
      relationDefinitions.Add(CreateFolderToFileSystemItemRelationDefinition());

      relationDefinitions.Add(CreateCompanyToClassWithOptionalOneToOneRelationAndOppositeDerivedClassRelationDefinition());
      relationDefinitions.Add(CreateClassWithGuidKeyToClassWithValidRelationsOptional());
      relationDefinitions.Add(CreateClassWithGuidKeyToClassWithValidRelationsNonOptional());
      relationDefinitions.Add(CreateClassWithGuidKeyToClassWithInvalidRelation());
      relationDefinitions.Add(CreateClassWithGuidKeyToClassWithRelatedClassIDColumnAndNoInheritanceRelation());
      relationDefinitions.Add(CreateIndustrialSectorToCompanyRelationDefinition());
      relationDefinitions.Add(CreateSupervisorToSubordinateRelationDefinition());
      relationDefinitions.Add(CreateEmployeeToComputerRelationDefinition());

      relationDefinitions.Add(CreateTargetClassForPersistentMixinMixedUnidirectionalRelationDefinition());
      relationDefinitions.Add(CreateTargetClassForPersistentMixinMixedRelationPropertyRelationDefinition());
      relationDefinitions.Add(CreateTargetClassForPersistentMixinMixedVirtualRelationPropertyRelationDefinition());
      relationDefinitions.Add(CreateTargetClassForPersistentMixinMixedCollectionProperty1SideCreateTargetClassForPersistentMixinMixedCollectionPropertyRelationDefinition());
      relationDefinitions.Add(CreateTargetClassForPersistentMixinMixedCollectionPropertyNSideRelationDefinition());

      CalculateAndSetRelationEndPointDefinitions(relationDefinitions);

      return relationDefinitions.ToDictionary(rd => rd.ID);
    }

    private void CalculateAndSetRelationEndPointDefinitions (ICollection<RelationDefinition> relationDefinitions)
    {
      var relationsByClass = (from relationDefinition in relationDefinitions
                              from endPoint in relationDefinition.EndPointDefinitions
                              where !endPoint.IsAnonymous
                              group endPoint by endPoint.ClassDefinition)
                             .ToDictionary(grouping => grouping.Key, grouping => (IEnumerable<IRelationEndPointDefinition>)grouping);

      foreach (var classDefinition in _typeDefinitions.Values)
      {
        IEnumerable<IRelationEndPointDefinition> relationEndPointsDefinitionsForClass;

        if (!relationsByClass.TryGetValue(classDefinition, out relationEndPointsDefinitionsForClass))
          relationEndPointsDefinitionsForClass = Enumerable.Empty<IRelationEndPointDefinition>();

        classDefinition.SetRelationEndPointDefinitions(new RelationEndPointDefinitionCollection(relationEndPointsDefinitionsForClass, true));
      }
    }

    private RelationDefinition CreateCustomerToOrderRelationDefinition ()
    {
      var customer = _typeDefinitions[typeof(Customer)];
      var orderClass = _typeDefinitions[typeof(Order)];
      var orderNumber = orderClass.GetPropertyDefinition(typeof(Order).FullName + ".OrderNumber");
      var sortExpressionDefinition = new SortExpressionDefinition(new[] { SortExpressionDefinitionObjectMother.CreateSortedPropertyAscending(orderNumber) });

      var endPoint1 =
          DomainObjectCollectionRelationEndPointDefinitionFactory.Create(
              customer,
              "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Customer.Orders",
              false,
              typeof(OrderCollection),
              new Lazy<SortExpressionDefinition>(() => sortExpressionDefinition));

      var endPoint2 = new RelationEndPointDefinition(
          orderClass["Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order.Customer"], true);

      var relation = CreateExpectedRelationDefinition("Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order"
        + ":Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order.Customer->"
        +"Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Customer.Orders", endPoint1, endPoint2);

      return relation;
    }

    private RelationDefinition CreateOrderToOrderTicketRelationDefinition ()
    {
      ClassDefinition orderClass = _typeDefinitions[typeof(Order)];
      VirtualObjectRelationEndPointDefinition endPoint1 =
          VirtualObjectRelationEndPointDefinitionFactory.Create(
              orderClass, "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order.OrderTicket", true, typeof(OrderTicket));

      ClassDefinition orderTicketClass = _typeDefinitions[typeof(OrderTicket)];
      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition(
          orderTicketClass["Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.OrderTicket.Order"], true);

      RelationDefinition relation = CreateExpectedRelationDefinition(
          "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.OrderTicket"
          + ":Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.OrderTicket.Order->"
          +"Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order.OrderTicket", endPoint1, endPoint2);

      return relation;
    }

    private RelationDefinition CreateOrderToOrderItemRelationDefinition ()
    {
      ClassDefinition orderClass = _typeDefinitions[typeof(Order)];
      DomainObjectCollectionRelationEndPointDefinition endPoint1 =
          DomainObjectCollectionRelationEndPointDefinitionFactory.Create(
              orderClass,
              "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order.OrderItems",
              true,
              typeof(ObjectList<OrderItem>));

      ClassDefinition orderItemClass = _typeDefinitions[typeof(OrderItem)];
      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition(
          orderItemClass["Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.OrderItem.Order"], true);

      RelationDefinition relation = CreateExpectedRelationDefinition("Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.OrderItem"
        + ":Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.OrderItem.Order->"
        +"Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order.OrderItems", endPoint1, endPoint2);

      return relation;
    }

    private RelationDefinition CreateOrderToOfficialRelationDefinition ()
    {
      var officialClass = _typeDefinitions[typeof(Official)];

      var endPoint1 = DomainObjectCollectionRelationEndPointDefinitionFactory.Create(
              officialClass,
              "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Official.Orders",
              false,
              typeof(ObjectList<Order>));

      var orderClass = _typeDefinitions[typeof(Order)];

      var endPoint2 = new RelationEndPointDefinition(
          orderClass["Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order.Official"], true);

      var relation = CreateExpectedRelationDefinition("Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order"
        + ":Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order.Official->"
        +"Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Official.Orders", endPoint1, endPoint2);

      return relation;
    }

    private RelationDefinition CreateOrderViewModelToOrderRelationDefinition ()
    {
      var orderViewModelClass = _typeDefinitions[typeof(OrderViewModel)];

      var endPoint1 = new RelationEndPointDefinition(
          orderViewModelClass["Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.OrderViewModel.Object"], true);

      var orderClass = _typeDefinitions[typeof(Order)];

      var endPoint2 = new AnonymousRelationEndPointDefinition(orderClass);

      var relation = CreateExpectedRelationDefinition("Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.OrderViewModel"
          + ":Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.OrderViewModel.Object", endPoint1, endPoint2);

      return relation;
    }

    private RelationDefinition CreateProductToProductReviewRelationDefinition ()
    {
      var productClass = _typeDefinitions[typeof(Product)];
      var productReviewClass = _typeDefinitions[typeof(ProductReview)];
      var createdAtPropertyDefinition = productReviewClass.GetPropertyDefinition(typeof(ProductReview).FullName + ".CreatedAt");
      var sortExpressionDefinition = new SortExpressionDefinition(new[] { SortExpressionDefinitionObjectMother.CreateSortedPropertyDescending(createdAtPropertyDefinition) });

      var endPoint1 =
          VirtualCollectionRelationEndPointDefinitionFactory.Create(
              productClass,
              "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Product.Reviews",
              false,
              typeof(IObjectList<ProductReview>),
              new Lazy<SortExpressionDefinition>(() => sortExpressionDefinition));

      var endPoint2 = new RelationEndPointDefinition(
          productReviewClass["Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ProductReview.Product"],
          true);

      var relation = CreateExpectedRelationDefinition(
          "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ProductReview"
          + ":Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ProductReview.Product->"
          + "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Product.Reviews",
          endPoint1,
          endPoint2);

      return relation;
    }

    private RelationDefinition CreatePersonToProductReviewRelationDefinition ()
    {
      var personClass = _typeDefinitions[typeof(Person)];
      var endPoint1 =
          VirtualCollectionRelationEndPointDefinitionFactory.Create(
              personClass,
              "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Person.Reviews",
              false,
              typeof(IObjectList<ProductReview>));

      var productReviewClass = _typeDefinitions[typeof(ProductReview)];
      var endPoint2 = new RelationEndPointDefinition(
          productReviewClass["Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ProductReview.Reviewer"], true);

      var relation = CreateExpectedRelationDefinition("Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ProductReview"
                                                                      + ":Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ProductReview.Reviewer->"
                                                                      +"Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Person.Reviews", endPoint1, endPoint2);

      return relation;
    }

    private RelationDefinition CreateCompanyToCeoRelationDefinition ()
    {
      ClassDefinition companyClass = _typeDefinitions[typeof(Company)];

      VirtualObjectRelationEndPointDefinition endPoint1 =
          VirtualObjectRelationEndPointDefinitionFactory.Create(
              companyClass, "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Company.Ceo", true, typeof(Ceo));

      ClassDefinition ceoClass = _typeDefinitions[typeof(Ceo)];

      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition(
          ceoClass["Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Ceo.Company"], true);

      RelationDefinition relation = CreateExpectedRelationDefinition("Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Ceo"
        + ":Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Ceo.Company->"
        +"Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Company.Ceo", endPoint1, endPoint2);

      return relation;
    }

    private RelationDefinition CreatePartnerToPersonRelationDefinition ()
    {
      ClassDefinition partnerClass = _typeDefinitions[typeof(Partner)];
      RelationEndPointDefinition endPoint1 = new RelationEndPointDefinition(
          partnerClass["Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Partner.ContactPerson"], true);

      ClassDefinition personClass = _typeDefinitions[typeof(Person)];
      VirtualObjectRelationEndPointDefinition endPoint2 =
          VirtualObjectRelationEndPointDefinitionFactory.Create(
              personClass,
              "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Person.AssociatedPartnerCompany",
              false,
              typeof(Partner));

      RelationDefinition relation =
          CreateExpectedRelationDefinition("Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Partner"
            + ":Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Partner.ContactPerson->"
            +"Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Person.AssociatedPartnerCompany", endPoint1, endPoint2);

      return relation;
    }

    private RelationDefinition CreateParentClientToChildClientRelationDefinition ()
    {
      ClassDefinition clientClass = _typeDefinitions[typeof(Client)];

      AnonymousRelationEndPointDefinition endPoint1 = new AnonymousRelationEndPointDefinition(clientClass);
      RelationEndPointDefinition endPoint2 =
          new RelationEndPointDefinition(clientClass["Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Client.ParentClient"], false);
      RelationDefinition relation =
          CreateExpectedRelationDefinition("Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Client"
            +":Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Client.ParentClient", endPoint1, endPoint2);

      return relation;
    }

    private RelationDefinition CreateClientToLocationRelationDefinition ()
    {
      ClassDefinition clientClass = _typeDefinitions[typeof(Client)];
      ClassDefinition locationClass = _typeDefinitions[typeof(Location)];

      AnonymousRelationEndPointDefinition endPoint1 = new AnonymousRelationEndPointDefinition(clientClass);

      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition(
          locationClass["Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Location.Client"], true);

      RelationDefinition relation = CreateExpectedRelationDefinition("Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Location"
        +":Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Location.Client", endPoint1, endPoint2);

      return relation;
    }

    private RelationDefinition CreateFolderToFileSystemItemRelationDefinition ()
    {
      ClassDefinition fileSystemItemClass = _typeDefinitions[typeof(FileSystemItem)];
      RelationEndPointDefinition endPoint1 = new RelationEndPointDefinition(
          fileSystemItemClass["Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.FileSystemItem.ParentFolder"], false);

      ClassDefinition folderClass = _typeDefinitions[typeof(Folder)];
      DomainObjectCollectionRelationEndPointDefinition endPoint2 =
          DomainObjectCollectionRelationEndPointDefinitionFactory.Create(
              folderClass,
              "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Folder.FileSystemItems",
              false,
              typeof(ObjectList<FileSystemItem>));

      RelationDefinition relation =
          CreateExpectedRelationDefinition("Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.FileSystemItem"
            + ":Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.FileSystemItem.ParentFolder->"
            +"Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Folder.FileSystemItems", endPoint1, endPoint2);

      return relation;
    }

    private RelationDefinition CreateCompanyToClassWithOptionalOneToOneRelationAndOppositeDerivedClassRelationDefinition ()
    {
      ClassDefinition companyClass = _typeDefinitions[typeof(Company)];
      ClassDefinition classWithOptionalOneToOneRelationAndOppositeDerivedClass =
          _typeDefinitions[typeof(ClassWithOptionalOneToOneRelationAndOppositeDerivedClass)];

      AnonymousRelationEndPointDefinition endPoint1 = new AnonymousRelationEndPointDefinition(companyClass);

      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition(
          classWithOptionalOneToOneRelationAndOppositeDerivedClass[
          "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ClassWithOptionalOneToOneRelationAndOppositeDerivedClass.Company"],
          false);

      RelationDefinition relation =
          CreateExpectedRelationDefinition(
              "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ClassWithOptionalOneToOneRelationAndOppositeDerivedClass"
              +":Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ClassWithOptionalOneToOneRelationAndOppositeDerivedClass.Company",
              endPoint1,
              endPoint2);

      return relation;
    }

    private RelationDefinition CreateClassWithGuidKeyToClassWithValidRelationsOptional ()
    {
      ClassDefinition classWithGuidKey = _typeDefinitions[typeof(ClassWithGuidKey)];
      VirtualObjectRelationEndPointDefinition endPoint1 =
          VirtualObjectRelationEndPointDefinitionFactory.Create(
              classWithGuidKey,
              "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ClassWithGuidKey.ClassWithValidRelationsOptional",
              false,
              typeof(ClassWithValidRelations));

      ClassDefinition classWithValidRelations = _typeDefinitions[typeof(ClassWithValidRelations)];
      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition(
          classWithValidRelations["Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ClassWithValidRelations.ClassWithGuidKeyOptional"], false);

      RelationDefinition relation =
          CreateExpectedRelationDefinition(
              "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ClassWithValidRelations"
              + ":Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ClassWithValidRelations.ClassWithGuidKeyOptional->"
              +"Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ClassWithGuidKey.ClassWithValidRelationsOptional",
              endPoint1,
              endPoint2);

      return relation;
    }

    private RelationDefinition CreateClassWithGuidKeyToClassWithValidRelationsNonOptional ()
    {
      ClassDefinition classWithGuidKey = _typeDefinitions[typeof(ClassWithGuidKey)];
      VirtualObjectRelationEndPointDefinition endPoint1 =
          VirtualObjectRelationEndPointDefinitionFactory.Create(
              classWithGuidKey,
              "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ClassWithGuidKey.ClassWithValidRelationsNonOptional",
              true,
              typeof(ClassWithValidRelations));

      ClassDefinition classWithValidRelations = _typeDefinitions[typeof(ClassWithValidRelations)];
      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition(
          classWithValidRelations["Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ClassWithValidRelations.ClassWithGuidKeyNonOptional"], true);

      RelationDefinition relation =
          CreateExpectedRelationDefinition(
              "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ClassWithValidRelations:"
              +"Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ClassWithValidRelations.ClassWithGuidKeyNonOptional->"
              + "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ClassWithGuidKey.ClassWithValidRelationsNonOptional",
              endPoint1,
              endPoint2);

      return relation;
    }

    private RelationDefinition CreateClassWithGuidKeyToClassWithInvalidRelation ()
    {
      ClassDefinition classWithGuidKey = _typeDefinitions[typeof(ClassWithGuidKey)];
      VirtualObjectRelationEndPointDefinition endPoint1 =
          VirtualObjectRelationEndPointDefinitionFactory.Create(
              classWithGuidKey,
              "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ClassWithGuidKey.ClassWithInvalidRelation",
              false,
              typeof(ClassWithInvalidRelation));

      ClassDefinition classWithInvalidRelation = _typeDefinitions[typeof(ClassWithInvalidRelation)];
      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition(
          classWithInvalidRelation["Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ClassWithInvalidRelation.ClassWithGuidKey"], false);

      RelationDefinition relation =
          CreateExpectedRelationDefinition(
              "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ClassWithInvalidRelation:"
              +"Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ClassWithInvalidRelation.ClassWithGuidKey->"
              + "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ClassWithGuidKey.ClassWithInvalidRelation",
              endPoint1,
              endPoint2);

      return relation;
    }

    private RelationDefinition CreateClassWithGuidKeyToClassWithRelatedClassIDColumnAndNoInheritanceRelation ()
    {
      ClassDefinition classWithGuidKey = _typeDefinitions[typeof(ClassWithGuidKey)];
      VirtualObjectRelationEndPointDefinition endPoint1 =
          VirtualObjectRelationEndPointDefinitionFactory.Create(
              classWithGuidKey,
              "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ClassWithGuidKey.ClassWithRelatedClassIDColumnAndNoInheritance",
              false,
              typeof(ClassWithRelatedClassIDColumnAndNoInheritance));

      ClassDefinition classWithRelatedClassIDColumnAndNoInheritance = _typeDefinitions[typeof(ClassWithRelatedClassIDColumnAndNoInheritance)];
      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition(
          classWithRelatedClassIDColumnAndNoInheritance[
          "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ClassWithRelatedClassIDColumnAndNoInheritance.ClassWithGuidKey"],
          false);

      RelationDefinition relation =
          CreateExpectedRelationDefinition(
              "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ClassWithRelatedClassIDColumnAndNoInheritance:"
              +"Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ClassWithRelatedClassIDColumnAndNoInheritance.ClassWithGuidKey->"
              + "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ClassWithGuidKey.ClassWithRelatedClassIDColumnAndNoInheritance",
              endPoint1,
              endPoint2);

      return relation;
    }

    private RelationDefinition CreateIndustrialSectorToCompanyRelationDefinition ()
    {
      ClassDefinition industrialSectorClass = _typeDefinitions[typeof(IndustrialSector)];

      DomainObjectCollectionRelationEndPointDefinition endPoint1 =
          DomainObjectCollectionRelationEndPointDefinitionFactory.Create(
              industrialSectorClass,
              "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.IndustrialSector.Companies",
              true,
              typeof(ObjectList<Company>));

      ClassDefinition companyClass = _typeDefinitions[typeof(Company)];

      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition(
          companyClass["Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Company.IndustrialSector"], false);

      RelationDefinition relation =
          CreateExpectedRelationDefinition("Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Company"
            +":Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Company.IndustrialSector->"
            + "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.IndustrialSector.Companies",
            endPoint1, endPoint2);

      return relation;
    }

    private RelationDefinition CreateSupervisorToSubordinateRelationDefinition ()
    {
      ClassDefinition employeeClass = _typeDefinitions[typeof(Employee)];

      DomainObjectCollectionRelationEndPointDefinition endPoint1 =
          DomainObjectCollectionRelationEndPointDefinitionFactory.Create(
              employeeClass,
              "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Employee.Subordinates",
              false,
              typeof(ObjectList<Employee>));

      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition(
          employeeClass["Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Employee.Supervisor"], false);

      RelationDefinition relation =
          CreateExpectedRelationDefinition("Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Employee:"
            +"Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Employee.Supervisor->"
            +"Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Employee.Subordinates", endPoint1, endPoint2);

      return relation;
    }

    private RelationDefinition CreateEmployeeToComputerRelationDefinition ()
    {
      ClassDefinition employeeClass = _typeDefinitions[typeof(Employee)];

      VirtualObjectRelationEndPointDefinition endPoint1 =
          VirtualObjectRelationEndPointDefinitionFactory.Create(
              employeeClass, "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Employee.Computer", false, typeof(Computer));

      ClassDefinition computerClass = _typeDefinitions[typeof(Computer)];

      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition(
          computerClass["Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Computer.Employee"], false);

      RelationDefinition relation = CreateExpectedRelationDefinition(
          "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Computer:"
          +"Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Computer.Employee->"
          +"Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Employee.Computer", endPoint1, endPoint2);

      return relation;
    }

    private RelationDefinition CreateTargetClassForPersistentMixinMixedUnidirectionalRelationDefinition ()
    {
      ClassDefinition mixedClass = _typeDefinitions[typeof(TargetClassForPersistentMixin)];

      RelationEndPointDefinition endPoint1 = new RelationEndPointDefinition(
          mixedClass["Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.MixedMapping.MixinAddingPersistentProperties.UnidirectionalRelationProperty"],
          false);

      ClassDefinition relatedClass = _typeDefinitions[typeof(RelationTargetForPersistentMixin)];

      AnonymousRelationEndPointDefinition endPoint2 = new AnonymousRelationEndPointDefinition(relatedClass);

      RelationDefinition relation = CreateExpectedRelationDefinition(
          "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.MixedMapping.TargetClassForPersistentMixin:"
          + "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.MixedMapping.MixinAddingPersistentProperties.UnidirectionalRelationProperty",
          endPoint1,
          endPoint2);

      return relation;
    }

    private RelationDefinition CreateTargetClassForPersistentMixinMixedRelationPropertyRelationDefinition ()
    {
      ClassDefinition mixedClass = _typeDefinitions[typeof(TargetClassForPersistentMixin)];

      RelationEndPointDefinition endPoint1 = new RelationEndPointDefinition(
          mixedClass["Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.MixedMapping.MixinAddingPersistentProperties.RelationProperty"],
          false);

      ClassDefinition relatedClass = _typeDefinitions[typeof(RelationTargetForPersistentMixin)];

      var endPoint2 = VirtualObjectRelationEndPointDefinitionFactory.Create(
          relatedClass,
          "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.MixedMapping.RelationTargetForPersistentMixin.RelationProperty1",
          false,
          typeof(TargetClassForPersistentMixin));

      RelationDefinition relation = CreateExpectedRelationDefinition(
          "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.MixedMapping.TargetClassForPersistentMixin:"
          + "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.MixedMapping.MixinAddingPersistentProperties.RelationProperty->"
          + "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.MixedMapping.RelationTargetForPersistentMixin.RelationProperty1",
          endPoint1,
          endPoint2);

      return relation;
    }

    private RelationDefinition CreateTargetClassForPersistentMixinMixedVirtualRelationPropertyRelationDefinition ()
    {
      ClassDefinition mixedClass = _typeDefinitions[typeof(TargetClassForPersistentMixin)];

      var endPoint1 = VirtualObjectRelationEndPointDefinitionFactory.Create(
          mixedClass,
          "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.MixedMapping.MixinAddingPersistentProperties.VirtualRelationProperty",
          false,
          typeof(RelationTargetForPersistentMixin));

      ClassDefinition relatedClass = _typeDefinitions[typeof(RelationTargetForPersistentMixin)];

      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition(
          relatedClass["Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.MixedMapping.RelationTargetForPersistentMixin.RelationProperty2"],
          false);

      RelationDefinition relation = CreateExpectedRelationDefinition(
          "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.MixedMapping.RelationTargetForPersistentMixin"
          +":Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.MixedMapping.RelationTargetForPersistentMixin.RelationProperty2->"
          + "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.MixedMapping.MixinAddingPersistentProperties.VirtualRelationProperty",
          endPoint1,
          endPoint2);

      return relation;
    }

    private RelationDefinition CreateTargetClassForPersistentMixinMixedCollectionProperty1SideCreateTargetClassForPersistentMixinMixedCollectionPropertyRelationDefinition ()
    {
      ClassDefinition mixedClass = _typeDefinitions[typeof(TargetClassForPersistentMixin)];

      var endPoint1 = DomainObjectCollectionRelationEndPointDefinitionFactory.Create(
          mixedClass,
          "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.MixedMapping.MixinAddingPersistentProperties.CollectionProperty1Side",
          false,
          typeof(ObjectList<RelationTargetForPersistentMixin>));

      ClassDefinition relatedClass = _typeDefinitions[typeof(RelationTargetForPersistentMixin)];

      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition(
          relatedClass["Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.MixedMapping.RelationTargetForPersistentMixin.RelationProperty3"],
          false);

      RelationDefinition relation = CreateExpectedRelationDefinition(
          "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.MixedMapping.RelationTargetForPersistentMixin:"
          +"Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.MixedMapping.RelationTargetForPersistentMixin.RelationProperty3->"
          + "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.MixedMapping.MixinAddingPersistentProperties.CollectionProperty1Side",
          endPoint1,
          endPoint2);

      return relation;
    }

    private RelationDefinition CreateTargetClassForPersistentMixinMixedCollectionPropertyNSideRelationDefinition ()
    {
      ClassDefinition mixedClass = _typeDefinitions[typeof(TargetClassForPersistentMixin)];

      RelationEndPointDefinition endPoint1 = new RelationEndPointDefinition(
          mixedClass["Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.MixedMapping.MixinAddingPersistentProperties.CollectionPropertyNSide"],
          false);

      ClassDefinition relatedClass = _typeDefinitions[typeof(RelationTargetForPersistentMixin)];

      var endPoint2 = DomainObjectCollectionRelationEndPointDefinitionFactory.Create(
          relatedClass,
          "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.MixedMapping.RelationTargetForPersistentMixin.RelationProperty4",
          false,
          typeof(ObjectList<TargetClassForPersistentMixin>));

      RelationDefinition relation = CreateExpectedRelationDefinition(
          "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.MixedMapping.TargetClassForPersistentMixin:"
          + "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.MixedMapping.MixinAddingPersistentProperties.CollectionPropertyNSide->"
          + "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.MixedMapping.RelationTargetForPersistentMixin.RelationProperty4",
          endPoint1,
          endPoint2);

      return relation;
    }

    private RelationDefinition CreateExpectedRelationDefinition (string id, IRelationEndPointDefinition endPointDefinition1, IRelationEndPointDefinition endPointDefinition2)
    {
      var relationDefinition = new RelationDefinition(id, endPointDefinition1, endPointDefinition2);
      ((IRelationEndPointDefinitionSetter)endPointDefinition1).SetRelationDefinition(relationDefinition);
      ((IRelationEndPointDefinitionSetter)endPointDefinition2).SetRelationDefinition(relationDefinition);
      return relationDefinition;
    }

    #endregion
  }
}
