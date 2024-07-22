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
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.Commands;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DomainImplementation
{
  /// <summary>
  /// Provides functionality for unloading the data that a <see cref="ClientTransaction"/> stores for <see cref="DomainObject"/> instances and for
  /// relations. Use the methods of this class to remove unneeded data from memory and, more importantly, to reload data from the underlying 
  /// data source.
  /// </summary>
  public static class UnloadService
  {
    /// <summary>
    /// Unloads the virtual relation end point indicated by the given <see cref="RelationEndPointID"/> in the specified
    /// <see cref="ClientTransaction"/>. If the end point has not been loaded or has already been unloaded, this method does nothing.
    /// The relation must be unchanged in order to be unloaded, and it must not belong to an object that is new or deleted.
    /// </summary>
    /// <param name="clientTransaction">The <see cref="ClientTransaction"/> to unload the data from. The unload operation always affects the whole transaction 
    /// hierarchy.</param>
    /// <param name="endPointID">The ID of the relation property to unload. This must denote a virtual relation end-point, ie., the relation side not 
    /// holding the foreign key property.</param>
    /// <exception cref="InvalidOperationException">The given end point is not in unchanged state.</exception>
    /// <exception cref="ArgumentNullException">One of the arguments passed to this method is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">The given <paramref name="endPointID"/> does not specify a virtual relation end point.</exception>
    /// <remarks>
    /// <para>
    /// The unload operation is atomic over the transaction hierarchy. If the operation cannot be performed or is canceled in any of the transactions,
    /// it will stop before any data is unloaded.
    /// </para>
    /// </remarks>
    public static void UnloadVirtualEndPoint (ClientTransaction clientTransaction, RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull("endPointID", endPointID);

      CheckVirtualEndPointID(endPointID);

      Func<ClientTransaction, IDataManagementCommand> commandFactory = tx => tx.DataManager.CreateUnloadVirtualEndPointsCommand(endPointID);
      var executor = new TransactionHierarchyCommandExecutor(commandFactory);
      executor.ExecuteCommandForTransactionHierarchy(clientTransaction);
    }

    /// <summary>
    /// Tries to unload the virtual end point indicated by the given <see cref="RelationEndPointID"/> in the specified
    /// <see cref="ClientTransaction"/>, returning a value indicating whether the unload operation succeeded. If the end point has not been loaded or
    /// has already been unloaded, this method returns <see langword="true" /> and does nothing.
    /// The relation must be unchanged in order to be unloaded, and it must not belong to an object that is new or deleted, otherwise this method 
    /// returns <see langword="false" />.
    /// </summary>
    /// <param name="clientTransaction">The <see cref="ClientTransaction"/> to unload the data from. The unload operation always affects the whole transaction 
    /// hierarchy.</param>
    /// <param name="endPointID">The ID of the relation property to unload. This must denote a virtual relation end-point, ie., the relation side not 
    /// holding the foreign key property.</param>
    /// <returns><see langword="true" /> if the unload operation succeeded (in all transactions), or <see langword="false" /> if it did not succeed
    /// (in one transaction).</returns>
    /// <exception cref="ArgumentNullException">One of the arguments passed to this method is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">The given <paramref name="endPointID"/> does not specify a virtual relation end point.</exception>
    /// <remarks>
    /// <para>
    /// If a <see cref="DomainObject.OnUnloading"/>, <see cref="IClientTransactionExtension.ObjectsUnloading"/>, or similar handler throws an 
    /// exception to cancel the operation, that exception is propagated to the caller (rather than returning <see langword="false" />).
    /// </para>
    /// <para>
    /// The unload operation is atomic over the transaction hierarchy. If the operation cannot be performed or is canceled in any of the transactions,
    /// it will stop before any data is unloaded.
    /// </para>
    /// </remarks>
    public static bool TryUnloadVirtualEndPoint (ClientTransaction clientTransaction, RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull("endPointID", endPointID);

      CheckVirtualEndPointID(endPointID);

      Func<ClientTransaction, IDataManagementCommand> commandFactory = tx => tx.DataManager.CreateUnloadVirtualEndPointsCommand(endPointID);
      var executor = new TransactionHierarchyCommandExecutor(commandFactory);
      return executor.TryExecuteCommandForTransactionHierarchy(clientTransaction);
    }

    /// <summary>
    /// Unloads the data held by the given <see cref="ClientTransaction"/> for the <see cref="DomainObject"/> with the specified 
    /// <paramref name="objectID"/>. The <see cref="DomainObject"/> reference 
    /// and <see cref="DomainObjectCollection"/> instances held by the object are not removed, only the data is. The object can only be unloaded if 
    /// it is in unchanged state and no relation end-points would remain inconsistent.
    /// </summary>
    /// <param name="clientTransaction">The <see cref="ClientTransaction"/> to unload the data from. The unload operation always affects the whole transaction 
    /// hierarchy.</param>
    /// <param name="objectID">The object ID.</param>
    /// <exception cref="InvalidOperationException">The object to be unloaded is not in unchanged state - or - the operation would affect an 
    /// opposite relation end-point that is not in unchanged state.</exception>
    /// <remarks>
    /// <para>
    /// The method unloads the <see cref="DataContainer"/>, the collection end points the object is part of (but not
    /// the collection end points the object owns), the non-virtual end points owned by the object and their respective opposite virtual object 
    /// end-points. This means that unloading an object will unload a relation if and only if the object's <see cref="DataContainer"/> is holding 
    /// the foreign key for the relation. Use <see cref="UnloadVirtualEndPoint"/> or <see cref="UnloadVirtualEndPointAndItemData"/> to unload 
    /// relations whose foreign keys are not held by the object.
    /// </para>
    /// <para>
    /// The unload operation is atomic over the transaction hierarchy. If the operation cannot be performed or is canceled in any of the transactions,
    /// it will stop before any data is unloaded.
    /// </para>
    /// </remarks>
    public static void UnloadData (ClientTransaction clientTransaction, ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull("objectID", objectID);

      Func<ClientTransaction, IDataManagementCommand> commandFactory = tx => tx.DataManager.CreateUnloadCommand(objectID);
      var executor = new TransactionHierarchyCommandExecutor(commandFactory);
      executor.ExecuteCommandForTransactionHierarchy(clientTransaction);
    }

    /// <summary>
    /// Unloads the data held by the given <see cref="ClientTransaction"/> for the <see cref="DomainObject"/> with the specified
    /// <paramref name="objectID"/>, returning a value indicating whether the unload operation succeeded. The <see cref="DomainObject"/> reference
    /// and <see cref="DomainObjectCollection"/> instances held by the object are not removed, only the data is. The object can only be unloaded if
    /// it is in unchanged state and no relation end-points would remain inconsistent.
    /// </summary>
    /// <param name="clientTransaction">The <see cref="ClientTransaction"/> to unload the data from. The unload operation always affects the whole transaction 
    /// hierarchy.</param>
    /// <param name="objectID">The object ID.</param>
    /// <returns><see langword="true" /> if the unload operation succeeded (in all transactions), or <see langword="false" /> if it did not succeed
    /// (in one transaction).</returns>
    /// <remarks>
    /// <para>
    /// The method unloads the <see cref="DataContainer"/>, the collection end points the object is part of (but not
    /// the collection end points the object owns), the non-virtual end points owned by the object and their respective opposite virtual object 
    /// end-points. This means that unloading an object will unload a relation if and only if the object's <see cref="DataContainer"/> is holding 
    /// the foreign key for the relation. Use <see cref="TryUnloadVirtualEndPoint"/> or <see cref="TryUnloadVirtualEndPointAndItemData"/> to unload 
    /// relations whose foreign keys are not held by the object.
    /// </para>
    /// <para>
    /// If a <see cref="DomainObject.OnUnloading"/>, <see cref="IClientTransactionExtension.ObjectsUnloading"/>, or similar handler throws an 
    /// exception to cancel the operation, that exception is propagated to the caller (rather than returning <see langword="false" />).
    /// </para>
    /// <para>
    /// The unload operation is atomic over the transaction hierarchy. If the operation cannot be performed or is canceled in any of the transactions,
    /// it will stop before any data is unloaded.
    /// </para>
    /// </remarks>
    public static bool TryUnloadData (ClientTransaction clientTransaction, ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull("objectID", objectID);

      Func<ClientTransaction, IDataManagementCommand> commandFactory = tx => tx.DataManager.CreateUnloadCommand(objectID);
      var executor = new TransactionHierarchyCommandExecutor(commandFactory);
      return executor.TryExecuteCommandForTransactionHierarchy(clientTransaction);
    }

    /// <summary>
    /// Unloads the virtual end point indicated by the given <see cref="RelationEndPointID"/> in the specified 
    /// <see cref="ClientTransaction"/> as well as the data of the items referenced by it. If the end point has not been loaded or has already been 
    /// unloaded, this method does nothing.
    /// The relation end-point must be unchanged in order to be unloaded, and it must not belong to an object that is new or deleted.
    /// </summary>
    /// <param name="clientTransaction">The <see cref="ClientTransaction"/> to unload the data from. The unload operation always affects the whole transaction 
    /// hierarchy.</param>
    /// <param name="endPointID">The end point ID. In order to retrieve this ID from a <see cref="DomainObjectCollection"/> representing a relation
    /// end point, specify the <see cref="DomainObjectCollection.AssociatedEndPointID"/>.</param>
    /// <exception cref="InvalidOperationException">The involved end points or one of the items it stores are not in unchanged state.</exception>
    /// <exception cref="ArgumentNullException">One of the arguments passed to this method is <see langword="null" />.</exception>
    /// <exception cref="ArgumentException">The given <paramref name="endPointID"/> does not specify a collection end point.</exception>
    /// <remarks>
    /// <para>
    /// The unload operation is atomic over the transaction hierarchy. If the operation cannot be performed or is canceled in any of the transactions,
    /// it will stop before any data is unloaded.
    /// </para>    
    /// </remarks>
    public static void UnloadVirtualEndPointAndItemData (ClientTransaction clientTransaction, RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull("endPointID", endPointID);

      CheckVirtualEndPointID(endPointID);

      Func<ClientTransaction, IDataManagementCommand> commandFactory = tx => CreateUnloadVirtualEndPointAndItemDataCommand(tx, endPointID);
      var executor = new TransactionHierarchyCommandExecutor(commandFactory);
      executor.ExecuteCommandForTransactionHierarchy(clientTransaction);
    }

    /// <summary>
    /// Unloads the virtual end point indicated by the given <see cref="RelationEndPointID"/> in the specified
    /// <see cref="ClientTransaction"/> as well as the data of the items referenced by it, returning a value indicating whether the unload operation 
    /// succeeded.
    /// If the end point has not been loaded or has already been unloaded, this method returns <see langword="true" /> and does nothing.
    /// The relation end-point must be unchanged in order to be unloaded, and it must not belong to an object that is new or deleted, otherwise this
    /// method will return <see langword="false" />.
    /// </summary>
    /// <param name="clientTransaction">The <see cref="ClientTransaction"/> to unload the data from. The unload operation always affects the whole transaction 
    /// hierarchy.</param>
    /// <param name="endPointID">The end point ID. In order to retrieve this ID from a <see cref="DomainObjectCollection"/> representing a relation
    /// end point, specify the <see cref="DomainObjectCollection.AssociatedEndPointID"/>.</param>
    /// <returns><see langword="true" /> if the unload operation succeeded (in all transactions), or <see langword="false" /> if it did not succeed
    /// (in one transaction).</returns>
    /// <exception cref="ArgumentNullException">One of the arguments passed to this method is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">The given <paramref name="endPointID"/> does not specify a collection end point.</exception>
    /// <remarks>
    /// <para>
    /// If a <see cref="DomainObject.OnUnloading"/>, <see cref="IClientTransactionExtension.ObjectsUnloading"/>, or similar handler throws an 
    /// exception to cancel the operation, that exception is propagated to the caller (rather than returning <see langword="false" />).
    /// </para>
    /// <para>
    /// The unload operation is atomic over the transaction hierarchy. If the operation cannot be performed or is canceled in any of the transactions,
    /// it will stop before any data is unloaded.
    /// </para>
    /// </remarks>
    public static bool TryUnloadVirtualEndPointAndItemData (ClientTransaction clientTransaction, RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull("endPointID", endPointID);

      CheckVirtualEndPointID(endPointID);

      Func<ClientTransaction, IDataManagementCommand> commandFactory = tx => CreateUnloadVirtualEndPointAndItemDataCommand(tx, endPointID);
      var executor = new TransactionHierarchyCommandExecutor(commandFactory);
      return executor.TryExecuteCommandForTransactionHierarchy(clientTransaction);
    }

    /// <summary>
    /// Unloads all the data and relation end-points from the <see cref="ClientTransaction"/> hierarchy indicated by the given 
    /// <paramref name="clientTransaction"/>. This operation always succeeds (unless it is canceled by an exception thrown from a
    /// <see cref="IClientTransactionListener.ObjectsUnloading"/> or <see cref="DomainObject.OnUnloading"/> notification method).
    /// </summary>
    /// <param name="clientTransaction">The <see cref="ClientTransaction"/> to unload the data from. The unload operation always affects the whole transaction 
    /// hierarchy.</param>
    /// <exception cref="ArgumentNullException">One of the arguments passed to this method is <see langword="null"/>.</exception>
    /// <remarks>
    /// <para>
    /// The unload operation is atomic over the transaction hierarchy. If the operation is canceled in any of the transactions,
    /// it will not unload any data.
    /// </para>
    /// <para>
    /// The effect of this operation is similar to that of a <see cref="ClientTransaction.Rollback"/> followed by calling 
    /// <see cref="UnloadVirtualEndPoint"/> and <see cref="UnloadData"/> for every piece of data in the <see cref="ClientTransaction"/>, although
    /// the operation won't raise any Rollback-related events. 
    /// </para>
    /// <para>
    /// When the operation completes, the objects that have the <see cref="DomainObject.State"/>.<see cref="DomainObjectState.IsChanged"/>,
    /// <see cref="DomainObject.State"/>.<see cref="DomainObjectState.IsDeleted"/>, or <see cref="DomainObject.State"/>.<see cref="DomainObjectState.IsUnchanged"/> flag set,
    /// are updated to have the <see cref="DomainObject.State"/>.<see cref="DomainObjectState.IsNotLoadedYet"/> flag set instead. Objects that have the
    ///<see cref="DomainObject.State"/>.<see cref="DomainObjectState.IsNew"/> flag set, are changed to have the
    /// <see cref="DomainObject.State"/>.<see cref="DomainObjectState.IsInvalid"/> flag set (this state is propagated over within the whole transaction hierarchy). ,
    /// Objects with the <see cref="DomainObject.State"/>.<see cref="DomainObjectState.IsInvalid"/>
    /// or the <see cref="DomainObject.State"/>.<see cref="DomainObjectState.IsNotLoadedYet"/> flag set, remain the same.
    /// </para>
    /// <para>
    /// When the operation completes, all virtual relation end-points will no longer be complete, and they will be reloaded on access. All changes,
    /// including <see cref="DomainObjectCollection"/> references set into relation properties, will be rolled back.
    /// </para>
    /// </remarks>
    public static void UnloadAll (ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull("clientTransaction", clientTransaction);

      Func<ClientTransaction, IDataManagementCommand> commandFactory = tx => tx.DataManager.CreateUnloadAllCommand();
      var executor = new TransactionHierarchyCommandExecutor(commandFactory);
      executor.ExecuteCommandForTransactionHierarchy(clientTransaction);
    }

    public static void UnloadFiltered (ClientTransaction clientTransaction, Predicate<DomainObject> domainObjectFilter)
    {
      ArgumentUtility.CheckNotNull("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull("domainObjectFilter", domainObjectFilter);

      Func<ClientTransaction, IDataManagementCommand> commandFactory = tx => tx.DataManager.CreateUnloadFilteredDomainObjectsCommand(domainObjectFilter);
      var executor = new TransactionHierarchyCommandExecutor(commandFactory);
      executor.ExecuteCommandForTransactionHierarchy(clientTransaction);
    }

    private static void CheckVirtualEndPointID (RelationEndPointID endPointID)
    {
      if (!endPointID.Definition.IsVirtual)
      {
        var message = string.Format("The given end point ID '{0}' does not denote a virtual end-point.", endPointID);
        throw new ArgumentException(message, "endPointID");
      }

      if (endPointID.Definition.IsAnonymous)
      {
        var message = string.Format("The given end point ID '{0}' denotes an anonymous end-point, which cannot be unloaded.", endPointID);
        throw new ArgumentException(message, "endPointID");
      }
    }

    private static IDataManagementCommand CreateUnloadVirtualEndPointAndItemDataCommand (ClientTransaction tx, RelationEndPointID endPointID)
    {
      CheckVirtualEndPointID(endPointID);
      var endPoint = (IVirtualEndPoint?)tx.DataManager.GetRelationEndPointWithoutLoading(endPointID);

      if (endPoint == null || !endPoint.IsDataComplete)
        return new NopCommand();

      var unloadEndPointCommand = tx.DataManager.CreateUnloadVirtualEndPointsCommand(endPointID);

      ObjectID[] unloadedObjectIDs;
      if (endPoint.Definition.Cardinality == CardinalityType.Many)
        unloadedObjectIDs = ((ICollectionEndPoint<ICollectionEndPointData>)endPoint).GetData().Select(data => data.ID).ToArray();
      else
      {
        var oppositeObjectID = ((IVirtualObjectEndPoint)endPoint).OppositeObjectID;
        unloadedObjectIDs = oppositeObjectID != null ? new[] { oppositeObjectID } : new ObjectID[0];
      }
      var unloadDataCommand = tx.DataManager.CreateUnloadCommand(unloadedObjectIDs);

      return new CompositeCommand(unloadEndPointCommand, unloadDataCommand);
    }
  }
}
