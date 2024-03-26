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
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Queries;

namespace Remotion.Data.DomainObjects.Persistence
{
  /// <summary>
  /// Defines an interface for classes instantiating <see cref="IStorageProviderCommand"/> instances for the basic storage provider operations.
  /// <see cref="StorageProvider"/> uses this factory interface when the respective provider methods are called.
  /// </summary>
  public interface IStorageProviderCommandFactory<in TExecutionContext>
  {
    IStorageProviderCommand<ObjectLookupResult<DataContainer>> CreateForSingleIDLookup (ObjectID objectID);

    IStorageProviderCommand<IEnumerable<ObjectLookupResult<DataContainer>>> CreateForSortedMultiIDLookup (
        IEnumerable<ObjectID> objectIDs);

    IStorageProviderCommand<IEnumerable<DataContainer>> CreateForRelationLookup (
        RelationEndPointDefinition foreignKeyEndPoint, ObjectID foreignKeyValue, SortExpressionDefinition? sortExpressionDefinition);

    IStorageProviderCommand<IEnumerable<DataContainer?>> CreateForDataContainerQuery (IQuery query);
    IStorageProviderCommand<IEnumerable<IQueryResultRow>> CreateForCustomQuery (IQuery query);

    IStorageProviderCommand<object?> CreateForScalarQuery (IQuery query);

    IStorageProviderCommand<IEnumerable<ObjectLookupResult<object>>> CreateForMultiTimestampLookup (
        IEnumerable<ObjectID> objectIDs);

    IStorageProviderCommand CreateForSave (IEnumerable<DataContainer> dataContainers);
  }
}
