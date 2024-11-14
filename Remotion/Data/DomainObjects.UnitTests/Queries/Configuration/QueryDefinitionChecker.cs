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
      Assert.That(actualQueries.Count, Is.EqualTo(expectedQueries.Count), "Number of queries does not match.");

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
      Assert.That(
          actualQuery.StorageProviderDefinition,
          Is.EqualTo(expectedQuery.StorageProviderDefinition),
          $"ProviderID of query definition {expectedQuery.ID} does not match.");

      Assert.That(
          actualQuery.Statement,
          Is.EqualTo(expectedQuery.Statement),
          $"Statement of query definition {expectedQuery.ID} does not match.");

      Assert.That(
          actualQuery.QueryType,
          Is.EqualTo(expectedQuery.QueryType),
          $"QueryType of query definition {expectedQuery.ID} does not match.");

      Assert.That(
          actualQuery.CollectionType,
          Is.EqualTo(expectedQuery.CollectionType),
          $"CollectionType of query definition {expectedQuery.ID} does not match.");
    }
  }
}
