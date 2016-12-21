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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Queries
{
  [TestFixture]
  public class LinqQueryTest : QueryTestBase
  {
    [Test]
    public void LinqQuery_CallsFilterQueryResult ()
    {
      var extensionKey = "LinqQuery_CallsFilterQueryResult_Key";
      var extensionMock = MockRepository.GenerateMock<IClientTransactionExtension>();
      extensionMock.Stub (stub => stub.Key).Return (extensionKey);
      extensionMock
          .Expect (mock => mock.FilterQueryResult (Arg.Is (TestableClientTransaction), Arg<QueryResult<DomainObject>>.Is.Anything))
          .Return (TestQueryFactory.CreateTestQueryResult<DomainObject>());

      TestableClientTransaction.Extensions.Add (extensionMock);
      try
      {
        var query = from o in QueryFactory.CreateLinqQuery<Order>() where o.Customer.ID == DomainObjectIDs.Customer1 select o;
        query.ToArray();

        extensionMock.VerifyAllExpectations();
      }
      finally
      {
        TestableClientTransaction.Extensions.Remove (extensionKey);
      }
    }

    [Test]
    public void LinqCustomQuery_CallsFilterCustomQueryResult ()
    {
      var listenerMock = MockRepository.GenerateMock<IClientTransactionListener>();
      listenerMock
          .Expect (
              mock => mock.FilterCustomQueryResult (
                  Arg.Is (TestableClientTransaction),
                  Arg<IQuery>.Is.Anything,
                  Arg<IEnumerable<int>>.List.Equal (new[] { 1, 2, 3, 4, 5 })))
          .Return (new[] { 1, 2, 3 });

      TestableClientTransaction.AddListener (listenerMock);
      try
      {
        var query = from o in QueryFactory.CreateLinqQuery<Order>() where o.OrderNumber <= 5 orderby o.OrderNumber select o.OrderNumber;
        var result = query.ToArray();

        listenerMock.VerifyAllExpectations ();
        Assert.That (result, Is.EqualTo (new[] { 1, 2, 3 }));
      }
      finally
      {
        TestableClientTransaction.RemoveListener (listenerMock);
      }
    }
  }
}