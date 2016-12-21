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
  /// Represents a <see cref="NullObjectEndPoint"/> for a non-virtual relation property.
  /// </summary>
  public class NullRealObjectEndPoint : NullObjectEndPoint, IRealObjectEndPoint
  {
    public NullRealObjectEndPoint (ClientTransaction clientTransaction, IRelationEndPointDefinition definition)
        : base(clientTransaction, definition)
    {
    }

    public DataContainer ForeignKeyDataContainer
    {
      get { throw new NotSupportedException (); }
    }

    public PropertyDefinition PropertyDefinition
    {
      get { throw new NotSupportedException (); }
    }

    public void MarkSynchronized ()
    {
      // Do nothing
    }

    public void MarkUnsynchronized ()
    {
      // Do nothing
    }

    public void ResetSyncState ()
    {
      // Do nothing
    }
  }
}