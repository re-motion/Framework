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
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Tracing;

namespace Remotion.Data.DomainObjects.Persistence
{
  [Obsolete("Use IReadOnlyStorageProvider or IStorageProvider instead. (Version 4.11.0)", true)]
  public abstract class StorageProvider
  {
    private const string c_messageForPublicMembers = "Use IReadOnlyStorageProvider or IStorageProvider instead. For derived classes/mixins, consider deriving from/mixing RdbmsProvider instead. (Version 4.11.0)";
    private const string c_messageForAbstractMembers = "Derive from/mix RdbmsProvider instead, or fully implement the IStorageProvider interface in your class. (Version 4.11.0)";

    [Obsolete(c_messageForAbstractMembers, true)]
    protected StorageProvider (StorageProviderDefinition storageProviderDefinition, IPersistenceExtension persistenceExtension)
    {
      throw new NotImplementedException();
    }

    [Obsolete(c_messageForPublicMembers, true)]
    public virtual void Dispose ()
    {
      throw new NotImplementedException();
    }

    [Obsolete(c_messageForAbstractMembers, true)]
    public abstract ObjectLookupResult<DataContainer> LoadDataContainer (ObjectID id);

    [Obsolete(c_messageForAbstractMembers, true)]
    public abstract IEnumerable<ObjectLookupResult<DataContainer>> LoadDataContainers (IReadOnlyCollection<ObjectID> ids);

    [Obsolete(c_messageForAbstractMembers, true)]
    public abstract IEnumerable<DataContainer> LoadDataContainersByRelatedID (
        RelationEndPointDefinition relationEndPointDefinition,
        SortExpressionDefinition? sortExpressionDefinition,
        ObjectID relatedID);

    [Obsolete(c_messageForAbstractMembers, true)]
    public abstract void Save (IReadOnlyCollection<DataContainer> dataContainers);

    [Obsolete(c_messageForAbstractMembers, true)]
    public abstract void UpdateTimestamps (IReadOnlyCollection<DataContainer> dataContainers);

    [Obsolete(c_messageForAbstractMembers, true)]
    public abstract void BeginTransaction ();

    [Obsolete(c_messageForAbstractMembers, true)]
    public abstract void Commit ();

    [Obsolete(c_messageForAbstractMembers, true)]
    public abstract void Rollback ();

    [Obsolete(c_messageForAbstractMembers, true)]
    public abstract ObjectID CreateNewObjectID (ClassDefinition classDefinition);

    [Obsolete(c_messageForAbstractMembers, true)]
    public abstract IEnumerable<DataContainer?> ExecuteCollectionQuery (IQuery query);

    [Obsolete(c_messageForAbstractMembers, true)]
    public abstract IEnumerable<IQueryResultRow> ExecuteCustomQuery (IQuery query);

    [Obsolete(c_messageForAbstractMembers, true)]
    public abstract object? ExecuteScalarQuery (IQuery query);

    [Obsolete(c_messageForPublicMembers, true)]
    public StorageProviderDefinition StorageProviderDefinition => throw new NotImplementedException();

    [Obsolete(c_messageForPublicMembers, true)]
    public IPersistenceExtension PersistenceExtension => throw new NotImplementedException();
  }
}
