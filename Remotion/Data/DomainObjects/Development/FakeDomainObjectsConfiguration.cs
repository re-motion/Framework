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
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Queries.Configuration;

namespace Remotion.Data.DomainObjects.Development
{
  /// <summary>
  /// Fake implementation of the <see cref="IDomainObjectsConfiguration"/> interface. Use this class for programmatically setting up the configuration 
  /// in unit test scenarios.
  /// </summary>
  public class FakeDomainObjectsConfiguration: IDomainObjectsConfiguration
  {
    private readonly StorageConfiguration _storage;
    private readonly QueryConfiguration _query;

    public FakeDomainObjectsConfiguration (StorageConfiguration? storage = null, QueryConfiguration? query = null)
    {
      _storage = storage ?? new StorageConfiguration();
      _query = query ?? new QueryConfiguration();
    }

    public StorageConfiguration Storage
    {
      get { return _storage; }
    }

    public QueryConfiguration Query
    {
      get { return _query; }
    }
  }
}
