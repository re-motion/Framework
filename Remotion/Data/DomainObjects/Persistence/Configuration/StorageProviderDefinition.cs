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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Configuration
{
  /// <summary>
  /// Defines the configuration for a specific <see cref="StorageProvider"/>. Use <see cref="StorageSettingsFactory"/> to create instances.
  /// </summary>
  public abstract class StorageProviderDefinition
  {
    public string Name { get; }

    public IReadOnlyCollection<Type> AssignedStorageGroups { get; }

    private readonly IStorageObjectFactory _factory;

    protected StorageProviderDefinition (string name, IStorageObjectFactory factory, IReadOnlyCollection<Type>? assignedStorageGroups = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty("name", name);
      ArgumentUtility.CheckNotNull("factory", factory);

      Name = name;
      _factory = factory;
      AssignedStorageGroups = assignedStorageGroups ?? Array.Empty<Type>();
    }

    public abstract bool IsIdentityTypeSupported (Type identityType);

    public void CheckIdentityType (Type identityType)
    {
      if (!IsIdentityTypeSupported(identityType))
        throw new IdentityTypeNotSupportedException(GetType(), identityType);
    }

    public IStorageObjectFactory Factory
    {
      get { return _factory; }
    }

    public override string ToString ()
    {
      return string.Format("{0}: '{1}'", GetType().Name, Name);
    }
  }
}
