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
  /// Represents the adding of an element into a <see cref="VirtualCollectionEndPoint"/>.
  /// </summary>
  public class VirtualCollectionEndPointAddCommand : RelationEndPointModificationCommand
  {
    private readonly int _index;

    private readonly IVirtualCollectionData _modifiedCollectionData;
    private readonly IDomainObjectCollectionEventRaiser _modifiedCollectionEventRaiser;
    private readonly IRelationEndPointProvider _endPointProvider;

    public VirtualCollectionEndPointAddCommand (
        IVirtualCollectionEndPoint modifiedEndPoint,
        DomainObject addedObject,
        IVirtualCollectionData collectionData,
        IRelationEndPointProvider endPointProvider,
        IClientTransactionEventSink transactionEventSink)
        : base (
            ArgumentUtility.CheckNotNull ("modifiedEndPoint", modifiedEndPoint),
            null,
            ArgumentUtility.CheckNotNull ("addedObject", addedObject),
            ArgumentUtility.CheckNotNull ("transactionEventSink", transactionEventSink))
    {
      ArgumentUtility.CheckNotNull ("collectionData", collectionData);
      ArgumentUtility.CheckNotNull ("endPointProvider", endPointProvider);

      if (modifiedEndPoint.IsNull)
        throw new ArgumentException ("Modified end point is null, a NullEndPointModificationCommand is needed.", "modifiedEndPoint");

      _index = collectionData.Count;
      _modifiedCollectionData = collectionData;
      //TODO: RM-7294: API is only implemented because of the interface on VirtualObjectList. Can probably be dropped since VirtualObjectList has no usage for it.
      _modifiedCollectionEventRaiser = modifiedEndPoint.GetCollectionEventRaiser();
      _endPointProvider = endPointProvider;
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
        ModifiedCollectionEventRaiser.BeginAdd (_index, NewRelatedObject);
      }

      base.Begin();
    }

    public override void Perform ()
    {
      ModifiedCollectionData.Add (NewRelatedObject);
      ModifiedEndPoint.Touch();
    }

    public override void End ()
    {
      base.End();
      using (EnterTransactionScope())
      {
        ModifiedCollectionEventRaiser.EndAdd (_index, NewRelatedObject);
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
      //TODO: RM-7294: Do not allow setting an object that is still part of another collection;
      //TBD: We're just updating a parent property. Requiring that we first set the parent property to null seems strange.
      //     This concept should be dropped.

      // the end point that will be linked to the collection end point after the operation
      var addedObjectEndPoint = (IRealObjectEndPoint) GetOppositeEndPoint (ModifiedEndPoint, NewRelatedObject, _endPointProvider);
      // the object that was linked to the new related object before the operation
      var oldRelatedObjectOfAddedObject = addedObjectEndPoint.GetOppositeObject();
      // the end point that was linked to the new related object before the operation
      var oldRelatedEndPointOfAddedObject = GetOppositeEndPoint (addedObjectEndPoint, oldRelatedObjectOfAddedObject, _endPointProvider);

      return new ExpandedCommand (
          // addedOrder.Customer = customer (previously oldCustomer)
          addedObjectEndPoint.CreateSetCommand (ModifiedEndPoint.GetDomainObject()),
          // customer.Orders.Add (addedOrder)
          this,
          // oldCustomer.Orders.Remove (addedOrder)
          oldRelatedEndPointOfAddedObject.CreateRemoveCommand (NewRelatedObject));
    }
  }
}
