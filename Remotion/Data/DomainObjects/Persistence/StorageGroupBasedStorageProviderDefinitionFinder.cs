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
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence
{
  /// <summary>
  /// The <see cref="StorageGroupBasedStorageProviderDefinitionFinder"/> is responsible for finding the <see cref="StorageProviderDefinition"/> for a 
  /// <see cref="ClassDefinition"/> based on the <see cref="ClassDefinition.StorageGroupType"/>.
  /// </summary>
  public class StorageGroupBasedStorageProviderDefinitionFinder : IStorageProviderDefinitionFinder
  {
    private readonly StorageConfiguration _storageConfiguration;

    public StorageGroupBasedStorageProviderDefinitionFinder (StorageConfiguration storageConfiguration)
    {
      ArgumentUtility.CheckNotNull ("storageConfiguration", storageConfiguration);

      _storageConfiguration = storageConfiguration;
    }

    public StorageProviderDefinition GetStorageProviderDefinition (ClassDefinition classDefinition, string errorMessageContext)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      var storageGroupTypeOrNull = classDefinition.StorageGroupType;

      return GetStorageProviderDefinition(storageGroupTypeOrNull, errorMessageContext);
    }

    public StorageProviderDefinition GetStorageProviderDefinition (Type storageGroupTypeOrNull, string errorMessageContext)
    {
      if (storageGroupTypeOrNull == null)
        return GetDefaultStorageProviderDefinition (errorMessageContext);

      string storageGroupName = TypeUtility.GetPartialAssemblyQualifiedName (storageGroupTypeOrNull);
      var storageGroup = _storageConfiguration.StorageGroups[storageGroupName];
      if (storageGroup == null)
        return GetDefaultStorageProviderDefinition(errorMessageContext);

      return _storageConfiguration.StorageProviderDefinitions.GetMandatory (storageGroup.StorageProviderName);
    }

    private StorageProviderDefinition GetDefaultStorageProviderDefinition (string errorMessageContext)
    {
      var defaultStorageProviderDefinition = _storageConfiguration.DefaultStorageProviderDefinition;
      if (defaultStorageProviderDefinition == null)
        throw _storageConfiguration.CreateMissingDefaultProviderException (errorMessageContext);

      return defaultStorageProviderDefinition;
    }
  }
}