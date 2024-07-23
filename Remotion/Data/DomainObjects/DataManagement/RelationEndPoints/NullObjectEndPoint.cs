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
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints
{
  /// <summary>
  /// Represents an <see cref="IObjectEndPoint"/> (with a specific <see cref="RelationEndPointDefinition"/>) for a <see langword="null"/> object.
  /// This is used by the different end point modification commands - when a bidirectional relation modification extends to a <see langword="null"/> 
  /// object, this end point (or <see cref="NullDomainObjectCollectionEndPoint"/>) is used to represent the object's part in the relation, and a 
  /// <see cref="NullEndPointModificationCommand"/> is used to represent the modification. The end point is created on the fly by 
  /// <see cref="RelationEndPointManager.GetRelationEndPointWithLazyLoad"/> and is usually discarded after it's used.
  /// </summary>
  public class NullObjectEndPoint : IObjectEndPoint
  {
    private readonly ClientTransaction _clientTransaction;
    private readonly IRelationEndPointDefinition _definition;

    public NullObjectEndPoint (ClientTransaction clientTransaction, IRelationEndPointDefinition definition)
    {
      ArgumentUtility.CheckNotNull("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull("definition", definition);

      _clientTransaction = clientTransaction;
      _definition = definition;
    }

    public IRelationEndPointDefinition Definition
    {
      get { return _definition; }
    }

    public ObjectID? ObjectID
    {
      get { return null; }
    }

    public RelationEndPointID ID
    {
      get { return RelationEndPointID.Create(null, Definition); }
    }

    public RelationDefinition RelationDefinition
    {
      get { return Definition.RelationDefinition; }
    }

    public bool IsDataComplete
    {
      get { return true; }
    }

    public ObjectID? OppositeObjectID
    {
      get { return null; }
    }

    public ObjectID OriginalOppositeObjectID
    {
      get { throw new InvalidOperationException("It is not possible to get the OriginalOppositeObjectID from a NullObjectEndPoint."); }
    }

    public bool? IsSynchronized
    {
      get { return true; }
    }

    public void Synchronize ()
    {
      // do nothing
    }

    public DomainObject? GetOppositeObject ()
    {
      return null;
    }

    public DomainObject GetOriginalOppositeObject ()
    {
      throw new InvalidOperationException("It is not possible to call GetOriginalOppositeObject on a NullObjectEndPoint.");
    }

    public bool HasChanged
    {
      get { return false; }
    }

    public bool HasBeenTouched
    {
      get { return false; }
    }

    public DomainObject? GetDomainObject ()
    {
      return null;
    }

    public DomainObject? GetDomainObjectReference ()
    {
      return null;
    }

    public ClientTransaction ClientTransaction
    {
      get { return _clientTransaction; }
    }

    public bool IsNull
    {
      get { return true; }
    }

    public void EnsureDataComplete ()
    {
      // do nothing
    }

    public void Touch ()
    {
      // do nothing
    }

    public void Commit ()
    {
      throw new InvalidOperationException("Commit cannot be called on a NullObjectEndPoint.");
    }

    public void Rollback ()
    {
      throw new InvalidOperationException("Rollback cannot be called on a NullObjectEndPoint.");
    }

    public IDataManagementCommand CreateRemoveCommand (DomainObject? removedRelatedObject)
    {
      // TODO RM-8241: removedRelatedObject can be null for null-object implementations. This is required with ObjectEndPointSetOneOneCommand.
      return new NullEndPointModificationCommand(this);
    }

    public IDataManagementCommand CreateDeleteCommand ()
    {
      return new NullEndPointModificationCommand(this);
    }

    public IDataManagementCommand CreateSetCommand (DomainObject? newRelatedObject)
    {
      return new NullEndPointModificationCommand(this);
    }

    public void ValidateMandatory ()
    {
      throw new InvalidOperationException("ValidateMandatory cannot be called on a NullObjectEndPoint.");
    }

    public RelationEndPointID GetOppositeRelationEndPointID ()
    {
      throw new InvalidOperationException("GetOppositeRelationEndPointID cannot be called on a NullObjectEndPoint.");
    }

    public IEnumerable<RelationEndPointID> GetOppositeRelationEndPointIDs ()
    {
      throw new InvalidOperationException("GetOppositeRelationEndPointIDs cannot be called on a NullObjectEndPoint.");
    }

    public void SetDataFromSubTransaction (IRelationEndPoint source)
    {
      throw new InvalidOperationException("SetDataFromSubTransaction cannot be called on a NullObjectEndPoint.");
    }
  }
}
