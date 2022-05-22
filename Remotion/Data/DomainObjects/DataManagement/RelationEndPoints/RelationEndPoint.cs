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
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints
{
  /// <summary>
  /// Provides an abstract base implementation of non-transient relation end points that can be stored in the <see cref="RelationEndPointManager"/>.
  /// </summary>
  public abstract class RelationEndPoint : IRelationEndPoint
  {
    private readonly ClientTransaction _clientTransaction;
    private readonly RelationEndPointID _id;

    protected RelationEndPoint (ClientTransaction clientTransaction, RelationEndPointID id)
    {
      ArgumentUtility.CheckNotNull("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull("id", id);

      if (id.Definition.IsAnonymous)
        throw new ArgumentException("End point ID must not refer to an anonymous end point.", "id");

      _clientTransaction = clientTransaction;
      _id = id;
    }

    public abstract bool HasChanged { get; }
    public abstract bool HasBeenTouched { get; }

    public abstract bool IsDataComplete { get; }
    public abstract bool? IsSynchronized { get; }

    public abstract void EnsureDataComplete ();
    public abstract void Synchronize ();

    public abstract void Touch ();
    public abstract IEnumerable<RelationEndPointID> GetOppositeRelationEndPointIDs ();

    public abstract void Commit ();
    public abstract void Rollback ();
    public abstract void SetDataFromSubTransaction (IRelationEndPoint source);

    public abstract void ValidateMandatory ();

    public abstract IDataManagementCommand CreateRemoveCommand (IDomainObject removedRelatedObject);
    public abstract IDataManagementCommand CreateDeleteCommand ();

    public ClientTransaction ClientTransaction
    {
      get { return _clientTransaction; }
    }

    public RelationEndPointID ID
    {
      get { return _id; }
    }

    public ObjectID? ObjectID
    {
      get { return _id.ObjectID; }
    }

    public IRelationEndPointDefinition Definition
    {
      get { return ID.Definition; }
    }

    public string PropertyName
    {
      get
      {
        Assertion.DebugAssert(!Definition.IsAnonymous, "!Definition.IsAnonymous");
        return Definition.PropertyName;
      }
    }

    public RelationDefinition RelationDefinition
    {
      get { return Definition.RelationDefinition; }
    }

    public bool IsVirtual
    {
      get { return Definition.IsVirtual; }
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }

    public IDomainObject? GetDomainObject ()
    {
      //TODO RM-8241: possible null bug, non-null RelationEndPoint could probably require non-null ObjectID during construction.
      if (ObjectID == null)
        return null;

      if (ClientTransaction.IsInvalid(ObjectID))
        return ClientTransaction.GetInvalidObjectReference(ObjectID);

      return ClientTransaction.GetObject(ObjectID, true);
    }

    public IDomainObject? GetDomainObjectReference ()
    {
      //TODO RM-8241: possible null bug, non-null RelationEndPoint could probably require non-null ObjectID during construction.
      if (ObjectID == null)
        return null;

      if (ClientTransaction.IsInvalid(ObjectID))
        return ClientTransaction.GetInvalidObjectReference(ObjectID);

      return ClientTransaction.GetObjectReference(ObjectID);
    }

    public override string ToString ()
    {
      return GetType().Name + ": " + ID;
    }

    #region Serialization

    protected RelationEndPoint (FlattenedDeserializationInfo info)
    {
      _clientTransaction = info.GetValueForHandle<ClientTransaction>();
      _id = info.GetValue<RelationEndPointID>();
    }

    protected abstract void SerializeIntoFlatStructure (FlattenedSerializationInfo info);

    void IFlattenedSerializable.SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      info.AddHandle(_clientTransaction);
      info.AddValue(_id);

      SerializeIntoFlatStructure(info);
    }

    #endregion
  }
}
