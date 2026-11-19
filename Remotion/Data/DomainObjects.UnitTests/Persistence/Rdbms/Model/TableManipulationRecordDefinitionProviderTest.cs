// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Parameters;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model.Building;
using Remotion.Data.DomainObjects.UnitTests.MixedDomains.TestDomain.SingleInheritance;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model;

[TestFixture]
public class TableManipulationRecordDefinitionProviderTest : StandardMappingTest
{
  private static readonly string[] s_orderedProductReviewInsertColumns = ["ID", "ClassID", "Comment", "CreatedAt", "ProductID", "ReviewerID"];
  private static readonly string[] s_orderedProductReviewUpdateColumns = ["ID", "ClassID", "Comment", "Comment__IsSet", "CreatedAt", "ProductID", "ReviewerID"];

  private TableManipulationRecordDefinitionProvider _tableManipulationRecordDefinitionProvider;
  private IInfrastructureStoragePropertyDefinitionProvider _infrastructureStoragePropertyDefinitionProvider;

  public override void SetUp ()
  {
    base.SetUp();

    var sqlStorageTypeInformationProvider = new SqlStorageTypeInformationProvider(new DateTimeDefaultStorageTypeProvider());
    var infrastructureStoragePropertyDefinitionProvider = new InfrastructureStoragePropertyDefinitionProvider(sqlStorageTypeInformationProvider, new ReflectionBasedStorageNameProvider());
    var rdbmsPersistenceModelProvider = new RdbmsPersistenceModelProvider();

    _tableManipulationRecordDefinitionProvider = new TableManipulationRecordDefinitionProvider(
        sqlStorageTypeInformationProvider,
        infrastructureStoragePropertyDefinitionProvider,
        rdbmsPersistenceModelProvider);

    _infrastructureStoragePropertyDefinitionProvider = infrastructureStoragePropertyDefinitionProvider;
  }

  [Test]
  public void GetRecordDefinition_MultipleTimes_ReturnsSameInstance ()
  {
    Func<TableManipulationRecordDefinitionProvider, ClassDefinition, RecordDefinition>[] operations =
    [
        (e, f) => e.GetDeleteRecordDefinition(f),
        (e, f) => e.GetInsertRecordDefinition(f),
        (e, f) => e.GetLockRecordDefinition(f),
        (e, f) => e.GetUpdateRecordDefinition(f)
    ];

    foreach (var operation in operations)
    {
      var classDefinition = MappingConfiguration.Current.GetClassDefinition("ClassWithAllDataTypes");
      var recordDefinition1 = operation(_tableManipulationRecordDefinitionProvider, classDefinition);
      var recordDefinition2 = operation(_tableManipulationRecordDefinitionProvider, classDefinition);

      Assert.That(recordDefinition1, Is.SameAs(recordDefinition2));
    }
  }

  [Test]
  public void GetRecordDefinition_ClassesInSameInheritance_UseSameTableDefinitionType ()
  {
    var relevantClasses = (string[])["Company", "Customer", "Partner", "Distributor", "Supplier"];
    Func<TableManipulationRecordDefinitionProvider, ClassDefinition, RecordDefinition>[] operations =
    [
        (e, f) => e.GetDeleteRecordDefinition(f),
        (e, f) => e.GetInsertRecordDefinition(f),
        (e, f) => e.GetLockRecordDefinition(f),
        (e, f) => e.GetUpdateRecordDefinition(f)
    ];

    var structureTypeDefinitionsFromOtherOperations = new List<IRdbmsStructuredTypeDefinition>();
    foreach (var operation in operations)
    {
      // Ensure that for the same operation all RecordDefinitions share the same TableTypeDefinition
      IRdbmsStructuredTypeDefinition structuredTypeDefinitionFromCurrentOperation = null;
      foreach (var className in relevantClasses)
      {
        var classDefinition = MappingConfiguration.Current.GetClassDefinition(className);
        var recordDefinition = operation(_tableManipulationRecordDefinitionProvider, classDefinition);

        Assert.That(recordDefinition, Is.Not.Null);

        structuredTypeDefinitionFromCurrentOperation ??= recordDefinition.StructuredTypeDefinition;
        Assert.That(
            recordDefinition.StructuredTypeDefinition,
            Is.SameAs(structuredTypeDefinitionFromCurrentOperation));
      }

      Assert.That(structuredTypeDefinitionFromCurrentOperation, Is.Not.Null);

      // but they do not share the definition with any other operations
      foreach (var structureTypeDefinitionFromOtherOperation in structureTypeDefinitionsFromOtherOperations)
      {
        Assert.That(
            structuredTypeDefinitionFromCurrentOperation,
            Is.Not.SameAs(structureTypeDefinitionFromOtherOperation));
      }

      structureTypeDefinitionsFromOtherOperations.Add(structuredTypeDefinitionFromCurrentOperation);
    }
  }

