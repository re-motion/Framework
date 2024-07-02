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
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.EagerFetching;
using Remotion.FunctionalProgramming;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence
{
  /// <summary>
  /// Extends an <see cref="IObjectLoader"/> with the ability to execute an eager fetch query.
  /// </summary>
  public class FetchEnabledObjectLoader : ObjectLoader, IFetchEnabledObjectLoader
  {
    private readonly IFetchEnabledPersistenceStrategy _persistenceStrategy;
    private readonly IEagerFetcher _eagerFetcher;

    public FetchEnabledObjectLoader (
        IFetchEnabledPersistenceStrategy persistenceStrategy,
        ILoadedObjectDataRegistrationAgent loadedObjectDataRegistrationAgent,
        ILoadedObjectDataProvider loadedObjectDataProvider,
        IEagerFetcher eagerFetcher)
        : base(persistenceStrategy, loadedObjectDataRegistrationAgent, loadedObjectDataProvider)
    {
      ArgumentUtility.CheckNotNull("persistenceStrategy", persistenceStrategy);
      ArgumentUtility.CheckNotNull("eagerFetcher", eagerFetcher);

      _persistenceStrategy = persistenceStrategy;
      _eagerFetcher = eagerFetcher;
    }

    public new IFetchEnabledPersistenceStrategy PersistenceStrategy
    {
      get { return _persistenceStrategy; }
    }

    public IEagerFetcher EagerFetcher
    {
      get { return _eagerFetcher; }
    }

    public override ICollection<ILoadedObjectData> GetOrLoadCollectionQueryResult (IQuery query)
    {
      ArgumentUtility.CheckNotNull("query", query);

      var pendingRegistrationCollector = new LoadedObjectDataPendingRegistrationCollector();

      var loadedObjectData = _persistenceStrategy.ExecuteCollectionQuery(query, LoadedObjectDataProvider);

      var loadedObjectDataAfterConsolidation = LoadedObjectDataRegistrationAgent
          .BeginRegisterIfRequired(loadedObjectData, true, pendingRegistrationCollector)
          .ConvertToCollection();

      try
      {
        _eagerFetcher.PerformEagerFetching(loadedObjectDataAfterConsolidation, query.EagerFetchQueries, this, pendingRegistrationCollector);
      }
      finally
      {
        // Even with an exception during eager fetching, go ahead and register everything.
        LoadedObjectDataRegistrationAgent.EndRegisterIfRequired(pendingRegistrationCollector);
      }

      return loadedObjectDataAfterConsolidation;
    }

    public ICollection<LoadedObjectDataWithDataSourceData> GetOrLoadFetchQueryResult (
        IQuery query,
        LoadedObjectDataPendingRegistrationCollector pendingRegistrationCollector)
    {
      ArgumentUtility.CheckNotNull("query", query);
      ArgumentUtility.CheckNotNull("pendingRegistrationCollector", pendingRegistrationCollector);

      var loadedObjectDataWithSource = _persistenceStrategy.ExecuteFetchQuery(query, LoadedObjectDataProvider).ConvertToCollection();

      var loadedObjectDataAfterConsolidation = LoadedObjectDataRegistrationAgent
          .BeginRegisterIfRequired(
              loadedObjectDataWithSource.Select(data => data.LoadedObjectData),
              true,
              pendingRegistrationCollector)
          .ToList();

      Assertion.IsTrue(loadedObjectDataWithSource.Count == loadedObjectDataAfterConsolidation.Count);
      var loadedObjectDataWithSourceAfterConsolidation = loadedObjectDataWithSource
          .Select((d, i) => new LoadedObjectDataWithDataSourceData(loadedObjectDataAfterConsolidation[i], d.DataSourceData))
          .ConvertToCollection();

      _eagerFetcher.PerformEagerFetching(loadedObjectDataAfterConsolidation, query.EagerFetchQueries, this, pendingRegistrationCollector);

      return loadedObjectDataWithSourceAfterConsolidation;
    }
  }
}
