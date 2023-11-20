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
using System.Linq.Expressions;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Linq.Development.UnitTesting;
using Remotion.Linq.SqlBackend.Development.UnitTesting;
using Remotion.Linq.SqlBackend.SqlStatementModel;
using Remotion.Linq.SqlBackend.SqlStatementModel.Resolved;
using Remotion.Linq.SqlBackend.SqlStatementModel.SqlSpecificExpressions;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Linq
{
  [TestFixture]
  public class StorageSpecificExpressionResolverTest : StandardMappingTest
  {
    private StorageSpecificExpressionResolver _storageSpecificExpressionResolver;
    private ClassDefinition _classDefinition;
    private Mock<IRdbmsPersistenceModelProvider> _rdbmsPersistenceModelProviderStub;
    private Mock<IRdbmsStoragePropertyDefinition> _rdbmsStoragePropertyDefinitionStub;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _rdbmsPersistenceModelProviderStub = new Mock<IRdbmsPersistenceModelProvider>();
      _rdbmsStoragePropertyDefinitionStub = new Mock<IRdbmsStoragePropertyDefinition>();

      _storageSpecificExpressionResolver = new StorageSpecificExpressionResolver(
          _rdbmsPersistenceModelProviderStub.Object);

      _classDefinition = ClassDefinitionObjectMother.CreateClassDefinition(classType: typeof(Order));
      _classDefinition.SetStorageEntity(
          TableDefinitionObjectMother.Create(
              TestDomainStorageProviderDefinition,
              new EntityNameDefinition(null, "Order"),
              new EntityNameDefinition(null, "OrderView")));
    }

    [Test]
    public void ResolveEntity ()
    {
      var objectIDProperty = ObjectIDStoragePropertyDefinitionObjectMother.ObjectIDProperty;
      var timestampProperty = SimpleStoragePropertyDefinitionObjectMother.TimestampProperty;

      var foreignKeyProperty = SimpleStoragePropertyDefinitionObjectMother.CreateGuidStorageProperty("ForeignKey");
      var simpleProperty = SimpleStoragePropertyDefinitionObjectMother.CreateStringStorageProperty("Column1");

      var tableDefinition = TableDefinitionObjectMother.Create(
          TestDomainStorageProviderDefinition,
          new EntityNameDefinition(null, "Test"),
          null,
          objectIDProperty,
          timestampProperty,
          foreignKeyProperty,
          simpleProperty);
      _rdbmsPersistenceModelProviderStub
          .Setup(stub => stub.GetEntityDefinition(_classDefinition))
          .Returns(tableDefinition);

      var result = _storageSpecificExpressionResolver.ResolveEntity(_classDefinition, "o");

      var expectedIdColumn = new SqlColumnDefinitionExpression(typeof(Guid), "o", "ID", true);
      var expectedClassIdColumn = new SqlColumnDefinitionExpression(typeof(string), "o", "ClassID", false);
      var expectedTimestampColumn = new SqlColumnDefinitionExpression(typeof(DateTime), "o", "Timestamp", false);
      var expectedForeignKeyColumn = new SqlColumnDefinitionExpression(typeof(Guid), "o", "ForeignKey", false);
      var expectedColumn = new SqlColumnDefinitionExpression(typeof(string), "o", "Column1", false);

      Assert.That(result.Type, Is.SameAs(typeof(Order)));
      Assert.That(result.TableAlias, Is.EqualTo("o"));
      Assert.That(result.Name, Is.Null);
      SqlExpressionTreeComparer.CheckAreEqualTrees(expectedIdColumn, result.GetIdentityExpression());
      Assert.That(result.Columns, Has.Count.EqualTo(5));
      SqlExpressionTreeComparer.CheckAreEqualTrees(expectedIdColumn, result.Columns[0]);
      SqlExpressionTreeComparer.CheckAreEqualTrees(expectedClassIdColumn, result.Columns[1]);
      SqlExpressionTreeComparer.CheckAreEqualTrees(expectedTimestampColumn, result.Columns[2]);
      SqlExpressionTreeComparer.CheckAreEqualTrees(expectedForeignKeyColumn, result.Columns[3]);
      SqlExpressionTreeComparer.CheckAreEqualTrees(expectedColumn, result.Columns[4]);
    }

    [Test]
    public void ResolveProperty_NoPrimaryKeyColumn ()
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo();
      var entityExpression = CreateEntityDefinition(typeof(Order), "o");

      _rdbmsPersistenceModelProviderStub
          .Setup(stub => stub.GetStoragePropertyDefinition(propertyDefinition))
          .Returns(_rdbmsStoragePropertyDefinitionStub.Object);

      var columnDefinition = ColumnDefinitionObjectMother.CreateColumn("OrderNumber");
      _rdbmsStoragePropertyDefinitionStub.Setup(stub => stub.PropertyType).Returns(typeof(string));
      _rdbmsStoragePropertyDefinitionStub.Setup(stub => stub.GetColumns()).Returns(new[] { columnDefinition });

      var result = (SqlColumnDefinitionExpression)_storageSpecificExpressionResolver.ResolveProperty(entityExpression, propertyDefinition);

      Assert.That(result.ColumnName, Is.EqualTo("OrderNumber"));
      Assert.That(result.OwningTableAlias, Is.EqualTo("o"));
      Assert.That(result.Type, Is.SameAs(typeof(string)));
      Assert.That(result.IsPrimaryKey, Is.False);
    }

    [Test]
    public void ResolveProperty_PrimaryKeyColumn ()
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo();
      var entityExpression = CreateEntityDefinition(typeof(Order), "o");

      _rdbmsPersistenceModelProviderStub
          .Setup(stub => stub.GetStoragePropertyDefinition(propertyDefinition))
          .Returns(_rdbmsStoragePropertyDefinitionStub.Object);

      var columnDefinition = ColumnDefinitionObjectMother.IDColumn;
      _rdbmsStoragePropertyDefinitionStub.Setup(stub => stub.PropertyType).Returns(typeof(string));
      _rdbmsStoragePropertyDefinitionStub.Setup(stub => stub.GetColumns()).Returns(new[] { columnDefinition });

      var result = (SqlColumnDefinitionExpression)_storageSpecificExpressionResolver.ResolveProperty(entityExpression, propertyDefinition);

      Assert.That(result.ColumnName, Is.EqualTo("ID"));
      Assert.That(result.OwningTableAlias, Is.EqualTo("o"));
      Assert.That(result.Type, Is.SameAs(typeof(Guid)));
      Assert.That(result.IsPrimaryKey, Is.True);
    }

    [Test]
    public void ResolveProperty_CompoundColumn ()
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo();
      var entityExpression = CreateEntityDefinition(typeof(Order), "o");

      _rdbmsPersistenceModelProviderStub
          .Setup(stub => stub.GetStoragePropertyDefinition(propertyDefinition))
          .Returns(_rdbmsStoragePropertyDefinitionStub.Object);
      _rdbmsStoragePropertyDefinitionStub
          .Setup(stub => stub.GetColumns())
          .Returns(new[] { ColumnDefinitionObjectMother.IDColumn, ColumnDefinitionObjectMother.ClassIDColumn });
      Assert.That(
          () => _storageSpecificExpressionResolver.ResolveProperty(entityExpression, propertyDefinition),
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo(
                  "Compound-column properties are not supported by this LINQ provider."));
    }

    [Test]
    public void ResoveIDProperty ()
    {
      var entityExpression = CreateEntityDefinition(typeof(Order), "o");
      var entityDefinition = TableDefinitionObjectMother.Create(TestDomainStorageProviderDefinition, new EntityNameDefinition(null, "Test"));

      _rdbmsPersistenceModelProviderStub
          .Setup(stub => stub.GetEntityDefinition(_classDefinition))
          .Returns(entityDefinition);

      var result = _storageSpecificExpressionResolver.ResolveIDProperty(entityExpression, _classDefinition);

      var ctor = MemberInfoFromExpressionUtility.GetConstructor(() => new ObjectID("ClassID", "value"));
      SqlExpressionTreeComparer.CheckAreEqualTrees(
          Expression.New(
              ctor,
              new[]
              {
                  new NamedExpression("ClassID", new SqlColumnDefinitionExpression(typeof(string), "o", "ClassID", false)),
                  new NamedExpression("Value", Expression.Convert(new SqlColumnDefinitionExpression(typeof(Guid), "o", "ID", true), typeof(object)))
              },
              new[]
              {
                  typeof(ObjectID).GetProperty("ClassID"),
                  typeof(ObjectID).GetProperty("Value")
              }),
          result);
    }

    [Test]
    public void ResolveTable_TableDefinitionWithNoSchemaName ()
    {
      var tableDefinition = TableDefinitionObjectMother.Create(
          TestDomainStorageProviderDefinition, new EntityNameDefinition(null, "Table"), new EntityNameDefinition(null, "TableView"));
      _classDefinition.SetStorageEntity(tableDefinition);

      _rdbmsPersistenceModelProviderStub
          .Setup(stub => stub.GetEntityDefinition(_classDefinition))
          .Returns(_classDefinition.StorageEntityDefinition as IRdbmsStorageEntityDefinition);

      var result = (ResolvedSimpleTableInfo)_storageSpecificExpressionResolver.ResolveTable(_classDefinition, "o");

      Assert.That(result, Is.Not.Null);
      Assert.That(result.TableName, Is.EqualTo("TableView"));
      Assert.That(result.TableAlias, Is.EqualTo("o"));
      Assert.That(result.ItemType, Is.EqualTo(typeof(Order)));
    }

    [Test]
    public void ResolveTable_TableDefinitionWithSchemaName ()
    {
      var tableDefinition = TableDefinitionObjectMother.Create(
          TestDomainStorageProviderDefinition, new EntityNameDefinition(null, "Table"), new EntityNameDefinition("schemaName", "TableView"));
      _classDefinition.SetStorageEntity(tableDefinition);

      _rdbmsPersistenceModelProviderStub
          .Setup(stub => stub.GetEntityDefinition(_classDefinition))
          .Returns(_classDefinition.StorageEntityDefinition as IRdbmsStorageEntityDefinition);

      var result = (ResolvedSimpleTableInfo)_storageSpecificExpressionResolver.ResolveTable(_classDefinition, "o");

      Assert.That(result, Is.Not.Null);
      Assert.That(result.TableName, Is.EqualTo("schemaName.TableView"));
      Assert.That(result.TableAlias, Is.EqualTo("o"));
      Assert.That(result.ItemType, Is.EqualTo(typeof(Order)));
    }

    [Test]
    public void ResolveTable_FilterViewDefinitionWithNoSchemaName ()
    {
      var filterViewDefinition = FilterViewDefinitionObjectMother.Create(
          TestDomainStorageProviderDefinition,
          new EntityNameDefinition(null, "FilterView"),
          (IRdbmsStorageEntityDefinition)_classDefinition.StorageEntityDefinition);
      _classDefinition.SetStorageEntity(filterViewDefinition);

      _rdbmsPersistenceModelProviderStub
          .Setup(stub => stub.GetEntityDefinition(_classDefinition))
          .Returns(_classDefinition.StorageEntityDefinition as IRdbmsStorageEntityDefinition);

      var result = (ResolvedSimpleTableInfo)_storageSpecificExpressionResolver.ResolveTable(_classDefinition, "o");

      Assert.That(result, Is.Not.Null);
      Assert.That(result.TableName, Is.EqualTo("FilterView"));
      Assert.That(result.TableAlias, Is.EqualTo("o"));
      Assert.That(result.ItemType, Is.EqualTo(typeof(Order)));
    }

    [Test]
    public void ResolveTable_FilterViewDefinitionWithSchemaName ()
    {
      var filterViewDefinition = FilterViewDefinitionObjectMother.Create(
          TestDomainStorageProviderDefinition,
          new EntityNameDefinition("schemaName", "FilterView"),
          (IRdbmsStorageEntityDefinition)_classDefinition.StorageEntityDefinition);
      _classDefinition.SetStorageEntity(filterViewDefinition);

      _rdbmsPersistenceModelProviderStub
          .Setup(stub => stub.GetEntityDefinition(_classDefinition))
          .Returns(_classDefinition.StorageEntityDefinition as IRdbmsStorageEntityDefinition);

      var result = (ResolvedSimpleTableInfo)_storageSpecificExpressionResolver.ResolveTable(_classDefinition, "o");

      Assert.That(result, Is.Not.Null);
      Assert.That(result.TableName, Is.EqualTo("schemaName.FilterView"));
      Assert.That(result.TableAlias, Is.EqualTo("o"));
      Assert.That(result.ItemType, Is.EqualTo(typeof(Order)));
    }

    [Test]
    public void ResolveTable_UnionViewDefinitionWithNoSchemaName ()
    {
      var unionViewDefinition = UnionViewDefinitionObjectMother.Create(
          TestDomainStorageProviderDefinition,
          new EntityNameDefinition(null, "UnionView"),
          new[] { (IRdbmsStorageEntityDefinition)_classDefinition.StorageEntityDefinition });
      _classDefinition.SetStorageEntity(unionViewDefinition);

      _rdbmsPersistenceModelProviderStub
          .Setup(stub => stub.GetEntityDefinition(_classDefinition))
          .Returns(_classDefinition.StorageEntityDefinition as IRdbmsStorageEntityDefinition);

      var result = (ResolvedSimpleTableInfo)_storageSpecificExpressionResolver.ResolveTable(_classDefinition, "o");

      Assert.That(result, Is.Not.Null);
      Assert.That(result.TableName, Is.EqualTo("UnionView"));
      Assert.That(result.TableAlias, Is.EqualTo("o"));
      Assert.That(result.ItemType, Is.EqualTo(typeof(Order)));
    }

    [Test]
    public void ResolveTable_UnionViewDefinitionWithSchemaName ()
    {
      var unionViewDefinition = UnionViewDefinitionObjectMother.Create(
          TestDomainStorageProviderDefinition,
          new EntityNameDefinition("schemaName", "UnionView"),
          new[] { (IRdbmsStorageEntityDefinition)_classDefinition.StorageEntityDefinition });
      _classDefinition.SetStorageEntity(unionViewDefinition);

      _rdbmsPersistenceModelProviderStub
          .Setup(stub => stub.GetEntityDefinition(_classDefinition))
          .Returns(_classDefinition.StorageEntityDefinition as IRdbmsStorageEntityDefinition);

      var result = (ResolvedSimpleTableInfo)_storageSpecificExpressionResolver.ResolveTable(_classDefinition, "o");

      Assert.That(result, Is.Not.Null);
      Assert.That(result.TableName, Is.EqualTo("schemaName.UnionView"));
      Assert.That(result.TableAlias, Is.EqualTo("o"));
      Assert.That(result.ItemType, Is.EqualTo(typeof(Order)));
    }

    [Test]
    public void ResolveTable_EmptyViewDefinitionWithNoSchemaName ()
    {
      var emptyViewDefinition = EmptyViewDefinitionObjectMother.Create(
          TestDomainStorageProviderDefinition,
          new EntityNameDefinition(null, "EmptyView"));
      _classDefinition.SetStorageEntity(emptyViewDefinition);

      _rdbmsPersistenceModelProviderStub
          .Setup(stub => stub.GetEntityDefinition(_classDefinition))
          .Returns(_classDefinition.StorageEntityDefinition as IRdbmsStorageEntityDefinition);

      var result = (ResolvedSimpleTableInfo)_storageSpecificExpressionResolver.ResolveTable(_classDefinition, "o");

      Assert.That(result, Is.Not.Null);
      Assert.That(result.TableName, Is.EqualTo("EmptyView"));
      Assert.That(result.TableAlias, Is.EqualTo("o"));
      Assert.That(result.ItemType, Is.EqualTo(typeof(Order)));
    }

    [Test]
    public void ResolveTable_EmptyViewDefinitionWithSchemaName ()
    {
      var emptyViewDefinition = EmptyViewDefinitionObjectMother.Create(
          TestDomainStorageProviderDefinition,
          new EntityNameDefinition("schemaName", "EmptyView"));
      _classDefinition.SetStorageEntity(emptyViewDefinition);

      _rdbmsPersistenceModelProviderStub
          .Setup(stub => stub.GetEntityDefinition(_classDefinition))
          .Returns(_classDefinition.StorageEntityDefinition as IRdbmsStorageEntityDefinition);

      var result = (ResolvedSimpleTableInfo)_storageSpecificExpressionResolver.ResolveTable(_classDefinition, "o");

      Assert.That(result, Is.Not.Null);
      Assert.That(result.TableName, Is.EqualTo("schemaName.EmptyView"));
      Assert.That(result.TableAlias, Is.EqualTo("o"));
      Assert.That(result.ItemType, Is.EqualTo(typeof(Order)));
    }

    [Test]
    public void ResolveJoin_LeftSideHoldsForeignKey ()
    {
      // Order.Customer
      var propertyDefinition = CreatePropertyDefinitionAndAssociateWithClass(_classDefinition, "Customer", "Customer");

      var columnDefinition = ColumnDefinitionObjectMother.CreateGuidColumn("Customer");
      _rdbmsStoragePropertyDefinitionStub.Setup(stub => stub.GetColumnsForComparison()).Returns(new[] { columnDefinition });
      _rdbmsStoragePropertyDefinitionStub.Setup(stub => stub.PropertyType).Returns(typeof(ObjectID));

      var leftEndPointDefinition = new RelationEndPointDefinition(propertyDefinition, false);
      _rdbmsPersistenceModelProviderStub
          .Setup(stub => stub.GetStoragePropertyDefinition(leftEndPointDefinition.PropertyDefinition))
          .Returns(_rdbmsStoragePropertyDefinitionStub.Object);

      // Customer.Order
      var customerClassDefinition = ClassDefinitionObjectMother.CreateClassDefinition(classType: typeof(Customer));
      var customerTableDefinition = TableDefinitionObjectMother.Create(
          TestDomainStorageProviderDefinition,
          new EntityNameDefinition(null, "CustomerTable"),
          new EntityNameDefinition(null, "CustomerView"));
      _rdbmsPersistenceModelProviderStub
          .Setup(stub => stub.GetEntityDefinition(customerClassDefinition))
          .Returns(customerTableDefinition);

      var rightEndPointDefinition = new AnonymousRelationEndPointDefinition(customerClassDefinition);

      var originatingEntity = CreateEntityDefinition(typeof(Order), "o");

      var result = _storageSpecificExpressionResolver.ResolveJoin(originatingEntity, leftEndPointDefinition, rightEndPointDefinition, "c");

      Assert.That(result, Is.Not.Null);
      Assert.That(result.ItemType, Is.EqualTo(typeof(Customer)));
      Assert.That(result.ForeignTableInfo, Is.TypeOf(typeof(ResolvedSimpleTableInfo)));
      Assert.That(((ResolvedSimpleTableInfo)result.ForeignTableInfo).TableName, Is.EqualTo("CustomerView"));
      Assert.That(((ResolvedSimpleTableInfo)result.ForeignTableInfo).TableAlias, Is.EqualTo("c"));

      var expected = Expression.Equal(
          new SqlColumnDefinitionExpression(typeof(Guid), "o", "Customer", false),
          new SqlColumnDefinitionExpression(typeof(Guid), "c", "ID", true));
      SqlExpressionTreeComparer.CheckAreEqualTrees(expected, result.JoinCondition);
    }

    [Test]
    public void ResolveJoin_LeftSideHoldsNoForeignKey ()
    {
      var propertyDefinition = CreatePropertyDefinitionAndAssociateWithClass(_classDefinition, "Customer", "Customer");

      var leftEndPointDefinition = new AnonymousRelationEndPointDefinition(_classDefinition);
      var rightEndPointDefinition = new RelationEndPointDefinition(propertyDefinition, false);

      var entityExpression = CreateEntityDefinition(typeof(Customer), "c");

      _rdbmsPersistenceModelProviderStub
          .Setup(stub => stub.GetEntityDefinition(rightEndPointDefinition.ClassDefinition))
          .Returns(rightEndPointDefinition.ClassDefinition.StorageEntityDefinition as IRdbmsStorageEntityDefinition);
      _rdbmsPersistenceModelProviderStub
          .Setup(stub => stub.GetStoragePropertyDefinition(rightEndPointDefinition.PropertyDefinition))
          .Returns(_rdbmsStoragePropertyDefinitionStub.Object);

      _rdbmsStoragePropertyDefinitionStub
          .Setup(stub => stub.GetColumnsForComparison())
          .Returns(new[] { ColumnDefinitionObjectMother.CreateGuidColumn("Customer") });
      _rdbmsStoragePropertyDefinitionStub.Setup(stub => stub.PropertyType).Returns(typeof(ObjectID));

      var result = _storageSpecificExpressionResolver.ResolveJoin(entityExpression, leftEndPointDefinition, rightEndPointDefinition, "o");

      SqlExpressionTreeComparer.CheckAreEqualTrees(
          Expression.Equal(
            entityExpression.GetIdentityExpression(), // c.ID
            new SqlColumnDefinitionExpression(typeof(Guid), "o", "Customer", false)),
          result.JoinCondition);
    }

    [Test]
    public void ResolveEntityIdentityViaForeignKey ()
    {
      // Order.Customer
      var propertyDefinition = CreatePropertyDefinitionAndAssociateWithClass(_classDefinition, "Customer", "Customer");

      var columnDefinition = ColumnDefinitionObjectMother.CreateStringColumn("Customer");
      _rdbmsStoragePropertyDefinitionStub.Setup(stub => stub.GetColumnsForComparison()).Returns(new[] { columnDefinition });
      _rdbmsStoragePropertyDefinitionStub.Setup(stub => stub.PropertyType).Returns(typeof(ObjectID));

      var foreignKeyEndPointDefinition = new RelationEndPointDefinition(propertyDefinition, false);
      _rdbmsPersistenceModelProviderStub
          .Setup(stub => stub.GetStoragePropertyDefinition(foreignKeyEndPointDefinition.PropertyDefinition))
          .Returns(_rdbmsStoragePropertyDefinitionStub.Object);

      var originatingEntity = CreateEntityDefinition(typeof(Order), "o");

      var result = _storageSpecificExpressionResolver.ResolveEntityIdentityViaForeignKey(originatingEntity, foreignKeyEndPointDefinition);

      var expected = new SqlColumnDefinitionExpression(typeof(string), "o", "Customer", false);
      SqlExpressionTreeComparer.CheckAreEqualTrees(expected, result);
    }

    [Test]
    public void ResolveIDPropertyViaForeignKey_WithFullObjectID_ResolvesToCompound ()
    {
      var propertyDefinition = CreatePropertyDefinitionAndAssociateWithClass(_classDefinition, "Customer", "Customer");
      var objectIDStorageProperty = ObjectIDStoragePropertyDefinitionObjectMother.Create(
          "CustomerID",
          "CustomerClassID",
          StorageTypeInformationObjectMother.CreateUniqueIdentifierStorageTypeInformation(),
          StorageTypeInformationObjectMother.CreateVarchar100StorageTypeInformation());

      var foreignKeyEndPointDefinition = new RelationEndPointDefinition(propertyDefinition, false);
      _rdbmsPersistenceModelProviderStub
          .Setup(stub => stub.GetStoragePropertyDefinition(foreignKeyEndPointDefinition.PropertyDefinition))
          .Returns(objectIDStorageProperty);

      var originatingEntity = CreateEntityDefinition(typeof(Order), "o");

      var result = _storageSpecificExpressionResolver.ResolveIDPropertyViaForeignKey(originatingEntity, foreignKeyEndPointDefinition);

      var expected = Expression.New(
          MemberInfoFromExpressionUtility.GetConstructor(() => new ObjectID("classID", "value")),
          new[]
          {
              new NamedExpression("ClassID", originatingEntity.GetColumn(typeof(string), "CustomerClassID", false)),
              new NamedExpression("Value", Expression.Convert(originatingEntity.GetColumn(typeof(Guid), "CustomerID", false), typeof(object)))
          },
          new[] { typeof(ObjectID).GetProperty("ClassID"), typeof(ObjectID).GetProperty("Value") });
      SqlExpressionTreeComparer.CheckAreEqualTrees(expected, result);
    }

    [Test]
    public void ResolveIDPropertyViaForeignKey_WithObjectIDWithoutClassID_ResolvesToCompoundWithFixedConditionalClassID ()
    {
      var propertyDefinition = CreatePropertyDefinitionAndAssociateWithClass(_classDefinition, "Customer", "Customer");
      var objectIDStorageProperty =
          new ObjectIDWithoutClassIDStoragePropertyDefinition(
              SimpleStoragePropertyDefinitionObjectMother.CreateGuidStorageProperty("CustomerID"),
              GetTypeDefinition(typeof(Customer)));

      var foreignKeyEndPointDefinition = new RelationEndPointDefinition(propertyDefinition, false);
      _rdbmsPersistenceModelProviderStub
          .Setup(stub => stub.GetStoragePropertyDefinition(foreignKeyEndPointDefinition.PropertyDefinition))
          .Returns(objectIDStorageProperty);

      var originatingEntity = CreateEntityDefinition(typeof(Order), "o");

      var result = _storageSpecificExpressionResolver.ResolveIDPropertyViaForeignKey(originatingEntity, foreignKeyEndPointDefinition);

      var expectedValueColumn = originatingEntity.GetColumn(typeof(Guid), "CustomerID", false);
      var expected = Expression.New(
          MemberInfoFromExpressionUtility.GetConstructor(() => new ObjectID("classID", "value")),
          new Expression[]
          {
              new NamedExpression(
                  "ClassID",
                  SqlCaseExpression.CreateIfThenElse(
                      typeof(string),
                      new SqlIsNotNullExpression(expectedValueColumn),
                      new SqlLiteralExpression("Customer"),
                      Expression.Constant(null, typeof(string)))),
              new NamedExpression("Value", Expression.Convert(expectedValueColumn, typeof(object)))
          },
          new[] { typeof(ObjectID).GetProperty("ClassID"), typeof(ObjectID).GetProperty("Value") });
      SqlExpressionTreeComparer.CheckAreEqualTrees(expected, result);
    }

    [Test]
    public void ResolveIDPropertyViaForeignKey_WithSomeOtherStorageProperty_ReturnsNull ()
    {
      var propertyDefinition = CreatePropertyDefinitionAndAssociateWithClass(_classDefinition, "Customer", "Customer");

      var foreignKeyEndPointDefinition = new RelationEndPointDefinition(propertyDefinition, false);
      _rdbmsPersistenceModelProviderStub
          .Setup(stub => stub.GetStoragePropertyDefinition(foreignKeyEndPointDefinition.PropertyDefinition))
          .Returns(_rdbmsStoragePropertyDefinitionStub.Object);

      var originatingEntity = CreateEntityDefinition(typeof(Order), "o");

      var result = _storageSpecificExpressionResolver.ResolveIDPropertyViaForeignKey(originatingEntity, foreignKeyEndPointDefinition);

      Assert.That(result, Is.Null);
    }

    private PropertyDefinition CreatePropertyDefinition (ClassDefinition classDefinition, string propertyName, string columnName)
    {
      var propertyInformationStub = new Mock<IPropertyInformation>();
      propertyInformationStub.Setup(_ => _.PropertyType).Returns(typeof(string));
      var propertyDefinition = new PropertyDefinition(
          classDefinition,
          propertyInformationStub.Object,
          propertyName,
          true,
          true,
          null,
          StorageClass.Persistent,
          null);
      propertyDefinition.SetStorageProperty(SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty(columnName));
      return propertyDefinition;
    }

    private SqlEntityDefinitionExpression CreateEntityDefinition (Type itemType, string tableAlias)
    {
      return new SqlEntityDefinitionExpression(itemType, tableAlias, null, e => e.GetColumn(typeof(Guid), "ID", true));
    }

    private PropertyDefinition CreatePropertyDefinitionAndAssociateWithClass (ClassDefinition classDefinition, string propertyName, string columnName)
    {
      var pd = CreatePropertyDefinition(classDefinition, propertyName, columnName);
      classDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection(new[] { pd }, true));
      return pd;
    }
  }
}
