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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Queries.Configuration
{
  /// <summary>
  /// Default implementation of the <see cref="IQueryDefinitionRepository"/> interface.
  /// </summary>
  /// <threadsafety static="true" instance="true" />
  public sealed class QueryDefinitionRepository : IQueryDefinitionRepository
  {
    private readonly IReadOnlyDictionary<string, QueryDefinition> _queryLookup;

    public QueryDefinitionRepository (IReadOnlyCollection<QueryDefinition> queryDefinitions)
    {
      ArgumentUtility.CheckNotNullOrItemsNull(nameof(queryDefinitions), queryDefinitions);

      var queryLookup = new Dictionary<string, QueryDefinition>();

      foreach (var queryDefinition in queryDefinitions)
      {
        if (queryLookup.ContainsKey(queryDefinition.ID))
          throw new ArgumentException($"Duplicate query definition with ID '{queryDefinition.ID}' found.", nameof(queryDefinitions));

        queryLookup.Add(queryDefinition.ID, queryDefinition);
      }

      _queryLookup = queryLookup;
    }

    /// <inheritdoc />
    public bool Contains (string queryID)
    {
      ArgumentUtility.CheckNotNullOrEmpty(nameof(queryID), queryID);

      return _queryLookup.ContainsKey(queryID);
    }

    /// <inheritdoc />
    public QueryDefinition GetMandatory (string queryID)
    {
      ArgumentUtility.CheckNotNullOrEmpty(nameof(queryID), queryID);

      if (!_queryLookup.TryGetValue(queryID, out var queryDefinition))
        throw new QueryConfigurationException($"QueryDefinition '{queryID}' does not exist.");

      return queryDefinition;
    }
  }
}
