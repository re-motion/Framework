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
  /// Represents the insertion of an element into a <see cref="DomainObjectCollectionEndPoint"/>.
  /// </summary>
  public class DomainObjectCollectionEndPointInsertCommand : RelationEndPointModificationCommand
  {
    private readonly int _index;
    private readonly IDomainObjectCollectionData _modifiedCollectionData;
    private readonly IRelationEndPointProvider _endPointProvider;

    private readonly IDomainObjectCollectionEventRaiser _modifiedCollectionEventRaiser;

    public DomainObjectCollectionEndPointInsertCommand (
        IDomainObjectCollectionEndPoint modifiedEndPoint,
        int index,
        IDomainObject insertedObject,
        IDomainObjectCollectionData collectionData,
        IRelationEndPointProvider endPointProvider,
        IClientTransactionEventSink transactionEventSink)
        : base(
            modifiedEndPoint,
            null,
            ArgumentUtility.CheckNotNull("insertedObject", insertedObject),
            transactionEventSink)
    {
      ArgumentUtility.CheckNotNull("collectionData", collectionData);
      ArgumentUtility.CheckNotNull("endPointProvider", endPointProvider);

      _index = index;
      _modifiedCollectionData = collectionData;
      _endPointProvider = endPointProvider;
      _modifiedCollectionEventRaiser = modifiedEndPoint.GetCollectionEventRaiser();
    }

    public int Index
    {
      get { return _index; }
    }

    public IDomainObjectCollectionEventRaiser ModifiedCollectionEventRaiser
    {
      get { return _modifiedCollectionEventRaiser; }
    }

    public IDomainObjectCollectionData ModifiedCollectionData
    {
      get { return _modifiedCollectionData; }
    }

    public IRelationEndPointProvider EndPointProvider
    {
      get { return _endPointProvider; }
    }

    public new IDomainObject NewRelatedObject
    {
      get { return base.NewRelatedObject!; }
    }

    public override void Begin ()
    {
      using (EnterTransactionScope())
      {
        ModifiedCollectionEventRaiser.BeginAdd(Index, NewRelatedObject);
      }
      base.Begin();
    }

    public override void Perform ()
    {
      ModifiedCollectionData.Insert(Index, NewRelatedObject);
      ModifiedEndPoint.Touch();
    }

    public override void End ()
    {
      base.End();
      using (EnterTransactionScope())
      {
        ModifiedCollectionEventRaiser.EndAdd(Index, NewRelatedObject);
      }
    }

    /// <summary>
    /// Creates all commands needed to perform a bidirectional insert operation into this collection end point.
    /// </summary>
    /// <remarks>
    /// An insert operation of the form "customer.Orders.Insert (insertedOrder, index)" needs three steps:
    /// <list type="bullet">
    ///   <item>insertedOrder.Customer = customer,</item>
    ///   <item>customer.Orders.Insert (insertedOrder, index), and</item>
    ///   <item>oldCustomer.Orders.Remove (insertedOrder) - with oldCustomer being the old customer of the inserted order (if non-null).</item>
    /// </list>
    /// </remarks>
    public override ExpandedCommand ExpandToAllRelatedObjects ()
    {
      // the end point that will be linked to the collection end point after the operation
      var insertedObjectEndPoint = (IRealObjectEndPoint)GetOppositeEndPoint(ModifiedEndPoint, NewRelatedObject, _endPointProvider);
      // the object that was linked to the new related object before the operation
      var oldRelatedObjectOfInsertedObject = insertedObjectEndPoint.GetOppositeObject();
      // the end point that was linked to the new related object before the operation
      var oldRelatedEndPointOfInsertedObject = GetOppositeEndPoint(insertedObjectEndPoint, oldRelatedObjectOfInsertedObject, _endPointProvider);

      return new ExpandedCommand(
          // insertedOrder.Customer = customer (previously oldCustomer)
          insertedObjectEndPoint.CreateSetCommand(DomainObject),
          // customer.Orders.Insert (insertedOrder, index)
          this,
          // oldCustomer.Orders.Remove (insertedOrder)
          oldRelatedEndPointOfInsertedObject.CreateRemoveCommand(NewRelatedObject));
    }
  }
}
