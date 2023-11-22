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
using Moq;
using Remotion.Context;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests
{
  // Used by the Official class definition.
  public class UnitTestStorageProviderStub : StorageProvider
  {
    private class MockStorageProviderScope : IDisposable
    {
      private readonly StorageProvider _previous;
      private bool _disposed = false;

      public MockStorageProviderScope (StorageProvider previous)
      {
        _previous = previous;
      }

      public void Dispose ()
      {
        if (!_disposed)
        {
          _innerMockStorageProvider.SetCurrent(_previous);
          _disposed = true;
        }
      }
    }

    private static int s_nextID = 0;

    private static readonly SafeContextSingleton<StorageProvider> _innerMockStorageProvider =
        new SafeContextSingleton<StorageProvider>(typeof(UnitTestStorageProviderStub) + "._innerMockStorageProvider", () => null);

    public static IDisposable EnterMockStorageProviderScope (StorageProvider mock)
    {
      var previous = _innerMockStorageProvider.Current;
      _innerMockStorageProvider.SetCurrent(mock);
      return new MockStorageProviderScope(previous);
    }

    public static T ExecuteWithMock<T> (StorageProvider mockedStorageProvider, Func<T> func)
    {
      using (EnterMockStorageProviderScope(mockedStorageProvider))
      {
        return func();
      }
    }

    public static StorageProvider CreateStorageProviderMockForOfficial ()
    {
      var storageProviderDefinition = MappingConfiguration.Current.GetTypeDefinition(typeof(Official)).StorageEntityDefinition.StorageProviderDefinition;
      return new Mock<StorageProvider>(storageProviderDefinition, NullPersistenceExtension.Instance).Object;
    }

    public UnitTestStorageProviderStub (
        UnitTestStorageProviderStubDefinition definition,
        IPersistenceExtension persistenceExtension)
        : base(definition, persistenceExtension)
    {
    }

    public StorageProvider InnerProvider
    {
      get { return _innerMockStorageProvider.Current; }
    }

    public override ObjectLookupResult<DataContainer> LoadDataContainer (ObjectID id)
    {
      if (InnerProvider != null)
        return InnerProvider.LoadDataContainer(id);
      else
      {
        DataContainer container = DataContainer.CreateForExisting(
            id,
            null,
            delegate (PropertyDefinition propertyDefinition)
            {
              if (propertyDefinition.PropertyName.EndsWith(".Name"))
                return "Max Sachbearbeiter";
              else
                return propertyDefinition.DefaultValue;
            });

        var idAsInt = (int)id.Value;
        if (s_nextID <= idAsInt)
          s_nextID = idAsInt + 1;
        return new ObjectLookupResult<DataContainer>(id, container);
      }
    }

    public override IEnumerable<ObjectLookupResult<DataContainer>> LoadDataContainers (IReadOnlyCollection<ObjectID> ids)
    {
      if (InnerProvider != null)
        return InnerProvider.LoadDataContainers(ids);
      else
        return ids.Select(LoadDataContainer);
    }

    public override IEnumerable<DataContainer> ExecuteCollectionQuery (IQuery query)
    {
      if (InnerProvider != null)
        return InnerProvider.ExecuteCollectionQuery(query);
      else
        return null;
    }

    public override IEnumerable<IQueryResultRow> ExecuteCustomQuery (IQuery query)
    {
      if (InnerProvider != null)
        return InnerProvider.ExecuteCustomQuery(query);
      else
        return null;
    }

    public override object ExecuteScalarQuery (IQuery query)
    {
      if (InnerProvider != null)
        return InnerProvider.ExecuteScalarQuery(query);
      else
        return null;
    }

    public override void Save (IReadOnlyCollection<DataContainer> dataContainers)
    {
      if (InnerProvider != null)
        InnerProvider.Save(dataContainers);
    }

    public override void UpdateTimestamps (IReadOnlyCollection<DataContainer> dataContainers)
    {
      if (InnerProvider != null)
        InnerProvider.UpdateTimestamps(dataContainers);
    }

    public override IEnumerable<DataContainer> LoadDataContainersByRelatedID (
        RelationEndPointDefinition relationEndPointDefinition,
        SortExpressionDefinition sortExpressionDefinition,
        ObjectID relatedID)
    {
      if (InnerProvider != null)
        return InnerProvider.LoadDataContainersByRelatedID(relationEndPointDefinition, sortExpressionDefinition, relatedID);
      else
        return null;
    }

    public override void BeginTransaction ()
    {
      if (InnerProvider != null)
        InnerProvider.BeginTransaction();
    }

    public override void Commit ()
    {
      if (InnerProvider != null)
        InnerProvider.Commit();
    }

    public override void Rollback ()
    {
      if (InnerProvider != null)
        InnerProvider.Rollback();
    }

    public override ObjectID CreateNewObjectID (ClassDefinition classDefinition)
    {
      if (InnerProvider != null)
        return InnerProvider.CreateNewObjectID(classDefinition);
      else
        return new ObjectID(classDefinition, s_nextID++);
    }
  }
}
