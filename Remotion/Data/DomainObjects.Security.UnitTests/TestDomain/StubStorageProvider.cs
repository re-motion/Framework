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
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Security.UnitTests.TestDomain
{
  public class StubStorageProvider : StorageProvider
  {
    public const string StorageProviderID = "StubStorageProvider";
    public const string GetSecurableObjectsQueryID = "StubQueryID";

    public StubStorageProvider (StorageProviderDefinition definition, IPersistenceExtension persistenceExtension)
      : base(definition, persistenceExtension)
    {
    }

    public override ObjectLookupResult<DataContainer> LoadDataContainer (ObjectID id)
    {
      throw new NotImplementedException();
    }

    public override IEnumerable<ObjectLookupResult<DataContainer>> LoadDataContainers (IReadOnlyCollection<ObjectID> ids)
    {
      throw new NotImplementedException();
    }

    public override IEnumerable<DataContainer> ExecuteCollectionQuery (IQuery query)
    {
      ArgumentUtility.CheckNotNull("query", query);

      var collection = new List<DataContainer>();
      if (query.ID == GetSecurableObjectsQueryID)
        collection.Add(DataContainer.CreateNew(CreateNewObjectID(MappingConfiguration.Current.GetClassDefinition(typeof(SecurableObject)))));

      return collection.ToArray();
    }

    public override IEnumerable<IQueryResultRow> ExecuteCustomQuery (IQuery query)
    {
      ArgumentUtility.CheckNotNull("query", query);

      return new QueryResultRow[0];
    }

    public override object ExecuteScalarQuery (IQuery query)
    {
      throw new NotImplementedException();
    }

    public override void Save (IReadOnlyCollection<DataContainer> dataContainers)
    {
    }

    public override void UpdateTimestamps (IReadOnlyCollection<DataContainer> dataContainers)
    {
    }

    public override IEnumerable<DataContainer> LoadDataContainersByRelatedID (
        RelationEndPointDefinition relationEndPointDefinition,
        SortExpressionDefinition sortExpressionDefinition,
        ObjectID relatedID)
    {
      throw new NotImplementedException();
    }

    public override void BeginTransaction ()
    {
    }

    public override void Commit ()
    {
    }

    public override void Rollback ()
    {
    }

    public override ObjectID CreateNewObjectID (ClassDefinition classDefinition)
    {
      CheckDisposed();
      ArgumentUtility.CheckNotNull("classDefinition", classDefinition);
      CheckClassDefinition(classDefinition, "classDefinition");

      return new ObjectID(classDefinition.ID, Guid.NewGuid());
    }

    private void CheckClassDefinition (ClassDefinition classDefinition, string argumentName)
    {
      if (classDefinition.StorageEntityDefinition.StorageProviderDefinition != StorageProviderDefinition)
      {
        throw CreateArgumentException(
            argumentName,
            "The StorageProviderID '{0}' of the provided ClassDefinition does not match with this StorageProvider's ID '{1}'.",
            classDefinition.StorageEntityDefinition.StorageProviderDefinition.Name,
            StorageProviderDefinition.Name);
      }
    }
  }
}
