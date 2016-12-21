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
  internal class RelatedObjectCollectionPropertyAccessorStrategy : IPropertyAccessorStrategy
  {
    public static readonly RelatedObjectCollectionPropertyAccessorStrategy Instance = new RelatedObjectCollectionPropertyAccessorStrategy();

    private RelatedObjectCollectionPropertyAccessorStrategy ()
    {
    }

    public RelationEndPointID CreateRelationEndPointID (PropertyAccessor propertyAccessor)
    {
      ArgumentUtility.CheckNotNull ("propertyAccessor", propertyAccessor);
      return RelatedObjectPropertyAccessorStrategy.Instance.CreateRelationEndPointID (propertyAccessor);
    }

    public Type GetPropertyType (PropertyDefinition propertyDefinition, IRelationEndPointDefinition relationEndPointDefinition)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointDefinition", relationEndPointDefinition);
      return RelatedObjectPropertyAccessorStrategy.Instance.GetPropertyType (propertyDefinition, relationEndPointDefinition);
    }

    public bool HasChanged (PropertyAccessor propertyAccessor, ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("propertyAccessor", propertyAccessor);
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      return RelatedObjectPropertyAccessorStrategy.Instance.HasChanged (propertyAccessor, transaction);
    }

    public bool HasBeenTouched (PropertyAccessor propertyAccessor, ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("propertyAccessor", propertyAccessor);
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      return RelatedObjectPropertyAccessorStrategy.Instance.HasBeenTouched (propertyAccessor, transaction);
    }

    public bool IsNull (PropertyAccessor propertyAccessor, ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("propertyAccessor", propertyAccessor);
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      return false;
    }

    public object GetValueWithoutTypeCheck (PropertyAccessor propertyAccessor, ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("propertyAccessor", propertyAccessor);
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      return transaction.GetRelatedObjects (CreateRelationEndPointID (propertyAccessor));
    }

    public void SetValueWithoutTypeCheck (PropertyAccessor propertyAccessor, ClientTransaction transaction, object value)
    {
      ArgumentUtility.CheckNotNull ("propertyAccessor", propertyAccessor);
      ArgumentUtility.CheckNotNull ("transaction", transaction);
      var newCollection = ArgumentUtility.CheckNotNullAndType<DomainObjectCollection> ("value", value);

      DomainObjectCheckUtility.EnsureNotDeleted (propertyAccessor.DomainObject, transaction);

      RelationEndPointID id = CreateRelationEndPointID (propertyAccessor);
      var endPoint = (ICollectionEndPoint) transaction.DataManager.GetRelationEndPointWithLazyLoad (id);

      if (newCollection.AssociatedEndPointID != null && newCollection.AssociatedEndPointID != endPoint.ID)
        throw new ArgumentException ("The given collection is already associated with an end point.", "value");

      if (newCollection.RequiredItemType != endPoint.Collection.RequiredItemType
          && !newCollection.IsReadOnly
          && !endPoint.Collection.IsReadOnly)
      {
        throw new InvalidOperationException ("The given collection has a different item type than the end point's current opposite collection.");
      }

      if (newCollection.GetType () != endPoint.Collection.GetType ())
      {
        var message = string.Format (
            "The given collection ('{0}') is not of the same type as the end point's current opposite collection ('{1}').",
            newCollection.GetType (),
            endPoint.Collection.GetType ());
        throw new InvalidOperationException (message);
      }

      var command = endPoint.CreateSetCollectionCommand(newCollection);
      var bidirectionalModification = command.ExpandToAllRelatedObjects();
      bidirectionalModification.NotifyAndPerform();
    }

    public object GetOriginalValueWithoutTypeCheck (PropertyAccessor propertyAccessor, ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("propertyAccessor", propertyAccessor);
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      return transaction.GetOriginalRelatedObjects (CreateRelationEndPointID (propertyAccessor));
    }
  }
}
