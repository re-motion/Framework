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
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Linq.ExecutableQueries
{
  /// <summary>
  /// Adapts a query with a scalar projection to implement the <see cref="IExecutableQuery{T}"/> interface.
  /// </summary>
  /// <typeparam name="T">The type of scalar value to return. The <see cref="object"/> values returned by the query
  /// are converted to this type via the <see cref="ResultConversion"/> delegate.</typeparam>
  public class ScalarQueryAdapter<T> : QueryAdapterBase<T>
  {
    private readonly Func<object?, T> _resultConversion;

    public ScalarQueryAdapter (IQuery query, Func<object?, T> resultConversion)
      : base(ArgumentUtility.CheckNotNull("query", query))
    {
      ArgumentUtility.CheckNotNull("resultConversion", resultConversion);

      if (query.QueryType != QueryType.ScalarReadOnly)
        throw new ArgumentException("Only readonly scalar queries can be used to load scalar results.", "query");

      _resultConversion = resultConversion;
    }

    public Func<object?, T> ResultConversion
    {
      get { return _resultConversion; }
    }

    public override T Execute (IQueryManager queryManager)
    {
      ArgumentUtility.CheckNotNull("queryManager", queryManager);

      var scalarValue = queryManager.GetScalar(this);
      return _resultConversion(scalarValue);
    }
  }
}
