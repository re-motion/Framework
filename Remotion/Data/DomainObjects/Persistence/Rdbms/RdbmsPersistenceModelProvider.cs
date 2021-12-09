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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms
{
  /// <summary>
  /// The <see cref="RdbmsPersistenceModelProvider"/> implements methods that retrieve rdbms-specific persistence model definitions from mapping objects.
  /// </summary>
  public class RdbmsPersistenceModelProvider : IRdbmsPersistenceModelProvider
  {
    public IRdbmsStorageEntityDefinition GetEntityDefinition (TypeDefinition typeDefinition)
    {
      ArgumentUtility.CheckNotNull("typeDefinition", typeDefinition);

      if (!typeDefinition.HasStorageEntityDefinitionBeenSet)
      {
        throw new RdbmsProviderException(
            string.Format(
                "The Rdbms provider classes require a storage definition object of type '{0}' for type definition '{1}', "
                + "but that type definition has no storage definition object.",
                typeof(IRdbmsStorageEntityDefinition).Name,
                typeDefinition.Type.GetFullNameSafe()));
      }

      var storageEntityDefinitionAsIEntityDefinition = typeDefinition.StorageEntityDefinition as IRdbmsStorageEntityDefinition;
      if (storageEntityDefinitionAsIEntityDefinition == null)
      {
        throw new RdbmsProviderException(
            string.Format(
                "The Rdbms provider classes require a storage definition object of type '{0}' for type definition '{1}', "
                + "but that type definition has a storage definition object of type '{2}'.",
                typeof(IRdbmsStorageEntityDefinition).Name,
                typeDefinition.Type.GetFullNameSafe(),
                typeDefinition.StorageEntityDefinition.GetType().Name));
      }

      return storageEntityDefinitionAsIEntityDefinition;
    }

    public IRdbmsStoragePropertyDefinition GetStoragePropertyDefinition (PropertyDefinition propertyDefinition)
    {
      ArgumentUtility.CheckNotNull("propertyDefinition", propertyDefinition);

      if (!propertyDefinition.HasStoragePropertyDefinitionBeenSet)
      {
        throw new MappingException(
            string.Format(
                "The Rdbms provider classes require a storage definition object of type '{0}' for property '{1}' of class-definition '{2}', "
                + "but that property has no storage definition object.",
                typeof(IRdbmsStoragePropertyDefinition).Name,
                propertyDefinition.PropertyName,
                propertyDefinition.TypeDefinition.Type.GetFullNameSafe()));
      }

      var storagePropertyDefinitionAsIColumnDefinition = propertyDefinition.StoragePropertyDefinition as IRdbmsStoragePropertyDefinition;
      if (storagePropertyDefinitionAsIColumnDefinition == null)
      {
        throw new MappingException(
            string.Format(
                "The Rdbms provider classes require a storage definition object of type '{0}' for property '{1}' of class-definition '{2}', "
                + "but that property has a storage definition object of type '{3}'.",
                typeof(IRdbmsStoragePropertyDefinition).Name,
                propertyDefinition.PropertyName,
                propertyDefinition.TypeDefinition.Type.GetFullNameSafe(),
                propertyDefinition.StoragePropertyDefinition.GetType().Name));
      }

      return storagePropertyDefinitionAsIColumnDefinition;
    }
  }
}
