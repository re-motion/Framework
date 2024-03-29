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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications
{
  /// <summary>
  /// Represents the removal of an element from a <see cref="DomainObjectCollectionEndPoint"/>.
  /// </summary>
  public class VirtualCollectionEndPointRemoveCommand : RelationEndPointModificationCommand
  {
    private readonly int _index;

    private readonly IVirtualCollectionData _modifiedCollectionData;
    private readonly IRelationEndPointProvider _endPointProvider;

    public VirtualCollectionEndPointRemoveCommand (
        IVirtualCollectionEndPoint modifiedEndPoint,
        DomainObject removedObject,
        IVirtualCollectionData collectionData,
        IRelationEndPointProvider endPointProvider,
        IClientTransactionEventSink transactionEventSink)
        : base(
            modifiedEndPoint,
            ArgumentUtility.CheckNotNull("removedObject", removedObject),
            null,
            transactionEventSink)
    {
      ArgumentUtility.CheckNotNull("collectionData", collectionData);
      ArgumentUtility.CheckNotNull("endPointProvider", endPointProvider);

      _index = collectionData.IsDataComplete ? collectionData.IndexOf(removedObject.ID) : 0;
      _modifiedCollectionData = collectionData;
      _endPointProvider = endPointProvider;
    }

    public IVirtualCollectionData ModifiedCollectionData
    {
      get { return _modifiedCollectionData; }
    }

    public IRelationEndPointProvider EndPointProvider
    {
      get { return _endPointProvider; }
    }

    public new DomainObject OldRelatedObject
    {
      get { return base.OldRelatedObject!; }
    }

    public override void Perform ()
    {
      ModifiedCollectionData.Remove(OldRelatedObject);
      ModifiedEndPoint.Touch();
    }

    /// <summary>
    /// Creates all commands needed to perform a bidirectional remove operation from this collection end point.
    /// </summary>
    /// <remarks>
    /// A remove operation of the form "customer.Orders.Remove (RemovedOrder)" needs two steps:
    /// <list type="bullet">
    ///   <item>RemovedOrder.Customer = null and</item>
    ///   <item>customer.Orders.Remove (removedOrder).</item>
    /// </list>
    /// </remarks>
    public override ExpandedCommand ExpandToAllRelatedObjects ()
    {
      var removedEndPoint = GetOppositeEndPoint(ModifiedEndPoint, OldRelatedObject, _endPointProvider);
      var removedRelatedObject = DomainObject;

      return new ExpandedCommand(
          removedEndPoint.CreateRemoveCommand(removedRelatedObject),
          this);
    }
  }
}
