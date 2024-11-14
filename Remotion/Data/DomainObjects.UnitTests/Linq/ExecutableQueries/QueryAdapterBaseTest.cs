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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Linq.ExecutableQueries;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.DomainObjects.Queries.EagerFetching;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.Moq.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Linq.ExecutableQueries
{
  [TestFixture]
  public class QueryAdapterBaseTest : StandardMappingTest
  {
    private Mock<IQuery> _queryMock;
    private QueryAdapterBase<object> _queryAdapterBase;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _queryMock = new Mock<IQuery>(MockBehavior.Strict);
      _queryAdapterBase = new TestableQueryAdapterBase<object>(_queryMock.Object);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That(_queryAdapterBase.Query, Is.SameAs(_queryMock.Object));
    }

    [Test]
    public void DelegatedMembers ()
    {
      var testHelper = new DecoratorTestHelper<IQuery>(_queryAdapterBase, _queryMock);

      var parameterCollection = new QueryParameterCollection { { "p1", 7 } };
      var eagerFetchQueries =
          new EagerFetchQueryCollection { { GetEndPointDefinition(typeof(Order), "OrderTicket"), new Mock<IQuery>().Object } };

      testHelper.CheckDelegation(q => q.ID, "Some ID");
      testHelper.CheckDelegation(q => q.Statement, "Some Statement");
      testHelper.CheckDelegation(q => q.StorageProviderDefinition, TestDomainStorageProviderDefinition);
      testHelper.CheckDelegation(q => q.CollectionType, typeof(OrderCollection));
      testHelper.CheckDelegation(q => q.QueryType, QueryType.CollectionReadOnly);
      testHelper.CheckDelegation(q => q.Parameters, parameterCollection);
      testHelper.CheckDelegation(q => q.EagerFetchQueries, eagerFetchQueries);
    }
  }
}
