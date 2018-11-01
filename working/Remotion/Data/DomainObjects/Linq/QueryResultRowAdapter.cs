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
using Remotion.Linq.SqlBackend.SqlGeneration;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Linq
{
  /// <summary>
  /// Adapts a custom query result row to implement the <see cref="IDatabaseResultRow"/> interface. 
  /// </summary>
  public class QueryResultRowAdapter : IDatabaseResultRow
  {
    private readonly IQueryResultRow _queryResultRow;

    public QueryResultRowAdapter (IQueryResultRow queryResultRow)
    {
      ArgumentUtility.CheckNotNull ("queryResultRow", queryResultRow);

      _queryResultRow = queryResultRow;
    }

    public IQueryResultRow QueryResultRow
    {
      get { return _queryResultRow; }
    }

    public T GetValue<T> (ColumnID columnID)
    {
      ArgumentUtility.CheckNotNull ("columnID", columnID);

      return _queryResultRow.GetConvertedValue<T> (columnID.Position);
    }

    public T GetEntity<T> (params ColumnID[] columnIDs)
    {
      throw new NotSupportedException (
          "This LINQ provider does not support queries with complex projections that include DomainObjects."
          + Environment.NewLine
          + "Either change the query to return just a sequence of DomainObjects (e.g., 'from o in QueryFactory.CreateLinqQuery<Order>() select o') "
          + "or change the complex projection to contain no DomainObjects "
          + "(e.g., 'from o in QueryFactory.CreateLinqQuery<Order>() select new { o.OrderNumber, o.OrderDate }').");
    }
  }
}