  [Test]
  public void GetDeleteRecordDefinition_SimpleDataClass_ReturnsRecordDefinitionWithExpectedLayout ()
  {
    var classDefinition = MappingConfiguration.Current.GetClassDefinition("ClassWithAllDataTypes");
    var recordDefinition = _tableManipulationRecordDefinitionProvider.GetDeleteRecordDefinition(classDefinition);

    Assert.That(recordDefinition.RecordName, Is.EqualTo("TVP_AllTables_Delete"));

    var properties = recordDefinition.PropertyDefinitions.ToArray();
    Assert.That(properties.Length, Is.EqualTo(1));

    Assert.That(
        properties[0].PropertyName,
        Is.EqualTo("ID"));
    Assert.That(
        properties[0].StoragePropertyDefinition,
        Is.SameAs(_infrastructureStoragePropertyDefinitionProvider.GetObjectIDStoragePropertyDefinition().ValueProperty));
  }

  [Test]
  public void GetDeleteRecordDefinition_DifferentClassDefinitions_ReturnSameRecord ()
  {
    var classDefinition1 = MappingConfiguration.Current.GetClassDefinition("ClassWithAllDataTypes");
    var recordDefinition1 = _tableManipulationRecordDefinitionProvider.GetDeleteRecordDefinition(classDefinition1);

    var classDefinition2 = MappingConfiguration.Current.GetClassDefinition("Company");
    var recordDefinition2 = _tableManipulationRecordDefinitionProvider.GetDeleteRecordDefinition(classDefinition2);

    Assert.That(
        recordDefinition1,
        Is.SameAs(recordDefinition2));
  }

  [Test]
  public void GetInsertRecordDefinition_ClassWithMixin_ReturnsRecordDefinitionWithExpectedLayout ()
  {
    var classDefinition = MappingConfiguration.Current.GetClassDefinition(nameof(SingleInheritanceFirstDerivedClass));
    var tableDefinition = (TableDefinition)((FilterViewDefinition)classDefinition.StorageEntityDefinition).BaseEntity;
    var insertRecordDefinition = _tableManipulationRecordDefinitionProvider.GetInsertRecordDefinition(classDefinition);

    AssertRecordDefinition(insertRecordDefinition, classDefinition, tableDefinition, "TVP_SingleInheritanceBaseClass_Insert", [
        AssertedProperty.ID,
        AssertedProperty.CopiedProperty(typeof(SingleInheritanceBaseClass), nameof(SingleInheritanceFirstDerivedClass.BaseProperty)),
        AssertedProperty.CopiedProperty(nameof(SingleInheritanceFirstDerivedClass.FirstDerivedProperty)),
        AssertedProperty.CopiedProperty(typeof(SingleInheritancePersistentMixin), nameof(SingleInheritancePersistentMixin.PersistentProperty)),
        AssertedProperty.UnknownProperty(nameof(SingleInheritanceSecondDerivedClass.SecondDerivedProperty)),
        // Column "VectorOpposingPropertyID", so it sorts after "SecondDerivedProperty".
        AssertedProperty.CopiedProperty(typeof(SingleInheritanceBaseClass), nameof(SingleInheritanceBaseClass.VectorOpposingProperty)),
      ]);

  }

  [Test]
  public void GetUpdateRecordDefinition_ClassWithMixin_ReturnsRecordDefinitionWithExpectedLayout ()
  {
    var classDefinition = MappingConfiguration.Current.GetClassDefinition(nameof(SingleInheritanceFirstDerivedClass));
    var tableDefinition = (TableDefinition)((FilterViewDefinition)classDefinition.StorageEntityDefinition).BaseEntity;

    var updateRecordDefinition = _tableManipulationRecordDefinitionProvider.GetUpdateRecordDefinition(classDefinition);
    const string isSetSuffix = "__IsSet";
    AssertRecordDefinition(updateRecordDefinition, classDefinition, tableDefinition, "TVP_SingleInheritanceBaseClass_Update", [
        AssertedProperty.ID,
        AssertedProperty.CopiedProperty(typeof(SingleInheritanceBaseClass), nameof(SingleInheritanceFirstDerivedClass.BaseProperty)),
        AssertedProperty.BitflagProperty(nameof(SingleInheritanceFirstDerivedClass.BaseProperty) + isSetSuffix ),
        AssertedProperty.CopiedProperty(nameof(SingleInheritanceFirstDerivedClass.FirstDerivedProperty)),
        AssertedProperty.BitflagProperty(nameof(SingleInheritanceFirstDerivedClass.FirstDerivedProperty) + isSetSuffix ),
        AssertedProperty.CopiedProperty(typeof(SingleInheritancePersistentMixin), nameof(SingleInheritancePersistentMixin.PersistentProperty)),
        AssertedProperty.BitflagProperty(nameof(SingleInheritancePersistentMixin.PersistentProperty) + isSetSuffix ),
        AssertedProperty.UnknownProperty(nameof(SingleInheritanceSecondDerivedClass.SecondDerivedProperty)),
        AssertedProperty.BitflagProperty(nameof(SingleInheritanceSecondDerivedClass.SecondDerivedProperty) + isSetSuffix),
        // Column "VectorOpposingPropertyID", so it sorts after "SecondDerivedProperty".
        AssertedProperty.CopiedProperty(typeof(SingleInheritanceBaseClass), nameof(SingleInheritanceBaseClass.VectorOpposingProperty)),
      ]);

  }

