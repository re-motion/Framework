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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.SerializableFakes
{
  public class SerializableRealObjectEndPointFake : IRealObjectEndPoint
  {
    private readonly RelationEndPointID _id;
    private readonly DomainObject _owningObject;

    public SerializableRealObjectEndPointFake (RelationEndPointID id, DomainObject owningObject)
    {
      _id = id;
      _owningObject = owningObject;
    }

    public SerializableRealObjectEndPointFake (FlattenedDeserializationInfo info)
    {
      _owningObject = info.GetValue<DomainObject>();
      _id = info.GetValue<RelationEndPointID>();
    }

    public bool IsNull
    {
      get { throw new NotImplementedException(); }
    }

    public void SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      info.AddValue (_owningObject);
      info.AddValue (_id);
    }

    public RelationEndPointID ID
    {
      get { return _id; }
    }

    public ClientTransaction ClientTransaction
    {
      get { throw new NotImplementedException(); }
    }

    public ObjectID ObjectID
    {
      get { return _owningObject.ID; }
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
      return _owningObject;
    }

    public DomainObject GetDomainObjectReference ()
    {
      return _owningObject;
    }

    public void EnsureDataComplete ()
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

    public ObjectID OppositeObjectID
    {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }

    public ObjectID OriginalOppositeObjectID
    {
      get { throw new NotImplementedException(); }
    }

    public bool? IsSynchronized
    {
      get { throw new NotImplementedException(); }
    }

    public void MarkSynchronized ()
    {
    }

    public void MarkUnsynchronized ()
    {
      throw new NotImplementedException();
    }

    public void ResetSyncState ()
    {
   }

    public DataContainer ForeignKeyDataContainer
    {
      get { throw new NotImplementedException(); }
    }

    public PropertyDefinition PropertyDefinition
    {
      get { throw new NotImplementedException(); }
    }

    public void Synchronize (IRelationEndPoint oppositeEndPoint)
    {
      throw new NotImplementedException();
    }

    public DomainObject GetOppositeObject ()
    {
      throw new NotImplementedException();
    }

    public DomainObject GetOriginalOppositeObject ()
    {
      throw new NotImplementedException();
    }

    public IDataManagementCommand CreateSetCommand (DomainObject newRelatedObject)
    {
      throw new NotImplementedException();
    }

    public RelationEndPointID GetOppositeRelationEndPointID ()
    {
      throw new NotImplementedException();
    }
  }
}