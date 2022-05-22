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
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.SerializableFakes
{
  public class SerializableVirtualCollectionEndPointFake : IVirtualCollectionEndPoint
  {
    public SerializableVirtualCollectionEndPointFake ()
    {
    }

    public SerializableVirtualCollectionEndPointFake (FlattenedDeserializationInfo info)
    {
    }

    public void SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
    }

    public bool IsNull => throw new NotImplementedException();

    public RelationEndPointID ID => throw new NotImplementedException();

    public ClientTransaction ClientTransaction => throw new NotImplementedException();

    public ObjectID ObjectID => throw new NotImplementedException();

    public IRelationEndPointDefinition Definition => throw new NotImplementedException();

    public RelationDefinition RelationDefinition => throw new NotImplementedException();

    public bool HasChanged => throw new NotImplementedException();

    public bool HasBeenTouched => throw new NotImplementedException();

    public IDomainObject GetDomainObject ()
    {
      throw new NotImplementedException();
    }

    public IDomainObject GetDomainObjectReference ()
    {
      throw new NotImplementedException();
    }

    public bool IsDataComplete => throw new NotImplementedException();

    public void EnsureDataComplete ()
    {
      throw new NotImplementedException();
    }

    public bool? IsSynchronized => throw new NotImplementedException();

    public void Synchronize ()
    {
      throw new NotImplementedException();
    }

    public void Touch ()
    {
      throw new NotImplementedException();
    }

    public void Commit ()
    {
      throw new NotImplementedException();
    }

    public void Rollback ()
    {
      throw new NotImplementedException();
    }

    public IDataManagementCommand CreateRemoveCommand (IDomainObject removedRelatedObject)
    {
      throw new NotImplementedException();
    }

    public IDataManagementCommand CreateDeleteCommand ()
    {
      throw new NotImplementedException();
    }

    public void ValidateMandatory ()
    {
      throw new NotImplementedException();
    }

    public IEnumerable<RelationEndPointID> GetOppositeRelationEndPointIDs ()
    {
      throw new NotImplementedException();
    }

    public void SetDataFromSubTransaction (IRelationEndPoint source)
    {
      throw new NotImplementedException();
    }

    public bool CanBeCollected => throw new NotImplementedException();

    public bool CanBeMarkedIncomplete => throw new NotImplementedException();

    public void MarkDataIncomplete ()
    {
      throw new NotImplementedException();
    }

    public void RegisterOriginalOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      throw new NotImplementedException();
    }

    public void UnregisterOriginalOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      throw new NotImplementedException();
    }

    public void RegisterCurrentOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      throw new NotImplementedException();
    }

    public void UnregisterCurrentOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      throw new NotImplementedException();
    }

    public void SynchronizeOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      throw new NotImplementedException();
    }

    public ReadOnlyVirtualCollectionDataDecorator GetData ()
    {
      throw new NotImplementedException();
    }

    public ReadOnlyVirtualCollectionDataDecorator GetOriginalData ()
    {
      throw new NotImplementedException();
    }

    public bool? HasChangedFast => throw new NotImplementedException();

    public void MarkDataComplete (IDomainObject[] items)
    {
      throw new NotImplementedException();
    }

    public IDataManagementCommand CreateAddCommand (IDomainObject addedRelatedObject)
    {
      throw new NotImplementedException();
    }

    public void SortCurrentData (Comparison<IDomainObject> comparison)
    {
      throw new NotImplementedException();
    }

    public IObjectList<IDomainObject> Collection => throw new NotImplementedException();

    public IObjectList<IDomainObject> GetCollectionWithOriginalData ()
    {
      throw new NotImplementedException();
    }
  }
}
