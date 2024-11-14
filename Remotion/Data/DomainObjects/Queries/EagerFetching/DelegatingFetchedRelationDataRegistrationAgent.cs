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
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Queries.EagerFetching
{
  /// <summary>
  /// Implements the <see cref="IFetchedRelationDataRegistrationAgent"/> interface by checking the given <see cref="IRelationEndPointDefinition"/> 
  /// and delegating to a specific <see cref="IFetchedRelationDataRegistrationAgent"/> implementation for that end-point type.
  /// </summary>
  public class DelegatingFetchedRelationDataRegistrationAgent : IFetchedRelationDataRegistrationAgent
  {
    private readonly IFetchedRelationDataRegistrationAgent _realObjectDataRegistrationAgent;
    private readonly IFetchedRelationDataRegistrationAgent _virtualObjectDataRegistrationAgent;
    private readonly IFetchedRelationDataRegistrationAgent _collectionDataRegistrationAgent;

    public DelegatingFetchedRelationDataRegistrationAgent (
        IFetchedRelationDataRegistrationAgent realObjectDataRegistrationAgent,
        IFetchedRelationDataRegistrationAgent virtualObjectDataRegistrationAgent,
        IFetchedRelationDataRegistrationAgent collectionDataRegistrationAgent)
    {
      ArgumentUtility.CheckNotNull("realObjectDataRegistrationAgent", realObjectDataRegistrationAgent);
      ArgumentUtility.CheckNotNull("virtualObjectDataRegistrationAgent", virtualObjectDataRegistrationAgent);
      ArgumentUtility.CheckNotNull("collectionDataRegistrationAgent", collectionDataRegistrationAgent);

      _realObjectDataRegistrationAgent = realObjectDataRegistrationAgent;
      _virtualObjectDataRegistrationAgent = virtualObjectDataRegistrationAgent;
      _collectionDataRegistrationAgent = collectionDataRegistrationAgent;
    }

    public IFetchedRelationDataRegistrationAgent RealObjectDataRegistrationAgent
    {
      get { return _realObjectDataRegistrationAgent; }
    }

    public IFetchedRelationDataRegistrationAgent VirtualObjectDataRegistrationAgent
    {
      get { return _virtualObjectDataRegistrationAgent; }
    }

    public IFetchedRelationDataRegistrationAgent CollectionDataRegistrationAgent
    {
      get { return _collectionDataRegistrationAgent; }
    }

    public void GroupAndRegisterRelatedObjects (
        IRelationEndPointDefinition relationEndPointDefinition,
        ICollection<ILoadedObjectData> originatingObjects,
        ICollection<LoadedObjectDataWithDataSourceData> relatedObjects)
    {
      ArgumentUtility.CheckNotNull("relationEndPointDefinition", relationEndPointDefinition);
      ArgumentUtility.CheckNotNullOrEmpty("originatingObjects", originatingObjects);

      var specificAgent = GetSpecificAgent(relationEndPointDefinition);
      specificAgent.GroupAndRegisterRelatedObjects(relationEndPointDefinition, originatingObjects, relatedObjects);
    }

    private IFetchedRelationDataRegistrationAgent GetSpecificAgent (IRelationEndPointDefinition relationEndPointDefinition)
    {
      if (relationEndPointDefinition.IsAnonymous)
        throw new InvalidOperationException("Anonymous relation end-points cannot have data registered.");

      if (relationEndPointDefinition.Cardinality == CardinalityType.Many)
        return _collectionDataRegistrationAgent;
      else if (relationEndPointDefinition.IsVirtual)
        return _virtualObjectDataRegistrationAgent;
      else
        return _realObjectDataRegistrationAgent;
    }
  }
}
