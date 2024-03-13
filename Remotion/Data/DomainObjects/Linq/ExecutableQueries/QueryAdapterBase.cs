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
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.DomainObjects.Queries.EagerFetching;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Linq.ExecutableQueries
{
  /// <summary>
  /// Represents the base class for all query classes that can be executed using an <see cref="IQueryManager"/>.
  /// </summary>
  public abstract class QueryAdapterBase<T> : IExecutableQuery<T>
  {
    private readonly IQuery _query;

    protected QueryAdapterBase (IQuery query)
    {
      ArgumentUtility.CheckNotNull("query", query);

      _query = query;
    }

    public abstract T Execute (IQueryManager queryManager);

    public IQuery Query
    {
      get { return _query; }
    }

    public string ID
    {
      get { return _query.ID; }
    }

    public IReadOnlyDictionary<string, object> Metadata
    {
      get { return _query.Metadata; }
    }

    public string Statement
    {
      get { return _query.Statement; }
    }

    public StorageProviderDefinition StorageProviderDefinition
    {
      get { return _query.StorageProviderDefinition; }
    }

    public Type? CollectionType
    {
      get { return _query.CollectionType;  }
    }

    public QueryType QueryType
    {
      get { return _query.QueryType; }
    }

    public QueryParameterCollection Parameters
    {
      get { return _query.Parameters; }
    }

    public EagerFetchQueryCollection EagerFetchQueries
    {
      get { return _query.EagerFetchQueries; }
    }
  }
}
