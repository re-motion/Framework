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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Queries
{
  [TestFixture]
  public class QueryCacheTest : StandardMappingTest
  {
    private QueryCache _cache;

    [SetUp]
    public override void SetUp()
    {
      base.SetUp();
      _cache = new QueryCache();
    }

    [Test]
    public void GetOrCreateQuery_Uncached()
    {
      IQuery query = _cache.GetQuery<Order> ("id", orders => from o in orders where o.OrderNumber > 1 select o);

      Assert.That (query.Statement, Is.EqualTo (
        "SELECT [t0].[ID],[t0].[ClassID],[t0].[Timestamp],[t0].[OrderNo],[t0].[DeliveryDate],[t0].[OfficialID],[t0].[CustomerID],[t0].[CustomerIDClassID] "
        +"FROM [OrderView] AS [t0] WHERE ([t0].[OrderNo] > @1)"));
      Assert.That (query.Parameters.Count, Is.EqualTo (1));
      Assert.That (query.ID, Is.EqualTo ("id"));
    }

    [Test]
    public void GetOrCreateQuery_Cached ()
    {
      IQuery query1 = _cache.GetQuery<Order> ("id", orders => from o in orders where o.OrderNumber > 1 select o);
      IQuery query2 = _cache.GetQuery<Order> ("id", orders => from o in orders where o.OrderNumber > 1 select o);

      Assert.That (query1, Is.SameAs (query2));
    }

    [Test]
    public void ExecuteCollectionQuery ()
    {
      IQuery query = _cache.GetQuery<Order> ("id", orders => from o in orders where o.OrderNumber > 1 select o);

      var queryManagerMock = MockRepository.GenerateMock<IQueryManager> ();
      var clientTransactionStub = ClientTransactionObjectMother.CreateWithComponents<ClientTransaction> (queryManager: queryManagerMock);

      var expectedResult = new QueryResult<Order> (query, new Order[0]);
      queryManagerMock.Expect (mock => mock.GetCollection<Order> (query)).Return (expectedResult);
      queryManagerMock.Replay ();

      var result = _cache.ExecuteCollectionQuery<Order> (clientTransactionStub, "id", orders => from o in orders where o.OrderNumber > 1 select o);

      queryManagerMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (expectedResult));
    }
  }
}
