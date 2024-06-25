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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.FunctionalProgramming;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence
{
  /// <summary>
  /// Implements the mechanisms for loading a set of <see cref="DomainObject"/> objects into a <see cref="ClientTransaction"/> by acting as a
  /// facade for <see cref="IPersistenceStrategy"/> and <see cref="ILoadedObjectDataRegistrationAgent"/>.
  /// </summary>
  /// <remarks>
  /// <para>
  /// This class (indirectly, via <see cref="ObjectPersistence.LoadedObjectDataRegistrationAgent"/>) signals all load-related events, but it does not 
  /// signal the <see cref="IClientTransactionListener.FilterQueryResult{T}"/> event.
  /// </para>
  /// </remarks>
  public class ObjectLoader : IObjectLoader
  {
    private readonly IPersistenceStrategy _persistenceStrategy;
    private readonly ILoadedObjectDataRegistrationAgent _loadedObjectDataRegistrationAgent;
    private readonly ILoadedObjectDataProvider _loadedObjectDataProvider;

    public ObjectLoader (
        IPersistenceStrategy persistenceStrategy,
        ILoadedObjectDataRegistrationAgent loadedObjectDataRegistrationAgent,
        ILoadedObjectDataProvider loadedObjectDataProvider)
    {
      ArgumentUtility.CheckNotNull("persistenceStrategy", persistenceStrategy);
      ArgumentUtility.CheckNotNull("loadedObjectDataRegistrationAgent", loadedObjectDataRegistrationAgent);
      ArgumentUtility.CheckNotNull("loadedObjectDataProvider", loadedObjectDataProvider);

      _persistenceStrategy = persistenceStrategy;
      _loadedObjectDataRegistrationAgent = loadedObjectDataRegistrationAgent;
      _loadedObjectDataProvider = loadedObjectDataProvider;
    }

    public IPersistenceStrategy PersistenceStrategy
    {
      get { return _persistenceStrategy; }
    }

    public ILoadedObjectDataRegistrationAgent LoadedObjectDataRegistrationAgent
    {
      get { return _loadedObjectDataRegistrationAgent; }
    }

    public ILoadedObjectDataProvider LoadedObjectDataProvider
    {
      get { return _loadedObjectDataProvider; }
    }

    public virtual ILoadedObjectData LoadObject (ObjectID id, bool throwOnNotFound)
    {
      ArgumentUtility.CheckNotNull("id", id);

      var loadedObjectData = _persistenceStrategy.LoadObjectData(id);
      _loadedObjectDataRegistrationAgent.RegisterIfRequired(new[] { loadedObjectData }, throwOnNotFound);
      return loadedObjectData;
    }

    public virtual ICollection<ILoadedObjectData> LoadObjects (IEnumerable<ObjectID> idsToBeLoaded, bool throwOnNotFound)
    {
      ArgumentUtility.CheckNotNull("idsToBeLoaded", idsToBeLoaded);

      var idsToBeLoadedAsCollection = idsToBeLoaded.ConvertToCollection();
      var loadedObjectData = _persistenceStrategy.LoadObjectData(idsToBeLoadedAsCollection).ConvertToCollection();

      Assertion.IsTrue(loadedObjectData.Count == idsToBeLoadedAsCollection.Count, "Persistence strategy must return exactly as many items as requested.");
      Assertion.DebugAssert(
          loadedObjectData.Zip(idsToBeLoadedAsCollection).All(tuple => tuple.Item1.ObjectID == tuple.Item2),
          "Persistence strategy result must be in the same order as the input IDs.");

      _loadedObjectDataRegistrationAgent.RegisterIfRequired(loadedObjectData, throwOnNotFound);

      return loadedObjectData;
    }

    public virtual ILoadedObjectData GetOrLoadRelatedObject (RelationEndPointID relationEndPointID)
    {
      ArgumentUtility.CheckNotNull("relationEndPointID", relationEndPointID);

      if (!relationEndPointID.Definition.IsVirtual)
        throw new ArgumentException("GetOrLoadRelatedObject can only be used with virtual end points.", "relationEndPointID");

      if (relationEndPointID.Definition.Cardinality != CardinalityType.One)
        throw new ArgumentException("GetOrLoadRelatedObject can only be used with one-valued end points.", "relationEndPointID");

      var loadedObjectData = _persistenceStrategy.ResolveObjectRelationData(relationEndPointID, _loadedObjectDataProvider);
      _loadedObjectDataRegistrationAgent.RegisterIfRequired(new[] { loadedObjectData }, true);
      return loadedObjectData;
    }

    public virtual ICollection<ILoadedObjectData> GetOrLoadRelatedObjects (RelationEndPointID relationEndPointID)
    {
      ArgumentUtility.CheckNotNull("relationEndPointID", relationEndPointID);

      if (relationEndPointID.Definition.Cardinality != CardinalityType.Many)
        throw new ArgumentException("GetOrLoadRelatedObjects can only be used with many-valued end points.", "relationEndPointID");

      var loadedObjectData = _persistenceStrategy.ResolveCollectionRelationData(relationEndPointID, _loadedObjectDataProvider).ConvertToCollection();
      _loadedObjectDataRegistrationAgent.RegisterIfRequired(loadedObjectData, true);
      return loadedObjectData;
    }

    public virtual ICollection<ILoadedObjectData> GetOrLoadCollectionQueryResult (IQuery query)
    {
      ArgumentUtility.CheckNotNull("query", query);

      var loadedObjectData = _persistenceStrategy.ExecuteCollectionQuery(query, _loadedObjectDataProvider).ConvertToCollection();
      _loadedObjectDataRegistrationAgent.RegisterIfRequired(loadedObjectData, true);
      return loadedObjectData;
    }
  }
}
