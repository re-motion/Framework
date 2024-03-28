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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Data.DomainObjects.Queries;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms
{
  /// <summary>
  /// Defines an interface for classes instantiating <see cref="IRdbmsProviderReadWriteCommand"/> instances for the basic storage provider operations.
  /// <see cref="RdbmsProvider"/> uses this factory interface when the respective provider methods are called.
  /// </summary>
  public interface IRdbmsProviderCommandFactory
  {
    IRdbmsProviderReadOnlyCommand<ObjectLookupResult<DataContainer>> CreateForSingleIDLookup (ObjectID objectID);

    IRdbmsProviderReadOnlyCommand<IEnumerable<ObjectLookupResult<DataContainer>>> CreateForSortedMultiIDLookup (IEnumerable<ObjectID> objectIDs);

    IRdbmsProviderReadOnlyCommand<IEnumerable<DataContainer>> CreateForRelationLookup (
        RelationEndPointDefinition foreignKeyEndPoint,
        ObjectID foreignKeyValue,
        SortExpressionDefinition? sortExpressionDefinition);

    IRdbmsProviderReadOnlyCommand<IEnumerable<DataContainer?>> CreateForDataContainerQuery (IQuery query);
    IRdbmsProviderReadWriteCommand<IEnumerable<IQueryResultRow>> CreateForCustomQuery (IQuery query);

    IRdbmsProviderReadWriteCommand<object?> CreateForScalarQuery (IQuery query);

    IRdbmsProviderReadOnlyCommand<IEnumerable<ObjectLookupResult<object>>> CreateForMultiTimestampLookup (IEnumerable<ObjectID> objectIDs);

    IRdbmsProviderReadWriteCommand CreateForSave (IEnumerable<DataContainer> dataContainers);
  }
}
