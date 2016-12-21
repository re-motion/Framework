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
using System.Reflection;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.UnitTests.Linq.TestDomain;
using Remotion.Data.DomainObjects.UnitTests.MixedDomains.TestDomain;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Data.DomainObjects.UnitTests.TestDomain.InheritanceRootSample;
using Remotion.Linq;
using Remotion.Linq.Development.UnitTesting;
using Remotion.Linq.SqlBackend.Development.UnitTesting;
using Remotion.Linq.SqlBackend.MappingResolution;
using Remotion.Linq.SqlBackend.SqlStatementModel;
using Remotion.Linq.SqlBackend.SqlStatementModel.Resolved;
using Remotion.Linq.SqlBackend.SqlStatementModel.SqlSpecificExpressions;
using Remotion.Linq.SqlBackend.SqlStatementModel.Unresolved;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Linq
{
  [TestFixture]
  public class MappingResolverTest : StandardMappingTest
  {
    private MappingResolver _resolver;
    private UniqueIdentifierGenerator _generator;
    private SqlTable _orderTable;
    private IStorageSpecificExpressionResolver _storageSpecificExpressionResolverStub;
    private ResolvedSimpleTableInfo _fakeSimpleTableInfo;
    private SqlColumnDefinitionExpression _fakeColumnDefinitionExpression;
    private ResolvedJoinInfo _fakeJoinInfo;
    private IStorageNameProvider _storageNameProviderStub;

    public override void SetUp ()
    {
      base.SetUp();
      _storageSpecificExpressionResolverStub = MockRepository.GenerateStub<IStorageSpecificExpressionResolver>();
      _storageNameProviderStub = MockRepository.GenerateStub<IStorageNameProvider>();
      _storageNameProviderStub.Stub (stub => stub.GetIDColumnName()).Return ("ID");
      _storageNameProviderStub.Stub (stub => stub.GetClassIDColumnName()).Return ("ClassID");
      _storageNameProviderStub.Stub (stub => stub.GetTimestampColumnName()).Return ("Timestamp");
      _resolver = new MappingResolver (_storageSpecificExpressionResolverStub);
      _generator = new UniqueIdentifierGenerator();
      _orderTable = new SqlTable (new ResolvedSimpleTableInfo (typeof (Order), "Order", "o"), JoinSemantics.Inner);
      _fakeSimpleTableInfo = new ResolvedSimpleTableInfo (typeof (Order), "OrderTable", "o");
      _fakeColumnDefinitionExpression = new SqlColumnDefinitionExpression (typeof (int), "o", "ColumnName", false);
      _fakeJoinInfo = new ResolvedJoinInfo (_fakeSimpleTableInfo, Expression.Constant (true));
    }

    [Test]
    public void ResolveSimpleTableInfo ()
    {
      var fakeEntityExpression = CreateFakeEntityExpression (typeof (Order));

      _storageSpecificExpressionResolverStub
          .Stub (stub => stub.ResolveEntity (MappingConfiguration.Current.GetTypeDefinition (typeof (Order)), "o"))
          .Return (fakeEntityExpression);

      var sqlEntityExpression =
          (SqlEntityExpression) _resolver.ResolveSimpleTableInfo (_orderTable.GetResolvedTableInfo(), _generator);

      Assert.That (sqlEntityExpression, Is.SameAs (fakeEntityExpression));
    }

    [Test]
    [ExpectedException (typeof (UnmappedItemException))]
    public void ResolveSimpleTableInfo_NoDomainObject_ThrowsException ()
    {
      var simpleTableInfo = new ResolvedSimpleTableInfo(typeof (Student), "StudentTable", "StudentTableAlias");

      _resolver.ResolveSimpleTableInfo (simpleTableInfo, _generator);
    }

    [Test]
    public void ResolveTableInfo ()
    {
      var unresolvedTableInfo = new UnresolvedTableInfo (typeof (Order));
      _storageSpecificExpressionResolverStub
          .Stub (stub => stub.ResolveTable (MappingConfiguration.Current.GetTypeDefinition (typeof (Order)), "t0"))
          .Return (_fakeSimpleTableInfo);

      var resolvedTableInfo = (ResolvedSimpleTableInfo) _resolver.ResolveTableInfo (unresolvedTableInfo, _generator);

      Assert.That (resolvedTableInfo, Is.SameAs (_fakeSimpleTableInfo));
    }

    [Test]
    [ExpectedException (typeof (UnmappedItemException))]
    public void ResolveTableInfo_NoDomainObject_ThrowsException ()
    {
      var unresolvedTableInfo = new UnresolvedTableInfo (typeof (Student));

      _resolver.ResolveTableInfo (unresolvedTableInfo, _generator);
    }

    [Test]
    public void ResolveJoinInfo ()
    {
      var entityExpression = CreateFakeEntityExpression (typeof (Customer));
      var property = typeof (Customer).GetProperty ("Orders");
      var unresolvedJoinInfo = new UnresolvedJoinInfo (entityExpression, property, JoinCardinality.Many);
      var leftEndPoint = GetEndPointDefinition (property);

      _storageSpecificExpressionResolverStub
          .Stub (stub => stub.ResolveJoin (entityExpression, leftEndPoint, leftEndPoint.GetOppositeEndPointDefinition(), "t0"))
          .Return (_fakeJoinInfo);

      var result = _resolver.ResolveJoinInfo (unresolvedJoinInfo, _generator);

      Assert.That (result, Is.SameAs (_fakeJoinInfo));
    }

    [Test]
    public void ResolveJoinInfo_WithMixedRelationProperty ()
    {
      var entityExpression = CreateFakeEntityExpression (typeof (TargetClassForPersistentMixin));
      var memberInfo = typeof (IMixinAddingPersistentProperties).GetProperty ("RelationProperty");
      var unresolvedJoinInfo = new UnresolvedJoinInfo (entityExpression, memberInfo, JoinCardinality.One);
      var leftEndPoint = GetEndPointDefinition (typeof (TargetClassForPersistentMixin), typeof (MixinAddingPersistentProperties), memberInfo.Name);

      _storageSpecificExpressionResolverStub
          .Stub (stub => stub.ResolveJoin (entityExpression, leftEndPoint, leftEndPoint.GetOppositeEndPointDefinition(), "t0"))
          .Return (_fakeJoinInfo);

      var result = _resolver.ResolveJoinInfo (unresolvedJoinInfo, _generator);

      Assert.That (result, Is.SameAs (_fakeJoinInfo));
    }

    [Test]
    public void ResolveJoinInfo_WithDerivedClassRelationProperty ()
    {
      var entityExpression = CreateFakeEntityExpression (typeof (Company));
      var memberInfo = typeof (Customer).GetProperty ("Orders");
      var unresolvedJoinInfo = new UnresolvedJoinInfo (entityExpression, memberInfo, JoinCardinality.Many);
      var leftEndPoint = GetEndPointDefinition (memberInfo);

      _storageSpecificExpressionResolverStub
          .Stub (stub => stub.ResolveJoin (entityExpression, leftEndPoint, leftEndPoint.GetOppositeEndPointDefinition (), "t0"))
          .Return (_fakeJoinInfo);

      var result = _resolver.ResolveJoinInfo (unresolvedJoinInfo, _generator);

      Assert.That (result, Is.SameAs (_fakeJoinInfo));
    }

    [Test]
    [ExpectedException (typeof (UnmappedItemException), ExpectedMessage = 
        "The type 'Remotion.Data.DomainObjects.UnitTests.MixedDomains.TestDomain.IMixinAddingPersistentProperties' "
        + "does not identify a queryable table.")]
    public void ResolveJoinInfo_UnknownType ()
    {
      var entityExpression = CreateFakeEntityExpression (typeof (IMixinAddingPersistentProperties));
      var memberInfo = typeof (IMixinAddingPersistentProperties).GetProperty ("RelationProperty");
      var unresolvedJoinInfo = new UnresolvedJoinInfo (entityExpression, memberInfo, JoinCardinality.One);

      _resolver.ResolveJoinInfo (unresolvedJoinInfo, _generator);
    }

    [Test]
    [ExpectedException (typeof (UnmappedItemException), ExpectedMessage = "The member 'Order.OrderNumber' does not identify a relation.")]
    public void ResolveJoinInfo_NoRelation_CardinalityOne_ThrowsException ()
    {
      var entityExpression = CreateFakeEntityExpression (typeof (Order));
      var unresolvedJoinInfo = new UnresolvedJoinInfo (entityExpression, typeof (Order).GetProperty ("OrderNumber"), JoinCardinality.One);

      _resolver.ResolveJoinInfo (unresolvedJoinInfo, _generator);
    }

    [Test]
    [ExpectedException (typeof (UnmappedItemException), ExpectedMessage = "The member 'Student.Scores' does not identify a relation.")]
    public void ResolveJoinInfo_NoRelation_CardinalityMany_ThrowsExcpetion ()
    {
      var entityExpression = CreateFakeEntityExpression (typeof (Order));
      var unresolvedJoinInfo = new UnresolvedJoinInfo (entityExpression, typeof (Student).GetProperty ("Scores"), JoinCardinality.Many);

      _resolver.ResolveJoinInfo (unresolvedJoinInfo, _generator);
    }

    [Test]
    public void ResolveConstantExpression_ConstantExpression ()
    {
      var constantExpression = Expression.Constant (10);

      var expression = _resolver.ResolveConstantExpression (constantExpression);

      Assert.That (expression, Is.TypeOf (typeof (ConstantExpression)));
      Assert.That (expression, Is.SameAs (constantExpression));
    }

    [Test]
    public void ResolveConstantExpression_EntityExpression ()
    {
      Order order;
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        order = Order.NewObject();
      }
      var constantExpression = Expression.Constant (order);

      var expression = _resolver.ResolveConstantExpression (constantExpression);

      var expected = new SqlEntityConstantExpression (constantExpression.Type, order, Expression.Constant (order.ID));
      SqlExpressionTreeComparer.CheckAreEqualTrees (expected, expression);
    }

    [Test]
    public void ResolveMemberExpression_ReturnsSqlColumnDefinitionExpression ()
    {
      var property = typeof (Order).GetProperty ("OrderNumber");
      var entityExpression = CreateFakeEntityExpression (typeof (Order));
      var propertyDefinition = GetPropertyDefinition (property);

      _storageSpecificExpressionResolverStub
          .Stub (stub => stub.ResolveProperty (entityExpression, propertyDefinition))
          .Return (_fakeColumnDefinitionExpression);

      var sqlColumnExpression = (SqlColumnExpression) _resolver.ResolveMemberExpression (entityExpression, property);

      Assert.That (sqlColumnExpression, Is.SameAs (_fakeColumnDefinitionExpression));
    }

    [Test]
    public void ResolveMemberExpression_PropertyAboveInheritanceRoot ()
    {
      var property = typeof (StorageGroupClass).GetProperty ("AboveInheritanceIdentifier");
      var entityExpression = CreateFakeEntityExpression (typeof (StorageGroupClass));
      var propertyDefinition = GetPropertyDefinition (typeof (StorageGroupClass), property.DeclaringType, property.Name);

      _storageSpecificExpressionResolverStub
          .Stub (stub => stub.ResolveProperty (entityExpression, propertyDefinition))
          .Return (_fakeColumnDefinitionExpression);

      var result = _resolver.ResolveMemberExpression (entityExpression, property);

      Assert.That (result, Is.SameAs (_fakeColumnDefinitionExpression));
    }

    [Test]
    public void ResolveMemberExpression_PropertyFromDerivedClass ()
    {
      var property = typeof (Customer).GetProperty ("Type");
      var entityExpression = CreateFakeEntityExpression (typeof (Company));
      var propertyDefinition = GetPropertyDefinition (property);

      _storageSpecificExpressionResolverStub
          .Stub (stub => stub.ResolveProperty (entityExpression, propertyDefinition))
          .Return (_fakeColumnDefinitionExpression);

      var result = _resolver.ResolveMemberExpression (entityExpression, property);

      Assert.That (result, Is.SameAs (_fakeColumnDefinitionExpression));
    }

    [Test]
    public void ResolveMemberExpression_IDMember ()
    {
      var property = typeof (Order).GetProperty ("ID");
      var entityExpression = CreateFakeEntityExpression (typeof (Order));
      var fakeIDColumnExpression = new SqlColumnDefinitionExpression (typeof (ObjectID), "c", "ID", true);

      _storageSpecificExpressionResolverStub
          .Stub (stub => stub.ResolveIDProperty (entityExpression, MappingConfiguration.Current.GetTypeDefinition (typeof (Order))))
          .Return (fakeIDColumnExpression);

      var result = (SqlColumnExpression) _resolver.ResolveMemberExpression (entityExpression, property);

      Assert.That (result, Is.SameAs (fakeIDColumnExpression));
    }

    [Test]
    public void ResolveMemberExpression_RelationMember_RealSide ()
    {
      var property = typeof (Order).GetProperty ("Customer");
      var entityExpression = CreateFakeEntityExpression (typeof (Order));

      var sqlEntityRefMemberExpression = (SqlEntityRefMemberExpression) _resolver.ResolveMemberExpression (entityExpression, property);

      Assert.That (sqlEntityRefMemberExpression, Is.Not.Null);
      Assert.That (sqlEntityRefMemberExpression.MemberInfo, Is.SameAs (property));
      Assert.That (sqlEntityRefMemberExpression.OriginatingEntity, Is.SameAs (entityExpression));
    }

    [Test]
    public void ResolveMemberExpression_RelationMember_VirtualSide ()
    {
      var property = typeof (Employee).GetProperty ("Computer");
      var entityExpression = CreateFakeEntityExpression (typeof (Employee));

      var sqlEntityRefMemberExpression = (SqlEntityRefMemberExpression) _resolver.ResolveMemberExpression (entityExpression, property);

      Assert.That (sqlEntityRefMemberExpression, Is.Not.Null);
      Assert.That (sqlEntityRefMemberExpression.MemberInfo, Is.SameAs (property));
      Assert.That (sqlEntityRefMemberExpression.OriginatingEntity, Is.SameAs (entityExpression));
    }

    [Test]
    public void ResolveMemberExpression_RelationMember_VirtualSide_Collection ()
    {
      var property = typeof (Order).GetProperty ("OrderItems");
      var entityExpression = CreateFakeEntityExpression (typeof (Order));

      Assert.That (
          () => _resolver.ResolveMemberExpression (entityExpression, property), 
          Throws.TypeOf<NotSupportedException>().With.Message.EqualTo (
              "Cannot resolve a collection-valued end-point definition. ('Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems')"));
    }

    [Test]
    public void ResolveMemberExpression_MixedRelationProperty ()
    {
      var property = typeof (IMixinAddingPersistentProperties).GetProperty ("RelationProperty");
      var entityExpression = CreateFakeEntityExpression (typeof (TargetClassForPersistentMixin));

      var result = _resolver.ResolveMemberExpression (entityExpression, property);

      Assert.That (result, Is.TypeOf (typeof (SqlEntityRefMemberExpression)));
      Assert.That (((SqlEntityRefMemberExpression) result).MemberInfo, Is.SameAs (property));
      Assert.That (((SqlEntityRefMemberExpression) result).OriginatingEntity, Is.SameAs (entityExpression));
    }

    [Test]
    [ExpectedException (typeof (UnmappedItemException),
        ExpectedMessage = "The type 'Remotion.Data.DomainObjects.DomainObject' does not identify a queryable table.")]
    public void ResolveMemberExpression_UnmappedDeclaringType_ThrowsUnmappedItemException ()
    {
      var property = typeof (Student).GetProperty ("First");
      var entityExpression = CreateFakeEntityExpression (typeof (DomainObject));

      _resolver.ResolveMemberExpression (entityExpression, property);
    }

    [Test]
    [ExpectedException (typeof (UnmappedItemException),
        ExpectedMessage = "The member 'Order.NotInMapping' does not have a queryable database mapping.")]
    public void ResolveMemberExpression_UnmappedProperty_ThrowsUnmappedItemException ()
    {
      var property = typeof (Order).GetProperty ("NotInMapping");
      var entityExpression = CreateFakeEntityExpression (typeof (Order));

      _resolver.ResolveMemberExpression (entityExpression, property);
    }

    [Test]
    [ExpectedException (typeof (UnmappedItemException),
        ExpectedMessage = "The member 'Order.OriginalCustomer' does not have a queryable database mapping.")]
    public void ResolveMemberExpression_UnmappedRelationMember ()
    {
      var property = typeof (Order).GetProperty ("OriginalCustomer");
      var entityExpression = CreateFakeEntityExpression (typeof (Order));

      _resolver.ResolveMemberExpression (entityExpression, property);
    }

    [Test]
    [ExpectedException (typeof (UnmappedItemException), ExpectedMessage = "The member 'Order.NotInMapping' does not identify a mapped property.")]
    public void ResolveMemberExpression_OnColumn_WithUnmappedProperty ()
    {
      var property = typeof (Order).GetProperty ("NotInMapping");
      var columnExpression = new SqlColumnDefinitionExpression (typeof (string), "o", "Name", false);

      _resolver.ResolveMemberExpression (columnExpression, property);
    }

    [Test]
    public void ResolveTypeCheck_DesiredTypeIsAssignableFromExpressionType ()
    {
      var sqlEntityExpression = (SqlEntityExpression) CreateFakeEntityExpression (typeof (Company));

      var result = _resolver.ResolveTypeCheck (sqlEntityExpression, typeof (Company));

      Assert.That (result, Is.TypeOf (typeof (ConstantExpression)));
    }

    [Test]
    public void ResolveTypeCheck_NoDomainTypeWithSameDesiredType ()
    {
      var sqlEntityExpression = CreateFakeEntityExpression (typeof (Student));
      var result = _resolver.ResolveTypeCheck (sqlEntityExpression, typeof (Student));

      SqlExpressionTreeComparer.CheckAreEqualTrees (result, Expression.Constant (true));
    }

    [Test]
    [ExpectedException (typeof (UnmappedItemException))]
    public void ResolveTypeCheck_NoDomainTypeWithDifferentDesiredType ()
    {
      var sqlEntityExpression = CreateFakeEntityExpression (typeof (object));
      _resolver.ResolveTypeCheck (sqlEntityExpression, typeof (Student));
    }

    [Test]
    public void ResolveTypeCheck_NoQueryableTable ()
    {
      var sqlEntityExpression = (SqlEntityExpression) CreateFakeEntityExpression (typeof (AboveInheritanceRootClass));

      var result = _resolver.ResolveTypeCheck (sqlEntityExpression, typeof (StorageGroupClass));

      var idExpression = Expression.MakeMemberAccess (sqlEntityExpression, typeof (DomainObject).GetProperty ("ID"));
      var classIDExpression = Expression.MakeMemberAccess (idExpression, typeof (ObjectID).GetProperty ("ClassID"));
      var expectedExpression = new SqlInExpression (
          classIDExpression, new SqlCollectionExpression (typeof (string[]), new Expression[] { new SqlLiteralExpression ("StorageGroupClass") }));

      SqlExpressionTreeComparer.CheckAreEqualTrees (result, expectedExpression);
    }

    [Test]
    public void ResolveTypeCheck_DesiredTypeNotRelatedWithExpressionType ()
    {
      var sqlEntityExpression = (SqlEntityExpression) CreateFakeEntityExpression (typeof (Company));

      var result = _resolver.ResolveTypeCheck (sqlEntityExpression, typeof (Student));

      SqlExpressionTreeComparer.CheckAreEqualTrees (result, Expression.Constant (false));
    }

    [Test]
    public void ResolveTypeCheck_ExpressionTypeIsAssignableFromDesiredType_WithMultiplePossibleClassIDs ()
    {
      var sqlEntityExpression = (SqlEntityExpression) CreateFakeEntityExpression (typeof (Company));

      var result = _resolver.ResolveTypeCheck (sqlEntityExpression, typeof (Partner));

      var idExpression = Expression.MakeMemberAccess (sqlEntityExpression, typeof (DomainObject).GetProperty ("ID"));
      var classIDExpression = Expression.MakeMemberAccess (idExpression, typeof (ObjectID).GetProperty ("ClassID"));
      var expectedExpression = new SqlInExpression (
          classIDExpression,
          new SqlCollectionExpression (
              typeof (string[]),
              new Expression[]
              { 
                  new SqlLiteralExpression ("Partner"), 
                  new SqlLiteralExpression ("Distributor"), 
                  new SqlLiteralExpression ("Supplier") 
              }));
      
      SqlExpressionTreeComparer.CheckAreEqualTrees (result, expectedExpression);
    }

    [Test]
    public void ResolveTypeCheck_ExpressionTypeIsAssignableFromDesiredType_WithSingleClassID ()
    {
      var sqlEntityExpression = (SqlEntityExpression) CreateFakeEntityExpression (typeof (Company));

      var result = _resolver.ResolveTypeCheck (sqlEntityExpression, typeof (Distributor));

      var idExpression = Expression.MakeMemberAccess (sqlEntityExpression, typeof (DomainObject).GetProperty ("ID"));
      var classIDExpression = Expression.MakeMemberAccess (idExpression, typeof (ObjectID).GetProperty ("ClassID"));
      var expectedExpression = new SqlInExpression (
          classIDExpression, new SqlCollectionExpression (typeof (string[]), new Expression[] { new SqlLiteralExpression ("Distributor") }));
      
      SqlExpressionTreeComparer.CheckAreEqualTrees (result, expectedExpression);
    }

    [Test]
    public void TryResolveOptimizedIdentity_ForForeignKeyRelationProperty_ResolvesRelationEndPoint ()
    {
      var entityExpression = CreateFakeEntityExpression (typeof (Order));
      var property = typeof (Order).GetProperty ("Customer");
      var entityRefMemberExpression = new SqlEntityRefMemberExpression (entityExpression, property);

      var endPointDefinition = (RelationEndPointDefinition) GetEndPointDefinition (property);
      var fakeResolvedIdentity = Expression.Constant (0);
      _storageSpecificExpressionResolverStub
          .Stub (stub => stub.ResolveEntityIdentityViaForeignKey (entityExpression, endPointDefinition))
          .Return (fakeResolvedIdentity);

      var result = _resolver.TryResolveOptimizedIdentity (entityRefMemberExpression);

      Assert.That (result, Is.SameAs (fakeResolvedIdentity));
    }

    [Test]
    public void TryResolveOptimizedIdentity_ForVirtualRelationProperty_ReturnsNull ()
    {
      var entityExpression = CreateFakeEntityExpression (typeof (Order));
      var property = typeof (Order).GetProperty ("OrderTicket");
      var entityRefMemberExpression = new SqlEntityRefMemberExpression (entityExpression, property);

      var result = _resolver.TryResolveOptimizedIdentity (entityRefMemberExpression);

      _storageSpecificExpressionResolverStub.AssertWasNotCalled (
          stub => stub.ResolveEntityIdentityViaForeignKey (Arg<SqlEntityExpression>.Is.Anything, Arg<RelationEndPointDefinition>.Is.Anything));
      Assert.That (result, Is.Null);
    }

    [Test]
    public void TryResolveOptimizedMemberExpression_ForForeignKeyRelationProperty_WithIDMember_ResolvesRelationEndPoint ()
    {
      var entityExpression = CreateFakeEntityExpression (typeof (Order));
      var property = typeof (Order).GetProperty ("Customer");
      var entityRefMemberExpression = new SqlEntityRefMemberExpression (entityExpression, property);

      var endPointDefinition = (RelationEndPointDefinition) GetEndPointDefinition (property);
      var fakeResolvedIdentity = Expression.Constant (0);
      _storageSpecificExpressionResolverStub
          .Stub (stub => stub.ResolveIDPropertyViaForeignKey (entityExpression, endPointDefinition))
          .Return (fakeResolvedIdentity);

      var result = _resolver.TryResolveOptimizedMemberExpression (entityRefMemberExpression, typeof (DomainObject).GetProperty ("ID"));

      Assert.That (result, Is.SameAs (fakeResolvedIdentity));
    }

    [Test]
    public void TryResolveOptimizedMemberExpression_ForForeignKeyRelationProperty_WithNonIDMember_ReturnsNull ()
    {
      var entityExpression = CreateFakeEntityExpression (typeof (Order));
      var property = typeof (Order).GetProperty ("Customer");
      var entityRefMemberExpression = new SqlEntityRefMemberExpression (entityExpression, property);
      
      var result = _resolver.TryResolveOptimizedMemberExpression (entityRefMemberExpression, typeof (DomainObject).GetProperty ("State"));

      _storageSpecificExpressionResolverStub
          .AssertWasNotCalled (
              stub => stub.ResolveIDPropertyViaForeignKey (Arg<SqlEntityExpression>.Is.Anything, Arg<RelationEndPointDefinition>.Is.Anything));
      Assert.That (result, Is.Null);
    }

    [Test]
    public void TryResolveOptimizedMemberExpression_ForForeignKeyRelationProperty_WithNonDomainObjectID_ReturnsNull ()
    {
      var entityExpression = CreateFakeEntityExpression (typeof (Order));
      var property = typeof (Order).GetProperty ("Customer");
      var entityRefMemberExpression = new SqlEntityRefMemberExpression (entityExpression, property);

      var result = _resolver.TryResolveOptimizedMemberExpression (entityRefMemberExpression, typeof (FakeObject).GetProperty ("ID"));

      _storageSpecificExpressionResolverStub
          .AssertWasNotCalled (
              stub => stub.ResolveIDPropertyViaForeignKey (Arg<SqlEntityExpression>.Is.Anything, Arg<RelationEndPointDefinition>.Is.Anything));
      Assert.That (result, Is.Null);
    }

    [Test]
    public void TryResolveOptimizedMemberExpression_ForVirtualRelationProperty_ReturnsNull ()
    {
      var entityExpression = CreateFakeEntityExpression (typeof (Order));
      var property = typeof (Order).GetProperty ("OrderTicket");
      var entityRefMemberExpression = new SqlEntityRefMemberExpression (entityExpression, property);

      var result = _resolver.TryResolveOptimizedMemberExpression (entityRefMemberExpression, typeof (DomainObject).GetProperty ("ID"));

      _storageSpecificExpressionResolverStub.AssertWasNotCalled (
          stub => stub.ResolveIDPropertyViaForeignKey (Arg<SqlEntityExpression>.Is.Anything, Arg<RelationEndPointDefinition>.Is.Anything));
      Assert.That (result, Is.Null);
    }

    private SqlEntityDefinitionExpression CreateFakeEntityExpression (Type classType)
    {
      var starColumn = new SqlColumnDefinitionExpression (classType, "o", "*", false);
      return new SqlEntityDefinitionExpression (classType, "o", null, e => e.GetColumn (typeof (ObjectID), "ID", true), starColumn);
    }

    private PropertyDefinition GetPropertyDefinition (PropertyInfo property)
    {
      return GetPropertyDefinition (property.DeclaringType, property.Name);
    }

    private IRelationEndPointDefinition GetEndPointDefinition (PropertyInfo property)
    {
      return GetEndPointDefinition (property.DeclaringType, property.Name);
    }

    public class FakeObject
    {
      public ObjectID ID { get; set; }
    }
  }
}