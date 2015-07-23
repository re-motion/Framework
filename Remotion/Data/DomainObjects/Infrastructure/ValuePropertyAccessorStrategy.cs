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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  internal class ValuePropertyAccessorStrategy : IPropertyAccessorStrategy
  {
    public static readonly ValuePropertyAccessorStrategy Instance = new ValuePropertyAccessorStrategy();

    private ValuePropertyAccessorStrategy ()
    {
    }

    public Type GetPropertyType (PropertyDefinition propertyDefinition, IRelationEndPointDefinition relationEndPointDefinition)
    {
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);
      return propertyDefinition.PropertyType;
    }

    public bool HasChanged (PropertyAccessor propertyAccessor, ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("propertyAccessor", propertyAccessor);
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      return GetDataContainer (propertyAccessor, transaction).HasValueChanged (propertyAccessor.PropertyData.PropertyDefinition);
    }

    public bool HasBeenTouched (PropertyAccessor propertyAccessor, ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("propertyAccessor", propertyAccessor);
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      return GetDataContainer (propertyAccessor, transaction).HasValueBeenTouched (propertyAccessor.PropertyData.PropertyDefinition);
    }

    public bool IsNull (PropertyAccessor propertyAccessor, ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("propertyAccessor", propertyAccessor);
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      return GetValueWithoutTypeCheck (propertyAccessor, transaction) == null;
    }

    public object GetValueWithoutTypeCheck (PropertyAccessor propertyAccessor, ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("propertyAccessor", propertyAccessor);
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      return GetDataContainer (propertyAccessor, transaction).GetValue (propertyAccessor.PropertyData.PropertyDefinition);
    }

    public void SetValueWithoutTypeCheck (PropertyAccessor propertyAccessor, ClientTransaction transaction, object value)
    {
      ArgumentUtility.CheckNotNull ("propertyAccessor", propertyAccessor);
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      GetDataContainer (propertyAccessor, transaction).SetValue (propertyAccessor.PropertyData.PropertyDefinition, value);
    }

    public object GetOriginalValueWithoutTypeCheck (PropertyAccessor propertyAccessor, ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("propertyAccessor", propertyAccessor);
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      return GetDataContainer (propertyAccessor, transaction).GetValue (propertyAccessor.PropertyData.PropertyDefinition, ValueAccess.Original);
    }

    private DataContainer GetDataContainer (PropertyAccessor propertyAccessor, ClientTransaction transaction)
    {
      return transaction.DataManager.GetDataContainerWithLazyLoad (propertyAccessor.DomainObject.ID, throwOnNotFound: true);
    }
  }
}