  [Test]
  public void GetInsertRecordDefinition_SimpleDataClass_ReturnsRecordDefinitionWithExpectedLayout ()
  {
    var classDefinition = MappingConfiguration.Current.GetClassDefinition("ClassWithAllDataTypes");
    var recordDefinition = _tableManipulationRecordDefinitionProvider.GetInsertRecordDefinition(classDefinition);

    AssertRecordDefinition(
        recordDefinition,
        classDefinition,
        "TVP_TableWithAllDataTypes_Insert",
        [
            AssertedProperty.ID,
            AssertedProperty.CopiedProperty("BinaryProperty"),
            AssertedProperty.CopiedProperty("BooleanProperty"),
            AssertedProperty.CopiedProperty("ByteProperty"),
            AssertedProperty.CopiedProperty("DateProperty"),
            AssertedProperty.CopiedProperty("DateTimeProperty"),
            AssertedProperty.CopiedProperty("DecimalProperty"),
            AssertedProperty.CopiedProperty("DoubleProperty"),
            AssertedProperty.CopiedProperty("EnumProperty"),
            AssertedProperty.CopiedProperty("ExtensibleEnumProperty"),
            AssertedProperty.CopiedProperty("ExtensibleEnumWithNullValueProperty"),
            AssertedProperty.CopiedProperty("FlagsProperty"),
            AssertedProperty.CopiedProperty("GuidProperty"),
            AssertedProperty.CopiedProperty("Int16Property"),
            AssertedProperty.CopiedProperty("Int32Property"),
            AssertedProperty.CopiedProperty("Int64Property"),
            AssertedProperty.CopiedProperty("NaBooleanProperty"),
            AssertedProperty.CopiedProperty("NaBooleanWithNullValueProperty"),
            AssertedProperty.CopiedProperty("NaByteProperty"),
            AssertedProperty.CopiedProperty("NaByteWithNullValueProperty"),
            AssertedProperty.CopiedProperty("NaDateProperty"),
            AssertedProperty.CopiedProperty("NaDateTimeProperty"),
            AssertedProperty.CopiedProperty("NaDateTimeWithNullValueProperty"),
            AssertedProperty.CopiedProperty("NaDateWithNullValueProperty"),
            AssertedProperty.CopiedProperty("NaDecimalProperty"),
            AssertedProperty.CopiedProperty("NaDecimalWithNullValueProperty"),
            AssertedProperty.CopiedProperty("NaDoubleProperty"),
            AssertedProperty.CopiedProperty("NaDoubleWithNullValueProperty"),
            AssertedProperty.CopiedProperty("NaEnumProperty"),
            AssertedProperty.CopiedProperty("NaEnumWithNullValueProperty"),
            AssertedProperty.CopiedProperty("NaFlagsProperty"),
            AssertedProperty.CopiedProperty("NaFlagsWithNullValueProperty"),
            AssertedProperty.CopiedProperty("NaGuidProperty"),
            AssertedProperty.CopiedProperty("NaGuidWithNullValueProperty"),
            AssertedProperty.CopiedProperty("NaInt16Property"),
            AssertedProperty.CopiedProperty("NaInt16WithNullValueProperty"),
            AssertedProperty.CopiedProperty("NaInt32Property"),
            AssertedProperty.CopiedProperty("NaInt32WithNullValueProperty"),
            AssertedProperty.CopiedProperty("NaInt64Property"),
            AssertedProperty.CopiedProperty("NaInt64WithNullValueProperty"),
            AssertedProperty.CopiedProperty("NaSingleProperty"),
            AssertedProperty.CopiedProperty("NaSingleWithNullValueProperty"),
            AssertedProperty.CopiedProperty("NullableBinaryProperty"),
            AssertedProperty.CopiedProperty("SingleProperty"),
            AssertedProperty.CopiedProperty("StringProperty"),
            AssertedProperty.CopiedProperty("StringWithNullValueProperty"),
            AssertedProperty.CopiedProperty("StringPropertyWithoutMaxLength"),
        ]);
  }

