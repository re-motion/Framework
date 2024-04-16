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
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.ServiceLocation;

namespace Remotion.Data.DomainObjects.ObjectBinding.UnitTests;

[Serializable]
public class SerializableStorageSettings : IStorageSettings,
#pragma warning disable SYSLIB0050
    IObjectReference
#pragma warning restore SYSLIB0050
{
  [NonSerialized]
  private IStorageSettings _storageSettingsImplementation;

  public SerializableStorageSettings (IStorageSettings storageSettingsImplementation)
  {
    _storageSettingsImplementation = storageSettingsImplementation;
  }

  public StorageProviderDefinition GetDefaultStorageProviderDefinition ()
  {
    return _storageSettingsImplementation.GetDefaultStorageProviderDefinition();
  }

  public IReadOnlyCollection<StorageProviderDefinition> GetStorageProviderDefinitions ()
  {
    return _storageSettingsImplementation.GetStorageProviderDefinitions();
  }

  public StorageProviderDefinition GetStorageProviderDefinition (TypeDefinition typeDefinition)
  {
    return _storageSettingsImplementation.GetStorageProviderDefinition(typeDefinition);
  }

  public StorageProviderDefinition GetStorageProviderDefinition (Type storageGroupTypeOrNull)
  {
    return _storageSettingsImplementation.GetStorageProviderDefinition(storageGroupTypeOrNull);
  }

  public StorageProviderDefinition GetStorageProviderDefinition (string storageProviderName)
  {
    return _storageSettingsImplementation.GetStorageProviderDefinition(storageProviderName);
  }

  object IObjectReference.GetRealObject (StreamingContext context)
  {
    return (SerializableStorageSettings)SafeServiceLocator.Current.GetInstance<IStorageSettings>();
  }
}
