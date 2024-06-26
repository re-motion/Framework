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
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms
{
  public class RdbmsProviderDefinition: StorageProviderDefinition
  {
    private readonly string _connectionString;
    private readonly string _readOnlyConnectionString;

    public RdbmsProviderDefinition (string name, IRdbmsStorageObjectFactory factory, string connectionString, string readOnlyConnectionString, IReadOnlyCollection<Type>? assignedStorageGroups = null)
        : base(name, factory, assignedStorageGroups)
    {
      ArgumentUtility.CheckNotNullOrEmpty("connectionString", connectionString);
      ArgumentUtility.CheckNotNullOrEmpty("readOnlyConnectionString", readOnlyConnectionString);

      _connectionString = connectionString;
      _readOnlyConnectionString = readOnlyConnectionString;
    }

    public new IRdbmsStorageObjectFactory Factory
    {
      get { return (IRdbmsStorageObjectFactory)base.Factory; }
    }

    public string ConnectionString
    {
      get { return _connectionString; }
    }

    public string ReadOnlyConnectionString
    {
      get { return _readOnlyConnectionString; }
    }

    public override bool IsIdentityTypeSupported (Type identityType)
    {
      ArgumentUtility.CheckNotNull("identityType", identityType);

      return (identityType == typeof(Guid));
    }
  }
}