  [Test]
  public void GetInsertRecordDefinition_ClassWithInheritance_ReturnsRecordDefinitionWithExpectedLayout ()
  {
    var companyTableDefinition = (TableDefinition)MappingConfiguration.Current.GetClassDefinition("Company").StorageEntityDefinition;

    var distributorClassDefinition = MappingConfiguration.Current.GetClassDefinition("Distributor");
    var distributorRecordDefinition = _tableManipulationRecordDefinitionProvider.GetInsertRecordDefinition(distributorClassDefinition);

    AssertRecordDefinition(
        distributorRecordDefinition,
        distributorClassDefinition,
        companyTableDefinition,
        "TVP_Company_Insert",
        [
            AssertedProperty.ID,
            AssertedProperty.CopiedProperty(typeof(Partner), "ContactPerson"),
            AssertedProperty.UnknownProperty("CustomerSince"),
            AssertedProperty.UnknownProperty("CustomerType"),
            AssertedProperty.CopiedProperty(typeof(Company), "IndustrialSector"),
            AssertedProperty.CopiedProperty(typeof(Company), "Name"),
            AssertedProperty.CopiedProperty("NumberOfShops"),
            AssertedProperty.UnknownProperty("SupplierQuality"),
        ]);

    var customerClassDefinition = MappingConfiguration.Current.GetClassDefinition("Customer");
    var customerRecordDefinition = _tableManipulationRecordDefinitionProvider.GetInsertRecordDefinition(customerClassDefinition);

    AssertRecordDefinition(
        customerRecordDefinition,
        customerClassDefinition,
        companyTableDefinition,
        "TVP_Company_Insert",
        [
            AssertedProperty.ID,
            AssertedProperty.UnknownProperty("ContactPersonID"),
            AssertedProperty.CopiedProperty("CustomerSince"),
            AssertedProperty.CopiedProperty("Type"),
            AssertedProperty.CopiedProperty(typeof(Company), "IndustrialSector"),
            AssertedProperty.CopiedProperty(typeof(Company), "Name"),
            AssertedProperty.UnknownProperty("NumberOfShops"),
            AssertedProperty.UnknownProperty("SupplierQuality"),
        ]);
  }

  [Test]
  public void GetLockRecordDefinition_SimpleDataClass_ReturnsRecordDefinitionWithExpectedLayout ()
  {
    var classDefinition = MappingConfiguration.Current.GetClassDefinition("ClassWithAllDataTypes");
    var recordDefinition = _tableManipulationRecordDefinitionProvider.GetLockRecordDefinition(classDefinition);

    Assert.That(recordDefinition.RecordName, Is.EqualTo("TVP_AllTables_Lock"));

    var properties = recordDefinition.PropertyDefinitions.ToArray();
    Assert.That(properties.Length, Is.EqualTo(2));

    Assert.That(
        properties[0].PropertyName,
        Is.EqualTo("ID"));
    Assert.That(
        properties[0].StoragePropertyDefinition,
        Is.SameAs(_infrastructureStoragePropertyDefinitionProvider.GetObjectIDStoragePropertyDefinition().ValueProperty));

    Assert.That(
        properties[1].PropertyName,
        Is.EqualTo("Timestamp"));
    Assert.That(
        properties[1].StoragePropertyDefinition,
        Is.SameAs(_infrastructureStoragePropertyDefinitionProvider.GetTimestampStoragePropertyDefinition()));
  }

  [Test]
  public void GetLockRecordDefinition_DifferentClassDefinitions_ReturnSameRecord ()
  {
    var classDefinition1 = MappingConfiguration.Current.GetClassDefinition("ClassWithAllDataTypes");
    var recordDefinition1 = _tableManipulationRecordDefinitionProvider.GetLockRecordDefinition(classDefinition1);

    var classDefinition2 = MappingConfiguration.Current.GetClassDefinition("Company");
    var recordDefinition2 = _tableManipulationRecordDefinitionProvider.GetLockRecordDefinition(classDefinition2);

    Assert.That(
        recordDefinition1,
        Is.SameAs(recordDefinition2));
  }

