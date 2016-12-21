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
using System.Linq;
using System.Linq.Expressions;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Utilities;

namespace Remotion.Development.Data.UnitTesting.DomainObjects.Linq
{
  /// <summary>
  /// The <see cref="QueryableComparer"/> compares two <see cref="IQueryable"/> for as to wether they result in the same SQL statement and parameters.
  /// Use this comparer to unit test Linq expressions and manually build <see cref="Expression"/> trees.
  /// </summary>
  public class QueryableComparer
  {
    public delegate void AssertThatActualIsEqualToExpected (object actual, object expected);

    private readonly AssertThatActualIsEqualToExpected _assertThatActualIsEqualToExpected;

    public QueryableComparer (AssertThatActualIsEqualToExpected thatActualIsEqualToExpected)
    {
      ArgumentUtility.CheckNotNull ("thatActualIsEqualToExpected", thatActualIsEqualToExpected);

      _assertThatActualIsEqualToExpected = thatActualIsEqualToExpected;
    }

    public void Compare<T> (IQueryable<T> expected, IQueryable<T> actual)
        where T: DomainObject
    {
      ArgumentUtility.CheckNotNull ("expected", expected);
      ArgumentUtility.CheckNotNull ("actual", actual);

      IQuery expectedQuery = GetQuery (expected);
      IQuery actualQuery = GetQuery (actual);

      _assertThatActualIsEqualToExpected (expectedQuery.Statement, actualQuery.Statement);
      _assertThatActualIsEqualToExpected (expectedQuery.Parameters, actualQuery.Parameters);
    }

    private IQuery GetQuery<T> (IQueryable<T> queryable)
    {
      return QueryFactory.CreateQuery<T> ("<dynamic data>", queryable);
    }
  }
}
