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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  internal class RelatedObjectPropertyAccessorStrategy : IPropertyAccessorStrategy
  {
    public static readonly RelatedObjectPropertyAccessorStrategy Instance = new RelatedObjectPropertyAccessorStrategy();

    private RelatedObjectPropertyAccessorStrategy ()
    {
    }

    public Type GetPropertyType (PropertyDefinition? propertyDefinition, IRelationEndPointDefinition? relationEndPointDefinition)
    {
      ArgumentUtility.CheckNotNull("relationEndPointDefinition", relationEndPointDefinition!);
      Assertion.IsFalse(relationEndPointDefinition.IsAnonymous);
      Assertion.DebugIsNotNull(relationEndPointDefinition.PropertyInfo, "relationEndPointDefinition.PropertyInfo != null when relationEndPointDefinition.IsAnonymous == false");

      return relationEndPointDefinition.PropertyInfo.PropertyType;
    }

    public RelationEndPointID CreateRelationEndPointID (PropertyAccessor propertyAccessor)
    {
      ArgumentUtility.CheckNotNull("propertyAccessor", propertyAccessor);
      Assertion.IsNotNull(propertyAccessor.PropertyData.RelationEndPointDefinition, "A value property accessor cannot be used with a relation property definition.");

      return RelationEndPointID.Create(propertyAccessor.DomainObject.ID, propertyAccessor.PropertyData.RelationEndPointDefinition);
    }

    public IRelationEndPoint? GetRelationEndPoint (PropertyAccessor propertyAccessor, ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull("propertyAccessor", propertyAccessor);
      ArgumentUtility.CheckNotNull("transaction", transaction);

      return transaction.DataManager.GetRelationEndPointWithoutLoading(CreateRelationEndPointID(propertyAccessor));
    }

    public bool HasChanged (PropertyAccessor propertyAccessor, ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull("propertyAccessor", propertyAccessor);
      ArgumentUtility.CheckNotNull("transaction", transaction);

      var endPoint = GetRelationEndPoint(propertyAccessor, transaction);
      return endPoint != null && endPoint.HasChanged;
    }

    public bool HasBeenTouched (PropertyAccessor propertyAccessor, ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull("propertyAccessor", propertyAccessor);
      ArgumentUtility.CheckNotNull("transaction", transaction);

      var endPoint = GetRelationEndPoint(propertyAccessor, transaction);
      return endPoint != null && endPoint.HasBeenTouched;
    }

    public bool IsNull (PropertyAccessor propertyAccessor, ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull("propertyAccessor", propertyAccessor);
      ArgumentUtility.CheckNotNull("transaction", transaction);
      Assertion.IsNotNull(propertyAccessor.PropertyData.RelationEndPointDefinition, "A value property accessor cannot be used with a relation property definition.");

      if (propertyAccessor.PropertyData.RelationEndPointDefinition.IsVirtual)
        return GetValueWithoutTypeCheck(propertyAccessor, transaction) == null;
      else // for nonvirtual end points check out the ObjectID, which is stored in the DataContainer; this allows IsNull to avoid loading the object
        return ValuePropertyAccessorStrategy.Instance.GetValueWithoutTypeCheck(propertyAccessor, transaction) == null;
    }

    public object? GetValueWithoutTypeCheck (PropertyAccessor propertyAccessor, ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull("propertyAccessor", propertyAccessor);
      ArgumentUtility.CheckNotNull("transaction", transaction);

      return transaction.GetRelatedObject(CreateRelationEndPointID(propertyAccessor));
    }

    public void SetValueWithoutTypeCheck (PropertyAccessor propertyAccessor, ClientTransaction transaction, object? value)
    {
      ArgumentUtility.CheckNotNull("propertyAccessor", propertyAccessor);
      ArgumentUtility.CheckNotNull("transaction", transaction);
      var newRelatedObject = ArgumentUtility.CheckType<DomainObject>("value", value);

      var endPointID = CreateRelationEndPointID(propertyAccessor);
      var endPoint = (IObjectEndPoint)transaction.DataManager.GetRelationEndPointWithLazyLoad(endPointID);

      if (newRelatedObject != null && transaction.RootTransaction != newRelatedObject.RootTransaction)
      {
        var formattedMessage = String.Format(
            "Property '{1}' of DomainObject '{2}' cannot be set to DomainObject '{0}'.",
            newRelatedObject.ID,
            endPoint.Definition.PropertyName,
            endPoint.ObjectID);
        throw new ClientTransactionsDifferException(formattedMessage + " The objects do not belong to the same ClientTransaction.");
      }

      var domainObjectOfEndPoint = endPoint.GetDomainObjectReference();
      Assertion.DebugIsNotNull(domainObjectOfEndPoint, "endPoint.GetDomainObjectReference() != null");
      DomainObjectCheckUtility.EnsureNotDeleted(domainObjectOfEndPoint, endPoint.ClientTransaction);

      if (newRelatedObject != null)
      {
        DomainObjectCheckUtility.EnsureNotDeleted(newRelatedObject, endPoint.ClientTransaction);
        CheckNewRelatedObjectType(endPoint, newRelatedObject);
      }

      var setCommand = endPoint.CreateSetCommand(newRelatedObject);
      var bidirectionalModification = setCommand.ExpandToAllRelatedObjects();
      bidirectionalModification.NotifyAndPerform();
    }

    public object? GetOriginalValueWithoutTypeCheck (PropertyAccessor propertyAccessor, ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull("propertyAccessor", propertyAccessor);
      ArgumentUtility.CheckNotNull("transaction", transaction);

      return transaction.GetOriginalRelatedObject(CreateRelationEndPointID(propertyAccessor));
    }

    private void CheckNewRelatedObjectType (IObjectEndPoint objectEndPoint, DomainObject newRelatedObject)
    {
      if (!objectEndPoint.Definition.GetOppositeEndPointDefinition().TypeDefinition.IsAssignableFrom(newRelatedObject.ID.ClassDefinition))
      {
        var message = string.Format(
            "DomainObject '{0}' cannot be assigned to property '{1}' of DomainObject '{2}', because it is not compatible with the type of the property.",
            newRelatedObject.ID,
            objectEndPoint.Definition.PropertyName,
            objectEndPoint.ObjectID);
        throw new ArgumentException(
            message,
            "newRelatedObject");
      }
    }
  }
}
