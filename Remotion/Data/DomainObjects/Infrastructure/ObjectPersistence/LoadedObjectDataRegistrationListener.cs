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
using Remotion.Data.DomainObjects.Infrastructure.HierarchyManagement;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence
{
  /// <summary>
  /// Implements <see cref="ILoadedObjectDataRegistrationListener"/> by distributing the events to <see cref="IClientTransactionEventSink"/> and 
  /// <see cref="ITransactionHierarchyManager"/> implementations.
  /// </summary>
  [Serializable]
  public class LoadedObjectDataRegistrationListener : ILoadedObjectDataRegistrationListener
  {
    private readonly IClientTransactionEventSink _eventSink;
    private readonly ITransactionHierarchyManager _hierarchyManager;

    public LoadedObjectDataRegistrationListener (
        IClientTransactionEventSink eventSink, ITransactionHierarchyManager hierarchyManager)
    {
      ArgumentUtility.CheckNotNull("eventSink", eventSink);
      ArgumentUtility.CheckNotNull("hierarchyManager", hierarchyManager);

      _eventSink = eventSink;
      _hierarchyManager = hierarchyManager;
    }

    public IClientTransactionEventSink EventSink
    {
      get { return _eventSink; }
    }

    public ITransactionHierarchyManager HierarchyManager
    {
      get { return _hierarchyManager; }
    }

    public void OnBeforeObjectRegistration (IReadOnlyList<ObjectID> loadedObjectIDs)
    {
      ArgumentUtility.CheckNotNull("loadedObjectIDs", loadedObjectIDs);

      // The ObjectsLoadingEvent is allowed to cancel; therefore, we execute it before indicating that we're starting to register objects.
      // _eventSink.RaiseObjectsLoadingEvent (loadedObjectIDs);

      _hierarchyManager.OnBeforeObjectRegistration(loadedObjectIDs);
      try
      {
        _eventSink.RaiseObjectsLoadingEvent(loadedObjectIDs);
      }
      catch
      {
        _hierarchyManager.OnAfterObjectRegistration(loadedObjectIDs);
        throw;
      }
    }

    public void OnAfterObjectRegistration (IReadOnlyList<ObjectID> loadedObjectIDs, IReadOnlyList<IDomainObject> actuallyLoadedDomainObjects)
    {
      ArgumentUtility.CheckNotNull("loadedObjectIDs", loadedObjectIDs);
      ArgumentUtility.CheckNotNull("actuallyLoadedDomainObjects", actuallyLoadedDomainObjects);

      try
      {
        if (actuallyLoadedDomainObjects.Count > 0)
          _eventSink.RaiseObjectsLoadedEvent(actuallyLoadedDomainObjects);
      }
      finally
      {
        _hierarchyManager.OnAfterObjectRegistration(loadedObjectIDs);
      }
    }

    public void OnObjectsNotFound (IReadOnlyList<ObjectID> notFoundObjectIDs)
    {
      ArgumentUtility.CheckNotNull("notFoundObjectIDs", notFoundObjectIDs);

      _eventSink.RaiseObjectsNotFoundEvent(notFoundObjectIDs);
    }
  }
}
