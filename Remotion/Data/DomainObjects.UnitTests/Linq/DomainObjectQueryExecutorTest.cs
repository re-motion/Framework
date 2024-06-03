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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Linq.ExecutableQueries;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.UnitTests.Queries;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.EagerFetching;

namespace Remotion.Data.DomainObjects.UnitTests.Linq
{
  [TestFixture]
  public class DomainObjectQueryExecutorTest : StandardMappingTest
  {
    private Mock<IDomainObjectQueryGenerator> _queryGeneratorMock;
    private DomainObjectQueryExecutor _queryExecutor;

    private Mock<IQueryManager> _queryManagerMock;
    private ClientTransactionScope _transactionScope;

    private QueryModel _someQueryModel;
    private Order _someOrder;
    private Mock<IExecutableQuery<int>> _scalarExecutableQueryMock;
    private Mock<IExecutableQuery<IEnumerable<Order>>> _collectionExecutableQueryMock;

    public override void SetUp ()
    {
      base.SetUp();

      _queryGeneratorMock = new Mock<IDomainObjectQueryGenerator>(MockBehavior.Default);

      _queryExecutor = new DomainObjectQueryExecutor(TestDomainStorageProviderDefinition, _queryGeneratorMock.Object, "DefinitelyUniqueQueryID", QueryObjectMother.EmptyMetadata);

      _queryManagerMock = new Mock<IQueryManager>(MockBehavior.Strict);
      var transaction = ClientTransactionObjectMother.CreateTransactionWithQueryManager<ClientTransaction>(_queryManagerMock.Object);
      _transactionScope = transaction.EnterDiscardingScope();

      _someQueryModel = QueryModelObjectMother.Create();
      _someOrder = DomainObjectMother.CreateFakeObject<Order>();

      _scalarExecutableQueryMock = new Mock<IExecutableQuery<int>>(MockBehavior.Strict);
      _collectionExecutableQueryMock = new Mock<IExecutableQuery<IEnumerable<Order>>>(MockBehavior.Strict);
    }

    public override void TearDown ()
    {
      _transactionScope.Leave();
      base.TearDown();
    }

    [Test]
    public void Initialize ()
    {
      var executor = new DomainObjectQueryExecutor(TestDomainStorageProviderDefinition, _queryGeneratorMock.Object, "dummyID", QueryObjectMother.EmptyMetadata);

      Assert.That(executor.Metadata, Is.SameAs(QueryObjectMother.EmptyMetadata));
      Assert.That(executor.ID, Is.EqualTo("dummyID"));
      Assert.That(executor.StorageProviderDefinition, Is.SameAs(TestDomainStorageProviderDefinition));
      Assert.That(executor.QueryGenerator, Is.SameAs(_queryGeneratorMock.Object));
    }

    [Test]
    public void ExecuteScalar ()
    {
      _queryGeneratorMock
          .Setup(mock => mock.CreateScalarQuery<int>("DefinitelyUniqueQueryID", TestDomainStorageProviderDefinition, _someQueryModel, QueryObjectMother.EmptyMetadata))
          .Returns(_scalarExecutableQueryMock.Object)
          .Verifiable();

      var fakeResult = 7;
      _scalarExecutableQueryMock.Setup(mock => mock.Execute(_queryManagerMock.Object)).Returns(fakeResult).Verifiable();

      var result = _queryExecutor.ExecuteScalar<int>(_someQueryModel);

      _queryGeneratorMock.Verify();
      _scalarExecutableQueryMock.Verify();
      Assert.That(result, Is.EqualTo(fakeResult));
    }

    [Test]
    public void ExecuteScalar_WithFetchRequests ()
    {
      AddFetchRequest(_someQueryModel);
      Assert.That(
          () => _queryExecutor.ExecuteScalar<int>(_someQueryModel),
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo("Scalar queries cannot perform eager fetching."));
    }

    [Test]
    public void ExecuteScalar_NoActiveClientTransaction ()
    {
      using (ClientTransactionScope.EnterNullScope())
      {
        Assert.That(
            () => _queryExecutor.ExecuteScalar<int>(_someQueryModel),
            Throws.InvalidOperationException
                .With.Message.EqualTo("No ClientTransaction has been associated with the current thread."));
      }
    }

