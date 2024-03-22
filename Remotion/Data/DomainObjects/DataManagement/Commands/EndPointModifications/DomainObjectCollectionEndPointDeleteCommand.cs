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
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications
{
  /// <summary>
  /// Represents the deletion of an object owning a <see cref="DomainObjectCollectionEndPoint"/> from the end-point's point of view.
  /// </summary>
  public class DomainObjectCollectionEndPointDeleteCommand : RelationEndPointModificationCommand
  {
    private readonly IDomainObjectCollectionData _modifiedCollectionData;
    private readonly IDomainObjectCollectionEventRaiser _modifiedCollectionEventRaiser;

    public DomainObjectCollectionEndPointDeleteCommand (
        IDomainObjectCollectionEndPoint modifiedEndPoint,
        IDomainObjectCollectionData collectionData,
        IClientTransactionEventSink transactionEventSink)
        : base(
            modifiedEndPoint,
            null,
            null,
            transactionEventSink)
    {
      _modifiedCollectionData = collectionData;
      _modifiedCollectionEventRaiser = modifiedEndPoint.GetCollectionEventRaiser();
    }

    public IDomainObjectCollectionEventRaiser ModifiedCollectionEventRaiser
    {
      get { return _modifiedCollectionEventRaiser; }
    }

    public IDomainObjectCollectionData ModifiedCollectionData
    {
      get { return _modifiedCollectionData; }
    }

    public override void Begin ()
    {
      // do not call base - no transaction notification

      using (EnterTransactionScope())
      {
        ModifiedCollectionEventRaiser.BeginDelete();
      }
    }

    public override void Perform ()
    {
      ModifiedCollectionData.Clear();
      ModifiedEndPoint.Touch();
    }

    public override void End ()
    {
      // do not call base - no transaction notification

      using (EnterTransactionScope())
      {
        ModifiedCollectionEventRaiser.EndDelete();
      }
    }

    public override ExpandedCommand ExpandToAllRelatedObjects ()
    {
      return new ExpandedCommand(this);
    }
  }
}
