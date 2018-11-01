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
  public class SerializableCollectionEndPointFake : ICollectionEndPoint
  {
    public SerializableCollectionEndPointFake ()
    {
    }

    public SerializableCollectionEndPointFake (FlattenedDeserializationInfo info)
    {
    }

    public void SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
    }

    public bool IsNull
    {
      get { throw new NotImplementedException(); }
    }

    public RelationEndPointID ID
    {
      get { throw new NotImplementedException(); }
    }

    public ClientTransaction ClientTransaction
    {
      get { throw new NotImplementedException(); }
    }

    public ObjectID ObjectID
    {
      get { throw new NotImplementedException(); }
    }

    public IRelationEndPointDefinition Definition
    {
      get { throw new NotImplementedException(); }
    }

    public RelationDefinition RelationDefinition
    {
      get { throw new NotImplementedException(); }
    }

    public bool IsDataComplete
    {
      get { throw new NotImplementedException(); }
    }

    public bool HasChanged
    {
      get { throw new NotImplementedException(); }
    }

    public bool HasBeenTouched
    {
      get { throw new NotImplementedException(); }
    }

    public DomainObject GetDomainObject ()
    {
      throw new NotImplementedException();
    }

    public DomainObject GetDomainObjectReference ()
    {
      throw new NotImplementedException();
    }

    public void EnsureDataComplete ()
    {
      throw new NotImplementedException();
    }

    public bool? IsSynchronized
    {
      get { throw new NotImplementedException(); }
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

    public IDataManagementCommand CreateRemoveCommand (DomainObject removedRelatedObject)
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

    public bool? HasChangedFast
    {
      get { throw new NotImplementedException(); }
    }

    public DomainObjectCollection Collection
    {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }

    public ReadOnlyCollectionDataDecorator GetData ()
    {
      throw new NotImplementedException();
    }

    public ReadOnlyCollectionDataDecorator GetOriginalData ()
    {
      throw new NotImplementedException();
    }

    public IDomainObjectCollectionEventRaiser GetCollectionEventRaiser ()
    {
      throw new NotImplementedException();
    }

    public DomainObjectCollection GetCollectionWithOriginalData ()
    {
      throw new NotImplementedException();
    }

    public void MarkDataComplete (DomainObject[] items)
    {
      throw new NotImplementedException();
    }

    public DomainObjectCollection OriginalCollection
    {
      get { throw new NotImplementedException(); }
    }

    public IDataManagementCommand CreateSetCollectionCommand (DomainObjectCollection newCollection)
    {
      throw new NotImplementedException();
    }

    public IDataManagementCommand CreateInsertCommand (DomainObject insertedRelatedObject, int index)
    {
      throw new NotImplementedException();
    }

    public IDataManagementCommand CreateAddCommand (DomainObject addedRelatedObject)
    {
      throw new NotImplementedException();
    }

    public IDataManagementCommand CreateReplaceCommand (int index, DomainObject replacementObject)
    {
      throw new NotImplementedException();
    }

    public void SortCurrentData (Comparison<DomainObject> comparison)
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

    public void Synchronize ()
    {
      throw new NotImplementedException();
    }

    public void SynchronizeOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      throw new NotImplementedException();
    }

    public bool CanBeCollected
    {
      get { throw new NotImplementedException(); }
    }

    public bool CanBeMarkedIncomplete
    {
      get { throw new NotImplementedException(); }
    }

    public void MarkDataIncomplete ()
    {
      throw new NotImplementedException();
    }
  }
}