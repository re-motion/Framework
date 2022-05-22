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
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Validation
{
  /// <summary>
  /// Validates that not-nullable properties are not assigned a <see langword="null" /> value.
  /// </summary>
  /// <threadsafety static="true" instance="true" />
  [ImplementationFor(typeof(IDataContainerValidator), RegistrationType = RegistrationType.Multiple, Position = DataContainerValidatorPosition)]
  [ImplementationFor(typeof(IPersistableDataValidator), RegistrationType = RegistrationType.Multiple, Position = PersistableDataValidatorPosition)]
  public class NotNullablePropertyValidator : IPersistableDataValidator, IDataContainerValidator
  {
    public const int DataContainerValidatorPosition = 0;
    public const int PersistableDataValidatorPosition = 0;

    public NotNullablePropertyValidator ()
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

      foreach (var propertyDefinition in data.DomainObject.ID.ClassDefinition.GetPropertyDefinitions())
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

    private static void ValidatePropertyDefinition (IDomainObject? domainObject, DataContainer dataContainer, PropertyDefinition propertyDefinition)
    {
      if (propertyDefinition.IsNullable)
        return;

      object? propertyValue = dataContainer.GetValueWithoutEvents(propertyDefinition, ValueAccess.Current);
      if (propertyValue == null)
      {
        throw new PropertyValueNotSetException(
            domainObject,
            propertyDefinition.PropertyName,
            string.Format(
                "Not-nullable property '{0}' of domain object '{1}' cannot be null.",
                propertyDefinition.PropertyName,
                dataContainer.ID));
      }
    }
  }
}
