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
using System.Linq;
using System.Linq.Expressions;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Linq.ExecutableQueries;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.DomainObjects.Queries.EagerFetching;
using Remotion.Data.DomainObjects.UnitTests.Linq.TestDomain.Error.SortExpressionForPropertyWithoutInterface;
using Remotion.Data.DomainObjects.UnitTests.Linq.TestDomain.Success.SortExpressionForPropertyOnDerivedType;
using Remotion.Data.DomainObjects.UnitTests.MixedDomains.TestDomain;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.Clauses.StreamedData;
using Remotion.Linq.EagerFetching;
using Remotion.Linq.SqlBackend.SqlGeneration;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Linq
{
  [TestFixture]
  public class DomainObjectQueryGeneratorTest : StandardMappingTest
  {
    private Mock<ISqlQueryGenerator> _sqlQueryGeneratorMock;
    private ITypeConversionProvider _typeConversionProvider;
    private Mock<IStorageTypeInformationProvider> _storageTypeInformationProviderStub;

    private DomainObjectQueryGenerator _generator;

    private TypeDefinition _customerTypeDefinition;
    private QueryModel _customerQueryModel;

    public override void SetUp ()
    {
      base.SetUp();

      _sqlQueryGeneratorMock = new Mock<ISqlQueryGenerator>(MockBehavior.Strict);
      _typeConversionProvider = SafeServiceLocator.Current.GetInstance<ITypeConversionProvider>();
      _storageTypeInformationProviderStub = new Mock<IStorageTypeInformationProvider>();

      _generator = new DomainObjectQueryGenerator(
          _sqlQueryGeneratorMock.Object,
          _typeConversionProvider,
          _storageTypeInformationProviderStub.Object,
          Configuration);

      _customerTypeDefinition = GetTypeDefinition(typeof(Customer));
      _customerQueryModel = QueryModelObjectMother.Create(Expression.Constant(null, typeof(Customer)));
    }

    [Test]
    public void CreateScalarQuery ()
    {
      Expression<Func<IDatabaseResultRow, int>> inMemoryProjection = row => CheckScalarResultRowAdapter(row, "an object", 42);

      var fakeSqlQueryResult = CreateSqlQueryGeneratorResult(
          "SELECT x",
          inMemoryProjectionParameter: inMemoryProjection.Parameters.Single(),
          inMemoryProjectionBody: inMemoryProjection.Body);
      _sqlQueryGeneratorMock.Setup(mock => mock.CreateSqlQuery(_customerQueryModel)).Returns(fakeSqlQueryResult).Verifiable();

      var result = _generator.CreateScalarQuery<int>("id", TestDomainStorageProviderDefinition, _customerQueryModel);

      _sqlQueryGeneratorMock.Verify();

      Assert.That(result, Is.Not.Null);
      Assert.That(result, Is.TypeOf(typeof(ScalarQueryAdapter<int>)));
      Assert.That(result.ID, Is.EqualTo("id"));
      Assert.That(result.StorageProviderDefinition, Is.SameAs(TestDomainStorageProviderDefinition));
      Assert.That(result.Statement, Is.EqualTo("SELECT x"));
      Assert.That(result.QueryType, Is.EqualTo(QueryType.Scalar));
      Assert.That(result.CollectionType, Is.Null);
      Assert.That(result.EagerFetchQueries, Is.Empty);

      var resultConversion = ((ScalarQueryAdapter<int>)result).ResultConversion;
      Assert.That(resultConversion("an object"), Is.EqualTo(42));
    }

    [Test]
    public void CreateScalarQuery_NoParameters ()
    {
      var fakeSqlQueryResult = CreateSqlQueryGeneratorResult(parameters: new CommandParameter[0]);
      _sqlQueryGeneratorMock.Setup(mock => mock.CreateSqlQuery(_customerQueryModel)).Returns(fakeSqlQueryResult).Verifiable();

      var result = _generator.CreateScalarQuery<int>("id", TestDomainStorageProviderDefinition, _customerQueryModel);

      _sqlQueryGeneratorMock.Verify();
      Assert.That(result.Parameters, Is.Empty);
    }

    [Test]
    public void CreateScalarQuery_WithParameters ()
    {
      var fakeSqlQueryResult = CreateSqlQueryGeneratorResult(parameters: new[] { new CommandParameter("p0", "paramval") });
      _sqlQueryGeneratorMock.Setup(stub => stub.CreateSqlQuery(_customerQueryModel)).Returns(fakeSqlQueryResult).Verifiable();

      var result = _generator.CreateScalarQuery<int>("id", TestDomainStorageProviderDefinition, _customerQueryModel);

      _sqlQueryGeneratorMock.Verify();
      Assert.That(result.Parameters, Is.EqualTo(new[] { new QueryParameter("p0", "paramval", QueryParameterType.Value) }));
    }

    [Test]
    public void CreateSequenceQuery_EntityQuery ()
    {
      var fakeSqlQueryResult = CreateSqlQueryGeneratorResult("SELECT x", selectedEntityType: typeof(Order));
      _sqlQueryGeneratorMock.Setup(mock => mock.CreateSqlQuery(_customerQueryModel)).Returns(fakeSqlQueryResult).Verifiable();

      var result = _generator.CreateSequenceQuery<Order>(
          "id",
          TestDomainStorageProviderDefinition,
          _customerQueryModel,
          Enumerable.Empty<FetchQueryModelBuilder>());

      _sqlQueryGeneratorMock.Verify();
      Assert.That(result, Is.TypeOf(typeof(DomainObjectSequenceQueryAdapter<Order>)));
      Assert.That(result.ID, Is.EqualTo("id"));
      Assert.That(result.StorageProviderDefinition, Is.EqualTo(TestDomainStorageProviderDefinition));
      Assert.That(result.Statement, Is.EqualTo("SELECT x"));
      Assert.That(result.QueryType, Is.EqualTo(QueryType.Collection));
      Assert.That(result.CollectionType, Is.EqualTo(typeof(ObjectList<Order>)));
      Assert.That(result.EagerFetchQueries, Is.Empty);
    }

    [Test]
    public void CreateSequenceQuery_EntityQuery_WithProjectionTypeOtherThanDomainObject_InfersCollectionTypeFromSelectedEntityType ()
    {
      var fakeSqlQueryResult = CreateSqlQueryGeneratorResult("SELECT x", selectedEntityType: typeof(Order));
      _sqlQueryGeneratorMock.Setup(mock => mock.CreateSqlQuery(_customerQueryModel)).Returns(fakeSqlQueryResult).Verifiable();

      var result = _generator.CreateSequenceQuery<IOrder>(
          "id",
          TestDomainStorageProviderDefinition,
          _customerQueryModel,
          Enumerable.Empty<FetchQueryModelBuilder>());

      _sqlQueryGeneratorMock.Verify();
      Assert.That(result, Is.TypeOf(typeof(DomainObjectSequenceQueryAdapter<IOrder>)));
      Assert.That(result.QueryType, Is.EqualTo(QueryType.Collection));
      Assert.That(result.CollectionType, Is.EqualTo(typeof(ObjectList<Order>)));
    }

    [Test]
    public void CreateSequenceQuery_EntityQuery_NoParameters ()
    {
      var fakeSqlQueryResult = CreateSqlQueryGeneratorResult(
        selectedEntityType: typeof(Order),
        parameters: new CommandParameter[0]);
      _sqlQueryGeneratorMock.Setup(mock => mock.CreateSqlQuery(_customerQueryModel)).Returns(fakeSqlQueryResult).Verifiable();

      var result = _generator.CreateSequenceQuery<Order>(
          "id", TestDomainStorageProviderDefinition, _customerQueryModel, Enumerable.Empty<FetchQueryModelBuilder>());

      _sqlQueryGeneratorMock.Verify();
      Assert.That(result.Parameters, Is.Empty);
    }

    [Test]
    public void CreateSequenceQuery_EntityQuery_WithParameters ()
    {
      var fakeSqlQueryResult = CreateSqlQueryGeneratorResult(
          selectedEntityType: typeof(Order),
          parameters: new[] { new CommandParameter("p0", "paramval") });
      _sqlQueryGeneratorMock.Setup(stub => stub.CreateSqlQuery(_customerQueryModel)).Returns(fakeSqlQueryResult);

      var result = _generator.CreateSequenceQuery<Order>(
          "id", TestDomainStorageProviderDefinition, _customerQueryModel, Enumerable.Empty<FetchQueryModelBuilder>());

      Assert.That(result.Parameters, Is.EqualTo(new[] { new QueryParameter("p0", "paramval", QueryParameterType.Value) }));
    }

    [Test]
    public void CreateSequenceQuery_EntityQuery_WithFetchRequests ()
    {
      var fakeSqlQuery = CreateSqlQueryGeneratorResult(selectedEntityType: typeof(Customer));
      _sqlQueryGeneratorMock.Setup(stub => stub.CreateSqlQuery(_customerQueryModel)).Returns(fakeSqlQuery);

      var fetchQueryModelBuilder = CreateFetchOneQueryModelBuilder((Customer o) => o.Ceo);
      var fakeFetchSqlQueryResult = CreateSqlQueryGeneratorResult(commandText: "FETCH", selectedEntityType: typeof(Ceo));

      _sqlQueryGeneratorMock
          .Setup(mock => mock.CreateSqlQuery(It.Is<QueryModel>(qm => qm.SelectClause.Selector.Type == typeof(Ceo))))
          .Returns(fakeFetchSqlQueryResult)
          .Callback((QueryModel queryModel) =>
{
            var actualQueryModel = queryModel;
            var fetchQueryModel = fetchQueryModelBuilder.GetOrCreateFetchQueryModel();
            CheckActualFetchQueryModel(actualQueryModel, fetchQueryModel);
          })
          .Verifiable();

      var result = _generator.CreateSequenceQuery<Customer>("id", TestDomainStorageProviderDefinition, _customerQueryModel, new[] { fetchQueryModelBuilder });

      _sqlQueryGeneratorMock.Verify();
      CheckSingleFetchRequest(result.EagerFetchQueries, typeof(Company), "Ceo", "FETCH", typeof(Ceo));
    }

    [Test]
    public void CreateSequenceQuery_EntityQuery_WithDownCast ()
    {
      var sequence = new VerifiableSequence();
      var fakeSqlQuery = CreateSqlQueryGeneratorResult(selectedEntityType: typeof(Company));
      var targetTypeQueryModel = QueryModelObjectMother.Create(Expression.Constant(null, typeof(Company)));
      _sqlQueryGeneratorMock.InVerifiableSequence(sequence).Setup(stub => stub.CreateSqlQuery(targetTypeQueryModel)).Returns(fakeSqlQuery);

      var fetchQueryModelBuilder = CreateFetchOneQueryModelBuilder((Partner o) => o.ContactPerson, targetTypeQueryModel);
      var fakeFetchSqlQueryResult = CreateSqlQueryGeneratorResult(commandText: "FETCH", selectedEntityType: typeof(Person));

      _sqlQueryGeneratorMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.CreateSqlQuery(It.Is<QueryModel>(p => p != targetTypeQueryModel)))
          .Returns(fakeFetchSqlQueryResult)
          .Callback(
              (QueryModel queryModel) =>
              {
                var actualQueryModel = queryModel;
                var fetchQueryModel = fetchQueryModelBuilder.GetOrCreateFetchQueryModel();
                CheckActualFetchQueryModel(actualQueryModel, fetchQueryModel);
              })
          .Verifiable();

      var result = _generator.CreateSequenceQuery<Company>("id", TestDomainStorageProviderDefinition, targetTypeQueryModel, new[] { fetchQueryModelBuilder });

      _sqlQueryGeneratorMock.Verify();
      sequence.Verify();
      var expectedEndPointDefinition = GetEndPointDefinition(typeof(Partner), typeof(Partner), "ContactPerson");
      CheckSingleFetchRequest(result.EagerFetchQueries, expectedEndPointDefinition, "FETCH", typeof(Person));
    }

    [Test]
    public void CreateSequenceQuery_EntityQuery_WithMixinFetchRequest ()
    {
      var sequence = new VerifiableSequence();

      var fakeSqlQuery = CreateSqlQueryGeneratorResult(selectedEntityType: typeof(TargetClassForPersistentMixin));
      var targetTypeQueryModel = QueryModelObjectMother.Create(Expression.Constant(null, typeof(TargetClassForPersistentMixin)));
      _sqlQueryGeneratorMock.InVerifiableSequence(sequence).Setup(stub => stub.CreateSqlQuery(targetTypeQueryModel)).Returns(fakeSqlQuery);

      var fetchQueryModelBuilder = CreateFetchOneQueryModelBuilder((IMixinAddingPersistentProperties o) => o.RelationProperty);
      var fakeFetchSqlQueryResult = CreateSqlQueryGeneratorResult(commandText: "FETCH", selectedEntityType: typeof(RelationTargetForPersistentMixin));

      _sqlQueryGeneratorMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.CreateSqlQuery(It.Is<QueryModel>(p => p != targetTypeQueryModel)))
          .Returns(fakeFetchSqlQueryResult)
          .Callback(
              (QueryModel queryModel) =>
              {
                var actualQueryModel = queryModel;
                var fetchQueryModel = fetchQueryModelBuilder.GetOrCreateFetchQueryModel();
                CheckActualFetchQueryModel(actualQueryModel, fetchQueryModel);
              })
          .Verifiable();

      var result = _generator.CreateSequenceQuery<TargetClassForPersistentMixin>("id", TestDomainStorageProviderDefinition, targetTypeQueryModel, new[] { fetchQueryModelBuilder });

      _sqlQueryGeneratorMock.Verify();
      sequence.Verify();
      var expectedEndPointDefinition = GetEndPointDefinition(
          typeof(TargetClassForPersistentMixin), typeof(MixinAddingPersistentProperties), "RelationProperty");
      CheckSingleFetchRequest(result.EagerFetchQueries, expectedEndPointDefinition, "FETCH", typeof(RelationTargetForPersistentMixin));
    }

    [Test]
    [Ignore("TODO RM-7294: implement test for VirtualCollection")]
    public void CreateSequenceQuery_EntityQuery_WithFetchRequestDoesNotSupportExpression ()
    {
    }

    [Test]
    public void CreateSequenceQuery_EntityQuery_WithFetchRequestWithSortExpression ()
    {
      var sequence = new VerifiableSequence();

      var fakeSqlQuery = CreateSqlQueryGeneratorResult(selectedEntityType: typeof(Customer));
      _sqlQueryGeneratorMock.InVerifiableSequence(sequence).Setup(stub => stub.CreateSqlQuery(_customerQueryModel)).Returns(fakeSqlQuery);

      var fetchQueryModelBuilder = CreateFetchManyQueryModelBuilder((Customer o) => o.Orders);
      var fakeFetchSqlQueryResult = CreateSqlQueryGeneratorResult(commandText: "FETCH", selectedEntityType: typeof(Order));

      _sqlQueryGeneratorMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.CreateSqlQuery(It.Is<QueryModel>(p => p != _customerQueryModel)))
          .Returns(fakeFetchSqlQueryResult)
          .Callback(
              (QueryModel queryModel) =>
              {
                var actualQueryModel = queryModel;
                var fetchQueryModel = fetchQueryModelBuilder.GetOrCreateFetchQueryModel();

                Assert.That(actualQueryModel.MainFromClause.FromExpression, Is.TypeOf<SubQueryExpression>());
                CheckActualFetchQueryModel(((SubQueryExpression)actualQueryModel.MainFromClause.FromExpression).QueryModel, fetchQueryModel);

                Assert.That(actualQueryModel.BodyClauses, Has.Some.TypeOf<OrderByClause>());
                var orderByClause = (OrderByClause)actualQueryModel.BodyClauses.Single();
                var endPointDefinition = ((DomainObjectCollectionRelationEndPointDefinition)GetEndPointDefinition(typeof(Customer), "Orders"));
                Assert.That(endPointDefinition.GetSortExpression().ToString(), Is.EqualTo("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber ASC"));
                var orderNumberMember = NormalizingMemberInfoFromExpressionUtility.GetProperty((Order o) => o.OrderNumber);
                Assert.That(((MemberExpression)orderByClause.Orderings[0].Expression).Member, Is.SameAs(orderNumberMember));
                Assert.That(orderByClause.Orderings[0].OrderingDirection, Is.EqualTo(OrderingDirection.Asc));
              })
          .Verifiable();

      _generator.CreateSequenceQuery<Customer>("id", TestDomainStorageProviderDefinition, _customerQueryModel, new[] { fetchQueryModelBuilder });

      _sqlQueryGeneratorMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void CreateSequenceQuery_EntityQuery_WithFetchRequestWithSortExpressionBasedOnMixinProperty ()
    {
      var sequence = new VerifiableSequence();

      var fakeSqlQuery = CreateSqlQueryGeneratorResult(selectedEntityType: typeof(RelationTargetForPersistentMixin));
      var targetTypeQueryModel = QueryModelObjectMother.Create(Expression.Constant(null, typeof(RelationTargetForPersistentMixin)));
      _sqlQueryGeneratorMock.InVerifiableSequence(sequence).Setup(stub => stub.CreateSqlQuery(targetTypeQueryModel)).Returns(fakeSqlQuery);

      var fetchQueryModelBuilder = CreateFetchManyQueryModelBuilder((RelationTargetForPersistentMixin o) => o.RelationProperty4, targetTypeQueryModel);
      var fakeFetchSqlQueryResult = CreateSqlQueryGeneratorResult(commandText: "FETCH", selectedEntityType: typeof(TargetClassForPersistentMixin));

      _sqlQueryGeneratorMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.CreateSqlQuery(It.Is<QueryModel>(p => p != targetTypeQueryModel)))
          .Returns(fakeFetchSqlQueryResult)
          .Callback(
              (QueryModel queryModel) =>
              {
                var actualQueryModel = queryModel;
                var fetchQueryModel = fetchQueryModelBuilder.GetOrCreateFetchQueryModel();

                Assert.That(actualQueryModel.MainFromClause.FromExpression, Is.TypeOf<SubQueryExpression>());
                CheckActualFetchQueryModel(((SubQueryExpression)actualQueryModel.MainFromClause.FromExpression).QueryModel, fetchQueryModel);

                Assert.That(actualQueryModel.BodyClauses, Has.Some.TypeOf<OrderByClause>());
                var orderByClause = (OrderByClause)actualQueryModel.BodyClauses.Single();
                var endPointDefinition = ((DomainObjectCollectionRelationEndPointDefinition)GetEndPointDefinition(
                    typeof(RelationTargetForPersistentMixin),
                    "RelationProperty4"));
                Assert.That(
                    endPointDefinition.GetSortExpression().ToString(),
                    Is.EqualTo(
                        "Remotion.Data.DomainObjects.UnitTests.MixedDomains.TestDomain.MixinAddingPersistentProperties.PersistentProperty ASC"));
                var sortedByMember =
                    NormalizingMemberInfoFromExpressionUtility.GetProperty((IMixinAddingPersistentProperties o) => o.PersistentProperty);
                Assert.That(((MemberExpression)orderByClause.Orderings[0].Expression).Member, Is.SameAs(sortedByMember));
                Assert.That(orderByClause.Orderings[0].OrderingDirection, Is.EqualTo(OrderingDirection.Asc));
              })
          .Verifiable();
      _generator.CreateSequenceQuery<RelationTargetForPersistentMixin>(
          "id",
          TestDomainStorageProviderDefinition,
          targetTypeQueryModel,
          new[] { fetchQueryModelBuilder });

      _sqlQueryGeneratorMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void CreateSequenceQuery_EntityQuery_WithFetchRequestWithSortExpressionBasedOnMixinPropertyNotPartOfInterface_ThrowsNotSupportedException ()
    {
      var fakeSqlQuery =
          CreateSqlQueryGeneratorResult(
              selectedEntityType: typeof(RelationTarget));
      var targetTypeQueryModel =
          QueryModelObjectMother.Create(
              Expression.Constant(null, typeof(RelationTarget)));
      _sqlQueryGeneratorMock.Setup(stub => stub.CreateSqlQuery(targetTypeQueryModel)).Returns(fakeSqlQuery);

      var fetchQueryModelBuilder =
          CreateFetchManyQueryModelBuilder(
              (RelationTarget o) => o.CollectionProperty,
              targetTypeQueryModel);

      _sqlQueryGeneratorMock
          .Setup(mock => mock.CreateSqlQuery(It.Is<QueryModel>(p => p != targetTypeQueryModel)))
          .Throws(new InvalidOperationException("Should not be called because of expected NotSupportedException at earlier point in execution."));

      Assert.That(
          () => _generator.CreateSequenceQuery<RelationTarget>("id", TestDomainStorageProviderDefinition, targetTypeQueryModel, new[] { fetchQueryModelBuilder }),
          Throws.TypeOf<NotSupportedException>().And.Message.EqualTo(
              "The member 'Remotion.Data.DomainObjects.UnitTests.Linq.TestDomain.Error.SortExpressionForPropertyWithoutInterface.RelationMixin.SortProperty' "
              + "is not part of any interface introduced onto the target class "
              + "'Remotion.Data.DomainObjects.UnitTests.Linq.TestDomain.Error.SortExpressionForPropertyWithoutInterface.MixinTarget'. "
              + "Only mixed properties that are part of an introduced interface can be used within the sort-expression of a collection property."));
    }

    [Test]
    public void CreateSequenceQuery_EntityQuery_WithFetchRequestWithSortExpressionBasedOnPropertyDeclaredOnDerivedType ()
    {
      var sequence = new VerifiableSequence();

      var fakeSqlQuery =
          CreateSqlQueryGeneratorResult(
              selectedEntityType: typeof(RelationTargetManySide));
      var targetTypeQueryModel =
          QueryModelObjectMother.Create(
              Expression.Constant(null, typeof(RelationTargetManySide)));
      _sqlQueryGeneratorMock.InVerifiableSequence(sequence).Setup(stub => stub.CreateSqlQuery(targetTypeQueryModel)).Returns(fakeSqlQuery);

      var fetchQueryModelBuilder =
          CreateFetchManyQueryModelBuilder(
              (RelationTargetManySide o) => o.CollectionProperty,
              targetTypeQueryModel);
      var fakeFetchSqlQueryResult = CreateSqlQueryGeneratorResult(commandText: "FETCH", selectedEntityType: typeof(RelationTargetOneSide));

      _sqlQueryGeneratorMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.CreateSqlQuery(It.Is<QueryModel>(p => p != targetTypeQueryModel)))
          .Returns(fakeFetchSqlQueryResult)
          .Callback(
              (QueryModel queryModel) =>
              {
                var actualQueryModel = queryModel;
                var fetchQueryModel = fetchQueryModelBuilder.GetOrCreateFetchQueryModel();

                Assert.That(actualQueryModel.MainFromClause.FromExpression, Is.TypeOf<SubQueryExpression>());
                CheckActualFetchQueryModel(((SubQueryExpression)actualQueryModel.MainFromClause.FromExpression).QueryModel, fetchQueryModel);

                Assert.That(actualQueryModel.BodyClauses, Has.Some.TypeOf<OrderByClause>());
                var orderByClause = (OrderByClause)actualQueryModel.BodyClauses.Single();
                var endPointDefinition = (DomainObjectCollectionRelationEndPointDefinition)GetEndPointDefinition(typeof(RelationTargetManySide), "CollectionProperty");
                Assert.That(
                    endPointDefinition.GetSortExpression().ToString(),
                    Is.EqualTo(
                        "Remotion.Data.DomainObjects.UnitTests.Linq.TestDomain.Success.SortExpressionForPropertyOnDerivedType.DerivedRelationTargetOneSide.SortProperty ASC"));
                var sortedByMember = NormalizingMemberInfoFromExpressionUtility.GetProperty((DerivedRelationTargetOneSide o) => o.SortProperty);
                Assert.That(((MemberExpression)orderByClause.Orderings[0].Expression).Member, Is.SameAs(sortedByMember));
                Assert.That(orderByClause.Orderings[0].OrderingDirection, Is.EqualTo(OrderingDirection.Asc));
              })
          .Verifiable();

      _generator.CreateSequenceQuery<RelationTargetManySide>("id", TestDomainStorageProviderDefinition, targetTypeQueryModel, new[] { fetchQueryModelBuilder });

      _sqlQueryGeneratorMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void CreateSequenceQuery_EntityQuery_WithNestedFetchRequests ()
    {
      var sequence = new VerifiableSequence();

      var fakeSqlQuery = CreateSqlQueryGeneratorResult(selectedEntityType: typeof(Customer));
      _sqlQueryGeneratorMock.InVerifiableSequence(sequence).Setup(stub => stub.CreateSqlQuery(_customerQueryModel)).Returns(fakeSqlQuery);

      var fetchQueryModelBuilder = CreateFetchOneQueryModelBuilder((Customer c) => c.Ceo);
      var fakeFetchSqlQueryResult = CreateSqlQueryGeneratorResult(selectedEntityType: typeof(Ceo), commandText: "FETCH");
      _sqlQueryGeneratorMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.CreateSqlQuery(It.Is<QueryModel>(qm => qm.SelectClause.Selector.Type == typeof(Ceo))))
          .Returns(fakeFetchSqlQueryResult)
          .Verifiable();

      var innerFetchRequest = CreateFetchOneRequest((Ceo c) => c.Company);
      fetchQueryModelBuilder.FetchRequest.GetOrAddInnerFetchRequest(innerFetchRequest);

      var fakeInnerFetchSqlQueryResult = CreateSqlQueryGeneratorResult(commandText: "INNER FETCH", selectedEntityType: typeof(Company));
      _sqlQueryGeneratorMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.CreateSqlQuery(It.Is<QueryModel>(qm => qm.SelectClause.Selector.Type == typeof(Company))))
          .Returns(fakeInnerFetchSqlQueryResult)
          .Callback(
              (QueryModel queryModel) =>
              {
                var actualQueryModel = queryModel;
                Assert.That(((StreamedSequenceInfo)actualQueryModel.GetOutputDataInfo()).ItemExpression.Type, Is.SameAs(typeof(Company)));
              })
          .Verifiable();

      var result = _generator.CreateSequenceQuery<Customer>("id", TestDomainStorageProviderDefinition, _customerQueryModel, new[] { fetchQueryModelBuilder });

      _sqlQueryGeneratorMock.Verify();
      sequence.Verify();

      var fetchQuery = result.EagerFetchQueries.Single();
      CheckSingleFetchRequest(fetchQuery.Value.EagerFetchQueries, typeof(Ceo), "Company", "INNER FETCH", typeof(Company));
    }

    [Test]
    public void CreateSequenceQuery_EntityQuery_WithInvalidFetchRequest_MemberIsNoPropertyInfo ()
    {
      var fakeSqlQuery = CreateSqlQueryGeneratorResult(selectedEntityType: typeof(Order));
      _sqlQueryGeneratorMock.Setup(stub => stub.CreateSqlQuery(_customerQueryModel)).Returns(fakeSqlQuery);

      var fetchQueryModelBuilder = CreateFetchOneQueryModelBuilder((Customer o) => o.CtorCalled);
      Assert.That(
          () => _generator.CreateSequenceQuery<Order>("id", TestDomainStorageProviderDefinition, _customerQueryModel, new[] { fetchQueryModelBuilder }),
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo(
                  "The member 'CtorCalled' is a 'Field', which cannot be fetched by this LINQ provider. Only properties can be fetched."));
    }

    [Test]
    public void CreateSequenceQuery_EntityQuery_WithInvalidFetchRequest_MemberIsNoRelationProperty ()
    {
      var fakeSqlQuery = CreateSqlQueryGeneratorResult(selectedEntityType: typeof(Order));
      _sqlQueryGeneratorMock.Setup(stub => stub.CreateSqlQuery(_customerQueryModel)).Returns(fakeSqlQuery);

      var fetchQueryModelBuilder = CreateFetchOneQueryModelBuilder((Customer o) => o.Name);
      Assert.That(
          () => _generator.CreateSequenceQuery<Order>("id", TestDomainStorageProviderDefinition, _customerQueryModel, new[] { fetchQueryModelBuilder }),
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo(
                  "The property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Company.Name' is not a relation end point. "
                  + "Fetching it is not supported by this LINQ provider."));
    }

    [Test]
    public void CreateSequenceQuery_NonEntityQuery ()
    {
      var fakeQueryResultRow = new Mock<IQueryResultRow>(MockBehavior.Strict);
      Expression<Func<IDatabaseResultRow, int>> inMemoryProjection = row => CheckQueryResultRowAdapter(row, fakeQueryResultRow.Object, 42);
      var fakeSqlQueryResult = CreateSqlQueryGeneratorResult(
          "SELECT x",
          selectedEntityType: null,
          inMemoryProjectionParameter: inMemoryProjection.Parameters.Single(),
          inMemoryProjectionBody: inMemoryProjection.Body);
      _sqlQueryGeneratorMock.Setup(mock => mock.CreateSqlQuery(_customerQueryModel)).Returns(fakeSqlQueryResult).Verifiable();

      var result = _generator.CreateSequenceQuery<int>("id", TestDomainStorageProviderDefinition, _customerQueryModel, Enumerable.Empty<FetchQueryModelBuilder>());

      _sqlQueryGeneratorMock.Verify();
      Assert.That(result, Is.TypeOf(typeof(CustomSequenceQueryAdapter<int>)));
      Assert.That(result.ID, Is.EqualTo("id"));
      Assert.That(result.StorageProviderDefinition, Is.EqualTo(_customerTypeDefinition.StorageEntityDefinition.StorageProviderDefinition));
      Assert.That(result.Statement, Is.EqualTo("SELECT x"));
      Assert.That(result.QueryType, Is.EqualTo(QueryType.Custom));
      Assert.That(result.CollectionType, Is.Null);
      Assert.That(result.EagerFetchQueries, Is.Empty);

      var resultConversion = ((CustomSequenceQueryAdapter<int>)result).ResultConversion;
      Assert.That(resultConversion(fakeQueryResultRow.Object), Is.EqualTo(42));
    }

    [Test]
    public void CreateSequenceQuery_NonEntityQuery_NoParameters ()
    {
      var fakeSqlQueryResult = CreateSqlQueryGeneratorResult(selectedEntityType: null, parameters: new CommandParameter[0]);
      _sqlQueryGeneratorMock.Setup(mock => mock.CreateSqlQuery(_customerQueryModel)).Returns(fakeSqlQueryResult).Verifiable();

      var result = _generator.CreateSequenceQuery<int>("id", TestDomainStorageProviderDefinition, _customerQueryModel, Enumerable.Empty<FetchQueryModelBuilder>());

      _sqlQueryGeneratorMock.Verify();
      Assert.That(result.Parameters, Is.Empty);
    }

    [Test]
    public void CreateSequenceQuery_NonEntityQuery_WithParameters ()
    {
      var fakeSqlQueryResult = CreateSqlQueryGeneratorResult(
          selectedEntityType: null,
          parameters: new[] { new CommandParameter("p0", "paramval") });
      _sqlQueryGeneratorMock.Setup(stub => stub.CreateSqlQuery(_customerQueryModel)).Returns(fakeSqlQueryResult);

      var result = _generator.CreateSequenceQuery<int>("id", TestDomainStorageProviderDefinition, _customerQueryModel, Enumerable.Empty<FetchQueryModelBuilder>());

      Assert.That(result.Parameters, Is.EqualTo(new[] { new QueryParameter("p0", "paramval", QueryParameterType.Value) }));
    }

    [Test]
    public void CreateSequenceQuery_NonEntityQuery_WithFetchRequests ()
    {
      var fakeSqlQueryResult = CreateSqlQueryGeneratorResult(selectedEntityType: null);
      _sqlQueryGeneratorMock.Setup(stub => stub.CreateSqlQuery(_customerQueryModel)).Returns(fakeSqlQueryResult);

      var fetchQueryModelBuilder = CreateFetchOneQueryModelBuilder((Customer o) => o.Ceo);
      Assert.That(
          () => _generator.CreateSequenceQuery<int>("id", TestDomainStorageProviderDefinition, _customerQueryModel, new[] { fetchQueryModelBuilder }),
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo(
                  "Only queries returning DomainObjects can perform eager fetching."));
    }

    private SqlQueryGeneratorResult CreateSqlQueryGeneratorResult (
        string commandText = null,
        CommandParameter[] parameters = null,
        Type selectedEntityType = null,
        ParameterExpression inMemoryProjectionParameter = null,
        Expression inMemoryProjectionBody = null)
    {
      var sqlCommandData = CreateSqlCommandData(commandText, parameters, inMemoryProjectionParameter, inMemoryProjectionBody);
      return new SqlQueryGeneratorResult(sqlCommandData, selectedEntityType);
    }

    private SqlCommandData CreateSqlCommandData (
        string commandText = null,
        CommandParameter[] parameters = null,
        ParameterExpression inMemoryProjectionParameter = null,
        Expression inMemoryProjectionBody = null)
    {
      return new SqlCommandData(
          commandText ?? "bla",
          parameters ?? new CommandParameter[0],
          inMemoryProjectionParameter ?? Expression.Parameter(typeof(IDatabaseResultRow), "row"),
          inMemoryProjectionBody ?? Expression.Constant(null));
    }

    private void CheckActualFetchQueryModel (QueryModel actualQueryModel, QueryModel fetchQueryModel)
    {
      Assert.That(actualQueryModel, Is.Not.SameAs(fetchQueryModel));
      Assert.That(fetchQueryModel.ResultOperators, Has.No.TypeOf<DistinctResultOperator>());
      Assert.That(actualQueryModel.ResultOperators, Has.Some.TypeOf<DistinctResultOperator>());
      Assert.That(actualQueryModel.MainFromClause.ToString(), Is.EqualTo(fetchQueryModel.MainFromClause.ToString()));
      Assert.That(actualQueryModel.SelectClause.ToString(), Is.EqualTo(fetchQueryModel.SelectClause.ToString()));
    }

    private FetchQueryModelBuilder CreateFetchOneQueryModelBuilder<TSource, TDest> (Expression<Func<TSource, TDest>> memberExpression, QueryModel queryModel = null)
    {
      var fetchRequest = CreateFetchOneRequest(memberExpression);
      return new FetchQueryModelBuilder(fetchRequest, queryModel ?? _customerQueryModel, 0);
    }

    private FetchOneRequest CreateFetchOneRequest<TSource, TDest> (Expression<Func<TSource, TDest>> memberExpression)
    {
      var relationMember = NormalizingMemberInfoFromExpressionUtility.GetMember(memberExpression);
      return new FetchOneRequest(relationMember);
    }

    private FetchQueryModelBuilder CreateFetchManyQueryModelBuilder<TSource, TDest> (Expression<Func<TSource, TDest>> memberExpression, QueryModel queryModel = null)
    {
      var fetchRequest = CreateFetchManyRequest(memberExpression);
      return new FetchQueryModelBuilder(fetchRequest, queryModel ?? _customerQueryModel, 0);
    }

    private FetchManyRequest CreateFetchManyRequest<TSource, TDest> (Expression<Func<TSource, TDest>> memberExpression)
    {
      var relationMember = NormalizingMemberInfoFromExpressionUtility.GetProperty(memberExpression);
      return new FetchManyRequest(relationMember);
    }

    private void CheckSingleFetchRequest (
        EagerFetchQueryCollection fetchQueryCollection, Type sourceType, string fetchedProperty, string expectedFetchQueryText, Type fetchedType)
    {
      var relationEndPointDefinition = GetEndPointDefinition(sourceType, fetchedProperty);
      CheckSingleFetchRequest(fetchQueryCollection, relationEndPointDefinition, expectedFetchQueryText, fetchedType);
    }

    private static void CheckSingleFetchRequest (
        EagerFetchQueryCollection fetchQueryCollection,
        IRelationEndPointDefinition relationEndPointDefinition,
        string expectedFetchQueryText,
        Type fetchedType)
    {
      var queryType = typeof(QueryAdapterBase<>).MakeGenericType(typeof(IEnumerable<>).MakeGenericType(fetchedType));
      var collectionType = typeof(ObjectList<>).MakeGenericType(fetchedType);

      Assert.That(fetchQueryCollection.Count, Is.EqualTo(1));
      var fetchQuery = fetchQueryCollection.Single();
      Assert.That(fetchQuery.Key, Is.EqualTo(relationEndPointDefinition));
      Assert.That(fetchQuery.Value.Statement, Is.EqualTo(expectedFetchQueryText));
      Assert.That(fetchQuery.Value, Is.InstanceOf(queryType));
      Assert.That(fetchQuery.Value.CollectionType, Is.EqualTo(collectionType));
    }

    private int CheckScalarResultRowAdapter (IDatabaseResultRow row, string expectedScalarValue, int fakeResult)
    {
      Assert.That(row, Is.TypeOf<ScalarResultRowAdapter>());
      Assert.That(((ScalarResultRowAdapter)row).ScalarValue, Is.EqualTo(expectedScalarValue));
      Assert.That(((ScalarResultRowAdapter)row).StorageTypeInformationProvider, Is.SameAs(_storageTypeInformationProviderStub.Object));
      return fakeResult;
    }

    private int CheckQueryResultRowAdapter (IDatabaseResultRow row, IQueryResultRow expectedResultRow, int fakeResult)
    {
      Assert.That(row, Is.TypeOf<QueryResultRowAdapter>());
      Assert.That(((QueryResultRowAdapter)row).QueryResultRow, Is.EqualTo(expectedResultRow));
      return fakeResult;
    }
  }
}