  [Test]
  public void GetUpdateRecordDefinition_SimpleDataClass_ReturnsRecordDefinitionWithExpectedLayout ()
  {
    var classDefinition = MappingConfiguration.Current.GetClassDefinition("ClassWithAllDataTypes");
    var recordDefinition = _tableManipulationRecordDefinitionProvider.GetUpdateRecordDefinition(classDefinition);

    AssertRecordDefinition(
        recordDefinition,
        classDefinition,
        "TVP_TableWithAllDataTypes_Update",
        [
            AssertedProperty.ID,
            AssertedProperty.CopiedProperty("BinaryProperty"),
            AssertedProperty.BitflagProperty("Binary__IsSet"),
            AssertedProperty.CopiedProperty("BooleanProperty"),
            AssertedProperty.CopiedProperty("ByteProperty"),
            AssertedProperty.CopiedProperty("DateProperty"),
            AssertedProperty.CopiedProperty("DateTimeProperty"),
            AssertedProperty.CopiedProperty("DecimalProperty"),
            AssertedProperty.CopiedProperty("DoubleProperty"),
            AssertedProperty.CopiedProperty("EnumProperty"),
            AssertedProperty.CopiedProperty("ExtensibleEnumProperty"),
            AssertedProperty.CopiedProperty("ExtensibleEnumWithNullValueProperty"),
            AssertedProperty.CopiedProperty("FlagsProperty"),
            AssertedProperty.CopiedProperty("GuidProperty"),
            AssertedProperty.CopiedProperty("Int16Property"),
            AssertedProperty.CopiedProperty("Int32Property"),
            AssertedProperty.CopiedProperty("Int64Property"),
            AssertedProperty.CopiedProperty("NaBooleanProperty"),
            AssertedProperty.CopiedProperty("NaBooleanWithNullValueProperty"),
            AssertedProperty.CopiedProperty("NaByteProperty"),
            AssertedProperty.CopiedProperty("NaByteWithNullValueProperty"),
            AssertedProperty.CopiedProperty("NaDateProperty"),
            AssertedProperty.CopiedProperty("NaDateTimeProperty"),
            AssertedProperty.CopiedProperty("NaDateTimeWithNullValueProperty"),
            AssertedProperty.CopiedProperty("NaDateWithNullValueProperty"),
            AssertedProperty.CopiedProperty("NaDecimalProperty"),
            AssertedProperty.CopiedProperty("NaDecimalWithNullValueProperty"),
            AssertedProperty.CopiedProperty("NaDoubleProperty"),
            AssertedProperty.CopiedProperty("NaDoubleWithNullValueProperty"),
            AssertedProperty.CopiedProperty("NaEnumProperty"),
            AssertedProperty.CopiedProperty("NaEnumWithNullValueProperty"),
            AssertedProperty.CopiedProperty("NaFlagsProperty"),
            AssertedProperty.CopiedProperty("NaFlagsWithNullValueProperty"),
            AssertedProperty.CopiedProperty("NaGuidProperty"),
            AssertedProperty.CopiedProperty("NaGuidWithNullValueProperty"),
            AssertedProperty.CopiedProperty("NaInt16Property"),
            AssertedProperty.CopiedProperty("NaInt16WithNullValueProperty"),
            AssertedProperty.CopiedProperty("NaInt32Property"),
            AssertedProperty.CopiedProperty("NaInt32WithNullValueProperty"),
            AssertedProperty.CopiedProperty("NaInt64Property"),
            AssertedProperty.CopiedProperty("NaInt64WithNullValueProperty"),
            AssertedProperty.CopiedProperty("NaSingleProperty"),
            AssertedProperty.CopiedProperty("NaSingleWithNullValueProperty"),
            AssertedProperty.CopiedProperty("NullableBinaryProperty"),
            AssertedProperty.BitflagProperty("NullableBinary__IsSet"),
            AssertedProperty.CopiedProperty("SingleProperty"),
            AssertedProperty.CopiedProperty("StringProperty"),
            AssertedProperty.BitflagProperty("String__IsSet"),
            AssertedProperty.CopiedProperty("StringWithNullValueProperty"),
            AssertedProperty.BitflagProperty("StringWithNullValue__IsSet"),
            AssertedProperty.CopiedProperty("StringPropertyWithoutMaxLength"),
            AssertedProperty.BitflagProperty("StringWithoutMaxLength__IsSet"),
        ]);
  }

  [Test]
  public void GetUpdateRecordDefinition_ClassWithInheritance_ReturnsRecordDefinitionWithExpectedLayout ()
  {
    var companyTableDefinition = (TableDefinition)MappingConfiguration.Current.GetClassDefinition("Company").StorageEntityDefinition;

    var distributorClassDefinition = MappingConfiguration.Current.GetClassDefinition("Distributor");
    var distributorRecordDefinition = _tableManipulationRecordDefinitionProvider.GetUpdateRecordDefinition(distributorClassDefinition);

    AssertRecordDefinition(
        distributorRecordDefinition,
        distributorClassDefinition,
        companyTableDefinition,
        "TVP_Company_Update",
        [
            AssertedProperty.ID,
            AssertedProperty.CopiedProperty(typeof(Partner), "ContactPerson"),
            AssertedProperty.UnknownProperty("CustomerSince"),
            AssertedProperty.UnknownProperty("CustomerType"),
            AssertedProperty.CopiedProperty(typeof(Company), "IndustrialSector"),
            AssertedProperty.CopiedProperty(typeof(Company), "Name"),
            AssertedProperty.BitflagProperty("Name__IsSet"),
            AssertedProperty.CopiedProperty("NumberOfShops"),
            AssertedProperty.UnknownProperty("SupplierQuality"),
        ]);

    var customerClassDefinition = MappingConfiguration.Current.GetClassDefinition("Customer");
    var customerRecordDefinition = _tableManipulationRecordDefinitionProvider.GetUpdateRecordDefinition(customerClassDefinition);

    AssertRecordDefinition(
        customerRecordDefinition,
        customerClassDefinition,
        companyTableDefinition,
        "TVP_Company_Update",
        [
            AssertedProperty.ID,
            AssertedProperty.UnknownProperty("ContactPersonID"),
            AssertedProperty.CopiedProperty("CustomerSince"),
            AssertedProperty.CopiedProperty("Type"),
            AssertedProperty.CopiedProperty(typeof(Company), "IndustrialSector"),
            AssertedProperty.CopiedProperty(typeof(Company), "Name"),
            AssertedProperty.BitflagProperty("Name__IsSet"),
            AssertedProperty.UnknownProperty("NumberOfShops"),
            AssertedProperty.UnknownProperty("SupplierQuality"),
        ]);
  }

