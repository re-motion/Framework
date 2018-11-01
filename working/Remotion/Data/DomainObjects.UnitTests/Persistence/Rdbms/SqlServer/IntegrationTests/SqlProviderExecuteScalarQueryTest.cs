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
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.IntegrationTests
{
  [TestFixture]
  public class SqlProviderExecuteScalarQueryTest : SqlProviderBaseTest
  {
    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp ();
      SetDatabaseModifyable ();
    }

    [Test]
    public void ScalarQueryWithoutParameter ()
    {
      Assert.That (Provider.ExecuteScalarQuery (QueryFactory.CreateQueryFromConfiguration ("QueryWithoutParameter")), Is.EqualTo (42));
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException))]
    public void InvalidScalarQuery ()
    {
      QueryDefinition definition = new QueryDefinition ("InvalidQuery", TestDomainStorageProviderDefinition, "This is not T-SQL", QueryType.Scalar);

      Provider.ExecuteScalarQuery (QueryFactory.CreateQuery (definition));
    }

    [Test]
    public void ScalarQueryWithParameter ()
    {
      var query = QueryFactory.CreateQueryFromConfiguration ("OrderNoSumByCustomerNameQuery");
      query.Parameters.Add ("@customerName", "Kunde 1");

      Assert.That (Provider.ExecuteScalarQuery (query), Is.EqualTo (3));
    }

    [Test]
    public void ParameterWithTextReplacement ()
    {
      var query = QueryFactory.CreateQueryFromConfiguration ("OrderNoSumForMultipleCustomers");
      query.Parameters.Add ("{companyNames}", "'Kunde 1', 'Kunde 3'", QueryParameterType.Text);

      Assert.That (Provider.ExecuteScalarQuery (query), Is.EqualTo (6));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Expected query type is 'Scalar', but was 'Collection'.\r\nParameter name: query")]
    public void CollectionQuery ()
    {
      Provider.ExecuteScalarQuery (QueryFactory.CreateQueryFromConfiguration ("OrderQuery"));
    }

    [Test]
    public void BulkUpdateQuery ()
    {
      var query = QueryFactory.CreateQueryFromConfiguration ("BulkUpdateQuery");
      query.Parameters.Add ("@customerID", DomainObjectIDs.Customer1.Value);

      Assert.That (Provider.ExecuteScalarQuery (query), Is.EqualTo (2));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException))]
    public void DifferentStorageProviderID ()
    {
      QueryDefinition definition = new QueryDefinition (
          "QueryWithDifferentStorageProviderID",
          UnitTestStorageProviderDefinition,
          "select 42",
          QueryType.Scalar);

      Provider.ExecuteScalarQuery (QueryFactory.CreateQuery (definition));
    }
  }
}
