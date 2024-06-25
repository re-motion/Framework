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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints
{
  /// <summary>
  /// Implements the <see cref="IVirtualEndPointStateUpdateListener"/> interface by passing information about state updates on to a 
  /// <see cref="ClientTransaction"/>.
  /// </summary>
  public class VirtualEndPointStateUpdateListener : IVirtualEndPointStateUpdateListener
  {
    private readonly IClientTransactionEventSink _transactionEventSink;

    public VirtualEndPointStateUpdateListener (IClientTransactionEventSink transactionEventSink)
    {
      ArgumentUtility.CheckNotNull("transactionEventSink", transactionEventSink);
      _transactionEventSink = transactionEventSink;
    }

    public IClientTransactionEventSink TransactionEventSink
    {
      get { return _transactionEventSink; }
    }

    public void VirtualEndPointStateUpdated (RelationEndPointID endPointID, bool? newChangedState)
    {
      ArgumentUtility.CheckNotNull("endPointID", endPointID);
      _transactionEventSink.RaiseVirtualRelationEndPointStateUpdatedEvent(endPointID, newChangedState);
    }
  }
}