    [Test]
    public void ExecuteCollection ()
    {
      _queryGeneratorMock
          .Setup(
              mock => mock.CreateSequenceQuery<Order>(
                  "DefinitelyUniqueQueryID",
                  TestDomainStorageProviderDefinition,
                  _someQueryModel,
                  It.Is<IEnumerable<FetchQueryModelBuilder>>(p => p.Count() == 0),
                  QueryObjectMother.EmptyMetadata))
          .Returns(_collectionExecutableQueryMock.Object)
          .Verifiable();

      var fakeResult = new[] { _someOrder };
      _collectionExecutableQueryMock.Setup(mock => mock.Execute(_queryManagerMock.Object)).Returns(fakeResult).Verifiable();

      var result = _queryExecutor.ExecuteCollection<Order>(_someQueryModel);

      _queryGeneratorMock.Verify();
      _collectionExecutableQueryMock.Verify();
      Assert.That(result, Is.SameAs(fakeResult));
    }

    [Test]
    public void ExecuteCollection_WithFetchRequests ()
    {
      ExpectCreateQueryWithFetchQueryModelBuilders(_someQueryModel, _collectionExecutableQueryMock.Object);

      var fakeResult = new[] { _someOrder };
      _collectionExecutableQueryMock.Setup(mock => mock.Execute(_queryManagerMock.Object)).Returns(fakeResult).Verifiable();

      var result = _queryExecutor.ExecuteCollection<Order>(_someQueryModel);

      _queryGeneratorMock.Verify();
      _collectionExecutableQueryMock.Verify();
      Assert.That(result, Is.SameAs(fakeResult));
    }

    [Test]
    public void ExecuteCollection_NoActiveClientTransaction ()
    {
      using (ClientTransactionScope.EnterNullScope())
      {
        Assert.That(
            () => _queryExecutor.ExecuteCollection<Order>(_someQueryModel),
            Throws.InvalidOperationException
                .With.Message.EqualTo("No ClientTransaction has been associated with the current thread."));
      }
    }

    [Test]
    public void ExecuteSingle ()
    {
      _queryGeneratorMock
          .Setup(
              mock => mock.CreateSequenceQuery<Order>(
                  "DefinitelyUniqueQueryID",
                  TestDomainStorageProviderDefinition,
                  _someQueryModel,
                  It.Is<IEnumerable<FetchQueryModelBuilder>>(p => p.Count() == 0),
                  QueryObjectMother.EmptyMetadata))
          .Returns(_collectionExecutableQueryMock.Object)
          .Verifiable();

      var fakeResult = new[] { _someOrder };
      _collectionExecutableQueryMock.Setup(mock => mock.Execute(_queryManagerMock.Object)).Returns(fakeResult).Verifiable();

      var result = _queryExecutor.ExecuteSingle<Order>(_someQueryModel, false);

      _queryGeneratorMock.Verify();
      _collectionExecutableQueryMock.Verify();
      Assert.That(result, Is.SameAs(_someOrder));
    }

    [Test]
    public void ExecuteSingle_MoreThanOneItem ()
    {
      _queryGeneratorMock
          .Setup(
              mock => mock.CreateSequenceQuery<Order>(
                  "DefinitelyUniqueQueryID",
                  TestDomainStorageProviderDefinition,
                  _someQueryModel,
                  It.Is<IEnumerable<FetchQueryModelBuilder>>(p => p.Count() == 0),
                  QueryObjectMother.EmptyMetadata))
          .Returns(_collectionExecutableQueryMock.Object)
          .Verifiable();

      var fakeResult = new[] { _someOrder, DomainObjectMother.CreateFakeObject<Order>() };
      _collectionExecutableQueryMock.Setup(mock => mock.Execute(_queryManagerMock.Object)).Returns(fakeResult).Verifiable();
      Assert.That(
          () => _queryExecutor.ExecuteSingle<Order>(_someQueryModel, false),
          Throws.InvalidOperationException
              .With.Message.EqualTo("Sequence contains more than one element"));
    }

    [Test]
    public void ExecuteSingle_ZeroItems_ReturnDefaultTrue ()
    {
      _queryGeneratorMock
          .Setup(
              mock => mock.CreateSequenceQuery<Order>(
                  "DefinitelyUniqueQueryID",
                  TestDomainStorageProviderDefinition,
                  _someQueryModel,
                  It.Is<IEnumerable<FetchQueryModelBuilder>>(p => p.Count() == 0),
                  QueryObjectMother.EmptyMetadata))
          .Returns(_collectionExecutableQueryMock.Object)
          .Verifiable();

      var fakeResult = new Order[0];
      _collectionExecutableQueryMock.Setup(mock => mock.Execute(_queryManagerMock.Object)).Returns(fakeResult).Verifiable();

      var result = _queryExecutor.ExecuteSingle<Order>(_someQueryModel, true);

      Assert.That(result, Is.Null);
    }

