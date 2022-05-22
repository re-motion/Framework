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
using System.Collections.Generic;
using System.Linq;
using Remotion.Collections;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure.HierarchyManagement
{
  /// <summary>
  /// Extends <see cref="ReadOnlyClientTransactionListener"/> with special rules that apply only while a <see cref="DomainObject"/>'s data is being
  /// loaded. During that phase, it is forbidden to create or delete objects, but it is allowed to modify the data of (only) the object currently
  /// being loaded.
  /// </summary>
  [Serializable]
  public class ReadOnlyClientTransactionListenerWithLoadRules : ReadOnlyClientTransactionListener
  {
    private readonly HashSet<ObjectID> _currentlyLoadingObjectIDs = new HashSet<ObjectID>();

    public bool IsInLoadMode
    {
      get { return _currentlyLoadingObjectIDs.Count != 0; }
    }

    public IReadOnlyCollection<ObjectID> CurrentlyLoadingObjectIDs
    {
      get { return _currentlyLoadingObjectIDs.AsReadOnly(); }
    }

    public virtual void AddCurrentlyLoadingObjectIDs (IEnumerable<ObjectID> objectIds)
    {
      ArgumentUtility.CheckNotNull("objectIds", objectIds);

      Assertion.DebugAssert(!objectIds.Any(id => _currentlyLoadingObjectIDs.Contains(id)));
      _currentlyLoadingObjectIDs.UnionWith(objectIds);
    }

    public virtual void RemoveCurrentlyLoadingObjectIDs (IEnumerable<ObjectID> objectIds)
    {
      ArgumentUtility.CheckNotNull("objectIds", objectIds);

      Assertion.DebugAssert(objectIds.All(id => _currentlyLoadingObjectIDs.Contains(id)));
      _currentlyLoadingObjectIDs.ExceptWith(objectIds);
    }

    public override void NewObjectCreating (ClientTransaction clientTransaction, Type type)
    {
      ArgumentUtility.CheckNotNull("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull("type", type);

      if (IsInLoadMode)
      {
        var specificErrorMessage = string.Format("An object of type '{0}' cannot be created.", type.Name);
        throw CreateModificationException(specificErrorMessage);
      }
      base.NewObjectCreating(clientTransaction, type);
    }

    public override void ObjectDeleting (ClientTransaction clientTransaction, IDomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull("domainObject", domainObject);

      if (IsInLoadMode)
      {
        var specificErrorMessage = string.Format("Object '{0}' cannot be deleted.", domainObject.ID);
        throw CreateModificationException(specificErrorMessage);
      }
      base.ObjectDeleting(clientTransaction, domainObject);
    }

    public override void PropertyValueChanging (
        ClientTransaction clientTransaction,
        IDomainObject domainObject,
        PropertyDefinition propertyDefinition,
        object? oldValue,
        object? newValue)
    {
      ArgumentUtility.CheckNotNull("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull("domainObject", domainObject);
      ArgumentUtility.CheckNotNull("propertyDefinition", propertyDefinition);

      if (IsInLoadMode)
      {
        CheckModifiedObjectID(domainObject.ID, propertyDefinition.PropertyName);
        // This check is here only for defensiveness purposes; it's not actually possible to write an integration test for this scenario without
        // manually injecting DataContainers (because we guard against subtransactions loading the objects currently loaded in the parent transaction).
        if (DataContainerExistsInSubTransaction(clientTransaction, domainObject.ID))
        {
          var message = string.Format(
              "Object '{0}' can no longer be modified because its data has already been loaded into the subtransaction.", domainObject.ID);
          throw new InvalidOperationException(message);
        }
      }
      else
        base.PropertyValueChanging(clientTransaction, domainObject, propertyDefinition, oldValue, newValue);
    }

    public override void RelationChanging (
        ClientTransaction clientTransaction,
        IDomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        IDomainObject? oldRelatedObject,
        IDomainObject? newRelatedObject)
    {
      ArgumentUtility.CheckNotNull("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull("domainObject", domainObject);
      ArgumentUtility.CheckNotNull("relationEndPointDefinition", relationEndPointDefinition);

      if (IsInLoadMode)
      {
        CheckModifiedObjectID(domainObject.ID, relationEndPointDefinition.PropertyName);
        // This is here mostly for defensiveness purposes; it would be very difficult to construct a scenario where this is triggered.
        // There is no integration test for this check.
        if (clientTransaction.SubTransaction != null)
        {
          var endPointID = RelationEndPointID.Create(domainObject.ID, relationEndPointDefinition);
          var endPointInSubTransaction = clientTransaction.SubTransaction.DataManager.RelationEndPoints[endPointID];
          if (endPointInSubTransaction != null && endPointInSubTransaction.IsDataComplete)
          {
            var message = string.Format(
                "The relation property '{0}' of object '{1}' can no longer be modified because its data has already been loaded into the subtransaction.",
                relationEndPointDefinition.PropertyName,
                domainObject.ID);
            throw new InvalidOperationException(message);
          }
        }
      }
      else
        base.RelationChanging(clientTransaction, domainObject, relationEndPointDefinition, oldRelatedObject, newRelatedObject);
    }

    public override void DataContainerStateUpdated (ClientTransaction clientTransaction, DataContainer container, DataContainerState newDataContainerState)
    {
      if (IsInLoadMode && _currentlyLoadingObjectIDs.Contains(container.ID))
      {
        Assertion.DebugAssert(
            !DataContainerExistsInSubTransaction(clientTransaction, container.ID),
            "Data existing in subtransaction should have been prevented by PropertyValueChanging event.");
        return;
      }

      base.DataContainerStateUpdated(clientTransaction, container, newDataContainerState);
    }

    private void CheckModifiedObjectID (ObjectID objectID, string? modifiedPropertyIdentifier)
    {
      if (!_currentlyLoadingObjectIDs.Contains(objectID))
      {
        var specificErrorMessage = string.Format(
            "The object '{0}' cannot be modified. (Modified property: '{1}'.)", objectID, modifiedPropertyIdentifier);
        throw CreateModificationException(specificErrorMessage);
      }
    }

    private InvalidOperationException CreateModificationException (string specificErrorMessage)
    {
      string mainMessage;
      if (_currentlyLoadingObjectIDs.Count == 1)
      {
        mainMessage = string.Format("While the object '{0}' is being loaded, only this object can be modified.", _currentlyLoadingObjectIDs.Single());
      }
      else
      {
        mainMessage = string.Format(
            "While the objects {0} are being loaded, only these object can be modified.",
            string.Join(", ", _currentlyLoadingObjectIDs.Select(id => "'" + id + "'")));
      }
      var message = mainMessage + " " + specificErrorMessage;
      return new InvalidOperationException(message);
    }

    private bool DataContainerExistsInSubTransaction (ClientTransaction clientTransaction, ObjectID objectID)
    {
      return clientTransaction.SubTransaction != null && clientTransaction.SubTransaction.DataManager.DataContainers[objectID] != null;
    }
  }
}
