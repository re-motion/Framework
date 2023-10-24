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
using Remotion.Data.DomainObjects.Queries.Configuration;

namespace Remotion.Data.DomainObjects.UnitTests.Queries.Configuration
{
  public class QueryDefinitionChecker
  {
    public void Check (IReadOnlyList<QueryDefinition> expectedQueries, IReadOnlyList<QueryDefinition> actualQueries)
    {
      Assert.AreEqual(expectedQueries.Count, actualQueries.Count, "Number of queries does not match.");

      var actualQueriesLookup = actualQueries.ToDictionary(e => e.ID, e => e);
      foreach (var expectedQuery in expectedQueries)
      {
        if (!actualQueriesLookup.TryGetValue(expectedQuery.ID, out var actualQuery))
          throw new InvalidOperationException($"Actual queries does not contain the query '{expectedQuery.ID}'.");

        CheckQuery(expectedQuery, actualQuery);
      }
    }

    private void CheckQuery (QueryDefinition expectedQuery, QueryDefinition actualQuery)
    {
      Assert.AreEqual(
          expectedQuery.StorageProviderDefinition,
          actualQuery.StorageProviderDefinition,
          "ProviderID of query definition {0} does not match.",
          expectedQuery.ID);

      Assert.AreEqual(expectedQuery.Statement, actualQuery.Statement, "Statement of query definition {0} does not match.", expectedQuery.ID);

      Assert.AreEqual(expectedQuery.QueryType, actualQuery.QueryType, "QueryType of query definition {0} does not match.", expectedQuery.ID);

      Assert.AreEqual(
          expectedQuery.CollectionType,
          actualQuery.CollectionType,
          "CollectionType of query definition {0} does not match.",
          expectedQuery.ID);
    }
  }
}
