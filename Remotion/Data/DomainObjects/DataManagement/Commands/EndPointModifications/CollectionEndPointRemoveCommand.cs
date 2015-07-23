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
  /// Represents the removal of an element from a <see cref="CollectionEndPoint"/>.
  /// </summary>
  public class CollectionEndPointRemoveCommand : RelationEndPointModificationCommand
  {
    private readonly int _index;

    private readonly IDomainObjectCollectionData _modifiedCollectionData;
    private readonly DomainObjectCollection _modifiedCollection;
    private readonly IRelationEndPointProvider _endPointProvider;

    public CollectionEndPointRemoveCommand (
        ICollectionEndPoint modifiedEndPoint, 
        DomainObject removedObject, 
        IDomainObjectCollectionData collectionData,
        IRelationEndPointProvider endPointProvider, 
        IClientTransactionEventSink transactionEventSink)
        : base (
            ArgumentUtility.CheckNotNull ("modifiedEndPoint", modifiedEndPoint),
            ArgumentUtility.CheckNotNull ("removedObject", removedObject),
            null,
            ArgumentUtility.CheckNotNull ("transactionEventSink", transactionEventSink))
    {
      ArgumentUtility.CheckNotNull ("collectionData", collectionData);
      ArgumentUtility.CheckNotNull ("endPointProvider", endPointProvider);

      if (modifiedEndPoint.IsNull)
        throw new ArgumentException ("Modified end point is null, a NullEndPointModificationCommand is needed.", "modifiedEndPoint");

      _index = modifiedEndPoint.Collection.IndexOf (removedObject);
      _modifiedCollectionData = collectionData;
      _modifiedCollection = modifiedEndPoint.Collection;
      _endPointProvider = endPointProvider;
    }

    public DomainObjectCollection ModifiedCollection
    {
      get { return _modifiedCollection; }
    }

    public IDomainObjectCollectionData ModifiedCollectionData
    {
      get { return _modifiedCollectionData; }
    }

    public IRelationEndPointProvider EndPointProvider
    {
      get { return _endPointProvider; }
    }

    public override void Begin ()
    {
      using (EnterTransactionScope())
      {
        ((IDomainObjectCollectionEventRaiser) ModifiedCollection).BeginRemove (_index, OldRelatedObject);
      }
      base.Begin ();
    }

    public override void Perform ()
    {
      ModifiedCollectionData.Remove (OldRelatedObject);
      ModifiedEndPoint.Touch ();
    }

    public override void End ()
    {
      base.End ();
      using (EnterTransactionScope())
      {
        ((IDomainObjectCollectionEventRaiser) ModifiedCollection).EndRemove (_index, OldRelatedObject);
      }
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
      var removedEndPoint = GetOppositeEndPoint (ModifiedEndPoint, OldRelatedObject, _endPointProvider);
      return new ExpandedCommand (
          removedEndPoint.CreateRemoveCommand (ModifiedEndPoint.GetDomainObject ()), 
          this);
    }
  }
}
