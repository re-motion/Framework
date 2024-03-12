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
using System.Linq;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Linq.ExecutableQueries
{
  /// <summary>
  /// Adapts a query with a <see cref="DomainObject"/> sequence projection to implement the <see cref="IExecutableQuery{T}"/> interface.
  /// </summary>
  /// <typeparam name="TItem">
  /// The type of items a sequence of which is to be returned. This needs not be a <see cref="DomainObject"/> type, it can be any type the result
  /// items are assignable to, e.g., an interface. The <see cref="DomainObject"/> instances returned from the query are cast to this type.
  /// </typeparam>
  public class DomainObjectSequenceQueryAdapter<TItem> : QueryAdapterBase<IEnumerable<TItem>>
  {
    public DomainObjectSequenceQueryAdapter (IQuery query)
        : base(ArgumentUtility.CheckNotNull("query", query))
    {
      if (query.QueryType != QueryType.CollectionReadOnly)
        throw new ArgumentException("Only readonly collection queries can be used to load data containers.", "query");
    }

    public override IEnumerable<TItem> Execute (IQueryManager queryManager)
    {
      ArgumentUtility.CheckNotNull("queryManager", queryManager);

      return queryManager.GetCollection(this).AsEnumerable().Cast<TItem>();
    }
  }
}
