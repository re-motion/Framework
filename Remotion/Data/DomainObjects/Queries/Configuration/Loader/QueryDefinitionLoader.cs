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
using System.Collections.ObjectModel;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Queries.Configuration.Loader
{
  /// <summary>
  /// Default implementation of the <see cref="IQueryDefinitionLoader"/> interface.
  /// </summary>
  /// <threadsafety static="true" instance="true" />
  [ImplementationFor(typeof(IQueryDefinitionLoader), Lifetime = LifetimeKind.Singleton)]
  public class QueryDefinitionLoader : IQueryDefinitionLoader
  {
    private readonly IQueryFileFinder _queryFileFinder;

    public QueryDefinitionLoader (IQueryFileFinder queryFileFinder)
    {
      ArgumentUtility.CheckNotNull(nameof(queryFileFinder), queryFileFinder);

      _queryFileFinder = queryFileFinder;
    }

    /// <inheritdoc />
    public IReadOnlyCollection<QueryDefinition> LoadAllQueryDefinitions ()
    {
      var storageProviderDefinitionFinder = new StorageGroupBasedStorageProviderDefinitionFinder(DomainObjectsConfiguration.Current.Storage);
      var loader = new QueryDefinitionFileLoader(storageProviderDefinitionFinder);

      var result = new List<QueryDefinition>();
      var addedQueryIDs = new HashSet<string>();
      foreach (var queryFilePath in _queryFileFinder.GetQueryFilePaths())
      {
        var queryDefinitions = loader.LoadQueryDefinitions(queryFilePath);
        foreach (var queryDefinition in queryDefinitions)
        {
          if (!addedQueryIDs.Add(queryDefinition.ID))
            throw new ConfigurationException($"File '{queryFilePath}' defines a duplicate for query definition '{queryDefinition.ID}'.");

          result.Add(queryDefinition);
        }
      }

      return new ReadOnlyCollection<QueryDefinition>(result);
    }
  }
}
