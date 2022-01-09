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
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Linq.SqlBackend.SqlGeneration;

namespace Remotion.Data.DomainObjects.UnitTests.Linq
{
  [TestFixture]
  public class QueryResultRowAdapterTest
  {
    private Mock<IQueryResultRow> _queryResultRowStub;
    private QueryResultRowAdapter _queryResultRowAdapter;

    [SetUp]
    public void SetUp ()
    {
      _queryResultRowStub = new Mock<IQueryResultRow>();

      _queryResultRowAdapter = new QueryResultRowAdapter(_queryResultRowStub.Object);
    }

    [Test]
    public void GetValue ()
    {
      _queryResultRowStub.Setup(stub => stub.GetConvertedValue<int>(4)).Returns(10);

      var result = _queryResultRowAdapter.GetValue<int>(new ColumnID("test", 4));

      Assert.That(result, Is.EqualTo(10));
    }

    [Test]
    public void GetEntity ()
    {
      Assert.That(
          () => _queryResultRowAdapter.GetEntity<int>(),
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo(
                  "This LINQ provider does not support queries with complex projections that include DomainObjects.\r\n"
                  + "Either change the query to return just a sequence of DomainObjects "
                  + "(e.g., 'from o in QueryFactory.CreateLinqQuery<Order>() select o') or change the complex projection to contain no DomainObjects "
                  + "(e.g., 'from o in QueryFactory.CreateLinqQuery<Order>() select new { o.OrderNumber, o.OrderDate }')."));
    }
  }
}