    [Test]
    public void ExecuteSingle_ZeroItems_ReturnDefaultFalse ()
    {
      _queryGeneratorMock
          .Setup(
              mock => mock.CreateSequenceQuery<Order>(
                  "DefinitelyUniqueQueryID",
                  TestDomainStorageProviderDefinition,
                  _someQueryModel,
                  It.Is<IEnumerable<FetchQueryModelBuilder>>(p => p.Count() == 0),
                  QueryObjectMother.EmptyMetadata))
          .Returns(_collectionExecutableQueryMock.Object)
          .Verifiable();

      var fakeResult = new Order[0];
      _collectionExecutableQueryMock.Setup(mock => mock.Execute(_queryManagerMock.Object)).Returns(fakeResult).Verifiable();
      Assert.That(
          () => _queryExecutor.ExecuteSingle<Order>(_someQueryModel, false),
          Throws.InvalidOperationException
              .With.Message.EqualTo("Sequence contains no elements"));
    }

    [Test]
    public void ExecuteSingle_NoActiveClientTransaction ()
    {
      using (ClientTransactionScope.EnterNullScope())
      {
        Assert.That(
            () => _queryExecutor.ExecuteSingle<Order>(_someQueryModel, false),
            Throws.InvalidOperationException
                .With.Message.EqualTo("No ClientTransaction has been associated with the current thread."));
      }
    }

    private void ExpectCreateQueryWithFetchQueryModelBuilders<T> (QueryModel queryModel, IExecutableQuery<IEnumerable<T>> fakeResult)
    {
      var nonTrailingFetchRequest = AddFetchRequest(queryModel);
      var someResultOperator = AddSomeResultOperator(queryModel);
      var trailingFetchRequest1 = AddFetchRequest(queryModel);
      var trailingFetchRequest2 = AddFetchRequest(queryModel);
      Assert.That(
          queryModel.ResultOperators,
          Is.EqualTo(new[] { nonTrailingFetchRequest, someResultOperator, trailingFetchRequest1, trailingFetchRequest2 }));

      _queryGeneratorMock
          .Setup(
              mock => mock.CreateSequenceQuery<T>(
                  It.IsAny<string>(),
                  It.IsAny<StorageProviderDefinition>(),
                  queryModel,
                  It.IsAny<IEnumerable<FetchQueryModelBuilder>>(),
                  QueryObjectMother.EmptyMetadata))
          .Returns(fakeResult)
          .Callback(
              (string id, StorageProviderDefinition _, QueryModel _, IEnumerable<FetchQueryModelBuilder> fetchQueryModelBuilders, IReadOnlyDictionary<string, object> _) =>
              {
                Assert.That(queryModel.ResultOperators, Is.EqualTo(new[] { nonTrailingFetchRequest, someResultOperator }));

                var builders = fetchQueryModelBuilders.ToArray();
                Assert.That(builders, Has.Length.EqualTo(2));
                CheckFetchQueryModelBuilder(builders[0], trailingFetchRequest2, queryModel, 3);
                CheckFetchQueryModelBuilder(builders[1], trailingFetchRequest1, queryModel, 2);
              })
          .Verifiable();
    }

    private void CheckFetchQueryModelBuilder (
        FetchQueryModelBuilder builder, FetchRequestBase expectedFetchRequest, QueryModel expectedQueryModel, int expectedResultOperatorPosition)
    {
      Assert.That(builder.FetchRequest, Is.SameAs(expectedFetchRequest));
      Assert.That(builder.SourceItemQueryModel, Is.SameAs(expectedQueryModel));
      Assert.That(builder.ResultOperatorPosition, Is.EqualTo(expectedResultOperatorPosition));
    }

    private FetchRequestBase AddFetchRequest (QueryModel queryModel)
    {
      var relationMember = NormalizingMemberInfoFromExpressionUtility.GetProperty((Order o) => o.OrderTicket);
      var fetchRequest = new FetchOneRequest(relationMember);
      queryModel.ResultOperators.Add(fetchRequest);
      return fetchRequest;
    }

    private ResultOperatorBase AddSomeResultOperator (QueryModel queryModel)
    {
      var someResultOperator = new DistinctResultOperator();
      queryModel.ResultOperators.Add(someResultOperator);
      return someResultOperator;
    }
  }
}
