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
  /// Represents the adding of an element into a <see cref="DomainObjectCollectionEndPoint"/>.
  /// </summary>
  public class VirtualCollectionEndPointAddCommand : RelationEndPointModificationCommand
  {
    private readonly IVirtualCollectionData _modifiedCollectionData;
    private readonly IRelationEndPointProvider _endPointProvider;

    private readonly IDomainObjectCollectionEventRaiser _modifiedCollectionEventRaiser;

    public VirtualCollectionEndPointAddCommand (
        IVirtualCollectionEndPoint modifiedEndPoint,
        DomainObject insertedObject,
        IVirtualCollectionData collectionData,
        IRelationEndPointProvider endPointProvider,
        IClientTransactionEventSink transactionEventSink)
        : base (
            ArgumentUtility.CheckNotNull ("modifiedEndPoint", modifiedEndPoint),
            null,
            ArgumentUtility.CheckNotNull ("insertedObject", insertedObject),
            ArgumentUtility.CheckNotNull ("transactionEventSink", transactionEventSink))
    {
      ArgumentUtility.CheckNotNull ("collectionData", collectionData);
      ArgumentUtility.CheckNotNull ("endPointProvider", endPointProvider);

      if (modifiedEndPoint.IsNull)
        throw new ArgumentException ("Modified end point is null, a NullEndPointModificationCommand is needed.", "modifiedEndPoint");

      _modifiedCollectionData = collectionData;
      _endPointProvider = endPointProvider;
      _modifiedCollectionEventRaiser = modifiedEndPoint.GetCollectionEventRaiser();
    }

    public IDomainObjectCollectionEventRaiser ModifiedCollectionEventRaiser
    {
      get { return _modifiedCollectionEventRaiser; }
    }

    public IVirtualCollectionData ModifiedCollectionData
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
        ModifiedCollectionEventRaiser.BeginAdd (ModifiedCollectionData.Count, NewRelatedObject);
      }
      base.Begin ();
    }

    public override void Perform ()
    {
      ModifiedCollectionData.Add (NewRelatedObject);
      ModifiedEndPoint.Touch();
    }

    public override void End ()
    {
      base.End ();
      using (EnterTransactionScope())
      {
        ModifiedCollectionEventRaiser.EndAdd (ModifiedCollectionData.Count, NewRelatedObject);
      }
    }

    /// <summary>
    /// Creates all commands needed to perform a bidirectional add operation into this collection end point.
    /// </summary>
    /// <remarks>
    /// An add operation of the form "customer.Orders.Add (addedOrder)" needs three steps:
    /// <list type="bullet">
    ///   <item>addedOrder.Customer = customer,</item>
    ///   <item>customer.Orders.Add (addedOrder), and</item>
    ///   <item>oldCustomer.Orders.Remove (addedOrder) - with oldCustomer being the old customer of the added order (if non-null).</item>
    /// </list>
    /// </remarks>
    public override ExpandedCommand ExpandToAllRelatedObjects ()
    {
      // the end point that will be linked to the collection end point after the operation
      var insertedObjectEndPoint = (IRealObjectEndPoint) GetOppositeEndPoint (ModifiedEndPoint, NewRelatedObject, _endPointProvider);
      // the object that was linked to the new related object before the operation
      var oldRelatedObjectOfInsertedObject = insertedObjectEndPoint.GetOppositeObject ();
      // the end point that was linked to the new related object before the operation
      var oldRelatedEndPointOfInsertedObject = GetOppositeEndPoint (insertedObjectEndPoint, oldRelatedObjectOfInsertedObject, _endPointProvider);

      return new ExpandedCommand (
          // addedOrder.Customer = customer (previously oldCustomer)
          insertedObjectEndPoint.CreateSetCommand (ModifiedEndPoint.GetDomainObject ()),
          // customer.Orders.Add (addedOrder)
          this,
          // oldCustomer.Orders.Remove (addedOrder)
          oldRelatedEndPointOfInsertedObject.CreateRemoveCommand (NewRelatedObject));
    }
  }
}
