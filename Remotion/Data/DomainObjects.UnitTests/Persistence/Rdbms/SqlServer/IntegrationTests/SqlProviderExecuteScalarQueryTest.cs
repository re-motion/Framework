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
using Remotion.Development.NUnit.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.IntegrationTests
{
  [TestFixture]
  public class SqlProviderExecuteScalarQueryTest : SqlProviderBaseTest
  {
    [Test]
    public void ScalarQueryWithoutParameter ()
    {
      Assert.That(Provider.ExecuteScalarQuery(QueryFactory.CreateQuery(Queries.GetMandatory("QueryWithoutParameter"))), Is.EqualTo(42));
    }

    [Test]
    public void InvalidScalarQuery ()
    {
      QueryDefinition definition = new QueryDefinition("InvalidQuery", TestDomainStorageProviderDefinition, "This is not T-SQL", QueryType.Scalar);
      Assert.That(
          () => Provider.ExecuteScalarQuery(QueryFactory.CreateQuery(definition)),
          Throws.InstanceOf<RdbmsProviderException>());
    }

    [Test]
    public void ScalarQueryWithParameter ()
    {
      var query = QueryFactory.CreateQuery(Queries.GetMandatory("OrderNoSumByCustomerNameQuery"));
      query.Parameters.Add("@customerName", "Kunde 1");

      Assert.That(Provider.ExecuteScalarQuery(query), Is.EqualTo(3));
    }

    [Test]
    public void ParameterWithTextReplacement ()
    {
      var query = QueryFactory.CreateQuery(Queries.GetMandatory("OrderNoSumForMultipleCustomers"));
      query.Parameters.Add("{companyNames}", "'Kunde 1', 'Kunde 3'", QueryParameterType.Text);

      Assert.That(Provider.ExecuteScalarQuery(query), Is.EqualTo(6));
    }

    [Test]
    public void CollectionQuery ()
    {
      Assert.That(
          () => Provider.ExecuteScalarQuery(QueryFactory.CreateQuery(Queries.GetMandatory("OrderQuery"))),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Expected query type is 'Scalar', but was 'Collection'.", "query"));
    }

    [Test]
    public void BulkUpdateQuery ()
    {
      var query = QueryFactory.CreateQuery(Queries.GetMandatory("BulkUpdateQuery"));
      query.Parameters.Add("@customerID", DomainObjectIDs.Customer1.Value);

      Assert.That(Provider.ExecuteScalarQuery(query), Is.EqualTo(2));
    }

    [Test]
    public void DifferentStorageProviderID ()
    {
      QueryDefinition definition = new QueryDefinition(
          "QueryWithDifferentStorageProviderID",
          UnitTestStorageProviderDefinition,
          "select 42",
          QueryType.Scalar);
      Assert.That(
          () => Provider.ExecuteScalarQuery(QueryFactory.CreateQuery(definition)),
          Throws.ArgumentException);
    }
  }
}