  [Test]
  public void GetDeleteRecordDefinition_HasPrimaryKeyConstraintOnID ()
  {
    var classDefinition = MappingConfiguration.Current.GetClassDefinition("ClassWithAllDataTypes");
    var recordDefinition = _tableManipulationRecordDefinitionProvider.GetDeleteRecordDefinition(classDefinition);

    AssertHasPrimaryKeyIDConstraint(recordDefinition, _infrastructureStoragePropertyDefinitionProvider.GetObjectIDStoragePropertyDefinition().ValueProperty);
  }

  [Test]
  public void GetLockRecordDefinition_HasPrimaryKeyConstraintOnID ()
  {
    var classDefinition = MappingConfiguration.Current.GetClassDefinition("ClassWithAllDataTypes");
    var recordDefinition = _tableManipulationRecordDefinitionProvider.GetLockRecordDefinition(classDefinition);

    AssertHasPrimaryKeyIDConstraint(recordDefinition, _infrastructureStoragePropertyDefinitionProvider.GetObjectIDStoragePropertyDefinition().ValueProperty);
  }

  [Test]
  public void GetInsertRecordDefinition_HasNoConstraints ()
  {
    // The Insert TVP is never joined against the target table (see BatchedInsertDbCommandBuilder), so unlike the Update/Delete/Lock
    // TVPs it gets no benefit from a primary key constraint on ID.
    var classDefinition = MappingConfiguration.Current.GetClassDefinition("ClassWithAllDataTypes");
    var recordDefinition = _tableManipulationRecordDefinitionProvider.GetInsertRecordDefinition(classDefinition);

    var tableTypeDefinition = (TableTypeDefinition)recordDefinition.StructuredTypeDefinition;
    Assert.That(tableTypeDefinition.Constraints, Is.Empty);
  }

  [Test]
  public void GetUpdateRecordDefinition_HasPrimaryKeyConstraintOnID ()
  {
    var classDefinition = MappingConfiguration.Current.GetClassDefinition("ClassWithAllDataTypes");
    var tableDefinition = (TableDefinition)classDefinition.StorageEntityDefinition;
    var recordDefinition = _tableManipulationRecordDefinitionProvider.GetUpdateRecordDefinition(classDefinition);

    AssertHasPrimaryKeyIDConstraint(recordDefinition, tableDefinition.ObjectIDProperty);
  }

  [Test]
  public void GetRecordDefinition_OrdersColumnsByName ()
  {
    var classDefinition = MappingConfiguration.Current.GetClassDefinition("ProductReview");

    Assert.That(
        GetColumnNames(_tableManipulationRecordDefinitionProvider.GetInsertRecordDefinition(classDefinition)),
        Is.EqualTo(s_orderedProductReviewInsertColumns));
    Assert.That(
        GetColumnNames(_tableManipulationRecordDefinitionProvider.GetUpdateRecordDefinition(classDefinition)),
        Is.EqualTo(s_orderedProductReviewUpdateColumns));
  }

  [Test]
  public void GetRecordDefinition_ColumnOrderDoesNotDependOnDataPropertyOrder ()
  {
    // The order of TableDefinition.DataProperties is not stable between processes, so the generated column order must not depend on it.
    var classDefinition = MappingConfiguration.Current.GetClassDefinition("ProductReview");
    var tableDefinition = (TableDefinition)classDefinition.StorageEntityDefinition;
    var provider = CreateProviderFor(
        classDefinition,
        CloneWithDataProperties(tableDefinition, tableDefinition.DataProperties.Reverse()));

    Assert.That(
        GetColumnNames(provider.GetInsertRecordDefinition(classDefinition)),
        Is.EqualTo(s_orderedProductReviewInsertColumns));
    Assert.That(
        GetColumnNames(provider.GetUpdateRecordDefinition(classDefinition)),
        Is.EqualTo(s_orderedProductReviewUpdateColumns));
  }

  private static string[] GetColumnNames (RecordDefinition recordDefinition)
  {
    return ((TableTypeDefinition)recordDefinition.StructuredTypeDefinition).GetAllColumns().Select(c => c.Name).ToArray();
  }

  private static TableDefinition CloneWithDataProperties (TableDefinition tableDefinition, IEnumerable<IRdbmsStoragePropertyDefinition> dataProperties)
  {
    return new TableDefinition(
        tableDefinition.StorageProviderDefinition,
        tableDefinition.TableName,
        tableDefinition.ViewName,
        tableDefinition.ObjectIDProperty,
        tableDefinition.TimestampProperty,
        dataProperties,
        tableDefinition.Constraints,
        tableDefinition.Indexes,
        tableDefinition.Synonyms,
        tableDefinition.PersistenceModelSortingProvider);
  }

