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

namespace Remotion.Data.DomainObjects.Queries.EagerFetching
{
  /// <summary>
  /// Registers a set of related objects with a set of originating objects based on an <see cref="IRelationEndPointDefinition"/>. If one of the 
  /// relation end-points already has data registered, the new related object data is ignored.
  /// </summary>
  public interface IFetchedRelationDataRegistrationAgent
  {
    void GroupAndRegisterRelatedObjects (IRelationEndPointDefinition relationEndPointDefinition, ICollection<ILoadedObjectData> originatingObjects, ICollection<LoadedObjectDataWithDataSourceData> relatedObjects);
  }
}