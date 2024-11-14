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
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Linq.ExecutableQueries
{
  /// <summary>
  /// Adapts a query with a custom projection to implement the <see cref="IExecutableQuery{T}"/> interface.
  /// </summary>
  /// <typeparam name="TResultItem">The item type to return a sequence of. The <see cref="IQueryResultRow"/> instances returned by the query
  /// are converted to this type via the <see cref="ResultConversion"/> delegate.</typeparam>
  public class CustomSequenceQueryAdapter<TResultItem> : QueryAdapterBase<IEnumerable<TResultItem>>
  {
    private readonly Func<IQueryResultRow, TResultItem> _resultConversion;

    public CustomSequenceQueryAdapter (IQuery query, Func<IQueryResultRow, TResultItem> resultConversion)
        : base(query)
    {
      ArgumentUtility.CheckNotNull("resultConversion", resultConversion);

      if (query.QueryType != QueryType.CustomReadOnly)
        throw new ArgumentException("Only custom readonly queries can be used to load custom results.", "query");

      _resultConversion = resultConversion;
    }

    public Func<IQueryResultRow, TResultItem> ResultConversion
    {
      get { return _resultConversion; }
    }

    public override IEnumerable<TResultItem> Execute (IQueryManager queryManager)
    {
      ArgumentUtility.CheckNotNull("queryManager", queryManager);

      return queryManager.GetCustom(this, _resultConversion);
    }
  }
}