  private TableManipulationRecordDefinitionProvider CreateProviderFor (ClassDefinition classDefinition, TableDefinition tableDefinition)
  {
    var persistenceModelProviderStub = new Mock<IRdbmsPersistenceModelProvider>();
    persistenceModelProviderStub.Setup(_ => _.GetEntityDefinition(classDefinition)).Returns(tableDefinition);

    return new TableManipulationRecordDefinitionProvider(
        new SqlStorageTypeInformationProvider(new DateTimeDefaultStorageTypeProvider()),
        _infrastructureStoragePropertyDefinitionProvider,
        persistenceModelProviderStub.Object);
  }

  [Test]
  public void GetColumnValues_WithTableManipulationDataContainerAccessor_ReturnsCorrectValues ()
  {
    var productReviewClassDefinition = MappingConfiguration.Current.GetClassDefinition("ProductReview");
    var productReviewRecordDefinition = _tableManipulationRecordDefinitionProvider.GetUpdateRecordDefinition(productReviewClassDefinition);

    var objectId = new ObjectID(typeof(ProductReview), Guid.NewGuid());
    var product = new ObjectID(typeof(Product), Guid.NewGuid());
    var createdAt = DateTime.Now;
    var reviewer = new ObjectID(typeof(Person), Guid.NewGuid());

    var dataContainerAccessorStub = new Mock<ITableManipulationDataContainerAccessor>(MockBehavior.Strict);
    dataContainerAccessorStub.Setup(e => e.GetID()).Returns(objectId);
    dataContainerAccessorStub.Setup(e => e.GetValue(It.Is<PropertyDefinition>(e => e.PropertyName.EndsWith(".Product")))).Returns(product);
    dataContainerAccessorStub.Setup(e => e.GetValue(It.Is<PropertyDefinition>(e => e.PropertyName.EndsWith(".Reviewer")))).Returns(reviewer);
    dataContainerAccessorStub.Setup(e => e.GetValue(It.Is<PropertyDefinition>(e => e.PropertyName.EndsWith(".CreatedAt")))).Returns(createdAt);
    dataContainerAccessorStub.Setup(e => e.GetOptionalValue(It.Is<PropertyDefinition>(e => e.PropertyName.EndsWith(".Comment")), "")).Returns("dummy content");
    dataContainerAccessorStub.Setup(e => e.IsOptionalValueSet(It.Is<PropertyDefinition>(e => e.PropertyName.EndsWith(".Comment")))).Returns(true);

    var columnValues = productReviewRecordDefinition.GetColumnValues(dataContainerAccessorStub.Object);
    Assert.That(
        columnValues,
        Is.EqualTo(
        // Columns are ordered by name: ID, ClassID, Comment, Comment__IsSet, CreatedAt, ProductID, ReviewerID.
        (object[])[
            objectId.Value,
            "ProductReview",
            "dummy content",
            true,
            createdAt,
            product.Value,
            reviewer.Value
        ]));
  }

  private static void AssertRecordDefinition (
      RecordDefinition recordDefinition,
      ClassDefinition classDefinition,
      string expectedRecordDefinitionName,
      AssertedProperty[] expectedProperties)
  {
    var tableDefinition = (TableDefinition)classDefinition.StorageEntityDefinition;

    AssertRecordDefinition(recordDefinition, classDefinition, tableDefinition, expectedRecordDefinitionName, expectedProperties);
  }

