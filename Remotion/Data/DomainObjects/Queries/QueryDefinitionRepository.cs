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
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.DomainObjects.Queries.ConfigurationLoader;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Queries
{
  /// <summary>
  /// Default implementation of the <see cref="IQueryDefinitionRepository"/> interface.
  /// </summary>
  [ImplementationFor(typeof(IQueryDefinitionRepository), Lifetime = LifetimeKind.Singleton)]
  public class QueryDefinitionRepository : IQueryDefinitionRepository
  {
    public static QueryDefinitionRepository FromQueryDefinitions (IEnumerable<QueryDefinition> queryDefinitions)
    {
      ArgumentUtility.CheckNotNull(nameof(queryDefinitions), queryDefinitions);

      return new QueryDefinitionRepository(queryDefinitions);
    }

    private readonly Dictionary<string, QueryDefinition> _queryLookup;

    public QueryDefinitionRepository (IQueryDefinitionLoader queryDefinitionLoader)
        : this(ArgumentUtility.CheckNotNull(nameof(queryDefinitionLoader), queryDefinitionLoader).LoadAllQueryDefinitions())
    {
    }

    private QueryDefinitionRepository (IEnumerable<QueryDefinition> queryDefinitions)
    {
      ArgumentUtility.CheckNotNull(nameof(queryDefinitions), queryDefinitions);

      var queryLookup = new Dictionary<string, QueryDefinition>();
      HashSet<string>? duplicateQueryIDs = null;

      var index = 0;
      foreach (var queryDefinition in queryDefinitions)
      {
        if (queryDefinition == null)
          throw new ArgumentException($"Item {index} of parameter '{nameof(queryDefinitions)}' is null.", nameof(queryDefinitions));

        if (queryLookup.ContainsKey(queryDefinition.ID))
        {
          duplicateQueryIDs ??= new HashSet<string>();
          duplicateQueryIDs.Add(queryDefinition.ID);
          continue;
        }

        queryLookup.Add(queryDefinition.ID, queryDefinition);
        index += 1;
      }

      if (duplicateQueryIDs != null)
        throw new ArgumentException($"Duplicate query definitions with the following IDs: '{string.Join("', '", duplicateQueryIDs)}'.", nameof(queryDefinitions));

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
