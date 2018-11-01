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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications
{
  /// <summary>
  /// Represents the deletion of an object owning a <see cref="ObjectEndPoint"/> from the end-point's point of view.
  /// </summary>
  public class ObjectEndPointDeleteCommand : RelationEndPointModificationCommand
  {
    private readonly Action _oppositeObjectNullSetter;

    public ObjectEndPointDeleteCommand (
        IObjectEndPoint modifiedEndPoint, Action oppositeObjectNullSetter, IClientTransactionEventSink transactionEventSink)
        : base (
            ArgumentUtility.CheckNotNull ("modifiedEndPoint", modifiedEndPoint),
            null,
            null,
            ArgumentUtility.CheckNotNull ("transactionEventSink", transactionEventSink))
    {
      ArgumentUtility.CheckNotNull ("oppositeObjectNullSetter", oppositeObjectNullSetter);

      _oppositeObjectNullSetter = oppositeObjectNullSetter;
      if (modifiedEndPoint.IsNull)
        throw new ArgumentException ("Modified end point is null, a NullEndPointModificationCommand is needed.", "modifiedEndPoint");
    }

    public override void Begin ()
    {
      // no notification
    }

    public override void Perform ()
    {
      _oppositeObjectNullSetter();
      ModifiedEndPoint.Touch();
    }

    public override void End ()
    {
      // no notification
    }

    public override ExpandedCommand ExpandToAllRelatedObjects ()
    {
      return new ExpandedCommand (this);
    }
  }
}
