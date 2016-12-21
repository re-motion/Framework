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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Linq.ExecutableQueries;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Linq.ExecutableQueries
{
  [TestFixture]
  public class ScalarQueryAdapterTest
  {
    private IQuery _queryStub;
    private Func<object, string> _resultConversion;

    [SetUp]
    public void SetUp ()
    {
      _queryStub = MockRepository.GenerateStub<IQuery>();
      _resultConversion = o => o.ToString();
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Only scalar queries can be used to load scalar results.\r\nParameter name: query")]
    public void Initialization_QueryTypeNotScalar ()
    {
      _queryStub.Stub (stub => stub.QueryType).Return (QueryType.Collection);

      new ScalarQueryAdapter<string> (_queryStub, _resultConversion);
    }

    [Test]
    public void Execute ()
    {
      _queryStub.Stub (stub => stub.QueryType).Return (QueryType.Scalar);
      var scalarQueryAdapter = new ScalarQueryAdapter<string> (_queryStub, _resultConversion);

      var queryManagerMock = MockRepository.GenerateStrictMock<IQueryManager>();
      queryManagerMock.Expect (mock => mock.GetScalar (scalarQueryAdapter)).Return (5);

      var result = scalarQueryAdapter.Execute (queryManagerMock);

      queryManagerMock.VerifyAllExpectations();
      Assert.That (result, Is.EqualTo ("5"));
    }
  }
}