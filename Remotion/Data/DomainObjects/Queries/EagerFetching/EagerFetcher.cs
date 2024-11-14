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
using Microsoft.Extensions.Logging;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Logging;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Queries.EagerFetching
{
  /// <summary>
  /// Provides functionality for performing eager fetching by executing eager fetch queries and correlating their results with a collection of input
  /// objects. Eager fetching is not performed recursively, recursive fetching must be implemented by the given 
  /// <see cref="IFetchEnabledObjectLoader"/>'s <see cref="IFetchEnabledObjectLoader.GetOrLoadFetchQueryResult"/> method.
  /// </summary>
  public class EagerFetcher : IEagerFetcher
  {
    private static readonly ILogger s_logger = LazyLoggerFactory.CreateLogger<EagerFetcher>();

    private readonly IFetchedRelationDataRegistrationAgent _registrationAgent;

    public EagerFetcher (IFetchedRelationDataRegistrationAgent registrationAgent)
    {
      ArgumentUtility.CheckNotNull("registrationAgent", registrationAgent);
      _registrationAgent = registrationAgent;
    }

    public IFetchedRelationDataRegistrationAgent RegistrationAgent
    {
      get { return _registrationAgent; }
    }

    public void PerformEagerFetching (
        ICollection<ILoadedObjectData> originatingObjects,
        IEnumerable<KeyValuePair<IRelationEndPointDefinition, IQuery>> fetchQueries,
        IFetchEnabledObjectLoader fetchResultLoader,
        LoadedObjectDataPendingRegistrationCollector pendingRegistrationCollector)
    {
      ArgumentUtility.CheckNotNull("originatingObjects", originatingObjects);
      ArgumentUtility.CheckNotNull("fetchQueries", fetchQueries);
      ArgumentUtility.CheckNotNull("fetchResultLoader", fetchResultLoader);
      ArgumentUtility.CheckNotNull("pendingRegistrationCollector", pendingRegistrationCollector);

      if (originatingObjects.Count <= 0)
        return;

      foreach (var item in fetchQueries)
      {
        var relationEndPointDefinition = item.Key;
        var fetchQuery = item.Value;

        s_logger.LogDebug(
            "Eager fetching objects for {0} via query {1} ('{2}').",
            relationEndPointDefinition.PropertyName,
            fetchQuery.ID,
            fetchQuery.Statement);

        var fetchedObjects = fetchResultLoader.GetOrLoadFetchQueryResult(fetchQuery, pendingRegistrationCollector);
        s_logger.LogDebug(
            "The eager fetch query for {0} yielded {1} related objects for {2} original objects.",
            relationEndPointDefinition.PropertyName,
            fetchedObjects.Count,
            originatingObjects.Count);

        try
        {
          _registrationAgent.GroupAndRegisterRelatedObjects(relationEndPointDefinition, originatingObjects, fetchedObjects);
        }
        catch (InvalidOperationException ex)
        {
          throw new UnexpectedQueryResultException("Eager fetching encountered an unexpected query result: " + ex.Message, ex);
        }
      }
    }
  }
}
