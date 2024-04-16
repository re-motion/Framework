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
using Remotion.Context;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Queries;

namespace Remotion.Data.DomainObjects.UnitTests
{
  // Used by the Official class definition.
  public class UnitTestStorageProviderStub : IStorageProvider
  {
    private class MockStorageProviderScope : IDisposable
    {
      private readonly IStorageProvider _previous;
      private bool _disposed = false;

      public MockStorageProviderScope (IStorageProvider previous)
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

    private static readonly SafeContextSingleton<IStorageProvider> _innerMockStorageProvider = new(typeof(UnitTestStorageProviderStub) + "._innerMockStorageProvider", () => null);

    public static IDisposable EnterMockStorageProviderScope (IStorageProvider mock)
    {
      var previous = _innerMockStorageProvider.Current;
      _innerMockStorageProvider.SetCurrent(mock);
      return new MockStorageProviderScope(previous);
    }

    public static T ExecuteWithMock<T> (IStorageProvider mockedStorageProvider, Func<T> func)
    {
      using (EnterMockStorageProviderScope(mockedStorageProvider))
      {
        return func();
      }
    }

    public UnitTestStorageProviderStub ()
    {
    }

    public IStorageProvider InnerProvider => _innerMockStorageProvider.Current;

    public ObjectLookupResult<DataContainer> LoadDataContainer (ObjectID id)
    {
      if (InnerProvider != null)
      {
        return InnerProvider.LoadDataContainer(id);
      }
      else
      {
        var container = DataContainer.CreateForExisting(
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

    public IEnumerable<ObjectLookupResult<DataContainer>> LoadDataContainers (IReadOnlyCollection<ObjectID> ids)
    {
      if (InnerProvider != null)
        return InnerProvider.LoadDataContainers(ids);
      else
        return ids.Select(LoadDataContainer);
    }

    public IEnumerable<DataContainer> ExecuteCollectionQuery (IQuery query)
    {
      if (InnerProvider != null)
        return InnerProvider.ExecuteCollectionQuery(query);
      else
        return null;
    }

    public IEnumerable<IQueryResultRow> ExecuteCustomQuery (IQuery query)
    {
      if (InnerProvider != null)
        return InnerProvider.ExecuteCustomQuery(query);
      else
        return null;
    }

    public object ExecuteScalarQuery (IQuery query)
    {
      if (InnerProvider != null)
        return InnerProvider.ExecuteScalarQuery(query);
      else
        return null;
    }

    public void Save (IReadOnlyCollection<DataContainer> dataContainers)
    {
      if (InnerProvider != null)
        InnerProvider.Save(dataContainers);
    }

    public void UpdateTimestamps (IReadOnlyCollection<DataContainer> dataContainers)
    {
      if (InnerProvider != null)
        InnerProvider.UpdateTimestamps(dataContainers);
    }

    public IEnumerable<DataContainer> LoadDataContainersByRelatedID (
        RelationEndPointDefinition relationEndPointDefinition,
        SortExpressionDefinition sortExpressionDefinition,
        ObjectID relatedID)
    {
      if (InnerProvider != null)
        return InnerProvider.LoadDataContainersByRelatedID(relationEndPointDefinition, sortExpressionDefinition, relatedID);
      else
        return null;
    }

    public void BeginTransaction ()
    {
      if (InnerProvider != null)
        InnerProvider.BeginTransaction();
    }

    public void Commit ()
    {
      if (InnerProvider != null)
        InnerProvider.Commit();
    }

    public void Rollback ()
    {
      if (InnerProvider != null)
        InnerProvider.Rollback();
    }

    public ObjectID CreateNewObjectID (ClassDefinition classDefinition)
    {
      if (InnerProvider != null)
        return InnerProvider.CreateNewObjectID(classDefinition);
      else
        return new ObjectID(classDefinition, s_nextID++);
    }

    public void Dispose ()
    {
      if (InnerProvider != null)
        InnerProvider.Dispose();
    }
  }
}
