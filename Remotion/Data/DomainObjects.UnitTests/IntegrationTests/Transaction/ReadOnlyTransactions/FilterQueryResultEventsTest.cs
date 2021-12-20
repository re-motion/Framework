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
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Transaction.ReadOnlyTransactions
{
  [TestFixture]
  public class FilterQueryResultEventsTest : ReadOnlyTransactionsTestBase
  {
    private Order _order1;
    private Order _order3;
    private Order _order4;
    private Order _order5;
    private Mock<IQuery> _queryStub;

    public override void SetUp ()
    {
      base.SetUp();

      _order1 = ExecuteInWriteableSubTransaction(() => DomainObjectIDs.Order1.GetObject<Order>());
      _order3 = ExecuteInWriteableSubTransaction(() => DomainObjectIDs.Order3.GetObject<Order>());
      _order4 = ExecuteInWriteableSubTransaction(() => DomainObjectIDs.Order4.GetObject<Order>());
      _order5 = ExecuteInWriteableSubTransaction(() => DomainObjectIDs.Order5.GetObject<Order>());

      _queryStub = new Mock<IQuery>();

      InstallExtensionMock();
    }

    [Test]
    public void FilterQueryResult_RaisedInAllHierarchyLevels ()
    {
      var sequence = new MockSequence();
      ExtensionStrictMock
          .InSequence(sequence)
            .Setup(
                mock => mock.FilterQueryResult(
                    ReadOnlyRootTransaction,
                    It.Is<QueryResult<DomainObject>>(qr => qr.ToArray().SequenceEqual(new[] { _order1 }))))
            .Returns(new QueryResult<DomainObject>(_queryStub.Object, new[] { _order3 }))
            .Callback((ClientTransaction _, QueryResult<DomainObject> _) => Assert.That(ReadOnlyRootTransaction.IsWriteable, Is.False))
            .Verifiable();
      ExtensionStrictMock
          .InSequence(sequence)
            .Setup(
                mock => mock.FilterQueryResult(
                    ReadOnlyMiddleTransaction,
                    It.Is<QueryResult<DomainObject>>(qr => qr.ToArray().SequenceEqual(new[] { _order3 }))))
            .Returns(new QueryResult<DomainObject>(_queryStub.Object, new[] { _order4 }))
            .Callback((ClientTransaction _, QueryResult<DomainObject> _) => Assert.That(ReadOnlyMiddleTransaction.IsWriteable, Is.False))
            .Verifiable();
      ExtensionStrictMock
          .InSequence(sequence)
            .Setup(
                mock => mock.FilterQueryResult(
                    WriteableSubTransaction,
                    It.Is<QueryResult<DomainObject>>(qr => qr.ToArray().SequenceEqual(new[] { _order4 }))))
            .Returns(new QueryResult<DomainObject>(_queryStub.Object, new[] { _order5 }))
            .Callback((ClientTransaction _, QueryResult<DomainObject> _) => Assert.That(WriteableSubTransaction.IsWriteable, Is.True))
            .Verifiable();

      var result = ExecuteInWriteableSubTransaction(() => QueryFactory.CreateLinqQuery<Order>().Where(obj => obj.ID == _order1.ID).ToList());

      ExtensionStrictMock.Verify();
      Assert.That(result, Is.EqualTo(new[] { _order5 }));
    }

    [Test]
    public void FilterCustomQueryResult_RaisedInAllHierarchyLevels ()
    {
      var query = QueryFactory.CreateCustomQuery(
          "CustomQuery",
          TestDomainStorageProviderDefinition,
          "SELECT [OrderNo] FROM [Order] WHERE ID=@1",
          new QueryParameterCollection { { "@1", _order1.ID } });
      var fakeQueryResultRow1 = new Mock<IQueryResultRow>();
      var fakeQueryResultRow2 = new Mock<IQueryResultRow>();
      var fakeQueryResultRow3 = new Mock<IQueryResultRow>();

      var sequence = new MockSequence();

      ListenerDynamicMock
          .InSequence(sequence)
            .Setup(
                mock => mock.FilterCustomQueryResult(
                    ReadOnlyRootTransaction,
                    It.IsAny<IQuery>(),
                    It.Is<IEnumerable<IQueryResultRow>>(qrrs => qrrs.Select(qrr => qrr.GetConvertedValue<int>(0)).Single() == 1)))
            .Returns(new[] { fakeQueryResultRow1.Object })
            .Callback((ClientTransaction _, IQuery _, IEnumerable<IQueryResultRow> _) => Assert.That(ReadOnlyRootTransaction.IsWriteable, Is.False))
            .Verifiable();

      ListenerDynamicMock
          .InSequence(sequence)
            .Setup(
                mock => mock.FilterCustomQueryResult(
                    ReadOnlyMiddleTransaction,
                    It.IsAny<IQuery>(),
                    new[] { fakeQueryResultRow1.Object }))
            .Returns(new[] { fakeQueryResultRow2.Object })
            .Callback((ClientTransaction _, IQuery _, IEnumerable<IQueryResultRow> _) => Assert.That(ReadOnlyMiddleTransaction.IsWriteable, Is.False))
            .Verifiable();

      ListenerDynamicMock
          .InSequence(sequence)
            .Setup(
                mock => mock.FilterCustomQueryResult(
                    WriteableSubTransaction,
                    It.IsAny<IQuery>(),
                    new[] { fakeQueryResultRow2.Object }))
            .Returns(new[] { fakeQueryResultRow3.Object })
            .Callback((ClientTransaction _, IQuery _, IEnumerable<IQueryResultRow> _) => Assert.That(WriteableSubTransaction.IsWriteable, Is.True))
            .Verifiable();

      InstallListenerMock();

      var result = WriteableSubTransaction.QueryManager.GetCustom(query, qrr => qrr).ToList();

      ListenerDynamicMock.Verify();
      Assert.That(result, Is.EqualTo(new[] { fakeQueryResultRow3.Object }));
    }
  }
}