  private static void AssertRecordDefinition (
      RecordDefinition recordDefinition,
      ClassDefinition classDefinition,
      TableDefinition tableDefinition,
      string expectedRecordDefinitionName,
      AssertedProperty[] expectedProperties)
  {
    Assert.That(recordDefinition, Is.TypeOf<TableManipulationRecordDefinitionProvider.TableManipulationRecordDefinition>());
    Assert.That(recordDefinition.RecordName, Is.EqualTo(expectedRecordDefinitionName));

    var recordProperties = recordDefinition.PropertyDefinitions.ToArray();
    Assert.That(recordProperties.Length, Is.EqualTo(expectedProperties.Length));

    for (var i = 0; i < expectedProperties.Length; i++)
    {
      var actualRecord = recordProperties[i];
      var expectedAssertedProperty = expectedProperties[i];
      if (expectedAssertedProperty.Type == AssertedPropertyType.IDProperty)
      {
        Assert.That(
            actualRecord.PropertyName,
            Is.EqualTo(expectedAssertedProperty.RecordPropertyName));
        Assert.That(
            actualRecord.StoragePropertyDefinition,
            Is.SameAs(tableDefinition.ObjectIDProperty));
      }
      else if (expectedAssertedProperty.Type == AssertedPropertyType.TimestampProperty)
      {
        Assert.That(
            actualRecord.PropertyName,
            Is.EqualTo(expectedAssertedProperty.RecordPropertyName));
        Assert.That(
            actualRecord.StoragePropertyDefinition,
            Is.SameAs(tableDefinition.TimestampProperty));
      }
      else if (expectedAssertedProperty.Type == AssertedPropertyType.CopiedProperty)
      {
        var expectedPropertyName = expectedAssertedProperty.ReferenceType != null
            ? $"{expectedAssertedProperty.ReferenceType}.{expectedAssertedProperty.RecordPropertyName}"
            : $"{classDefinition.ClassType.FullName}.{expectedAssertedProperty.RecordPropertyName}";
        var propertyDefinition = classDefinition.GetPropertyDefinitions().Single(e => e.PropertyName == expectedPropertyName);

        Assert.That(
            actualRecord.PropertyName,
            Is.EqualTo(expectedPropertyName));
      }
      else if (expectedAssertedProperty.Type == AssertedPropertyType.BitflagProperty)
      {
        var actualStoragePropertyDefinition = recordProperties[i].StoragePropertyDefinition;

        Assert.That(actualStoragePropertyDefinition, Is.TypeOf<SimpleStoragePropertyDefinition>());

        var simpleStoragePropertyDefinition = (SimpleStoragePropertyDefinition)actualStoragePropertyDefinition;
        Assert.That(
            simpleStoragePropertyDefinition.PropertyType,
            Is.EqualTo(typeof(bool)));

        var columnDefinition = simpleStoragePropertyDefinition.ColumnDefinition;
        Assert.That(columnDefinition.Name, Is.EqualTo(expectedAssertedProperty.RecordPropertyName));
        Assert.That(columnDefinition.StorageTypeInfo.DotNetType, Is.EqualTo(typeof(bool)));
        Assert.That(columnDefinition.StorageTypeInfo.IsStorageTypeNullable, Is.False);
        Assert.That(columnDefinition.IsPartOfPrimaryKey, Is.False);
      }
      else if (expectedAssertedProperty.Type == AssertedPropertyType.UnknownProperty)
      {
        Assert.That(actualRecord, Is.TypeOf<TableManipulationRecordDefinitionProvider.UnknownRecordPropertyDefinition>());

        Assert.That(
            actualRecord.PropertyName,
            Is.EqualTo("Unknown"));

        var actualColumnName = actualRecord.StoragePropertyDefinition switch
        {
            SimpleStoragePropertyDefinition simpleStoragePropertyDefinition => simpleStoragePropertyDefinition.ColumnDefinition.Name,
            ObjectIDWithoutClassIDStoragePropertyDefinition objectIDPropertyDefinition => ((SimpleStoragePropertyDefinition)objectIDPropertyDefinition.ValueProperty).ColumnDefinition.Name,
            _ => throw new ArgumentOutOfRangeException()
        };

        Assert.That(
            actualColumnName,
            Is.EqualTo(expectedAssertedProperty.RecordPropertyName));
      }
      else
      {
        throw new NotSupportedException();
      }
    }
  }

  private static void AssertHasPrimaryKeyIDConstraint (RecordDefinition recordDefinition, IRdbmsStoragePropertyDefinition idProperty)
  {
    var tableTypeDefinition = (TableTypeDefinition)recordDefinition.StructuredTypeDefinition;
    var expectedColumns = idProperty.GetColumnsForComparison().ToArray();

    Assert.That(tableTypeDefinition.Constraints, Has.Count.EqualTo(1));

    var constraint = tableTypeDefinition.Constraints.Single();
    Assert.That(constraint, Is.TypeOf<PrimaryKeyConstraintDefinition>());

    var primaryKeyConstraint = (PrimaryKeyConstraintDefinition)constraint;
    Assert.That(primaryKeyConstraint.IsClustered, Is.True);
    Assert.That(primaryKeyConstraint.Columns, Is.EqualTo(expectedColumns));
    Assert.That(
        primaryKeyConstraint.ConstraintName,
        Is.EqualTo($"PK_{tableTypeDefinition.TypeName.EntityName}"));
  }

  private enum AssertedPropertyType
  {
    IDProperty,
    TimestampProperty,
    CopiedProperty,
    BitflagProperty,
    UnknownProperty
  }

  private class AssertedProperty
  {
    public static readonly AssertedProperty ID = new(AssertedPropertyType.IDProperty, "ID");

    public static AssertedProperty CopiedProperty (string name) => new(AssertedPropertyType.CopiedProperty, name);

    public static AssertedProperty CopiedProperty (Type type, string name) => new(AssertedPropertyType.CopiedProperty, name, type);

    public static AssertedProperty BitflagProperty (string name) => new(AssertedPropertyType.BitflagProperty, name);

    public static AssertedProperty UnknownProperty (string name) => new(AssertedPropertyType.UnknownProperty, name);

    public AssertedPropertyType Type { get; }

    public string RecordPropertyName { get; }

    [CanBeNull]
    public Type ReferenceType { get; }

    private AssertedProperty (AssertedPropertyType type, string recordPropertyName, [CanBeNull] Type referenceType = null)
    {
      Type = type;
      RecordPropertyName = recordPropertyName;
      ReferenceType = referenceType;
    }
  }
}
