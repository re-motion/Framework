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
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.IntegrationTests.CustomDataTypeSupport.TestDomain;
using Remotion.Data.DomainObjects.Validation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.IntegrationTests.CustomDataTypeSupport
{
  public class SimpleDataTypePropertyMaxLengthValidator : IPersistableDataValidator, IDataContainerValidator
  {
    public SimpleDataTypePropertyMaxLengthValidator ()
    {
    }

    public void Validate (ClientTransaction clientTransaction, PersistableData data)
    {
      ArgumentUtility.CheckNotNull("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull("data", data);

      if (data.DomainObjectState.IsDeleted)
        return;

      Assertion.IsFalse(data.DomainObjectState.IsNotLoadedYet, "No unloaded objects get this far.");
      Assertion.IsFalse(data.DomainObjectState.IsInvalid, "No invalid objects get this far.");

      foreach (var propertyDefinition in data.DataContainer.ID.ClassDefinition.GetPropertyDefinitions())
        ValidatePropertyDefinition(data.DomainObject, data.DataContainer, propertyDefinition);
    }

    public void Validate (DataContainer dataContainer)
    {
      ArgumentUtility.CheckNotNull("dataContainer", dataContainer);

      foreach (var propertyDefinition in dataContainer.ID.ClassDefinition.GetPropertyDefinitions())
      {
        // Skip validation when loading StorageClass.Transaction properties to allow initialization with default value
        if (propertyDefinition.StorageClass == StorageClass.Persistent)
          ValidatePropertyDefinition(null, dataContainer, propertyDefinition);
      }
    }

    private void ValidatePropertyDefinition (IDomainObject domainObject, DataContainer dataContainer, PropertyDefinition propertyDefinition)
    {
      var maxLength = propertyDefinition.MaxLength;
      if (maxLength == null)
        return;

      Type propertyType = propertyDefinition.PropertyType;
      if (propertyType != typeof(SimpleDataType))
        return;

      object propertyValue = dataContainer.GetValueWithoutEvents(propertyDefinition, ValueAccess.Current);
      if (propertyValue == null)
        return;

      if (((SimpleDataType)propertyValue).StringValue.Length > maxLength.Value)
      {
        string message = string.Format(
            "Value for property '{0}' of domain object '{1}' is too long. Maximum number of characters: {2}.",
            propertyDefinition.PropertyName,
            dataContainer.ID,
            maxLength.Value);

        throw new PropertyValueTooLongException(domainObject, propertyDefinition.PropertyName, maxLength.Value, message);
      }
    }
  }
}
