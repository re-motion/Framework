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
using System.Linq;
using System.Threading;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Queries.Configuration
{
  /// <summary>
  /// Implements the <see cref="IQueryDefinitionRepository"/> interface with a lazily resolved list of <see cref="QueryDefinition"/>.
  /// </summary>
  /// <threadsafety static="true" instance="true" />
  [ImplementationFor(typeof(IQueryDefinitionRepository), Lifetime = LifetimeKind.Singleton)]
  public class DeferredQueryDefinitionRepository : IQueryDefinitionRepository
  {
    private readonly Lazy<IQueryDefinitionRepository> _lazyRepository;

    public DeferredQueryDefinitionRepository ()
    {
      _lazyRepository = new Lazy<IQueryDefinitionRepository>(
          CreateQueryDefinitionRepository,
          LazyThreadSafetyMode.ExecutionAndPublication);
    }

    /// <inheritdoc />
    public bool Contains (string queryID)
    {
      ArgumentUtility.CheckNotNullOrEmpty("queryID", queryID);
      return _lazyRepository.Value.Contains(queryID);
    }

    /// <inheritdoc />
    public QueryDefinition GetMandatory (string queryID)
    {
      ArgumentUtility.CheckNotNullOrEmpty("queryID", queryID);
      return _lazyRepository.Value.GetMandatory(queryID);
    }

    private IQueryDefinitionRepository CreateQueryDefinitionRepository ()
    {
      // TODO RM-8992: Introduce QueryDefinitionLoader
      var queryDefinitions = DomainObjectsConfiguration.Current.Query.QueryDefinitions.Cast<QueryDefinition>().ToArray();

      var duplicateQueryIDs = queryDefinitions.GroupBy(qd => qd.ID).Where(g => g.Count() > 1).Select(g => g.Key).ToArray();
      if (duplicateQueryIDs.Length > 0)
        throw new QueryConfigurationException($"Duplicate query definitions with the following IDs: '{string.Join("', '", duplicateQueryIDs)}'.");

      return new QueryDefinitionRepository(queryDefinitions);
    }
  }
}
