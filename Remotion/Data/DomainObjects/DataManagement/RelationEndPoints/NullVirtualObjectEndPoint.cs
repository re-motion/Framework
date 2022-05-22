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
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints
{
  /// <summary>
  /// Represents a <see cref="NullObjectEndPoint"/> for a virtual relation property.
  /// </summary>
  public class NullVirtualObjectEndPoint : NullObjectEndPoint, IVirtualObjectEndPoint
  {
    public NullVirtualObjectEndPoint (ClientTransaction clientTransaction, IRelationEndPointDefinition definition)
        : base(clientTransaction, definition)
    {
    }

    public bool CanBeCollected
    {
      get { return false; }
    }

    public bool CanBeMarkedIncomplete
    {
      get { return false; }
    }

    public void SynchronizeOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      throw new InvalidOperationException("A NullObjectEndPoint cannot be used to synchronize an opposite end-point.");
    }

    public void MarkDataIncomplete ()
    {
      throw new InvalidOperationException("MarkDataIncomplete cannot be called on a NullVirtualObjectEndPoint.");
    }

    public void RegisterOriginalOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      oppositeEndPoint.MarkSynchronized();
    }

    public void UnregisterOriginalOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      oppositeEndPoint.ResetSyncState();
    }

    public void RegisterCurrentOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      // Ignore
    }

    public void UnregisterCurrentOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      // Ignore
    }

    public IDomainObject? GetData ()
    {
      return null;
    }

    public IDomainObject? GetOriginalData ()
    {
      throw new InvalidOperationException("It is not possible to call GetOriginalData on a NullVirtualObjectEndPoint.");
    }

    public void MarkDataComplete (IDomainObject? item)
    {
      // Ignore
    }
  }
}
