// // This file is part of the re-motion Core Framework (www.re-motion.org)
// // Copyright (c) rubicon IT GmbH, www.rubicon.eu
// //
// // The re-motion Core Framework is free software; you can redistribute it
// // and/or modify it under the terms of the GNU Lesser General Public License
// // as published by the Free Software Foundation; either version 2.1 of the
// // License, or (at your option) any later version.
// //
// // re-motion is distributed in the hope that it will be useful,
// // but WITHOUT ANY WARRANTY; without even the implied warranty of
// // MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// // GNU Lesser General Public License for more details.
// //
// // You should have received a copy of the GNU Lesser General Public License
// // along with re-motion; if not, see http://www.gnu.org/licenses.
// //
using System;
using System.Collections.Generic;
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.FunctionalProgramming;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DomainImplementation
{
  /// <summary>
  /// Provides functionality for manipulating the initial data of <see cref="DomainObject"/> instances. Use the methods of this class to
  /// apply the current state of a new <see cref="DomainObject"/> instance as the original (i.e. initial) state of the object.
  /// </summary>
  public static class InitializationService
  {
    /// <summary>
    /// Applies the current state of the <see cref="DomainObject"/> instances identified by <paramref name="objectIDs"/> as the original state.
    /// </summary>
    /// <param name="clientTransaction">The <see cref="ClientTransaction"/> that contains the affected objects.</param>
    /// <param name="objectIDs">
    /// The list of <see cref="ObjectID"/> values identifying the affected objects. If an <see cref="ObjectID"/> does not belong to an already loaded <see cref="DomainObject"/>,
    /// it is ignored.
    /// </param>
    /// <returns>The list of <see cref="DomainObject"/> instances that have been successfully updated. </returns>
    /// <exception cref="ArgumentNullException">One of the arguments passed to this method is <see langword="null"/>.</exception>
    /// <remarks>
    /// The <see cref="DomainObject"/> instances identified via <paramref name="objectIDs"/> must have <see cref="DomainObject.State"/>.<see cref="DomainObjectState.IsNew"/> set.
    /// All other instances will be skipped during the operation.
    /// <para>- and - </para>
    /// All <see cref="DomainObject"/> instances that belong to a modified relation must be part of the set identified via <paramref name="objectIDs"/>.
    /// All other instances will be skipped during the operation.
    /// <para>- and - </para>
    /// Any <see cref="DomainObject"/> instance skipped during the operation may also result in skipping dependent objects (i.e. if the object on one side of a bidirectional
    /// relation is skipped, both objects will be skipped).
    /// </remarks>
    public static IReadOnlyCollection<DomainObject> TryToApplyCurrentStateAsInitialValue (ClientTransaction clientTransaction, IEnumerable<ObjectID> objectIDs)
    {
      ArgumentUtility.CheckNotNull("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull("objectIDs", objectIDs);

      // - Exclude any object that has a state other than "new" from the set.
      // - Exclude any object from the set that contains changed relation endpoints where the referenced object is not part of the set.
      // - Commit all relation endpoints of the remaining set.
      // - Commit all DataContainer properties of the remaining set.

      var validDomainObjects = objectIDs
          .Select(id => clientTransaction.GetEnlistedDomainObject(id))
          .Where(obj => obj != null)
          .Select(obj => (DomainObject)obj!)
          .Where(obj => obj.State.IsNew)
          .ToDictionary(obj => obj.ID);

      var affectedVirtualRelationEndPoints = validDomainObjects.Keys
          .Select(id => (ObjectID: id, Relations: GetAffectedVirtualRelationEndPoints(clientTransaction, id).ToArray()))
          .Where(item => item.Relations.Length > 0)
          .ToList();

      var involvedObjects = GetInvolvedObjectsForRelationEndPoints(clientTransaction, affectedVirtualRelationEndPoints);
      bool hasUpdatedListOfValidDomainObjects;
      do
      {
        hasUpdatedListOfValidDomainObjects = false;
        var invalidObjects = involvedObjects
            .Where(kvp => kvp.Value.Any(objetID => !validDomainObjects.ContainsKey(objetID)))
            .SelectMany(item => item.Value.Concat(item.Key))
            .Distinct();

        foreach (var invalidObject in invalidObjects)
          hasUpdatedListOfValidDomainObjects |= validDomainObjects.Remove(invalidObject);

      } while (hasUpdatedListOfValidDomainObjects);

      var changedRealObjectEndPoints = new List<IRealObjectEndPoint>();
      var dataContainers = new List<DataContainer>();
      foreach (var domainObjectID in validDomainObjects.Keys)
      {
        var dataContainer = clientTransaction.DataManager.GetDataContainerWithoutLoading(domainObjectID);
        Assertion.DebugIsNotNull(dataContainer, "DomainObject '{0}' is not part of ClientTransaction", domainObjectID);

        changedRealObjectEndPoints.AddRange(
            dataContainer.AssociatedRelationEndPointIDs
                .Select(endPointID => clientTransaction.DataManager.GetRelationEndPointWithoutLoading(endPointID))
                .OfType<IRealObjectEndPoint>()
                .Where(ep => ep.HasChanged));

        dataContainers.Add(dataContainer);
      }

      foreach (var realEndPoint in changedRealObjectEndPoints)
        realEndPoint.Commit();

      var virtualEndPointsThatBelongToValidObjects = affectedVirtualRelationEndPoints
          .Where(data => validDomainObjects.ContainsKey(data.ObjectID))
          .SelectMany(data => data.Relations.Select(relation => relation.VirtualEndPoint))
          .Distinct();
      foreach (var virtualEndPoint in virtualEndPointsThatBelongToValidObjects)
        virtualEndPoint.Commit();

      foreach (var dataContainer in dataContainers)
        dataContainer.CommitPropertyValuesOnNewDataContainer();

      return validDomainObjects.Values;
    }

    private static IReadOnlyDictionary<ObjectID, IReadOnlyCollection<ObjectID>> GetInvolvedObjectsForRelationEndPoints (
        ClientTransaction clientTransaction,
        List<(ObjectID ObjectID, (IRealObjectEndPoint RealEndPoint, IVirtualEndPoint VirtualEndPoint)[] Relations)> affectedVirtualRelationEndPoints)
    {
      var involvedObjectIDs = new List<(ObjectID ObjectID, IReadOnlyCollection<ObjectID> InvolvedObjects)>();
      foreach (var (objectIDForRealEndPoint, relations) in affectedVirtualRelationEndPoints)
      {
        foreach (var relation in relations)
        {
          involvedObjectIDs.Add(
              (ObjectID: objectIDForRealEndPoint,
              InvolvedObjects: GetInvolvedObjectsForRealEndPoint(relation.RealEndPoint)));

          involvedObjectIDs.Add(
              (ObjectID: objectIDForRealEndPoint,
              InvolvedObjects: GetInvolvedObjectsForVirtualEndPoint(relation.VirtualEndPoint, clientTransaction, relation.RealEndPoint.PropertyDefinition)));
        }
      }

      return involvedObjectIDs
          .GroupBy(item => item.ObjectID, item => item.InvolvedObjects)
          .ToDictionary(item => item.Key, item => (IReadOnlyCollection<ObjectID>)item.SelectMany(obj => obj).Distinct().ToList());
    }

    private static IEnumerable<(IRealObjectEndPoint RealEndPoint, IVirtualEndPoint VirtualEndPoint)> GetAffectedVirtualRelationEndPoints (
        ClientTransaction clientTransaction,
        ObjectID domainObjectID)
    {
      var dataContainer = clientTransaction.DataManager.GetDataContainerWithoutLoading(domainObjectID);
      Assertion.DebugIsNotNull(dataContainer, "DomainObject '{0}' is not part of ClientTransaction", domainObjectID);

      var changedRealObjectEndPoints = dataContainer.AssociatedRelationEndPointIDs
          .Select(endPointID => clientTransaction.DataManager.GetRelationEndPointWithoutLoading(endPointID))
          .OfType<IRealObjectEndPoint>()
          .Where(ep => ep.HasChanged);

      var affectedVirtualEndPoints1 = changedRealObjectEndPoints
          .SelectMany(ep => ep.GetOppositeRelationEndPointIDs().Select(virtualEndPointID => (RealEndPoint: ep, virtualEndPointID)))
          .Select(item => (item.RealEndPoint, VirtualEndPoint: (IVirtualEndPoint?)clientTransaction.DataManager.GetRelationEndPointWithoutLoading(item.virtualEndPointID)))
          .Where(item => item.VirtualEndPoint != null)
          .Select(item => (item.RealEndPoint, VirtualEndPoint: (IVirtualEndPoint)item.VirtualEndPoint!))
          .Where(item => !item.VirtualEndPoint.IsNull)
          .Where(item => !item.VirtualEndPoint.Definition.IsAnonymous);

      var changedVirtualObjectEndPoints = dataContainer.AssociatedRelationEndPointIDs
          .Select(endPointID => clientTransaction.DataManager.GetRelationEndPointWithoutLoading(endPointID))
          .OfType<IVirtualEndPoint>()
          .Where(ep => ep.HasChanged);

      var affectedVirtualEndPoints2 = changedVirtualObjectEndPoints
          .SelectMany(ep => ep.GetOppositeRelationEndPointIDs().Select(realObjectEndPointID => (VirtualEndPoint: ep, realObjectEndPointID)))
          .Select(item => (RealEndPoint: (IRealObjectEndPoint?)clientTransaction.DataManager.GetRelationEndPointWithoutLoading(item.realObjectEndPointID), item.VirtualEndPoint))
          .Where(item => item.RealEndPoint != null)
          .Select(item => (RealEndPoint: (IRealObjectEndPoint)item.RealEndPoint!, item.VirtualEndPoint))
          .Where(item => item.RealEndPoint.HasChanged);

      return affectedVirtualEndPoints1.Concat(affectedVirtualEndPoints2);
    }


    private static IReadOnlyCollection<ObjectID> GetInvolvedObjectsForRealEndPoint (IRealObjectEndPoint realEndPoint)
    {
      var involvedObjectIDs = new List<ObjectID>();
      if (realEndPoint.OppositeObjectID != null)
        involvedObjectIDs.Add(realEndPoint.OppositeObjectID);

      if (realEndPoint.OriginalOppositeObjectID != null)
        involvedObjectIDs.Add(realEndPoint.OriginalOppositeObjectID);

      return involvedObjectIDs;
    }

    private static IReadOnlyCollection<ObjectID> GetInvolvedObjectsForVirtualEndPoint (
        IVirtualEndPoint virtualEndPoint,
        ClientTransaction clientTransaction,
        PropertyDefinition propertyDefinitionForRealEndPoint)
    {
      var involvedObjectIDs = new List<ObjectID>();

      if (virtualEndPoint.ObjectID != null)
        involvedObjectIDs.Add(virtualEndPoint.ObjectID);

      if (virtualEndPoint.Definition.Cardinality == CardinalityType.One)
      {
        var virtualObjectEndPoint = (IVirtualObjectEndPoint)virtualEndPoint;
        if (virtualObjectEndPoint.OppositeObjectID != null)
          involvedObjectIDs.Add(virtualObjectEndPoint.OppositeObjectID);

        if (virtualObjectEndPoint.OriginalOppositeObjectID != null)
          involvedObjectIDs.Add(virtualObjectEndPoint.OriginalOppositeObjectID);
      }
      else
      {
        var collectionEndPoint = (ICollectionEndPoint<ICollectionEndPointData>)virtualEndPoint;
        foreach (var oppositeObject in collectionEndPoint.GetData())
        {
          if (IsPropertyChanged(clientTransaction, oppositeObject.ID, propertyDefinitionForRealEndPoint))
            involvedObjectIDs.Add(oppositeObject.ID);
        }

        foreach (var originalOppositeObject in collectionEndPoint.GetOriginalData())
        {
          if (IsPropertyChanged(clientTransaction, originalOppositeObject.ID, propertyDefinitionForRealEndPoint))
            involvedObjectIDs.Add(originalOppositeObject.ID);
        }
      }

      return involvedObjectIDs;
    }

    private static bool IsPropertyChanged (ClientTransaction clientTransaction, ObjectID domainObjectID, PropertyDefinition propertyDefinition)
    {
      var dataContainer = clientTransaction.DataManager.GetDataContainerWithoutLoading(domainObjectID);
      Assertion.DebugIsNotNull(dataContainer, "DomainObject '{0}' is not part of ClientTransaction", domainObjectID);

      return dataContainer.HasValueChanged(propertyDefinition);
    }
  }
}
