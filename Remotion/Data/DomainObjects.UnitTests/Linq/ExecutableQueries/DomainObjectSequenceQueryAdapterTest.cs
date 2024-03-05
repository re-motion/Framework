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
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.NUnit.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Linq.ExecutableQueries
{
  [TestFixture]
  public class DomainObjectSequenceQueryAdapterTest : StandardMappingTest
  {
    private Mock<IQuery> _queryStub;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _queryStub = new Mock<IQuery>();
    }

    [Test]
    public void Initialization_QueryTypeNotCollection ()
    {
      _queryStub.Setup(stub => stub.QueryType).Returns(QueryType.ScalarReadOnly);
      Assert.That(
          () => new DomainObjectSequenceQueryAdapter<string>(_queryStub.Object),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo("Only readonly collection queries can be used to load data containers.", "query"));
    }

    [Test]
    public void Execute ()
    {
      _queryStub.Setup(stub => stub.QueryType).Returns(QueryType.CollectionReadOnly);
      var queryAdapter = new DomainObjectSequenceQueryAdapter<object>(_queryStub.Object);

      var order1 = DomainObjectMother.CreateFakeObject<Order>();
      var order3 = DomainObjectMother.CreateFakeObject<Order>();
      var fakeResult = new QueryResult<DomainObject>(_queryStub.Object, new[] { order1, order3 });

      var queryManagerMock = new Mock<IQueryManager>(MockBehavior.Strict);
      queryManagerMock.Setup(mock => mock.GetCollection(queryAdapter)).Returns(fakeResult).Verifiable();

      var result = queryAdapter.Execute(queryManagerMock.Object);

      Assert.That(result, Is.EqualTo(new[] { order1, order3 }));
    }
  }
}
