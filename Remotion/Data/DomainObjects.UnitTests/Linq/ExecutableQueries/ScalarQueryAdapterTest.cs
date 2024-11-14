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
using Remotion.Development.NUnit.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Linq.ExecutableQueries
{
  [TestFixture]
  public class ScalarQueryAdapterTest
  {
    private Mock<IQuery> _queryStub;
    private Func<object, string> _resultConversion;

    [SetUp]
    public void SetUp ()
    {
      _queryStub = new Mock<IQuery>();
      _resultConversion = o => o.ToString();
    }

    [Test]
    public void Initialization_QueryTypeNotScalar ()
    {
      _queryStub.Setup(stub => stub.QueryType).Returns(QueryType.CollectionReadOnly);
      Assert.That(
          () => new ScalarQueryAdapter<string>(_queryStub.Object, _resultConversion),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo("Only readonly scalar queries can be used to load scalar results.", "query"));
    }

    [Test]
    public void Execute ()
    {
      _queryStub.Setup(stub => stub.QueryType).Returns(QueryType.ScalarReadOnly);
      var scalarQueryAdapter = new ScalarQueryAdapter<string>(_queryStub.Object, _resultConversion);

      var queryManagerMock = new Mock<IQueryManager>(MockBehavior.Strict);
      queryManagerMock.Setup(mock => mock.GetScalar(scalarQueryAdapter)).Returns(5).Verifiable();

      var result = scalarQueryAdapter.Execute(queryManagerMock.Object);

      queryManagerMock.Verify();
      Assert.That(result, Is.EqualTo("5"));
    }
  }
}
